using UnityEngine;

public class RhythmicEnemy : MonoBehaviour
{
    public TrackType myTrack;
    public BulletPool bulletPool;
    public int bulletsToSpawn;
    private Animator anim;

    [Header("Bullet Settings")]
    public float bulletSpeed = 10f;
    public float spawnOffset = 1.5f;


    void Start()
    {
        anim = GetComponent<Animator>();

        if (RhythmManager.Instance == null) return;

        switch (myTrack)
        {
            case TrackType.Bass: RhythmManager.Instance.OnBassBeat += Shoot; break;
            case TrackType.Kit: RhythmManager.Instance.OnKitBeat += Shoot; break;
            case TrackType.Piano: RhythmManager.Instance.OnPianoBeat += Shoot; break;
        }
    }

    void Shoot()
    {
        anim.SetTrigger("Pulse");

        float angleStep = 360f / bulletsToSpawn;
        float angle = 0;

        //currentRotationOffset += 15f;

        for (int i = 0; i < bulletsToSpawn; i++)
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
            bulletScript.Setup(BulletType.Note, bulletDirection, bulletPool);

            angle += angleStep;
        }

    }

    public void OnHit()
    {
        anim.SetTrigger("Hit");
        StartCoroutine(RhythmManager.Instance.HitEffect());
        print("Enemy hit on track: " + myTrack);
    }

    public void OnDeath()
    {
        anim.SetTrigger("Die");
        print("Enemy defeated on track: " + myTrack);
        AudioMixer.Instance.EnemigoDerrotado(myTrack);

        if (myTrack == TrackType.Bass) RhythmManager.Instance.OnBassBeat -= Shoot;
        else if (myTrack == TrackType.Kit) RhythmManager.Instance.OnKitBeat -= Shoot;
        else if (myTrack == TrackType.Piano) RhythmManager.Instance.OnPianoBeat -= Shoot;

        Destroy(gameObject);
    }
}