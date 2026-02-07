using System.Collections.Generic;
using UnityEngine;

public class DamagePopupSpawner : MonoBehaviour
{
    public static DamagePopupSpawner Instance { get; private set; }

    [SerializeField] DamagePopup prefab;
    [SerializeField] int prewarm = 10;

    readonly Queue<DamagePopup> _pool = new Queue<DamagePopup>();

    void Awake()
    {
        Instance = this;

        for (int i = 0; i < prewarm; i++)
            _pool.Enqueue(Create());
    }

    DamagePopup Create()
    {
        var p = Instantiate(prefab, transform);
        p.gameObject.SetActive(false);
        return p;
    }

    public void Spawn(int amount, Vector3 pos)
    {
        var p = _pool.Count > 0 ? _pool.Dequeue() : Create();
        p.Play(amount, pos);

        // trả về pool sau khi life xong (dư 0.7s cho chắc)
        StartCoroutine(ReturnLater(p, 0.7f));
    }

    System.Collections.IEnumerator ReturnLater(DamagePopup p, float t)
    {
        yield return new WaitForSeconds(t);
        if (p) _pool.Enqueue(p);
    }
}
