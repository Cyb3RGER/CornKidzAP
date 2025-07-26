using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;

namespace CornKidzAP.Archipelago;

public class ArchipelagoData
{
    public Data GameData { get; set; }
    public int Index { get; set; }
    public int Rats { get; set; }
    public int Fish { get; set; }
    public string LastScene { get; set; }
    public Dictionary<Moves, bool> Moves { get; set; } = Enum.GetValues(typeof(Moves)).Cast<Moves>().ToDictionary(x => x, x => false);

    [JsonConstructor]
    public ArchipelagoData()
    {
    }
    public ArchipelagoData([NotNull] Data data)
    {
        GameData = data ?? throw new ArgumentNullException(nameof(data));
    }
    public static ArchipelagoData LoadFromDisk()
    {
        var fileName = GetFileName();
        try
        {
            var filePath = Path.Combine(Application.persistentDataPath, fileName);
            var apData = File.Exists(filePath) ? JsonConvert.DeserializeObject<ArchipelagoData>(File.ReadAllText(filePath)) : new ArchipelagoData(GameCtrl.instance.data);
            CornKidzAP.Logger.LogInfo($"Game loaded from {filePath}");
            return apData;
        }
        catch (Exception e)
        {
            CornKidzAP.Logger.LogError($"Error loading AP save data from disk: {e}");
        }
        return new ArchipelagoData(GameCtrl.instance.data);
    }

    public static bool SaveFileExists()
    {
        try
        {
            return File.Exists(Path.Combine(Application.persistentDataPath, GetFileName()));
        }
        catch
        {
            return false;
        }
    }

    public void SaveToDisk()
    {
        var fileName = GetFileName();
        try
        {
            var filePath = Path.Combine(Application.persistentDataPath, fileName);
            File.WriteAllText(filePath, JsonConvert.SerializeObject(this));
            CornKidzAP.Logger.LogInfo($"Game saved to {filePath}");
        }
        catch (Exception e)
        {
            CornKidzAP.Logger.LogError($"Error saving AP save data to disk: {e}");
        }
    }
    
    [NotNull]
    private static string GetFileName()
    {
        if (string.IsNullOrEmpty(ArchipelagoClient.ConnectionInfo.HostName) || string.IsNullOrEmpty(ArchipelagoClient.ConnectionInfo.SlotName) || string.IsNullOrEmpty(ArchipelagoClient.Session.RoomState.Seed))
        {
            throw new InvalidOperationException("Must be connected to a server");
        }
        var fileName = $"APSave_{ArchipelagoClient.ConnectionInfo.SlotName}_{ArchipelagoClient.Session.RoomState.Seed}.json";
        return Path.GetInvalidFileNameChars().Aggregate(fileName, (current, invalidChar) => current.Replace(invalidChar, '_'));
    }

    public bool HasMove(Moves move)
    {
        Moves.TryGetValue(move, out var result);
        return result;
    }
    
}