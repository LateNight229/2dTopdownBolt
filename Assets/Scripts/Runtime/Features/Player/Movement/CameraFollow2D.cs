using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow2D : MonoBehaviour
{
    [SerializeField] bool _stopMove = false;
    public Transform target;
    [SerializeField] float smooth = 1f;
    [SerializeField] float _defaultSize = 8f;

    Camera _cam;
    
    void Start()
    {   
        _cam = GetComponentInChildren<Camera>();
        UpdateSize(_defaultSize);
    }

    void LateUpdate()
    {
        if (!target || _stopMove) return;
        Vector3 pos = transform.position;
        Vector3 t = target.position;
        t.z = pos.z;
        transform.position = Vector3.Lerp(pos, t, 1f - Mathf.Exp(-smooth * Time.deltaTime));
    }
    public void UpdateSize(float size)
    {
        _cam.orthographicSize = size;
    }

    public void UpdateTarget(Transform pos) => target.position = pos.position;

    
}
