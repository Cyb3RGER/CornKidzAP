using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using APColor = Archipelago.MultiClient.Net.Models.Color;

namespace CornKidzAP.Archipelago;

public class APNotificationUI : MonoBehaviour
{
    private readonly Queue<IAPNotification> _notifications = new();
    private IAPNotification _currentItemNotification;
    private bool bDisplaying => _currentItemNotification != null;
    private Canvas _canvas;
    private Text _text;
    private RectTransform _rectTransform;
    private Font _font;
    private Vector3 _startPosition = new(0, 270, 0);
    private Vector3 _targetPosition = new(0, 170, 0);
    private float _slideDuration = 0.5f;
    private float _displayDuration = 2f;
    private Image _background;

    public void AddNotification(IAPNotification itemNotification)
    {
        _notifications.Enqueue(itemNotification);
    }
    
    private void Awake()
    {
        _font = UI.instance.transform.Find("Objective").GetComponent<Text>().font;
        var bgSprite = UI.instance.transform.Find("TextBox").GetComponent<Image>().sprite;
        var bgColor = UI.instance.transform.Find("TextBox").GetComponent<Image>().color;
        var bgMat = UI.instance.transform.Find("TextBox").GetComponent<Image>().material;
        var textMat = UI.instance.transform.Find("TextBox/TextBox String").GetComponent<Text>().material;
        // Create the Canvas
        _canvas = GetComponent<Canvas>();
        
        // Create the background GameObject
        var backgroundObj = new GameObject("Background");
        backgroundObj.transform.SetParent(_canvas.transform);
        backgroundObj.layer = LayerMask.NameToLayer("UI");
        _background = backgroundObj.AddComponent<Image>();
        _background.sprite = bgSprite;
        _background.color = bgColor;
        _background.material = bgMat;
        _background.type = Image.Type.Sliced;
        _background.canvasRenderer.cullTransparentMesh = false;
        // _background.isMaskingGraphic = true;
        // var mask = backgroundObj.AddComponent<Mask>();
        
        var textObj = new GameObject("NotificationText");
        textObj.transform.SetParent(_background.gameObject.transform);
        textObj.layer = LayerMask.NameToLayer("UI");
        _text = textObj.AddComponent<Text>();
        _text.text = string.Empty;
        _text.font = _font;
        _text.material = new Material(_font.material)
        {
            shader = Shader.Find("UI/Default Font FRONT"),
            renderQueue = 3003,
        };
        _text.fontSize = 16;
        _text.color = Color.white;
        _text.supportRichText = true;
        _text.alignment = TextAnchor.UpperLeft;
        _text.canvasRenderer.cullTransparentMesh = false;
        
        _rectTransform = _background.GetComponent<RectTransform>();
        _rectTransform.sizeDelta = new Vector2(300, 50);
        _rectTransform.localPosition = _startPosition; // Start position (off-screen)
        _rectTransform.localRotation = Quaternion.identity;
        _rectTransform.localScale = new Vector3(1.87f, 1.87f, 1.87f);
        
        var textRectTransform = _text.GetComponent<RectTransform>();
        textRectTransform.sizeDelta = _rectTransform.sizeDelta - new Vector2(10, 5); // Add padding from the background
        textRectTransform.localPosition = new Vector3(0, -2f, 0);
        textRectTransform.localRotation = Quaternion.identity;
        textRectTransform.localScale = new Vector3(1, 1, 1);
        
        CornKidzAP.Logger.LogDebug($"font: {_font} {_font == null}");
    }
    private void Update()
    {
        if (_notifications.Count > 0 && !bDisplaying)
        {
            Display();
        }
    }

    private void Display()
    {
        _currentItemNotification = _notifications.Dequeue();
        _text.text = _currentItemNotification.Text;
        StartCoroutine(SlideInAndOut(_rectTransform));
    }

    private void Hide()
    {
        _text.text = string.Empty;
        _currentItemNotification = null;
    }
    
    IEnumerator SlideInAndOut(RectTransform rectTransform)
    {
        // Slide in to target position
        yield return UIUtils.SlideRect(rectTransform, _startPosition, _targetPosition, _slideDuration);

        // Wait for display duration
        yield return new WaitForSeconds(_displayDuration);

        // Slide out off-screen
        yield return UIUtils.SlideRect(rectTransform, _targetPosition, _startPosition, _slideDuration);
        Hide();
    }


    public void Test()
    {
        var color1 =  APColor.Plum.APColorToHex();
        var color2 = APColor.Magenta.APColorToHex();
        var color3 = APColor.Green.APColorToHex();
        _text.text = $"<color=#{color1}>This</color> <color=#{color2}>is a</color> <color=#{color3}>test</color>";
        StartCoroutine(SlideInAndOut(_rectTransform));
    }
}