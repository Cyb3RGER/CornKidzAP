using System;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace CornKidzAP;

public class APConnectionInfo
{
    private const string CONN_INFO_FILE_NAME = "APConnInfo.json";
    
    public string HostName = string.Empty;
    public string SlotName = string.Empty;
    public string Password = string.Empty;
    
    public bool IsValid => HostName is { Length: > 0 } && SlotName is { Length: > 0 };

    public static APConnectionInfo LoadFromDisk()
    {
        try
        {
            var filePath = Path.Combine(Application.persistentDataPath, CONN_INFO_FILE_NAME);
            return File.Exists(filePath) ? JsonConvert.DeserializeObject<APConnectionInfo>(File.ReadAllText(filePath)) : new APConnectionInfo();
        }
        catch (Exception e)
        {
            CornKidzAP.Logger.LogError($"Error loading last connection info from disk: {e}");
        }
        return new APConnectionInfo();
    }

    public void SaveToDisk()
    {
        try
        {
            var filePath = Path.Combine(Application.persistentDataPath, CONN_INFO_FILE_NAME);
            File.WriteAllText(filePath, JsonConvert.SerializeObject(this));
        }
        catch (Exception e)
        {
            CornKidzAP.Logger.LogError($"Error saving last connection info to disk: {e}");
        }
    }
    
    
}