using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WitchUIHub : MonoBehaviour
{
    [SerializeField] private GameObject _contextWidget;
    private WitchManager _witchManager;

    void Start()
    {
        _witchManager = FindObjectOfType<WitchManager>();
    }


    void Update()
    {
        bool isOpen = _witchManager.IsMenuOpen();

        if (isOpen != _contextWidget.activeSelf)
        {
            _contextWidget.SetActive(isOpen);
        }
    }
}
