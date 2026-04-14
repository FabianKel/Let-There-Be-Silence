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

        StartCoroutine(_originPool.ReleaseAfterTime(gameObject, 3f));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (type == BulletType.Silence)
        {
            if (collision.CompareTag("Enemy") || collision.CompareTag("EnemyBullet"))
            {
                ReturnToPool();
            }
        }
        else if (type == BulletType.Note)
        {
            if (collision.CompareTag("Player") || collision.CompareTag("PlayerBullet"))
            {
                ReturnToPool();
            }
        }
    }

    public void ReturnToPool()
    {
        StopAllCoroutines();
        _originPool.ReleaseBullet(gameObject);
    }
}