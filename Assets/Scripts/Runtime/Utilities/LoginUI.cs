using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginUI : MonoBehaviour
{
    Button _btnPlay;
    public Button _btnGuidance;
    public delegate void CallbackPlayNow();
    public CallbackPlayNow _callbackPlayNow;
    public void Init(CallbackPlayNow callbackPlayNow)
    {   

        _callbackPlayNow = null;
        _callbackPlayNow = callbackPlayNow;

        _btnPlay = transform.Find("bg/play").GetComponent<Button>();
        _btnGuidance = transform.Find("bg/guidance")?.GetComponent<Button>();

        _btnGuidance?.onClick.RemoveAllListeners();
        _btnPlay.onClick.RemoveAllListeners();

        _btnPlay.onClick.AddListener(ClickPlayNow);

    }

    public void Show()
    {
        gameObject.SetActive(true);
        SlashScreenControl.instance.Show(true, SlashScreenControl.instance.Sprites.Length - 1, 1);
    }

    public void ClickPlayNow()
    {
        _callbackPlayNow?.Invoke();
        Hide();
    }

    public void Hide() => gameObject.SetActive(false);
}
