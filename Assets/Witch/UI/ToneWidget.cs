using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ToneWidget : MonoBehaviour
{
    private WitchManager _witchManager;
    [SerializeField] private int _userIndex = 0;

    [SerializeField] private Image _spanProto;
    [SerializeField] private TextMeshProUGUI _labelProto;
    [SerializeField] private RectTransform _container;
    [SerializeField] private Image _marker;

    private RectTransform _markerRectTransform;
    
    private Image[] _spans;
    private TextMeshProUGUI[] _labels;
    private RectTransform[] _labelTransforms;
    

    private static readonly int ArcStart = Shader.PropertyToID("_ArcStart");
    private static readonly int ArcEnd = Shader.PropertyToID("_ArcEnd");
    private static readonly int TintColor = Shader.PropertyToID("_TintColor");
    private static readonly int BorderColor = Shader.PropertyToID("_BorderColor");

    void Start()
    {
        _markerRectTransform = _marker.GetComponent<RectTransform>();
        _witchManager = FindObjectOfType<WitchManager>();

        _spans = new Image[WitchManager.MAXIMUM_SEMITONES];
        for (int i = 0; i < _spans.Length; ++i)
        {
            _spans[i] = Instantiate(_spanProto, _container);
            _spans[i].material = new Material(_spans[i].material);
            _spans[i].enabled = false;
            _spans[i].enabled = false;
        }
        _spanProto.enabled = false;

        _labels = new TextMeshProUGUI[WitchManager.MAXIMUM_SEMITONES];
        _labelTransforms = new RectTransform[WitchManager.MAXIMUM_SEMITONES];
        for (int i = 0; i < _labels.Length; ++i)
        {
            TextMeshProUGUI text = Instantiate(_labelProto, _container);
            _labels[i] = text;
            _labels[i].enabled = false;
            _labelTransforms[i] = text.GetComponent<RectTransform>();
            _labels[i].enabled = false;
        }
        _labelProto.enabled = false;

        _markerRectTransform.SetAsLastSibling();
        _marker.enabled = false;
    }

    void Update()
    {
        WitchUserController controller = _witchManager.GetControllerForUserIndex(_userIndex);
        if (controller != null)
        {
            UpdateFromController(controller);
        }
    }

    void UpdateFromController(WitchUserController controller)
    {
        float labelRadius = 0.75f;
        Vector2 toneHint = controller.InputState.ToneHint;
        _marker.enabled = true;
        
        Vector2 widgetCursorRange = _container.sizeDelta * (labelRadius / 2);
        _markerRectTransform.anchoredPosition = toneHint * widgetCursorRange;

        ToneSelector.DeliverableTone[] tones = controller.Selector.GetCurrentTones();
        
        for (int i = 0; i < tones.Length; ++i)
        {
            _spans[i].enabled = tones[i].active; 
            _labels[i].enabled = tones[i].active;

            bool isActive = (controller.MusicState.activeTone == i); 
            if (tones[i].active)
            {
                _spans[i].material.SetFloat(ArcStart, tones[i].originRad);
                _spans[i].material.SetFloat(ArcEnd, tones[i].finalRad);
                _spans[i].material.SetColor(TintColor, GetTintForTone(tones[i]));
                _spans[i].material.SetColor(BorderColor, GetBorderForTone(tones[i], isActive));

                float radCenter = (tones[i].originRad + tones[i].finalRad) * 0.5f;
                Vector2 labelCenter = new Vector2(Mathf.Cos(radCenter),Mathf.Sin(radCenter)) * labelRadius;
                _labels[i].text = tones[i].text;
                _labelTransforms[i].anchoredPosition = labelCenter * (_container.sizeDelta / 2);
            }
        }
    }

    private Color GetTintForTone(ToneSelector.DeliverableTone tone)
    {
        Color result = Color.white;
        switch (tone.offset)
        {
            case WitchUserController.ToneOffset.InScale: break;
            case WitchUserController.ToneOffset.Natural: result = new Color(0.5f,1.0f,0.5f); break;
            case WitchUserController.ToneOffset.Flat: result = new Color(0.6f,0.6f,1.0f); break;
            case WitchUserController.ToneOffset.DoubleFlat: result = new Color(0.25f,0.25f,1.0f); break;
            case WitchUserController.ToneOffset.Sharp: result = new Color(1.0f,0.6f, 0.6f); break;
            case WitchUserController.ToneOffset.DoubleSharp: result = new Color(1.0f, 0.25f, 0.25f); break;
        }

        switch (tone.viability)
        {
            case WitchUserController.ToneViability.Ideal: result.r *= 1.6f; result.g *= 1.6f; result.b *= 1.6f; break;
            case WitchUserController.ToneViability.Viable: break;
            case WitchUserController.ToneViability.Unusual: result.r *= 0.55f; result.g *= 0.55f; result.b *= 1.55f; break;
        }

        return result;
    }
    private Color GetBorderForTone(ToneSelector.DeliverableTone tone, bool isActive)
    {
        return isActive ? Color.white : Color.black;
    }
}
