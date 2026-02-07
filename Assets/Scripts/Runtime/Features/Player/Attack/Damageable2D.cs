using System.Collections.Generic;
using UnityEngine;

public class Damageable2D : MonoBehaviour
{

    [Header("HP")]
    [SerializeField] int maxHP = 100;
    [SerializeField] float defaultIFrame = 0.1f;

    [SerializeField] StaggerController2D staggerCtrl;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] SpriteFlash2D _spriteFlash;


    int _hp;
    float _invincibleUntil;

    // chống “double apply” theo từng attacker + attackInstanceId (phòng trường hợp nhiều hitbox/bug)
    readonly Dictionary<int, int> _lastAttackIdByAttacker = new Dictionary<int, int>(4);
    [SerializeField] Vector3 popupOffset = new Vector3(0f, 0.6f, 0f);

    public int HP => _hp;

    void Awake()
    {
        _hp = maxHP;
        if (!rb) rb = GetComponent<Rigidbody2D>();
        if(!_spriteFlash) _spriteFlash = GetComponent<SpriteFlash2D>();
    }

    public bool CanTakeHit(in Hitbox2D.HitInfo hit)
    {
        if (Time.time < _invincibleUntil) return false;

        if (hit.Attacker != null)
        {
            int attackerId = hit.Attacker.GetInstanceID();
            if (_lastAttackIdByAttacker.TryGetValue(attackerId, out int last) && last == hit.AttackInstanceId)
                return false;
        }

        return true;
    }

    public void TakeHit(in Hitbox2D.HitInfo hit)
    {
        if (!CanTakeHit(hit)) return;

        _spriteFlash.PlayRed();
        // DamagePopupSpawner.Instance?.Spawn(hit.Damage, transform.position + popupOffset);
        DamagePopupSpawner.Instance?.Spawn(hit.Damage, transform.position, new Vector3(0, 0.8f, 0));


        if (hit.Attacker != null)
            _lastAttackIdByAttacker[hit.Attacker.GetInstanceID()] = hit.AttackInstanceId;

        _hp -= hit.Damage;

        float iframe = hit.IFrame > 0 ? hit.IFrame : defaultIFrame;
        _invincibleUntil = Time.time + iframe;

        // knockback
        ApplyKnockback(hit);

        // stagger
        if (staggerCtrl && hit.StaggerTime > 0f)
            staggerCtrl.Stagger(hit.StaggerTime);

        Debug.Log($"{name} took {hit.Damage}, hp={_hp}");
        
        if (_hp <= 0) Die();
    }

    void ApplyKnockback(in Hitbox2D.HitInfo hit)
    {
        if (!rb || hit.KnockbackImpulse <= 0f || hit.Attacker == null) return;

        Vector2 dir = (Vector2)(transform.position - hit.Attacker.position);
        if (dir.sqrMagnitude < 0.0001f) dir = Vector2.up; // fallback
        dir.Normalize();

        rb.velocity = Vector2.zero; // để lực hất lùi “ăn” rõ
        rb.AddForce(dir * hit.KnockbackImpulse, ForceMode2D.Impulse);
    }

    void Die()
    {
        Debug.Log($"{name} DEAD");
        // TODO: play anim, disable collider, destroy, etc.
    }
}
