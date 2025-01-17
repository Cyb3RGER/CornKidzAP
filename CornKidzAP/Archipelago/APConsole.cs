using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CornKidzAP.Archipelago;

public class APConsole : MonoBehaviour
{
    public bool showConsole = true;
    private Vector2 scrollPos = Vector2.zero;
    private Queue<IAPNotification> pendingLogQueue = [];
    private Queue<IAPNotification> logQueue = [];
    private const int maxLogEntries = 100;
    private float lineHeight;

    private GUIStyle labelStyle;
    private GUIStyle scrollViewStyle;
    private GUIStyle disabledButtonStyle;
    private string commandMessage;
    private Rect contentRect;
    private Rect scrollViewRect;

    private bool shouldScrollToEnd;

    public void AddLogMessage(IAPNotification log)
    {
        pendingLogQueue.Enqueue(log);
    }

    private void ProcessLogQueue()
    {
        while (pendingLogQueue.Count > 0)
        {
            var log = pendingLogQueue.Dequeue();
            if (logQueue.Count >= maxLogEntries)
            {
                logQueue.Dequeue();
            }
            else
            {
                //if we remove beforehand we don't need to scroll
                shouldScrollToEnd = scrollPos.y >= contentRect.height - scrollViewRect.height - scrollViewRect.height - lineHeight;
            }

            logQueue.Enqueue(log);
        }
    }

    private void ScrollToEnd()
    {
        var maxScroll = contentRect.height - scrollViewRect.height;
        scrollPos.y = maxScroll;
    }

    public void ToggleConsole()
    {
        showConsole = !showConsole;
    }

    private float GetLabelHeight(string text, float width)
    {
        var textHeight = labelStyle.CalcHeight(new GUIContent(text), width);
        var amountOfLines = Mathf.CeilToInt(textHeight / lineHeight);
        return amountOfLines * lineHeight;
    }

    private void OnGUI()
    {
        // we can only set these up in OnGUI
        labelStyle ??= new GUIStyle(GUI.skin.label) { richText = true, wordWrap = true, fontSize = 13, alignment = TextAnchor.MiddleLeft };
        scrollViewStyle ??= new GUIStyle(GUI.skin.box);
        disabledButtonStyle ??= new GUIStyle(GUI.skin.button)
        {
            normal = { textColor = Color.gray },
            active = { textColor = Color.gray },
            hover = { textColor = Color.gray },
            fontStyle = FontStyle.Italic,
        };
        GUI.skin.scrollView = scrollViewStyle;
        lineHeight = labelStyle.lineHeight + labelStyle.padding.vertical;
        //CornKidzAP.Logger.LogDebug($"{labelStyle.lineHeight}");
        var e = Event.current;
        if (e.type == EventType.KeyDown && e.keyCode == KeyCode.F8)
        {
            e.Use();
            ToggleConsole();
        }

        if (!showConsole)
        {
            //Only render Show Button
            RenderButton(new Rect(Screen.width - 75, 0, 75, 20), "Show (F8)", true, ToggleConsole);
            return;
        }

        // Title
        var width = Screen.width * .33f; 
        var deathLinkStatus = (ArchipelagoClient.APDeathLinkHandler?.IsDeathLinkActive ?? false) ? "On" : "Off";
        GUI.Label(new Rect(Screen.width - width, 0, width, 20), $"AP Console v{MyPluginInfo.PLUGIN_VERSION} - Deathlink: {deathLinkStatus}", scrollViewStyle);
        // Render Log
        const int height = 100;
        scrollViewRect = new Rect(Screen.width - width, 20, width, height);
        var contentWidth = scrollViewRect.width - 20;
        var contentHeight = height - 20f;
        var labelPosY = 0f;
        foreach (var log in logQueue)
        {
            var labelHeight = GetLabelHeight(log.Text, contentWidth);
            labelPosY += labelHeight;
            contentHeight = Mathf.Max(contentHeight, labelPosY);
        }

        contentRect = new Rect(0, 0, contentWidth, contentHeight);
        if (shouldScrollToEnd)
        {
            ScrollToEnd();
            shouldScrollToEnd = false;
        }

        AdjustScroll(e);
        scrollPos = GUI.BeginScrollView(scrollViewRect, scrollPos, contentRect, false, true);
        labelPosY = 0f;
        foreach (var log in logQueue)
        {
            var text = log.Text;
            var labelHeight = GetLabelHeight(text, contentWidth);
            var labelRect = new Rect(0, labelPosY, contentWidth, labelHeight);
            GUI.Label(labelRect, text, labelStyle);
            labelPosY += labelHeight;
        }

        GUI.EndScrollView();

        //Render Chat
        var commandMessageRect = new Rect(scrollViewRect.x, scrollViewRect.y + scrollViewRect.height, scrollViewRect.width - 50, 20);
        var submit = e.type == EventType.KeyDown && e.keyCode is KeyCode.Return or KeyCode.KeypadEnter;
        commandMessage = GUI.TextField(commandMessageRect, commandMessage);
        if (submit && Event.current.type == EventType.KeyDown)
        {
            // The text field has not consumed the event, which means they were not focused.
            submit = false;
        }

        //Render Chat Button
        RenderButton(new Rect(commandMessageRect.x + commandMessageRect.width, commandMessageRect.y, 50, 20), "Send", !string.IsNullOrEmpty(commandMessage) && ArchipelagoClient.Authenticated && ArchipelagoClient.State == APState.InGame, Chat, submit);

        //Render other Buttons
        List<(string, bool, Action)> otherButtons =
        [
            ("Release", ArchipelagoClient.CanSendRelease, () => ArchipelagoClient.Session.Say("!release")),
            ("Collect", ArchipelagoClient.CanSendCollect, () => ArchipelagoClient.Session.Say("!collect")),
            ("Remaining", ArchipelagoClient.CanSendRemaining, () => ArchipelagoClient.Session.Say("!remaining")),
            ("Toggle Deathlink", ArchipelagoClient.SlotData?.IsDeathLink ?? false, () => ArchipelagoClient.APDeathLinkHandler?.ToggleDeathLink()),
        ];
        List<(string, bool, Action, float)> fixedSizeButtons =
        [
            ("+", true, IncreaseFontSize, 25),
            ("-", true, DecreaseFontSize, 25),
            ("Hide (F8)", true, ToggleConsole, 75)
        ];
        var fixedButtonsWidth = fixedSizeButtons.Sum(x => x.Item4);
        var otherButtonWidth = (scrollViewRect.width - fixedButtonsWidth) / otherButtons.Count;
        var buttonsRect = new Rect(commandMessageRect.x, commandMessageRect.y + commandMessageRect.height, otherButtonWidth, 20);
        foreach (var (text, isEnabled, onClick) in otherButtons)
        {
            buttonsRect = RenderButton(buttonsRect, text, isEnabled, onClick);
        }

        foreach (var (text, isEnabled, onClick, buttonWidth) in fixedSizeButtons)
        {
            buttonsRect.width = buttonWidth;
            buttonsRect = RenderButton(buttonsRect, text, isEnabled, onClick);
        }
    }

    private void IncreaseFontSize()
    {
        labelStyle.fontSize++;
    }

    private void DecreaseFontSize()
    {
        labelStyle.fontSize--;
    }

    private void LateUpdate()
    {
        ProcessLogQueue();
    }

    private void AdjustScroll(Event e)
    {
        if (e.type == EventType.ScrollWheel)
        {
            // delta is 3 for one "tick" on the wheel for me, let's hope it's the same for others lol
            var scrollAmount = e.delta.y / 3f;
            if (scrollAmount != 0)
            {
                scrollPos.y += scrollAmount * lineHeight;
            }

            e.Use(); // prevent default scroll behaviour
        }

        // Round to nearest line
        scrollPos.y = Mathf.Round(scrollPos.y / lineHeight) * lineHeight;
        // Clamp the scroll position to ensure it doesn't go out of bounds
        scrollPos.y = Mathf.Clamp(scrollPos.y, 0, contentRect.height - scrollViewRect.height);
    }

    private void Chat()
    {
        ArchipelagoClient.Session.Say(commandMessage);
        commandMessage = string.Empty;
    }

    private Rect RenderButton(Rect rect, string text = "", bool isEnabled = true, Action onClick = null, bool triggered = false)
    {
        if (isEnabled)
        {
            if (GUI.Button(rect, text) || triggered)
            {
                onClick?.Invoke();
            }
        }
        else
        {
            GUI.Label(rect, text, disabledButtonStyle);
        }

        rect.x += rect.width;
        return rect;
    }
}