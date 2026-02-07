using System.Collections;
using TMPro;
using UnityEngine;

public class DamagePopup : MonoBehaviour
{
    [SerializeField] TMP_Text text;
    [SerializeField] float life = 0.6f;
    [SerializeField] float rise = 0.8f;

    Color _startColor;
    Vector3 _startPos;

    void Awake()
    {
        if (!text) text = GetComponentInChildren<TMP_Text>();
        _startColor = text.color;
    }

    public void Play(int amount, Vector3 worldPos)
    {
        gameObject.SetActive(true);
        text.text = amount.ToString();
        text.color = _startColor;

        _startPos = worldPos;
        transform.position = _startPos;

        StopAllCoroutines();
        StartCoroutine(Co());
    }

    IEnumerator Co()
    {
        float t = 0f;
        while (t < life)
        {
            t += Time.deltaTime;
            float k = t / life;

            transform.position = _startPos + Vector3.up * (rise * k);

            var c = text.color;
            c.a = Mathf.Lerp(1f, 0f, k);
            text.color = c;

            yield return null;
        }
        gameObject.SetActive(false);
    }
}
