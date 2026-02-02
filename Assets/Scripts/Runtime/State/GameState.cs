using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameState : MonoBehaviour, GameStateMachine
{
    [Header("serialize")]
    public GameObject UIObject;
    Main MainScript;
    void Awake()
    {
        Main.LoadState(this, 0, false);
        UIObject = GameObject.Find("Game");
    }
    public IEnumerator Load(Main _MainScript)
    {
        MainScript = _MainScript;
        yield return null;
    }
    public void UpdateState()
    {
    }
    public void LateUpdateState() { }
    public void FixedUpdateState() { }
    public void StartInput(Interaction.Data _Data) { }
    public void UpdateInputMove(Interaction.Data _Data) { }
    public void EndInput(Interaction.Data _Data) { }
    public void UpdateInputStationary(Interaction.Data _Data) { }
    public void DoChangeState(System.Type _Type, ChangeStateEffect.EffectType _EffectType, float _TimeChange)
        => MainScript.StartChangeState(MainScript.GetState(_Type), _EffectType, _TimeChange);
    public void Enable()
    {
        gameObject.SetActive(true);
        if (UIObject != null)
            UIObject.SetActive(true);
        SoundManager.Instance.PlayMusic(Config.Music.MUSIC_2);
    }
    public void Disable()
    {
        gameObject.SetActive(false);
        if (UIObject != null)
            UIObject.SetActive(false);
    }
    public void ShutDown() { }
    public void StartChangeState(GameStateMachine _NextState, GameStateMachine _LastState)
    {
        if (_NextState.GetType() == GetType())
            Enable();
    }
    public void UpdateChangeState(GameStateMachine _NextState, GameStateMachine _LastState, float _Time) { }
    public void SetChangeState(GameStateMachine _NextState, GameStateMachine _LastState) { }
    public void EndChangeState(GameStateMachine _NextState, GameStateMachine _LastState)
    {
        if (_NextState.GetType() != GetType())
        {
            Disable();
        }
    }

    public void UpdateUIPosition(Vector3 localPosition)
    {
        UIObject.transform.localPosition = localPosition;
    }
    public void UpdateUISortingOder()
    {
        //UIObject.transform.SetAsLastSibling();
    }

    SettingUI _settingUI;

    public IEnumerator Init()
    {
        SetFrameApp();
        yield return null;
    }

    void SetFrameApp()
    {   
        Application.targetFrameRate = 60;
        Time.fixedDeltaTime = 1 / 50f;
        Application.runInBackground = true;
    }


    public void CallbackContinue()
    {
        print("CallbackContinue");
    }

    public void CallbackPlayAgain()
    {
    }

    public void CallbackBacktoMenu()
    {
        print("CallbackBacktoMenu");
        DoChangeState(typeof(LoadingState), ChangeStateEffect.EffectType.None, 0);
    }

    public void CallbackOpenSetting()
    {
        print("CallbackOpenSetting");
        _settingUI.Show();

    }

    public static bool IsPointerOverUIObject()
    {
        List<RaycastResult> raycastResults = new List<RaycastResult>();
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        EventSystem.current.RaycastAll(eventData, raycastResults);
        return raycastResults.Count > 0;
    }

}
