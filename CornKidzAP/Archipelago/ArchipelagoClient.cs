using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.BounceFeatures.DeathLink;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Helpers;
using Archipelago.MultiClient.Net.MessageLog.Messages;
using Archipelago.MultiClient.Net.Models;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine.SceneManagement;

namespace CornKidzAP.Archipelago;

public enum APState
{
    Menu,
    InGame,
}

public static class ArchipelagoClient
{
    private const string GameName = "Corn Kidz 64";

    private static ConcurrentQueue<ItemInfo> _itemQueue = new();
    public static APState State;
    public static bool Authenticated;
    public static bool IsConnecting;
    public static APConnectionInfo ConnectionInfo;
    public static ArchipelagoSession Session;
    public static APConnectionUI APConnectionUI;
    public static APNotificationUI APNotificationUI;
    public static APMainMenuTrackerUI APMainMenuTrackerUI;
    public static APConsole APConsole;
    public static ArchipelagoData ArchipelagoData;
    public static APTrapHandler APTrapHandler;
    public static APSlotData SlotData;
    public static APDeathLinkHandler APDeathLinkHandler;
    public static string LastSeed;
    public static bool IsNew { get; set; }
    public static bool HasBeatenGoal { get; private set; }
    
    [CanBeNull] public static Version ModVersion => Version.TryParse(MyPluginInfo.PLUGIN_VERSION, out var version) ? version : null;

    public static bool CanSendRelease
    {
        get
        {
            if (!Authenticated || Session == null)
                return false;
            return (Session.RoomState.ReleasePermissions.HasFlag(Permissions.Enabled) ||
                    HasBeatenGoal && Session.RoomState.ReleasePermissions.HasFlag(Permissions.Goal)) &&
                   Session.Locations.AllMissingLocations.Count > 0; //nothing left to release
        }
    }

    public static bool CanSendCollect
    {
        get
        {
            if (!Authenticated || Session == null)
                return false;
            return Session.RoomState.CollectPermissions.HasFlag(Permissions.Enabled) ||
                   HasBeatenGoal && Session.RoomState.CollectPermissions.HasFlag(Permissions.Goal);
        }
    }

    public static bool CanSendRemaining
    {
        get
        {
            if (!Authenticated || Session == null)
                return false;
            return Session.RoomState.RemainingPermissions.HasFlag(Permissions.Enabled) ||
                   HasBeatenGoal && Session.RoomState.RemainingPermissions.HasFlag(Permissions.Goal);
        }
    }


    private static async UniTask<LoginResult> TryConnectAndLogin()
    {
        try
        {
            await Session.ConnectAsync();
        }
        catch (AggregateException e)
        {
            if (e.GetBaseException() is OperationCanceledException)
                return new LoginFailure("Connection timed out.");

            return new LoginFailure(e.GetBaseException().Message);
        }
        catch (OperationCanceledException)
        {
            return new LoginFailure("Connection timed out.");
        }
        catch (Exception e)
        {
            return new LoginFailure(e.Message);
        }

        IsNew = ArchipelagoData.SaveFileExists();
        ArchipelagoData = ArchipelagoData.LoadFromDisk();
        Session.Items.ItemReceived += Session_ItemReceived;
        Session.MessageLog.OnMessageReceived += Session_MessageReceived;
        Session.Socket.ErrorReceived += Session_ErrorReceived;
        Session.Socket.SocketClosed += Session_SocketClosed;
        LoginResult result;
        try
        {
            result = await Session.LoginAsync(
                GameName,
                ConnectionInfo.SlotName,
                ItemsHandlingFlags.AllItems,
                null,
                null,
                "",
                ConnectionInfo.Password);
        }
        catch (Exception e)
        {
            return new LoginFailure(e.Message);
        }

        return result;
    }

    public static async UniTaskVoid Connect()
    {
        if (Authenticated || IsConnecting) return; // already connected
        try
        {
            Session = ArchipelagoSessionFactory.CreateSession(ConnectionInfo.HostName);
            IsConnecting = true;
            var loginResult = await TryConnectAndLogin();
            if (loginResult is LoginSuccessful loginSuccess)
            {
                Authenticated = true;
                IsConnecting = false;
                State = APState.InGame;
                ConnectionInfo.SaveToDisk();
                SlotData = new APSlotData(loginSuccess.SlotData);
                Session.DataStorage.TrackClientStatus(Session_TrackPlayerStatus);
                APDeathLinkHandler.DeathLinkService = Session.CreateDeathLinkService();
                APDeathLinkHandler.DeathLinkService.OnDeathLinkReceived += APDeathLinkHandler.HandleDeathLink;
                if (SlotData.IsDeathLink) APDeathLinkHandler.DeathLinkService.EnableDeathLink();
                var copySettings = GameCtrl.instance.data.GetCopySettings();
                GameCtrl.instance.ResetSav();
                GameCtrl.instance.LoadGame();
                GameCtrl.instance.data.ApplyCopySettings(copySettings);
                var activeScene = SceneManager.GetActiveScene().name;
                if (activeScene != "title01")
                {
                    if (activeScene != ArchipelagoData.LastScene)
                    {
                        GameCtrl.instance.FadeToScene("PizzaOut", 1);
                        GameCtrl.instance.HP = GameCtrl.instance.startHP;
                        GameCtrl.instance.nextTrack = 2;
                    }
                    else
                    {
                        GameCtrl.instance.LoadScene("");
                    }

                    UI.instance.UnPause();
                }
                CheckVersion().Forget();
            }
            else if (loginResult is LoginFailure loginFailure)
            {
                Reset();
                var message = $"Failed to connect to AP: {string.Join("\n", loginFailure.Errors)}";
                APConsole?.LogError(message);
                CornKidzAP.Logger.LogError(message);
            }
        }
        catch (Exception e)
        {
            Reset();
            CornKidzAP.Logger.LogError($"Exception while trying to connect to AP: {e}");
        }
    }

    private static async UniTask CheckVersion()
    {
        CornKidzAP.Logger.LogDebug($"ServerVersion: {SlotData.ServerVersion}, ModVersion: {ModVersion}");
        if (SlotData.ServerVersion != null && ModVersion != null && SlotData.ServerVersion == ModVersion) return;
        var message = $"Warning:\nVersion mismatch between apworld version on Server and Mod:\nServer version: {SlotData.ServerVersion?.ToString() ?? "unknown (<0.0.4?)"} =/= Mod version: {ModVersion?.ToString() ?? "unknown"}.\nThis may cause issue, please up-/downgrade your mod accordingly unless you know what you're doing.";
        CornKidzAP.Logger.LogWarning(message);
        await Task.Delay(500); //small delay so we print after the initial connect messages
        APConsole.LogWarning(message);
    }

    private static void Reset()
    {
        Session = null;
        SlotData = null;
        APDeathLinkHandler.DeathLinkService = null;
        Authenticated = false;
        IsConnecting = false;
        HasBeatenGoal = false;
        State = APState.Menu;
        if (APDeathLinkHandler) APDeathLinkHandler.DeathLinkService = null;
    }

    private static void Session_TrackPlayerStatus(ArchipelagoClientState state)
    {
        HasBeatenGoal = state == ArchipelagoClientState.ClientGoal;
    }

    private static void Session_ItemReceived(ReceivedItemsHelper helper)
    {
        var item = helper.DequeueItem();
        CornKidzAP.Logger.LogDebug($"Session_ItemReceived: {item.ItemDisplayName} {item.ItemId} {helper.Index} {ArchipelagoData.Index}");
        if (helper.Index <= ArchipelagoData.Index)
        {
            CornKidzAP.Logger.LogDebug($"Ignoring Item {item.ItemDisplayName} based on Index: {helper.Index} <= {ArchipelagoData.Index}");
            return;
        }

        ArchipelagoData.Index++;
        CornKidzAP.Logger.LogDebug($"new item index {ArchipelagoData.Index} {helper.Index}");
        if (IsInValidWorld() && ArchipelagoData != null)
        {
            GiveItem(item);
        }
        else
        {
            _itemQueue.Enqueue(item);
            CornKidzAP.Logger.LogDebug($"enqueued item {item.ItemDisplayName}");
        }
    }

    private static bool IsInValidWorld()
    {
        return GameCtrl.instance.currentWorld >= 0;
    }

    public static void TryGiveEnqueuedItems()
    {
        if (_itemQueue is not { Count: > 0 } || !IsInValidWorld()) return;
        while (_itemQueue.Count > 0)
        {
            if (!_itemQueue.TryDequeue(out var item))
                break;
            CornKidzAP.Logger.LogDebug($"dequeued item {item.ItemDisplayName}");
            GiveItem(item);
        }
    }

    private static void GiveItem(ItemInfo item)
    {
        CornKidzAP.Logger.LogDebug($"giving item {item.ItemId} {item.ItemDisplayName}");
        switch (item.ItemId)
        {
            case APLookup.BaseID + 0:
                ItemHandler.UnlockMove(Moves.Jump);
                break;
            case APLookup.BaseID + 1:
                ItemHandler.UnlockMove(Moves.Punch);
                break;
            case APLookup.BaseID + 2:
                ItemHandler.UnlockMove(Moves.Climb);
                break;
            case APLookup.BaseID + 3:
                ItemHandler.UnlockMove(Moves.Slam);
                break;
            case APLookup.BaseID + 4:
                ItemHandler.UnlockMove(Moves.Headbutt);
                break;
            case APLookup.BaseID + 5:
                ItemHandler.UnlockMove(Moves.WallJump);
                break;
            case APLookup.BaseID + 6:
                ItemHandler.UnlockMove(Moves.Dive);
                break;
            case APLookup.BaseID + 7:
                ItemHandler.UnlockMove(Moves.Crouch);
                break;
            case APLookup.BaseID + 8:
                ItemHandler.AddUpgrade(1);
                break;
            case APLookup.BaseID + 9:
                ItemHandler.AddUpgrade(2);
                break;
            case APLookup.BaseID + 10:
                ItemHandler.SetSwitch(108);
                break;
            case APLookup.BaseID + 11:
                ItemHandler.SetSwitch(228);
                break;
            case APLookup.BaseID + 12:
                ItemHandler.SetSwitch(229);
                break;
            case APLookup.BaseID + 13:
                ItemHandler.SetSwitch(410);
                break;
            case APLookup.BaseID + 14:
                ItemHandler.AddBottleCap();
                break;
            case APLookup.BaseID + 15:
                ItemHandler.AddLevelItem(1);
                break;
            case APLookup.BaseID + 16:
                ItemHandler.AddLevelItem(2);
                break;
            case APLookup.BaseID + 17:
                ItemHandler.AddRat();
                break;
            case APLookup.BaseID + 18:
                ItemHandler.AddFish();
                break;
            case APLookup.BaseID + 19:
                ItemHandler.SetSwitch(236);
                break;
            case APLookup.BaseID + 20:
                ItemHandler.AddMaxHP();
                break;
            case APLookup.BaseID + 21:
                ItemHandler.AddGrater();
                break;
            case APLookup.BaseID + 22:
                ItemHandler.AddVoidScrew();
                break;
            case APLookup.BaseID + 23:
                ItemHandler.AddXP(1);
                break;
            case APLookup.BaseID + 24:
                ItemHandler.AddXP(3);
                break;
            case APLookup.BaseID + 25:
                ItemHandler.AddXP(5);
                break;
            case APLookup.BaseID + 26:
                ItemHandler.AddXP(10);
                break;
            case APLookup.BaseID + 27:
                ItemHandler.AddHP();
                break;
            case APLookup.BaseID + 28:
                ItemHandler.AddTrap(Traps.SlapTrap);
                break;
            case APLookup.BaseID + 29:
                ItemHandler.AddTrap(Traps.SlipTrap);
                break;
            case APLookup.BaseID + 30:
                ItemHandler.AddTrap(Traps.SkidTrap);
                break;
        }

        ArchipelagoData.SaveToDisk();
    }

    private static void Session_SocketClosed(string reason)
    {
        CornKidzAP.Logger.LogError($"AP Session Socket closed: {reason}");
        Reset();
        if (!string.IsNullOrEmpty(reason))
        {
            APConsole?.LogError($"Disconnected from AP Server: {reason}");
        }
    }

    private static void Session_ErrorReceived(Exception e, string message)
    {
        CornKidzAP.Logger.LogError($"AP Session Error: {message} {e}");
        Reset();
        if (!string.IsNullOrEmpty(message))
        {
            APConsole?.LogError(message);
        }
    }

    private static void Session_MessageReceived(LogMessage message)
    {
        CornKidzAP.Logger.LogInfo(message);
        if (message is ItemSendLogMessage { IsRelatedToActivePlayer: true } and not HintItemSendLogMessage or HintItemSendLogMessage { IsRelatedToActivePlayer: true, IsFound: false } or GoalLogMessage { IsActivePlayer: true } or ReleaseLogMessage { IsActivePlayer: true } or CollectLogMessage { IsActivePlayer: true })
            APNotificationUI?.AddNotification(new APConsoleMessage(message));
        APConsole?.AddLogMessage(new APConsoleMessage(message));
    }

    public static async UniTaskVoid Disconnect()
    {
        if (Authenticated)
            GameCtrl.instance.SaveGame();
        if (Session is { Socket.Connected: true })
            await Session.Socket.DisconnectAsync();
        Reset();
        APConsole?.LogInfo("Disconnected from AP Server.");
    }

    public static void SetGoalAchieved()
    {
        if (!Authenticated || State != APState.InGame)
            return;
        if (HasBeatenGoal)
            return;
        HasBeatenGoal = true;
        Session.SetGoalAchieved();
    }
}