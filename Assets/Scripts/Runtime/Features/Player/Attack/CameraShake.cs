using System.Collections;
using UnityEngine;

public class SimpleCameraShake : MonoBehaviour
{
    [SerializeField] Transform camTransform; // kéo MainCamera vào
    [SerializeField] float defaultDuration = 0.08f;
    [SerializeField] float defaultMagnitude = 0.1f;

    Vector3 originalLocalPos;
    Coroutine co;

    void Awake()
    {
        if (camTransform == null) camTransform = Camera.main.transform;
        originalLocalPos = camTransform.localPosition;
    }

    public void Play(float magnitude =-1, float duration = -1f)
    {
        if (duration <= 0f) duration = defaultDuration;
        if (magnitude <= 0f) magnitude = defaultMagnitude;

        if (co != null) StopCoroutine(co);
        co = StartCoroutine(CoShake(magnitude, duration));
    }

    IEnumerator CoShake(float mag, float dur)
    {
        float t = 0f;
        while (t < dur)
        {
            t += Time.deltaTime;
            Vector2 offset = Random.insideUnitCircle * mag;
            camTransform.localPosition = originalLocalPos + new Vector3(offset.x, offset.y, 0f);
            print($"[CoShake] -> {offset} {mag} {camTransform.localPosition}");
            yield return null;
        }
        camTransform.localPosition = originalLocalPos;
    }
}
