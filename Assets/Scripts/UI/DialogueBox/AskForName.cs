using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class AskForName : MonoBehaviour
{
    [SerializeField] private TMP_Text _name;
    [SerializeField] private TMP_Text _count;

    public void SaveName()
    {
        string name = _name.text;
        name = name.Replace("\u200B", "");
        if (name.Contains("thomas", StringComparison.OrdinalIgnoreCase))
            name = name.Replace("thomas", "JusDeTomate-$dev$", StringComparison.OrdinalIgnoreCase);
        else if (name.Contains("edouard", StringComparison.OrdinalIgnoreCase))
            name = name.Replace("edouard", "\"Ca sort ce soir?\"-$dev$", StringComparison.OrdinalIgnoreCase);
        else if (name.Contains("gabriel", StringComparison.OrdinalIgnoreCase))
            name = name.Replace("gabriel", "DongDong-$dev$", StringComparison.OrdinalIgnoreCase);
        else if (name.Contains("nicolas", StringComparison.OrdinalIgnoreCase))
            name = name.Replace("nicolas", "UIMaster-$dev$", StringComparison.OrdinalIgnoreCase);
        SaveManager.DataInstance.GetPlayerInfo()._playerName = name;
    }

    private void Update()
    {
        _count.text = (_name.text.Length - 1) + "/10";
    }
}
