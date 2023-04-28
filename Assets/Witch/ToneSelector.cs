using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToneSelector
{
    
    public struct ToneState
    {
        public float desiredWeight;
        public float currentWeight;
        public float sizeRad;
    }
    
    public struct DeliverableTone
    {
        public bool active;
        public float originRad;
        public float finalRad;

        public string text;
        public WitchUserController.ToneViability viability;
        public WitchUserController.ToneOffset offset;
    }

    private ToneState[] _tones;
    private DeliverableTone[] _deliverableTones;

    public ToneSelector(int semitones)
    {
        _tones = new ToneState [semitones];
        _deliverableTones = new DeliverableTone [semitones];
    }

    public void Update()
    {
        float totalWeight = 0.0f;
        for (int i = 0; i < _tones.Length; ++i)
        {
            _tones[i].currentWeight = (_tones[i].currentWeight + _tones[i].desiredWeight) / 2;
            if (_tones[i].currentWeight < 0.01f && _tones[i].desiredWeight <= 0.01f)
            {
                _tones[i].currentWeight = 0;
            }

            totalWeight += _tones[i].currentWeight;
        }

        float radsPerWeight = Mathf.PI * 2 / totalWeight;
        for (int i = 0; i < _tones.Length; ++i)
        {
            _tones[i].sizeRad = radsPerWeight * _tones[i].currentWeight;
        }

        float spacer = 0.02f;
        float originRad = (Mathf.PI / 2) + (_tones[0].sizeRad / 2);
        for (int i = 0; i < _deliverableTones.Length; ++i)
        {
            bool active = _tones[i].currentWeight > 0.0f;
            _deliverableTones[i].active = active;
            float lowerBoundRad = originRad - _tones[i].sizeRad;
            _deliverableTones[i].originRad = lowerBoundRad + spacer;
            _deliverableTones[i].finalRad = originRad - spacer;
            originRad = lowerBoundRad;
        }
    }

    public void ChangeDesiredTones(WitchUserController.ModalTone[] inputTone, bool immediate)
    {
        for (int i = 0; i < _tones.Length; ++i)
        {
            if (i < inputTone.Length && inputTone[i].weight > 0)
            {
                _tones[i].desiredWeight = inputTone[i].weight;
                if (immediate)
                {
                    _tones[i].currentWeight = inputTone[i].weight;
                }
                _deliverableTones[i].text = inputTone[i].text;
                _deliverableTones[i].viability = inputTone[i].viability;
                _deliverableTones[i].offset = inputTone[i].offset;
            }
            else
            {
                _tones[i].desiredWeight = 0;
                _deliverableTones[i].text = "";
                _deliverableTones[i].viability = WitchUserController.ToneViability.Unusual;
            }
        }
    }

    public DeliverableTone[] GetCurrentTones()
    {
        return _deliverableTones;
    }
}
