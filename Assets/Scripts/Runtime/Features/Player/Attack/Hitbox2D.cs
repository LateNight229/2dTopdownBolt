using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox2D : MonoBehaviour
{
    public readonly struct HitInfo
    {
        public readonly Transform Attacker;
        public readonly int AttackInstanceId;
        public readonly int Damage;
        public readonly float IFrame;

        public readonly float KnockbackImpulse; // lực hất lùi (Impulse)
        public readonly float StaggerTime;      // thời gian choáng/khựng

        public HitInfo(Transform attacker, int attackInstanceId, int damage, float iFrame, float knockbackImpulse, float staggerTime)
        {
            Attacker = attacker;
            AttackInstanceId = attackInstanceId;
            Damage = damage;
            IFrame = iFrame;
            KnockbackImpulse = knockbackImpulse;
            StaggerTime = staggerTime;
        }
    }


    [Header("Shape")]
    [SerializeField] Transform origin;
    [SerializeField] Vector2 size = new Vector2(1.2f, 0.8f);
    [SerializeField] LayerMask hitMask;
    [SerializeField] int bufferSize = 16;

    Collider2D[] _buffer;
    readonly HashSet<Damageable2D> _hitThisSwing = new HashSet<Damageable2D>(16);

    bool _active;
    HitInfo _hit;
    PlayerBehavior _playerBehavior;

    public IEnumerator Init(PlayerBehavior playerBehavior)
    {   
        _playerBehavior = playerBehavior;
        if (origin == null) origin = transform;
        _buffer = new Collider2D[Mathf.Max(4, bufferSize)];
        yield return null;
    }

    public void BeginSwing(in HitInfo hit)
    {
        _active = true;
        _hit = hit;
        _hitThisSwing.Clear();
    }

    public void EndSwing()
    {
        _active = false;
    }

    void Update()
    {
        if (!_active) return;

        int count = Physics2D.OverlapBoxNonAlloc(
            origin.position,
            size,
            origin.eulerAngles.z,
            _buffer,
            hitMask
        );

        for (int i = 0; i < count; i++)
        {
            Collider2D col = _buffer[i];
            if (!col) continue;

            Hurtbox2D hurtbox = col.GetComponent<Hurtbox2D>();
            if (!hurtbox || !hurtbox.Owner) continue;

            Damageable2D dmg = hurtbox.Owner;

            // anti multi-hit: cùng 1 swing chỉ ăn 1 lần / 1 enemy (dù enemy có nhiều collider)
            if (!_hitThisSwing.Add(dmg)) continue;

            dmg.TakeHit(_hit);
            _playerBehavior.gameState._simpleCameraShake.Play();
        }
    }

    void OnDrawGizmosSelected()
    {
        if (!origin) return;
        Gizmos.color = Color.red;
        Gizmos.matrix = Matrix4x4.TRS(origin.position, origin.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, size);
    }
}
