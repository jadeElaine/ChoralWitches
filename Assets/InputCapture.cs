using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NoteTriggers
{
    EXPLICIT_BELOW_OCTAVE=0,
    ACCIDENTAL_DOWN_5,
    ACCIDENTAL_DOWN_4,
    ACCIDENTAL_DOWN_3,
    ACCIDENTAL_DOWN_2,
    ACCIDENTAL_DOWN_1,
    SCALE_DOWN_6,
    SCALE_DOWN_5,
    SCALE_DOWN_4,
    SCALE_DOWN_3,
    SCALE_DOWN_2,
    SCALE_DOWN_1,
    EXPLICIT_UNISON,
    SCALE_UP_1,
    SCALE_UP_2,
    SCALE_UP_3,
    SCALE_UP_4,
    SCALE_UP_5,
    SCALE_UP_6,
    ACCIDENTAL_UP_1,
    ACCIDENTAL_UP_2,
    ACCIDENTAL_UP_3,
    ACCIDENTAL_UP_4,
    ACCIDENTAL_UP_5,
    ACCIDENTAL_UP_6,
    ACCIDENTAL_UP_7,
    EXPLICIT_ABOVE_OCTAVE
}
public class InputCapture : MonoBehaviour
{
    public static readonly Dictionary<KeyCode, NoteTriggers> KeyMap = new Dictionary<KeyCode, NoteTriggers>
    {
        {KeyCode.Tab, NoteTriggers.EXPLICIT_BELOW_OCTAVE},
        {KeyCode.Backslash, NoteTriggers.EXPLICIT_ABOVE_OCTAVE},

        {KeyCode.BackQuote, NoteTriggers.SCALE_DOWN_6},
        {KeyCode.Alpha1, NoteTriggers.SCALE_DOWN_5},
        {KeyCode.Alpha2, NoteTriggers.SCALE_DOWN_4},
        {KeyCode.Alpha3, NoteTriggers.SCALE_DOWN_3},
        {KeyCode.Alpha4, NoteTriggers.SCALE_DOWN_2},
        {KeyCode.Alpha5, NoteTriggers.SCALE_DOWN_1},
        {KeyCode.Alpha6, NoteTriggers.EXPLICIT_UNISON},
        {KeyCode.Alpha7, NoteTriggers.SCALE_UP_1},
        {KeyCode.Alpha8, NoteTriggers.SCALE_UP_2},
        {KeyCode.Alpha9, NoteTriggers.SCALE_UP_3},
        {KeyCode.Alpha0, NoteTriggers.SCALE_UP_4},
        {KeyCode.Minus, NoteTriggers.SCALE_UP_5},
        {KeyCode.Equals, NoteTriggers.SCALE_UP_6},
        
        {KeyCode.Q, NoteTriggers.ACCIDENTAL_DOWN_5},
        {KeyCode.W, NoteTriggers.ACCIDENTAL_DOWN_4},
        {KeyCode.E, NoteTriggers.ACCIDENTAL_DOWN_3},
        {KeyCode.R, NoteTriggers.ACCIDENTAL_DOWN_2},
        {KeyCode.T, NoteTriggers.ACCIDENTAL_DOWN_1},
        {KeyCode.Y, NoteTriggers.ACCIDENTAL_UP_1},
        {KeyCode.U, NoteTriggers.ACCIDENTAL_UP_2},
        {KeyCode.I, NoteTriggers.ACCIDENTAL_UP_3},
        {KeyCode.O, NoteTriggers.ACCIDENTAL_UP_4},
        {KeyCode.P, NoteTriggers.ACCIDENTAL_UP_5},
        {KeyCode.LeftBracket, NoteTriggers.ACCIDENTAL_UP_6},
        {KeyCode.RightBracket, NoteTriggers.ACCIDENTAL_UP_7},

    };

    public delegate void ForwardInput(in NoteTriggers trigger);

    public ForwardInput down;
    public ForwardInput up;
        
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var kvp in KeyMap)
        {
            if (Input.GetKeyDown(kvp.Key)) { down(kvp.Value); }
            if (Input.GetKeyUp(kvp.Key)) { up(kvp.Value); }
        }
    }
}
