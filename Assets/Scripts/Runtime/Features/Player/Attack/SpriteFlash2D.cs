using System.Collections;
using UnityEngine;

public class SpriteFlash2D : MonoBehaviour
{
    [SerializeField] SpriteRenderer[] renderers;
    [SerializeField] Color flashColor = Color.white;
    [SerializeField] float flashDuration = 0.06f;
    [SerializeField] int flashes = 2;
    [SerializeField] float interval = 0.04f;

    Color[] _original;
    Coroutine _co;
    PlayerBehavior _playerBehavior;

    void Awake()
    {
        StartCoroutine(Init());
    }

    public IEnumerator Init(/* PlayerBehavior playerBehavior */)
    {   
        // _playerBehavior = playerBehavior;
        if (renderers == null || renderers.Length == 0)
            renderers = GetComponentsInChildren<SpriteRenderer>();

        _original = new Color[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
            _original[i] = renderers[i] ? renderers[i].color : Color.white;
        yield return null;
    }

    public void PlayWhite() => Play(Color.white);

    public void PlayRed() => Play(new Color(1f, 0.25f, 0.25f, 1f));

    public void Play(Color color)
    {
        if (_co != null) StopCoroutine(_co);
        _co = StartCoroutine(CoFlash(color));
    }

    IEnumerator CoFlash(Color c)
    {
        for (int n = 0; n < flashes; n++)
        {
            SetColor(c);
            yield return new WaitForSeconds(flashDuration);

            Restore();
            yield return new WaitForSeconds(interval);
        }
        Restore();
        _co = null;
    }

    void SetColor(Color c)
    {
        for (int i = 0; i < renderers.Length; i++)
            if (renderers[i]) renderers[i].color = c;
    }

    void Restore()
    {
        for (int i = 0; i < renderers.Length; i++)
            if (renderers[i]) renderers[i].color = _original[i];
    }
}
