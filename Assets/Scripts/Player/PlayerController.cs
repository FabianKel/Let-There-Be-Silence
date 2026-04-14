using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 moveInput;

    [Header("Shooting")]
    public BulletPool bulletPool;
    public int bulletCount = 4;
    public float bulletSpeed = 10f;
    public float spawnOffset = 1.5f;
    private float currentRotationOffset = 0f;

    [Header("Animation")]
    private Animator animator;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(KeyCode.Space))
        {
            FireCircular();
        }

        if (Input.GetKeyDown(KeyCode.U)) bulletCount *= 2;
        if (Input.GetKeyDown(KeyCode.V)) bulletCount = Mathf.Max(1, bulletCount / 2);
    }

    void FixedUpdate()
    {
        rb.linearVelocity = moveInput.normalized * moveSpeed;
    }

    void FireCircular()
    {
        animator.SetTrigger("Pulse");

        float angleStep = 360f / bulletCount;
        float angle = currentRotationOffset;

        currentRotationOffset += 15f;

        for (int i = 0; i < bulletCount; i++)
        {
            float bulletDirX = Mathf.Cos(angle * Mathf.Deg2Rad);
            float bulletDirY = Mathf.Sin(angle * Mathf.Deg2Rad);
            Vector2 bulletDirection = new Vector2(bulletDirX, bulletDirY);

            Vector3 spawnPosition = transform.position + (Vector3)(bulletDirection * spawnOffset);

            GameObject bulletObj = bulletPool.GetBullet();
            bulletObj.transform.position = spawnPosition;
            bulletObj.transform.right = bulletDirection;

            Bullet bulletScript = bulletObj.GetComponent<Bullet>();
            bulletScript.speed = bulletSpeed;
            bulletScript.Setup(BulletType.Silence, bulletDirection, bulletPool);

            angle += angleStep;
        }
    }

    public void TakeHit() => animator.SetTrigger("Hit");
    public void Die() => animator.SetTrigger("Die");
}