using UnityEngine;
using UnityEngine.Pool;
using System.Collections;

public class BulletPool : MonoBehaviour
{
    public GameObject bulletPrefab;
    private ObjectPool<GameObject> pool;

    private void Awake()
    {
        pool = new ObjectPool<GameObject>(
            createFunc: () => Instantiate(bulletPrefab),
            actionOnGet: (obj) => obj.SetActive(true),
            actionOnRelease: (obj) => obj.SetActive(false),
            actionOnDestroy: (obj) => Destroy(obj),
            collectionCheck: true,
            defaultCapacity: 10,
            maxSize: 30
        );
    }

    public GameObject GetBullet() => pool.Get();

    public void ReleaseBullet(GameObject bullet)
    {
        if (bullet.activeSelf) pool.Release(bullet);
    }

    public IEnumerator ReleaseAfterTime(GameObject bullet, float delay)
    {
        yield return new WaitForSeconds(delay);
        ReleaseBullet(bullet);
    }
}