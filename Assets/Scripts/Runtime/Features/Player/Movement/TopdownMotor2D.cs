using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class TopdownMotor2D : MonoBehaviour
{
    [Header("Move")]
    public float maxSpeed = 6.5f;
    public bool useInertia = true;

    [Tooltip("Chỉ dùng khi useInertia = true")]
    public float acceleration = 40f;
    public float deceleration = 55f;
    public bool normalizeDiagonal = true;

    Vector2 inputDir;
    Vector2 currentVelocity;

    [Header("Aim")]
    public bool rotateBodyToMouse = true;
    public Transform aimPivot;
    Rigidbody2D rb;
    Camera cam;

    [Header("Dash")]
    public bool enableDash = true;
    public float dashSpeed = 18f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 0.6f;
    public bool dashHasIFrame = true;

    [Tooltip("Nếu bật: dash xong đứng ngay, không trượt")]
    public bool stopAfterDash = true; // ⭐ THÊM


    PlayerBehavior _playerBehavior;    
    bool isDashing;
    float dashTimer;
    float dashCooldownTimer;
    Vector2 dashDirection;

    int defaultLayer;
    int invincibleLayer;


    public IEnumerator Init(PlayerBehavior playerBehavior)
    {   
        _playerBehavior = playerBehavior;
        rb = GetComponent<Rigidbody2D>();
        cam = Camera.main;

        rb.gravityScale = 0;
        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        defaultLayer = gameObject.layer;
        invincibleLayer = LayerMask.NameToLayer("Invincible");
        yield return null;
    }

    public void UpdateMove()
    {
        // INPUT
        Vector2 mouseWorld = cam.ScreenToWorldPoint(Input.mousePosition);
        _playerBehavior.mouseCursorFollow.UpdateCursor(mouseWorld);
        
        inputDir = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        );

        _playerBehavior.animationControl.UpdateMovement(inputDir);
        if (normalizeDiagonal && inputDir.sqrMagnitude > 1f)
            inputDir.Normalize();

        // AIM
        if (!cam) return;

        Vector2 aimDir = mouseWorld - rb.position;

        if (aimDir.sqrMagnitude > 0.0001f)
        {
            float angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;

            if (aimPivot != null)
                aimPivot.rotation = Quaternion.Euler(0, 0, angle);
            else if (rotateBodyToMouse)
                rb.SetRotation(angle);
        }

        // DASH INPUT
        if (enableDash && Input.GetKeyDown(KeyCode.Space))
            TryDash();
    }

    public void FixedUpdateMove()
    {
        // Cooldown
        if (dashCooldownTimer > 0f)
            dashCooldownTimer -= Time.fixedDeltaTime;

        // ===== DASH STATE =====
        if (isDashing)
        {
            dashTimer -= Time.fixedDeltaTime;

            currentVelocity = dashDirection * dashSpeed;
            rb.MovePosition(rb.position + currentVelocity * Time.fixedDeltaTime);

            if (dashTimer <= 0f)
            {
                isDashing = false;

                // ⭐ ĐÂY LÀ CHỖ ĐÚNG
                if (stopAfterDash)
                    currentVelocity = Vector2.zero;

                if (dashHasIFrame)
                    EnableIFrame(false);
            }

            return; // ⛔ không chạy movement thường khi dash
        }

        // ===== NORMAL MOVE =====
        Vector2 desiredVelocity = inputDir * maxSpeed;

        if (useInertia)
        {
            float rate = (inputDir.sqrMagnitude > 0.001f)
                ? acceleration
                : deceleration;

            currentVelocity = Vector2.MoveTowards(
                currentVelocity,
                desiredVelocity,
                rate * Time.fixedDeltaTime
            );
        }
        else
        {
            currentVelocity = desiredVelocity;
        }

        rb.MovePosition(rb.position + currentVelocity * Time.fixedDeltaTime);
    }

    void TryDash()
    {
        if (isDashing) return;
        if (dashCooldownTimer > 0f) return;

        if (inputDir.sqrMagnitude > 0.01f)
            dashDirection = inputDir.normalized;
        else
            dashDirection = aimPivot != null
                ? (Vector2)aimPivot.right
                : (Vector2)transform.right;

        isDashing = true;
        dashTimer = dashDuration;
        dashCooldownTimer = dashCooldown;

        ForceVelocity(dashDirection * dashSpeed);

        if (dashHasIFrame)
            EnableIFrame(true);
    }

    // ===== API =====
    public void ForceVelocity(Vector2 vel)
    {
        currentVelocity = vel;
    }

    public Vector2 GetVelocity() => currentVelocity;

    void EnableIFrame(bool enable)
    {
        gameObject.layer = enable ? invincibleLayer : defaultLayer;
    }
}
