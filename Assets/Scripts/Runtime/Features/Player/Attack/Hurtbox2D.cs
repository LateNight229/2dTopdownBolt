using UnityEngine;

public class Hurtbox2D : MonoBehaviour
{
    [SerializeField] Damageable2D owner;
    public Damageable2D Owner => owner;

    void Awake()
    {
        if (owner == null) owner = GetComponentInParent<Damageable2D>();
    }
}
