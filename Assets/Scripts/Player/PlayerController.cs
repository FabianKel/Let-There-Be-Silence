using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement (Asteroids Style)")]
    public float acceleration = 8f;
    public float reverseAcceleration = 4f;
    public float rotationSpeed = 200f;
    public float linearDragValue = 1f;

    private Rigidbody2D rb;
    private float rotationInput;
    private float moveInput;

    [Header("Special Ability: Grenade Starburst")]
    public float specialAbilityManaCost = 40f;
    public float grenadeDelay = 2.0f;
    public float grenadeSpeed = 8f;

    [Header("Shooting & Pooling")]
    public BulletPool bulletPool;
    public float bulletSpeed = 15f;
    public float spawnOffset = 1.2f;

    [Header("Mana/Stamina System (LoL Style)")]
    public float maxMana = 100f;
    public float currentMana;
    public float manaRegenRate = 15f;
    public float fireManaCost = 5f;

    [Header("Animation")]
    private Animator animator;

    private bool isShooting = false;
    private bool isKnockbackActive = false;
    private float knockbackTimer = 0f;
    private float originalKnockbackForce = 15f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        currentMana = maxMana;

        
        rb.linearDamping = linearDragValue;
    }

    void Update()
    {
        if (currentMana < maxMana)
        {
            currentMana += manaRegenRate * Time.deltaTime;
            currentMana = Mathf.Min(currentMana, maxMana);
        }

        if (isKnockbackActive)
        {
            knockbackTimer -= Time.deltaTime;
            if (knockbackTimer <= 0f)
            {
                isKnockbackActive = false;
                rb.linearDamping = linearDragValue;
            }
        }
    }

    void FixedUpdate()
    {
        if (!isKnockbackActive)
        {
            float rotationAmount = -rotationInput * rotationSpeed * Time.fixedDeltaTime;
            rb.MoveRotation(rb.rotation + rotationAmount);
        }

        if (!isKnockbackActive)
        {
            Vector2 forwardDirection = transform.up;

            if (moveInput > 0)
            {
                rb.AddForce(forwardDirection * acceleration, ForceMode2D.Force);
            }
            else if (moveInput < 0)
            {
                rb.AddForce(-forwardDirection * reverseAcceleration, ForceMode2D.Force);
            }
        }
    }

    #region Input System Callbacks

    public void OnRotate(InputAction.CallbackContext context)
    {
        rotationInput = context.ReadValue<float>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        float value = context.ReadValue<float>();
        if (context.performed) moveInput = value;
        if (context.canceled) moveInput = 0f;
    }

    public void OnReverse(InputAction.CallbackContext context)
    {
        float value = context.ReadValue<float>();
        if (context.performed) moveInput = -value;
        if (context.canceled) moveInput = 0f;
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        if (context.started && !isKnockbackActive && !isShooting)
        {
            if (currentMana >= fireManaCost)
            {
                currentMana -= fireManaCost;
                FireForward();
            }
            else
            {
                Debug.Log("Sin stamina para disparar");
            }
        }
    }

    public void OnSpecialAbility(InputAction.CallbackContext context)
    {
        if (context.started && !isKnockbackActive)
        {
            if (currentMana >= specialAbilityManaCost)
            {
                currentMana -= specialAbilityManaCost;

                StartCoroutine(GrenadeRoutine());
            }
            else
            {
                Debug.Log("Sin stamina para la habilidad especial");
            }
        }
    }

    #endregion

    IEnumerator GrenadeRoutine()
    {
        animator.SetTrigger("Pulse");

        Vector2 launchDirection = transform.up;
        Vector3 spawnPosition = transform.position + (Vector3)(launchDirection * spawnOffset);

        GameObject grenadeObj = bulletPool.GetBullet();
        grenadeObj.transform.position = spawnPosition;
        grenadeObj.transform.up = launchDirection;

        Bullet grenadeScript = grenadeObj.GetComponent<Bullet>();
        grenadeScript.speed = grenadeSpeed;
        grenadeScript.Setup(BulletType.Silence, launchDirection, bulletPool);

        yield return new WaitForSeconds(grenadeDelay);

        Vector3 explosionPosition = grenadeObj.transform.position;

        grenadeObj.SetActive(false);

        float startAngle = 0f;

        for (int i = 0; i < 5; i++)
        {
            float currentAngle = startAngle + (i * 72f);

            Vector2 bulletDirection = new Vector2(Mathf.Cos(currentAngle * Mathf.Deg2Rad), Mathf.Sin(currentAngle * Mathf.Deg2Rad));

            GameObject starBullet = bulletPool.GetBullet();
            starBullet.transform.position = explosionPosition;
            starBullet.transform.up = bulletDirection;

            Bullet starBulletScript = starBullet.GetComponent<Bullet>();
            starBulletScript.speed = bulletSpeed;
            starBulletScript.Setup(BulletType.Silence, bulletDirection, bulletPool);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("Enemy"))
        {
            isKnockbackActive = true;
            knockbackTimer = 0.15f;
            rb.linearVelocity = Vector2.zero;
            rb.linearDamping = 2f;

            Vector2 pushDirection = ((Vector2)transform.position - collision.contacts[0].point).normalized;
            rb.AddForce(pushDirection * (originalKnockbackForce * 0.7f), ForceMode2D.Impulse);

            animator.SetTrigger("Hit");
        }
    }

    void FireForward()
    {
        animator.SetTrigger("Pulse");
        StartCoroutine(BurstFireRoutine());
    }

    public void TakeHit() => animator.SetTrigger("Hit");
    public void Die() => animator.SetTrigger("Die");

    IEnumerator BurstFireRoutine()
    {
        isShooting = true;
        int shotsInBurst = 4;
        float timeBetweenShots = 0.1f;

        for (int i = 0; i < shotsInBurst; i++)
        {
            Vector2 fireDirection = transform.up;
            Vector3 spawnPosition = transform.position + (Vector3)(fireDirection * spawnOffset);

            GameObject bulletObj = bulletPool.GetBullet();
            bulletObj.transform.position = spawnPosition;
            bulletObj.transform.up = fireDirection;

            Bullet bulletScript = bulletObj.GetComponent<Bullet>();
            bulletScript.speed = bulletSpeed;

            bulletScript.Setup(BulletType.Silence, fireDirection, bulletPool);

            yield return new WaitForSeconds(timeBetweenShots);
        }
        isShooting = false;
    }
}