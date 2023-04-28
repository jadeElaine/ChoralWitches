using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WitchControllerSchemeInputParser
{
    private WitchExperienceController experienceTarget;
    private WitchUserController[] userTargets;

    private struct DoubleTapState
    {
        public float currentHit;
        public float previousHit;
        public bool active;
    }

    private DoubleTapState[] _doubleTaps;

    public static readonly float DOUBLE_TAP_THRESHOLD = 0.25f;

    public void ConnectUserControllers(List<WitchUserController> controllers, WitchExperienceController experience)
    {
        experienceTarget = experience;
        userTargets = new WitchUserController[controllers.Count];
        for (int i = 0; i < controllers.Count; ++i)
        {
            userTargets[i] = controllers[i];
        }

        _doubleTaps = new DoubleTapState[controllers.Count * 2];
    }

    public void Update()
    {
        WitchExperienceController.UserInputState experienceState = new WitchExperienceController.UserInputState();
        experienceState.Menu = false;
        experienceState.Submit = false;
        experienceState.Cancel = false;
        
        for (int i = 0; i < userTargets.Length; ++i)
        {
            string indexString = (i + 1).ToString();
            
            
            if (Input.GetButtonDown($"Raise{indexString}"))
            {
                int index = i * 2 + 0;
                _doubleTaps[index].previousHit = _doubleTaps[index].currentHit;
                _doubleTaps[index].currentHit = Time.time;
                _doubleTaps[index].active = 
                    (_doubleTaps[index].currentHit - _doubleTaps[index].previousHit) < DOUBLE_TAP_THRESHOLD;
            }
            if (Input.GetButtonDown($"Alternate{indexString}"))
            {
                int index = i * 2 + 1;
                _doubleTaps[index].previousHit = _doubleTaps[index].currentHit;
                _doubleTaps[index].currentHit = Time.time;
                _doubleTaps[index].active = 
                    (_doubleTaps[index].currentHit - _doubleTaps[index].previousHit) < DOUBLE_TAP_THRESHOLD;
            }

            
            WitchUserController.UserInputState state = new WitchUserController.UserInputState();
            state.Move = new Vector2(
                Input.GetAxis($"MoveX{indexString}"),
                Input.GetAxis($"MoveY{indexString}"));

            state.Walk = Input.GetButton($"Walk{indexString}");
            state.Dash = Input.GetButton($"Dash{indexString}");
            state.Jump = Input.GetButton($"Jump{indexString}");
            
            state.Interact = Input.GetButton($"Interact{indexString}");
            
            state.ToneHint = new Vector2(
                Input.GetAxis($"ToneX{indexString}"),
                Input.GetAxis($"ToneY{indexString}"));
            state.ToneExplicit = -1;

            state.RaiseNote = Input.GetButton($"Raise{indexString}");
            state.LowerNote = Input.GetButton($"Lower{indexString}");
            state.Alternate = Input.GetButton($"Alternate{indexString}");
            state.Stomp = Input.GetButton($"Stomp{indexString}");
            
            state.Chorus = Input.GetButton($"Chorus{indexString}");
            state.Major = state.RaiseNote;
            state.DoubleMajor = state.Major && _doubleTaps[i * 2 + 0].active;
            state.Minor = state.LowerNote;
            state.Seventh = state.Alternate;
            state.DoubleSeventh = state.Major && _doubleTaps[i * 2 + 1].active;
            state.Ninth = state.Stomp;
            state.Hold = Input.GetButton($"Hold{indexString}");
            state.Modulate = Input.GetButton($"Modulate{indexString}");
            
            experienceState.Menu |= Input.GetButtonDown($"Menu{indexString}");

            if (i == 0)
            {
                state.Move += new Vector2(
                    Input.GetAxis("MoveKX1"),
                    Input.GetAxis("MoveKY1"));

                if (Input.GetButton("ToneKI"))
                {
                    state.ToneExplicit = 0;
                }

                if (Input.GetButton("ToneKII"))
                {
                    state.ToneExplicit = 1;
                }
                
                if (Input.GetButton("ToneKIII"))
                {
                    state.ToneExplicit = 2;
                }
                
                if (Input.GetButton("ToneKIV"))
                {
                    state.ToneExplicit = 3;
                }
                
                if (Input.GetButton("ToneKV"))
                {
                    state.ToneExplicit = 4;
                }
                
                if (Input.GetButton("ToneKVI"))
                {
                    state.ToneExplicit = 5;
                }

                if (Input.GetButton("ToneKVII"))
                {
                    state.ToneExplicit = 6;
                }
                
                experienceState.Submit = Input.GetButtonDown($"Submit");
                experienceState.Cancel = Input.GetButtonDown($"Cancel");
            }

            userTargets[i].OverrideInputState(state);
        }

        experienceTarget.OverrideInputState(experienceState); 
    }
}