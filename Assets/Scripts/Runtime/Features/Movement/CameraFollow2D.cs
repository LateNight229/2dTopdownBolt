using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow2D : MonoBehaviour
{   
    public bool _stopMove;
    public Transform target;
    public float smooth = 12f;

    void LateUpdate()
    {
        if (!target||_stopMove) return;
        Vector3 pos = transform.position;
        Vector3 t = target.position;
        t.z = pos.z;
        transform.position = Vector3.Lerp(pos, t, 1f - Mathf.Exp(-smooth * Time.deltaTime));
    }
}
