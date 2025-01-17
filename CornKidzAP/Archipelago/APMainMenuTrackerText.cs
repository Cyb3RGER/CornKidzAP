using System;
using UnityEngine;
using UnityEngine.UI;

namespace CornKidzAP.Archipelago;

public class APMainMenuTrackerText : MonoBehaviour
{
    private Text _text;
    public Func<string> TextGetter;
    public string constText;

    private void Awake()
    {
        _text = GetComponent<Text>();
    }
    
    private void Update()
    {
        if (!ArchipelagoClient.Authenticated || ArchipelagoClient.State != APState.InGame)
        {
            _text.text = string.Empty;
            return;
        }
        _text.text = TextGetter != null ? TextGetter.Invoke() : constText;
    }
}