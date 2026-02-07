using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaggerController2D : MonoBehaviour
{
     [Tooltip("Các script cần tắt khi bị stagger (TopdownMotor2D, AI, Attack, ...)")]
    public MonoBehaviour[] disableOnStagger;

    float _until;
    bool _isStaggering;

    public bool IsStaggering => _isStaggering;

    public void Stagger(float seconds)
    {
        if (seconds <= 0f) return;

        _until = Mathf.Max(_until, Time.time + seconds);

        if (!_isStaggering)
        {
            _isStaggering = true;
            SetEnabled(false);
        }
    }

    void Update()
    {
        if (_isStaggering && Time.time >= _until)
        {
            _isStaggering = false;
            SetEnabled(true);
        }
    }

    void SetEnabled(bool on)
    {
        if (disableOnStagger == null) return;
        foreach (var mb in disableOnStagger)
            if (mb) mb.enabled = on;
    }
}
