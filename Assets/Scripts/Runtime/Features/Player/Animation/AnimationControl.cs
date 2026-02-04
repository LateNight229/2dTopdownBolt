using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationControl : MonoBehaviour
{
    public const string Idle = "Idle";
    public const string Walk = "Walk";
    public const string Run = "Run";
    public const string Attack1 = "Attack1";
    public const string Attack2 = "Attack2";
    public const string Attack3 = "Attack3";
    public const string Q = "Q";
    public const string E = "E";

    public Animator animator;
    [SerializeField] string current_name;
    public string[] clip = new string[] { Idle, Walk, Run, Attack1, Attack2, Attack3, Q, E };

    public delegate void CallBack(int value);
    public CallBack callBack;
    public delegate void CallBackSound(string value);
    public CallBackSound callBackSound;

    public int UpdateMovement(Vector3 move)
    {
        float speed = move.magnitude;
        if (speed > Mathf.Epsilon)
        {
            return Play(speed >= 0.65f ? "Run" : "Run", 0.25f);
        }
        else
        {
            return Play("Idle", 0.25f);
        }
    }

    public int Play(string name, float fade)
    {
        print($"{name}");
        if (current_name != name)
        {
            animator.CrossFade(name, fade);
            current_name = name;
            return GetClipIndex(name);
        }
        else return -1;
    }
    int GetClipIndex(string name)
    {
        for (int i = 0; i < clip.Length; i++)
        {
            if (clip[i] == name) return i;
        }
        return -1;
    }

    #region Set

    public void UpdateAction(int id)
    {
        Play(clip[id], 0.25f);
    }

    #endregion

    public void AddEvent(string clip_name, int value, float[] percents)
    {
        AnimationClip animationClip = FindAnimation(clip_name);
        print($"anim");
        AnimationEvent _aEvents = new()
        {
            intParameter = value,
            functionName = "AnimationEnd",
            time = animationClip.length,
        };
        animationClip.AddEvent(_aEvents);

        foreach (float percent in percents)
        {
            if (percent >= 0 && percent <= 1)
            {
                AnimationEvent _bEvents = new()
                {
                    stringParameter = clip_name,
                    functionName = "PlaySound",
                    time = animationClip.length * percent,
                };
                animationClip.AddEvent(_bEvents);
            }
        }
    }
    public void AnimationEnd(int value)
    {
        callBack?.Invoke(value);
    }
    public void PlaySound(string value)
    {
        callBackSound?.Invoke(value);
    }

    public AnimationClip FindAnimation(string name)
    {
        foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == name)
            {
                return clip;
            }
        }
        return null;
    }


}
