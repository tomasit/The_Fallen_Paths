using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class KeyManager : MonoBehaviour
{
    [SerializeField] private bool _isDisplay;
    [SerializeField] private GameObject _keyUI;

    [SerializeField] private TMP_Text _textValueKeys;
    [SerializeField] private uint _nbKeys;
    
    [SerializeField] private Keys _playerKeys;

    private void Update()
    {
        if (!_isDisplay) {
            _keyUI.SetActive(false);
        } else {
            _keyUI.SetActive(true);
        }
        _textValueKeys.text = _playerKeys.GetNbKeys().ToString();
    }

    public void SetNbKey(uint nbKey)
    {
        _nbKeys = nbKey;
    }

    public void Display(bool display)
    {
        _isDisplay = display;
    }
}