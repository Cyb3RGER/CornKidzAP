using System.Text;
using Archipelago.MultiClient.Net.MessageLog.Messages;
using JetBrains.Annotations;
using Color = Archipelago.MultiClient.Net.Models.Color;

namespace CornKidzAP.Archipelago;

public interface IAPNotification
{
    string Text { get; }
}

public enum ConsoleLogType
{
    Debug,
    Information,
    Warning,
    Error,
}

public class APConsoleLogEntry(string message, ConsoleLogType logType = ConsoleLogType.Error) : IAPNotification
{
    private Color Color => logType switch
    {
        ConsoleLogType.Debug => new Color(128, 128, 128),
        ConsoleLogType.Information => Color.White,
        ConsoleLogType.Warning => Color.Yellow,
        ConsoleLogType.Error => Color.Red,
        _ => Color.White
    };
    public string Text => $"<color=#{Color.APColorToHex()}>{message}</color>";
}

public class APConsoleMessage(LogMessage message) : IAPNotification
{
    [CanBeNull] private string _text;

    private void SetText()
    {
        var builder = new StringBuilder();
        foreach (var part in message.Parts)
        {
            builder.Append($"<color=#{part.Color.APColorToHex()}>{part.Text}</color>");
        }
        _text = builder.ToString();
    }
    public string Text {
        get
        {
            if (_text == null) SetText();
            return _text;
        }
    }
}