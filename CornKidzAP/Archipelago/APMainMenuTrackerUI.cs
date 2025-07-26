using System;
using System.Linq;
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
    private GameObject[] _rows = new GameObject[14];
    public GameObject HUDObject;
    
    private const string noString = "-";
    private const string yesString = "X";

    public void Awake()
    {
        _font = UI.instance.transform.Find("crankCounter").GetComponent<Text>().font;
        HUDObject = new GameObject("APMainMenuHUD")
        {
            layer = LayerMask.NameToLayer("UI")
        };
        _rectTransform = HUDObject.AddComponent<RectTransform>();
        _rectTransform.SetParent(UI.instance.transform);
        _rectTransform.localRotation = Quaternion.identity;
        _rectTransform.localScale = new(1.87f, 1.87f, 1.87f);
        _rectTransform.anchorMin = new(0.5f, 0);
        _rectTransform.anchorMax = new(0.5f, 0);
        _rectTransform.pivot = new(0.5f, 0);
        _rectTransform.anchoredPosition3D = _startPosition;
        _image = HUDObject.AddComponent<Image>();
        _image.color = new Color(0.1321f, 0.0758f, 0.0704f, 0.6588f);
        _image.material = new Material(_image.material)
        {
            shader = Shader.Find("UI/Default Font FRONT"),
            renderQueue = 3003
        };
        var sizeFitter = HUDObject.AddComponent<ContentSizeFitter>();
        sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        sizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        _grid = HUDObject.AddComponent<GridLayoutGroup>();
        _grid.cellSize = new(76, 18);
        _grid.constraint = GridLayoutGroup.Constraint.FixedRowCount;
        _grid.spacing = new Vector2(5, -7);
        _grid.startCorner = GridLayoutGroup.Corner.UpperLeft;
        _grid.startAxis = GridLayoutGroup.Axis.Horizontal;
        _grid.padding = new RectOffset(5, 5, 0, 0);
        _grid.constraintCount = 4;
        //_rows[0] = CreateRow("Jump:", () => $"{(ArchipelagoClient.ArchipelagoData.HasMove(Moves.Jump) ? yesString : noString)}");
        _rows[1] = CreateRow("Punch", () => $"{(ArchipelagoClient.ArchipelagoData.HasMove(Moves.Punch) ? yesString : noString)}");
        _rows[2] = CreateRow("Climb:", () => $"{(ArchipelagoClient.ArchipelagoData.HasMove(Moves.Climb) ? yesString : noString)}");
        _rows[3] = CreateRow("Slam:", () => $"{(ArchipelagoClient.ArchipelagoData.HasMove(Moves.Slam) ? yesString : noString)}");
        _rows[4] = CreateRow("Headbutt:", () => $"{(ArchipelagoClient.ArchipelagoData.HasMove(Moves.Headbutt) ? yesString : noString)}");
        _rows[5] = CreateRow("WallJump:", () => $"{(ArchipelagoClient.ArchipelagoData.HasMove(Moves.WallJump) ? yesString : noString)}");
        //_rows[6] = CreateRow("Dive:", () => $"{(ArchipelagoClient.ArchipelagoData.HasMove(Moves.Dive) ? yesString : noString)}");
        _rows[7] = CreateRow("Crouch:", () => $"{(ArchipelagoClient.ArchipelagoData.HasMove(Moves.Crouch) ? yesString : noString)}");
        _rows[8] = CreateRow("Drill:", () => $"{(GameCtrl.instance.data.upgrades[1] ? yesString : noString)}");
        _rows[9] = CreateRow("Fall Warp:", () => $"{(GameCtrl.instance.data.upgrades[2] ? yesString : noString)}");
        _rows[10] = CreateRow("Grater:", () => $"{(GameCtrl.instance.data.items[310] ? yesString : noString)}");
        _rows[11] = CreateRow("Worm:", () => $"{(GameCtrl.instance.data.switches[236] ? yesString : noString)}");
        _rows[12] = CreateRow("Rats:", () => $"{ArchipelagoClient.ArchipelagoData.Rats}/6");
        _rows[13] = CreateRow("Fish:", () => $"{ArchipelagoClient.ArchipelagoData.Fish}/3");
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
        sizeFitter.verticalFit = ContentSizeFitter.FitMode.Unconstrained;
        sizeFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        var text = textObject.AddComponent<Text>();
        text.text = string.Empty;
        text.font = _font;
        text.fontSize = 12;
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
            _rows[..7].ToList().ForEach(x => x?.SetActive(ArchipelagoClient.SlotData.IsMovesanity));
            _rows[12].SetActive(ArchipelagoClient.SlotData.IsRatsanity);
            _rows[13].SetActive(ArchipelagoClient.SlotData.IsFishsanity);
            _bLoaded = true;
        }

        //handle slide-in/-out
        _shouldShow = (Input.GetKey(KeyCode.JoystickButton6) || GameCtrl.instance.bPause) && GameCtrl.instance.currentWorld >= 0;
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