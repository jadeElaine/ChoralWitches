using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WitchExperienceController
{
    public struct UserInputState
    {
        public bool Menu;

        public bool Submit;
        public bool Cancel;
    }

    private UserInputState _inputState;
    public UserInputState InputState => _inputState;

    private bool _menuOpen = false;
    public bool MenuOpen => _menuOpen;
    
    public void OverrideInputState(UserInputState inputState)
    {
        _inputState = inputState;
    }

    public void Update()
    {
        if (_inputState.Menu)
        {
            _menuOpen = !_menuOpen;
        }
    }
    
}
