using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MeleeComboAttack : MonoBehaviour, IAttackModule
{
    [Header("Combo")]
    [SerializeField] int maxCombo = 3;
    [SerializeField] float comboResetTime = 0.5f;

    [Header("Timing")]
    [SerializeField] float windupTime = 0.05f;
    [SerializeField] float activeTime = 0.12f;   // ⭐ hitbox bật
    [SerializeField] float recoveryTime = 0.15f;

    [Header("Hitbox")]
    [SerializeField] Transform hitboxOrigin;
    [SerializeField] Vector2 hitboxSize = new Vector2(1.2f, 0.8f);
    [SerializeField] LayerMask hitMask;

    [Header("Damage")]
    [SerializeField] int baseDamage = 10;

    [Header("Animation")]
    [SerializeField] Animator _animator;

    PlayerBehavior _playerBehavior;
    int comboIndex = 0;
    float lastAttackTime;
    bool isAttacking;
    bool hitboxActive;

    HashSet<Collider2D> hitTargets = new HashSet<Collider2D>();

    public void Init(PlayerBehavior owner)
    {
        _playerBehavior = owner;
        // đăng ký callback animation end
        _playerBehavior.animationControl.callBack += OnAnimationEnd;
        for (int i = 0;i < _playerBehavior.animationControl.animator.runtimeAnimatorController.animationClips.Length;i++)
        {
            string clip_name = _playerBehavior.animationControl.animator.runtimeAnimatorController.animationClips[i].name; 
            switch (clip_name)
            {
                case AnimationControl.Attack1  :
                _playerBehavior.animationControl.AddEvent(clip_name, 0, null);
                break;
                case AnimationControl.Attack2  :
                _playerBehavior.animationControl.AddEvent(clip_name, 0, null);
                break;
                case AnimationControl.Attack3  :
                _playerBehavior.animationControl.AddEvent(clip_name, 0, null);
                break;
            }
        }
    }

    public void Tick()
    {
        if (comboIndex > 0 && Time.time - lastAttackTime > comboResetTime)
            comboIndex = 0;

        if (hitboxActive)
        {
            DoHit(comboIndex);
        }
    }

    public bool TryAttack()
    {
        if (isAttacking) return false;

        comboIndex++;
        if (comboIndex > maxCombo)
            comboIndex = 1;

        /*  print(comboIndex);
         _playerBehavior.animationControl.Play($"Attack{comboIndex}", 0.25f);

         lastAttackTime = Time.time;
         StartCoroutine(AttackRoutine(comboIndex)); */
        isAttacking = true;
        lastAttackTime = Time.time;

        _playerBehavior.animationControl.Play($"Attack{comboIndex}", 0.1f);
        return true;
    }

    IEnumerator AttackRoutine(int hitIndex)
    {
        isAttacking = true;
        hitTargets.Clear();

        // 1️⃣ Windup
        yield return new WaitForSeconds(windupTime);

        // 2️⃣ Active – bật hitbox
        float timer = 0f;
        while (timer < activeTime)
        {
            DoHit(hitIndex);
            timer += Time.deltaTime;

            yield return null;
        }

        // 3️⃣ Recovery
        yield return new WaitForSeconds(recoveryTime);

        isAttacking = false;
    }

    void OnDrawGizmosSelected()
    {
        if (!hitboxOrigin) return;
        Gizmos.color = Color.red;
        Gizmos.matrix = Matrix4x4.TRS(
            hitboxOrigin.position,
            hitboxOrigin.rotation,
            Vector3.one
        );
        Gizmos.DrawWireCube(Vector3.zero, hitboxSize);
    }

    // ===== ANIMATION EVENTS =====

    // GỌI TỪ Animation Event
    public void OnAttackActiveStart()
    {
        hitboxActive = true;
        hitTargets.Clear();
    }

    // GỌI TỪ Animation Event
    public void OnAttackActiveEnd()
    {
        hitboxActive = false;
    }

    // AnimationControl gọi khi clip kết thúc
    void OnAnimationEnd(int clipIndex)
    {   
        print("Done Anim " + clipIndex);
        isAttacking = false;
    }

    // ===== HIT =====
    void DoHit(int hitIndex)
    {
        var hits = Physics2D.OverlapBoxAll(
          hitboxOrigin.position,
          hitboxSize,
          hitboxOrigin.eulerAngles.z,
          hitMask
      );

        foreach (var col in hits)
        {
            if (!hitTargets.Add(col)) continue;
            Debug.Log($"Hit {col.name} by combo {hitIndex}");
            // col.GetComponent<Health>()?.TakeDamage(baseDamage * hitIndex);
        }
    }

}
