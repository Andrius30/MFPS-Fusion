using System;
using TMPro;
using UnityEngine;

public class LoadingScreen : MonoBehaviour
{
    public static Action<bool,string> onShowLoadingScreen;

    [SerializeField] TextMeshProUGUI infoText;
    [SerializeField] GameObject background;

    public void SetText(string txt) => infoText.text = txt;

    void ToggleLoadingSceen(bool enable, string text)
    {
        background.SetActive(enable);
        SetText(text);
    }

    void OnEnable() => onShowLoadingScreen += ToggleLoadingSceen;
    void OnDisable() => onShowLoadingScreen -= ToggleLoadingSceen;
}
