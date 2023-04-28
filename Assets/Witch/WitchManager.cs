using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WitchManager : MonoBehaviour
{
    public const int MAXIMUM_SEMITONES = 24;
    
    public struct WitchMusicContext
    {
        public MusicalTemperament temperament;
        public MusicalMode mode;
        public int keyIndex;
        public bool preferFlatOverSharp;
    }
    
    [SerializeField] private ContentMusicalDomain _domain;
    [SerializeField] private WitchPlayerAvatar[] _initialAvatars;
    
    //private List<WitchPlayerAvatar> _witches;
    private WitchMusicController _musicController;
    private WitchControllerSchemeInputParser _schemeInputParser;
    
    private List<WitchUserController> _userControllers;

    private WitchExperienceController _experienceController;

    
    public MusicalTemperament[] TemperamentDefinitions => _domain.temperaments;
    public MusicalMode[] ModeDefinitions => _domain.modes;

    private int _currentModeIndex;
    public int CurrentModeIndex => _currentModeIndex;
    private int _currentKeyIndex;
    public int CurrentKeyIndex => _currentKeyIndex;

    public MusicalMode CurrentMusicalMode =>
        _domain.modes[_currentModeIndex];
    public int CurrentTemperamentIndex => CurrentMusicalMode.tempermentIndex;
    public MusicalTemperament CurrentMusicalTemperament =>
        _domain.temperaments[CurrentTemperamentIndex];

    public WitchUserController GetControllerForUserIndex(int userIndex)
    {
        if (userIndex < 0 || userIndex > _userControllers.Count)
        {
            return null;
        }
        
        return _userControllers[userIndex];
    }

    private void Awake()
    {
        MusicalDomainContentValidator.Validate(_domain);

        _currentModeIndex = 0;
        for (int i = 0; i < _domain.modes.Length; ++i)
        {
            if (_domain.modes[i].name == _domain.defaultMode)
            {
                _currentModeIndex = i;
                Debug.Log($"Selecting Initial Mode {i} ({_domain.defaultMode})");
            }
        }

        _currentKeyIndex = 0;
        
        _musicController = new WitchMusicController();
        _experienceController = new WitchExperienceController();
        _userControllers = new List<WitchUserController>();
    }

    void Start()
    {
        for (int i = 0; i < _initialAvatars.Length; ++i)
        {
            //_witches.Add(_initialAvatars[0]);
            WitchUserController userController = new WitchUserController(_musicController);
            userController.AssignAvatar(_initialAvatars[i]);
            _userControllers.Add(userController);

            userController.ChangeContext(BuildMusicalContext());
        }
        
        _schemeInputParser = new WitchControllerSchemeInputParser();
        _schemeInputParser.ConnectUserControllers(_userControllers, _experienceController);
    }

    void Update()
    {
        _schemeInputParser.Update();
        
        _experienceController.Update();

        for (int i = 0; i < _userControllers.Count; ++i)
        {
            _userControllers[i].Update();
        }
    }

    public bool IsMenuOpen()
    {
        return _experienceController.MenuOpen;
    }

    public void ChangeMode(in int index)
    {
        if (_domain.modes[index].tempermentIndex != _domain.modes[_currentModeIndex].tempermentIndex)
        {
            _currentKeyIndex = 0;
        }
        
        _currentModeIndex = index;
        WitchMusicContext context = BuildMusicalContext();

        _musicController.ChangeContext(context);
        for (int i = 0; i < _userControllers.Count; ++i)
        {
            _userControllers[i].ChangeContext(context);
        }
    }

    public void ChangeKey(in int index)
    {
        _currentKeyIndex = index;

        WitchMusicContext context = BuildMusicalContext();
        _musicController.ChangeContext(context);
        for (int i = 0; i < _userControllers.Count; ++i)
        {
            _userControllers[i].ChangeContext(context);
        }
    }

    private WitchMusicContext BuildMusicalContext()
    {
        return new WitchMusicContext()
        {
            temperament = CurrentMusicalTemperament,
            mode = CurrentMusicalMode,
            keyIndex = CurrentKeyIndex,
            preferFlatOverSharp = false, // @TODO: figure this shit out
        };
    }
}
