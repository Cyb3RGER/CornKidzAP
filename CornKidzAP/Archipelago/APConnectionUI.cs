using UnityEngine;

namespace CornKidzAP.Archipelago;

public class APConnectionUI : MonoBehaviour
{
    private void OnGUI()
    {
        if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.LeftAlt)
        {
            Cursor.visible = !Cursor.visible;
        }

        // Yoinked directly from Subnautica randomizer, thanks Berserker
        // https://github.com/Berserker66/ArchipelagoSubnauticaModSrc/blob/master/mod/Archipelago.cs
        var apVersion = $"Archipelago v{MyPluginInfo.PLUGIN_VERSION}";
        if (ArchipelagoClient.Session != null)
        {
            if (ArchipelagoClient.Authenticated)
            {
                GUI.Label(new Rect(16, 16, 300, 20), apVersion + " Status: Connected");
            }
            else if (ArchipelagoClient.IsConnecting)
            {
                GUI.Label(new Rect(16, 16, 300, 20), apVersion + " Status: Connecting");
            }
            else
            {
                GUI.Label(new Rect(16, 16, 300, 20), apVersion + " Status: Authentication failed");
            }
        }
        else
        {
            GUI.Label(new Rect(16, 16, 300, 20), apVersion + " Status: Not Connected");
        }

        if ((ArchipelagoClient.Session == null || !ArchipelagoClient.Authenticated) && ArchipelagoClient.State == APState.Menu)
        {
            GUI.Label(new Rect(16, 36, 150, 20), "Host: ");
            GUI.Label(new Rect(16, 56, 150, 20), "Slot Name: ");
            GUI.Label(new Rect(16, 76, 150, 20), "Password: ");

            var submit = Event.current.type == EventType.KeyDown && Event.current.keyCode is KeyCode.Return or KeyCode.KeypadEnter;

            ArchipelagoClient.ConnectionInfo.HostName = GUI.TextField(new Rect(150 + 16 + 8, 36, 150, 20), ArchipelagoClient.ConnectionInfo.HostName);
            ArchipelagoClient.ConnectionInfo.SlotName = GUI.TextField(new Rect(150 + 16 + 8, 56, 150, 20), ArchipelagoClient.ConnectionInfo.SlotName);
            ArchipelagoClient.ConnectionInfo.Password = MaskedPasswordField(new Rect(150 + 16 + 8, 76, 150, 20), ArchipelagoClient.ConnectionInfo.Password, '*');

            if (submit && Event.current.type == EventType.KeyDown)
            {
                // The text fields have not consumed the event, which means they were not focused.
                submit = false;
            }

            if ((GUI.Button(new Rect(16, 96, 100, 20), "Connect") || submit) && ArchipelagoClient.ConnectionInfo.IsValid)
            {
                ArchipelagoClient.Connect().Forget();
            }
        }
        else if (ArchipelagoClient.State == APState.InGame && ArchipelagoClient.Session != null)
        {
            if (GUI.Button(new Rect(16, 36, 100, 20), "Disconnect"))
            {
                ArchipelagoClient.Disconnect().Forget();
            }
        }
    }

    private static string MaskedPasswordField(Rect rect, string password, char maskChar)
    {
        var input = GUI.TextField(rect, new string('*', password.Length));
        if (input.Length == password.Length) return password;
        if (input.Length > password.Length)
        {
            //push char to password
            password += Event.current.character;
        }
        else if (input.Length < password.Length && password.Length > 0)
        {
            //pop char from password
            password = password[..^1];
        }

        return password;
    }
}