using UnityEngine;

public enum BulletType { Silence, Note }

public class Bullet : MonoBehaviour
{
    public BulletType type;
    public float speed;
    public SpriteRenderer spriteRenderer;
    public Sprite silenceSprite;
    public Sprite noteSprite;

    private BulletPool _originPool;
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Setup(BulletType newType, Vector2 direction, BulletPool pool)
    {
        type = newType;
        _originPool = pool;
        spriteRenderer.sprite = (type == BulletType.Silence) ? silenceSprite : noteSprite;

        rb.linearVelocity = direction * speed;

        StartCoroutine(_originPool.ReleaseAfterTime(gameObject, 1f));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Health targetHealth = collision.GetComponent<Health>();

        if (targetHealth != null)
        {
            targetHealth.TakeDamage(1, type);
            ReturnToPool();
        }

        if ((type == BulletType.Silence && collision.CompareTag("EnemyBullet")) ||
            (type == BulletType.Note && collision.CompareTag("PlayerBullet")))
        {
            ReturnToPool();
        }
    }

    public void ReturnToPool()
    {
        StopAllCoroutines();
        _originPool.ReleaseBullet(gameObject);
    }
}