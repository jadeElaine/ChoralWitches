using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WitchContextWidget : MonoBehaviour
{
    private WitchManager _witchManager;

    [SerializeField]
    private RectTransform _panel;
    [SerializeField]
    private UnityEngine.UI.Button _contextButtonProto;
    [SerializeField]
    private UnityEngine.UI.Button _keyButtonProto;

    private UnityEngine.UI.Button[] _contextButtons;
    private UnityEngine.UI.Button[] _keyButtons;
    
    void Start()
    {
        _witchManager = FindObjectOfType<WitchManager>();

        _contextButtons = new UnityEngine.UI.Button[_witchManager.ModeDefinitions.Length];

        for (int i = 0; i < _witchManager.ModeDefinitions.Length; ++i)
        {
            MusicalMode mode = _witchManager.ModeDefinitions[i];
            
            UnityEngine.UI.Button button = Instantiate(_contextButtonProto, _panel);
            RectTransform buttonTransform = button.GetComponent<RectTransform>();
            TextMeshProUGUI label = button.GetComponentInChildren<TextMeshProUGUI>();
                
            Vector2 location = new Vector2(
                (0.5f+mode.uiPlacement.x)*2/4 - 1, 
                1-(0.5f+mode.uiPlacement.y)*2/8);
            buttonTransform.anchoredPosition = location * (_panel.sizeDelta/2);
            label.text = mode.name;

            int selectIndex = i;
            button.onClick.AddListener(() => SelectContext(selectIndex));

            _contextButtons[i] = button;
        }
        SetButtonAvailable(_contextButtonProto, false);
        
        _keyButtons = new UnityEngine.UI.Button[WitchManager.MAXIMUM_SEMITONES];
        for (int i = 0; i < WitchManager.MAXIMUM_SEMITONES; ++i)
        {
            UnityEngine.UI.Button button = Instantiate(_keyButtonProto, _panel);
            button.GetComponent<Image>().enabled = false;
            button.GetComponentInChildren<TextMeshProUGUI>().enabled = false;

            int selectIndex = i;
            button.onClick.AddListener(() => SelectKey(selectIndex));

            SetButtonAvailable(button, false);
            _keyButtons[i] = button;
        }
        SetButtonAvailable(_keyButtonProto, false);

        UpdateButtons();
    }

    void UpdateButtons()
    {
        //int contextIndex = _witchManager.CurrentModeIndex;
        //int keyIndex = _witchManager.CurrentKeyIndex;


        for (int i = 0; i < _contextButtons.Length; ++i)
        {
            Image image = _contextButtons[i].GetComponent<Image>();
            if (_witchManager.CurrentModeIndex == i)
            {
                image.color = new Color(0.6f,0.6f,0.6f);
            }
            else
            {
                image.color = Color.white;
            }
        }

        //MusicalMode mode = _witchManager.CurrentMusicalMode;
        MusicalTemperament temperament = _witchManager.CurrentMusicalTemperament;
        int keyIndex = _witchManager.CurrentKeyIndex;
        
        string lastTone = "";
        for (int i = 0; i < _keyButtons.Length; ++i)
        {
            Button button = _keyButtons[i];

            bool available = i < temperament.semitoneCount;
            SetButtonAvailable(button, available);
            if (available)
            {
                RectTransform buttonTransform = button.GetComponent<RectTransform>();
                Image image = button.GetComponent<Image>();
                TextMeshProUGUI text = button.GetComponentInChildren<TextMeshProUGUI>();

                Vector2 location = new Vector2(
                    (0.5f+i) * 2 / (temperament.semitoneCount) - 1, 
                    1-(0.5f+7)*2/8);
                buttonTransform.anchoredPosition = location * (_panel.sizeDelta/2);

                if (keyIndex == i)
                {
                    image.color = new Color(0.6f,0.6f,0.6f);
                }
                else
                {
                    image.color = Color.white;
                }

                if (temperament.rootLetterSpacing[i] != '-')
                {
                    lastTone = temperament.rootLetterSpacing[i].ToString();
                    text.text = lastTone;
                }
                else
                {
                    lastTone += "#";
                    text.text = lastTone;
                }
            }
        }
    }

    void SetButtonAvailable(UnityEngine.UI.Button button, bool available)
    {
        Image image = button.GetComponent<Image>();
        TextMeshProUGUI text = button.GetComponentInChildren<TextMeshProUGUI>();
        button.enabled = available;
        image.enabled = available;
        text.enabled = available;
    }

    void SelectContext(int index)
    {
        Debug.Log($"Selecting Mode... {index}");
        _witchManager.ChangeMode(index);
        UpdateButtons();
    }
    void SelectKey(int index)
    {
        Debug.Log($"Selecting Key... {index}");
        _witchManager.ChangeKey(index);
        UpdateButtons();
    }
}
