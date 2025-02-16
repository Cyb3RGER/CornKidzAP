using System;
using Archipelago.MultiClient.Net.BounceFeatures.DeathLink;
using UnityEngine;

namespace CornKidzAP.Archipelago;

public class APDeathLinkHandler : MonoBehaviour
{
    public DeathLinkService DeathLinkService;
    /// <summary>
    /// set to true when death link was send or when entering gameover screen with DieaFromDeathLink and reset when gameover screen is left
    /// </summary>
    private bool HasSendCurrentDeath;
    /// <summary>
    /// Force overwrite disable death link
    /// </summary>
    private bool DisableDeathLink;
    /// <summary>
    /// set to true when kill by death link and reset when entering gameover screen
    /// </summary>
    private bool DiedFromDeathLink;
    private DateTime LastDeathLinkTime = DateTime.MinValue;
    /// <summary>
    /// This is kinda long but so is the death animation
    /// </summary>
    private TimeSpan DeathLinkCooldown = TimeSpan.FromSeconds(10);
    private DeathLink QueuedDeathLink;
    private PlayerCtrl _player;

    public bool IsDeathLinkActive => ArchipelagoClient.Authenticated && ArchipelagoClient.State == APState.InGame && ArchipelagoClient.SlotData.IsDeathLink && !DisableDeathLink;

    private void Start()
    {
        _player = GetPlayer();
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private PlayerCtrl GetPlayer()
    {
        return GameObject.FindWithTag("Player").GetComponent<PlayerCtrl>();
    }
    
    private void Update()
    {
        if (!_player)
            _player = GetPlayer();
        if (DisableDeathLink)
            return;
        CheckForDeath();
        if (DiedFromDeathLink && GameCtrl.instance.currentWorld >= 0)
        {
            HasSendCurrentDeath = true;
            DiedFromDeathLink = false;
        }

        if (HasSendCurrentDeath && GameCtrl.instance.currentWorld < 0)
            HasSendCurrentDeath = false;
        if (QueuedDeathLink == null || !CanDie()) return;
        KillPlayer(QueuedDeathLink);
        LastDeathLinkTime = DateTime.Now;
        QueuedDeathLink = null;
    }

    private bool CanDie()
    {
        CornKidzAP.Logger.LogDebug($"CanDie: {GameCtrl.instance.currentWorld >= 0}, {!GameCtrl.instance.bPause}, {_player}, {_player.bNoHurt}");
        return GameCtrl.instance.currentWorld >= 0 && !GameCtrl.instance.bPause && _player && !_player.bNoHurt;
    }

    private void CheckForDeath()
    {
        if (HasSendCurrentDeath || DiedFromDeathLink)
            return;
        if (!ArchipelagoClient.Authenticated || ArchipelagoClient.State != APState.InGame)
            return;
        if (!ArchipelagoClient.SlotData.IsDeathLink || GameCtrl.instance.currentWorld < 0)
            return;
        if (_player.plState != PlayerCtrl.State.die)
            return;
        HasSendCurrentDeath = true;
        if (DeathLinkService == null)
            CornKidzAP.Logger.LogWarning("Tried to send death link with no death link service!");
        if (DateTime.Now - LastDeathLinkTime <= DeathLinkCooldown) return;
        CornKidzAP.Logger.LogDebug("Sending death link...");
        ArchipelagoClient.APConsole.LogInfo("Sending death link...");
        DeathLinkService?.SendDeathLink(new DeathLink(ArchipelagoClient.Session.Players.ActivePlayer.Name, "got corned"));
        LastDeathLinkTime = DateTime.Now;
    }

    public void KillPlayer(DeathLink deathlink)
    {
        DiedFromDeathLink = true;
        GameCtrl.instance.HP = 0;
        CornKidzAP.Logger.LogDebug($"Received death link: {deathlink.Source}: {deathlink.Cause} {deathlink.Timestamp}");
        ArchipelagoClient.APConsole.LogInfo($"Received death link: {deathlink.Source}: {deathlink.Cause} ");
    }

    public void HandleDeathLink(DeathLink deathlink)
    {
        if (DisableDeathLink)
            return;
        if (DateTime.Now - LastDeathLinkTime <= DeathLinkCooldown) return;
        if (!CanDie())
        {
            CornKidzAP.Logger.LogDebug("Queued death link");
            QueuedDeathLink = deathlink;
        }
        else
        {
            KillPlayer(deathlink);
        }
        LastDeathLinkTime = DateTime.Now;
    }

    public void ToggleDeathLink()
    {
        if (!ArchipelagoClient.Authenticated || ArchipelagoClient.State != APState.InGame)
            return;

        DisableDeathLink = !DisableDeathLink;
    }
}