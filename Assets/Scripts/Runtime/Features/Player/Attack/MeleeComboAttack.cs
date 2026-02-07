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
    [SerializeField] Hitbox2D hitbox; 

    [SerializeField] bool hitboxActive;
    PlayerBehavior _playerBehavior;
    int comboIndex = 0;
    float lastAttackTime;
    bool isAttacking;
    bool queuedNext;
    int queuedComboIndex;


    HashSet<Collider2D> hitTargets = new HashSet<Collider2D>();

    public bool IsAttacking => isAttacking;

    public void Init(PlayerBehavior owner)
    {   
        _playerBehavior = owner;
        // đăng ký callback animation end
        _playerBehavior.animationControl.callBackEnd += OnAnimationEnd;
        _playerBehavior.animationControl.callBackStartDamage += OnAttackActiveStart;
        print($"Init meleeAttack {_playerBehavior.animationControl.animator.runtimeAnimatorController.animationClips.Length}");
        for (int i = 0; i < _playerBehavior.animationControl.animator.runtimeAnimatorController.animationClips.Length; i++)
        {
            string clip_name = _playerBehavior.animationControl.animator.runtimeAnimatorController.animationClips[i].name;
            print($"{clip_name}");
            switch (clip_name)
            {
                case AnimationControl.Attack1:
                    _playerBehavior.animationControl.AddEvent(clip_name, 1, new float[1] {0.25f});
                    break;
                case AnimationControl.Attack2:
                    _playerBehavior.animationControl.AddEvent(clip_name, 2, new float[1] {0.25f});
                    break;
                case AnimationControl.Attack3:
                    _playerBehavior.animationControl.AddEvent(clip_name, 3, new float[1] {0.25f});
                    break;
            }
        }
    }

    public void Tick()
    {
        if (!isAttacking && comboIndex > 0 && Time.time - lastAttackTime > comboResetTime)
            comboIndex = 0;
       
    }

    public bool TryAttack()
    {
        if (isAttacking)
        {
            queuedNext = true;
            return false;
        }
        StartAttackNext();
        return true;
    }

    void StartAttackNext()
    {
        comboIndex++;
        if (comboIndex > maxCombo) comboIndex = 1;

        isAttacking = true;
        lastAttackTime = Time.time;
        _playerBehavior.animationControl.Play($"Attack{comboIndex}", 0.0f);
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
    
    int _attackInstanceId = 0;
    [SerializeField] float iframeOnHit = 0.08f;
    public void OnAttackActiveStart(string value)
    {   
        hitboxActive = true;
        hitTargets.Clear();

        int dmg = baseDamage * comboIndex;
        int id = ++_attackInstanceId;

        float kb = comboIndex == 3 ? 7f : 4f;          // combo 3 hất mạnh hơn
        float st = comboIndex == 3 ? 0.18f : 0.08f;    // combo 3 choáng lâu hơn

        Hitbox2D.HitInfo hit = new Hitbox2D.HitInfo(_playerBehavior.transform, id, dmg, iframeOnHit, kb, st);
        hitbox.BeginSwing(hit);
    }

    // GỌI TỪ Animation Event
    public void OnAttackActiveEnd()
    {
        hitboxActive = false;
        hitbox.EndSwing();
    }

    // AnimationControl gọi khi clip kết thúc
    void OnAnimationEnd(int clipIndex)
    {
        print("Done Anim " + clipIndex);
        isAttacking = false;
        OnAttackActiveEnd();
        if (queuedNext)
        {
            queuedNext = false;
            StartAttackNext();
            return;
        }
    }

   

}
