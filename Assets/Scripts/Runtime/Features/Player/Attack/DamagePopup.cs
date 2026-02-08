using System.Collections;
using TMPro;
using UnityEngine;

public class DamagePopup : MonoBehaviour
{
    [SerializeField] TMP_Text text;
    [SerializeField] float life = 1f;
    [SerializeField] float rise = 2f;

    Color _startColor;
    Vector3 _startPos;

    void Awake()
    {
        if (!text) text = GetComponent<TMP_Text>();
        _startColor = text.color;

        MeshRenderer mr = text.GetComponent<MeshRenderer>();
        if (mr != null)
        {
            mr.sortingLayerName = "UI";   // hoặc "Foreground" tùy bạn có layer nào
            mr.sortingOrder = 1000;       // số càng cao càng nổi
        }
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
            c.a = Mathf.Lerp(1f, 0.6f, k);
            text.color = c;

            yield return null;
        }
        gameObject.SetActive(false);
    }
}
