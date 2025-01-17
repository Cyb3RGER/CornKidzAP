using System;
using UnityEngine;
using UnityEngine.UI;

namespace CornKidzAP.Archipelago;

public class APMainMenuTrackerUI : MonoBehaviour
{
    private RectTransform _rectTransform;
    private bool _shouldShow;
    private bool _isShowing;
    private bool _bLoaded;
    private float _t;
    private float _waitTimer;
    private const float _duration = .5f;
    private const float _waitTime = 0f;
    private Vector3 _startPosition = new(0, -100, 0);
    private Vector3 _targetPosition = new(0, 50, 0);
    private float _elapsed;
    private Font _font;
    private GridLayoutGroup _grid;
    private Image _image;
    private GameObject[] _rows = new GameObject[6];

    public void Awake()
    {
        _font = UI.instance.transform.Find("Objective").GetComponent<Text>().font;
        var hudObject = new GameObject("APMainMenuHUD")
        {
            layer = LayerMask.NameToLayer("UI")
        };
        _rectTransform = hudObject.AddComponent<RectTransform>();
        _rectTransform.SetParent(UI.instance.transform);
        _rectTransform.localRotation = Quaternion.identity;
        _rectTransform.localScale = new(1.87f, 1.87f, 1.87f);
        _rectTransform.anchorMin = new(0.5f, 0);
        _rectTransform.anchorMax = new(0.5f, 0);
        _rectTransform.pivot = new(0.5f, 0);
        _rectTransform.anchoredPosition3D = new(-10, 50, 0);
        _image = hudObject.AddComponent<Image>();
        _image.color = new Color(0.1321f, 0.0758f, 0.0704f, 0.6588f);
        _image.material = new Material(_image.material)
        {
            shader = Shader.Find("UI/Default Font FRONT"),
            renderQueue = 3003
        };
        var sizeFitter = hudObject.AddComponent<ContentSizeFitter>();
        sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        sizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        _grid = hudObject.AddComponent<GridLayoutGroup>();
        _grid.cellSize = new(80, 20);
        _grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        _grid.spacing = new Vector2(0, -7);
        _grid.startCorner = GridLayoutGroup.Corner.UpperLeft;
        _grid.startAxis = GridLayoutGroup.Axis.Horizontal;
        _grid.constraintCount = 2;
        _rows[0] = CreateRow("Drill:", () => $"{(GameCtrl.instance.data.upgrades[1] ? "\u2713" : "x")}");
        _rows[1] = CreateRow("Fall Warp:", () => $"{(GameCtrl.instance.data.upgrades[2] ? "\u2713" : "x")}");
        _rows[2] = CreateRow("Grater:", () => $"{(GameCtrl.instance.data.items[310] ? "\u2713" : "x")}");
        _rows[3] = CreateRow("Worm:", () => $"{(GameCtrl.instance.data.switches[236] ? "\u2713" : "x")}");
        _rows[4] = CreateRow("Rats:", () => $"{ArchipelagoClient.ArchipelagoData.Rats}/6");
        _rows[5] = CreateRow("Fish:", () => $"{ArchipelagoClient.ArchipelagoData.Fish}/3");
    }

    private GameObject CreateRow(string title, Func<string> textGetter)
    {
        var row = new GameObject("Row");
        var rectTransform = row.AddComponent<RectTransform>();
        rectTransform.SetParent(_grid.transform);
        rectTransform.localPosition = Vector3.zero;
        rectTransform.localRotation = Quaternion.identity;
        rectTransform.localScale = Vector3.one;
        var layout = row.AddComponent<HorizontalLayoutGroup>();
        layout.childAlignment = TextAnchor.MiddleLeft;
        AddTextChild(row.transform, title);
        AddTextChild(row.transform, textGetter);
        return row;
    }

    private void AddTextChild(Transform parent, string text)
    {
        var trackerText = AddTextChild_internal(parent, TextAnchor.MiddleLeft);
        trackerText.constText = text;
    }

    private void AddTextChild(Transform parent, Func<string> textGetter)
    {
        var trackerText = AddTextChild_internal(parent, TextAnchor.MiddleRight);
        trackerText.TextGetter = textGetter;
    }

    private APMainMenuTrackerText AddTextChild_internal(Transform parent, TextAnchor alignment)
    {
        var textObject = new GameObject("Text");
        var rectTransform = textObject.AddComponent<RectTransform>();
        rectTransform.SetParent(parent);
        rectTransform.localPosition = Vector3.zero;
        rectTransform.localRotation = Quaternion.identity;
        rectTransform.localScale = Vector3.one;
        var sizeFitter = textObject.AddComponent<ContentSizeFitter>();
        sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        sizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        var text = textObject.AddComponent<Text>();
        text.text = string.Empty;
        text.font = _font;
        text.fontSize = 16;
        text.alignment = alignment;
        text.material = new Material(_font.material)
        {
            shader = Shader.Find("GUI/Text Shader"),
            renderQueue = 3003,
        };
        var trackerText = textObject.AddComponent<APMainMenuTrackerText>();
        return trackerText;
    }

    public void Update()
    {
        if (!ArchipelagoClient.Authenticated || ArchipelagoClient.State != APState.InGame)
        {
            //reset when disconnected
            _t = 0;
            _rectTransform.anchoredPosition3D = _startPosition;
            _bLoaded = false;
            return;
        }

        if (!_bLoaded)
        {
            //set visibility for info that is option-based
            _rows[4].SetActive(ArchipelagoClient.SlotData.IsRatsanity);
            _rows[5].SetActive(ArchipelagoClient.SlotData.IsFishsanity);
            _bLoaded = true;
        }

        //handle slide-in/-out
        _shouldShow = GameCtrl.instance.bPause && GameCtrl.instance.currentWorld >= 0;
        _isShowing = _t > 0f;
        _elapsed = 0f;
        if (_shouldShow || _isShowing)
        {
            if (_t < 1f)
                _elapsed = Time.unscaledDeltaTime;
        }

        if (_shouldShow)
            _waitTimer = 0;
        else
        {
            if (_isShowing)
            {
                if (_waitTimer < _waitTime)
                    _waitTimer += Time.unscaledDeltaTime;
                else
                    _elapsed = -Time.unscaledDeltaTime;
            }
            else
                _waitTimer = 0;
        }

        _t += _elapsed / _duration;
        _t = Mathf.Clamp(_t, 0f, 1f);
        _rectTransform.anchoredPosition3D = Vector3.Lerp(_startPosition, _targetPosition, Mathf.SmoothStep(0.0f, 1.0f, _t));
    }
}