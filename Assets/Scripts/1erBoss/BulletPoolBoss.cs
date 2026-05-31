using System.Collections.Generic;
using UnityEngine;

public class BulletPoolBoss : MonoBehaviour
{
    public static BulletPoolBoss bulletPoolInstanse;

    [SerializeField] private GameObject pooledBullet;
    [SerializeField] private bool notEnoughBulletsPool = true;

    private readonly List<GameObject> bullets = new();

    private void Awake()
    {
        bulletPoolInstanse = this;
    }

    public GameObject GetBullet()
    {
        for (int i = 0; i < bullets.Count; i++)
        {
            if (!bullets[i].activeInHierarchy)
                return bullets[i];
        }

        if (notEnoughBulletsPool && pooledBullet != null)
        {
            GameObject bul = Instantiate(pooledBullet, transform);
            bul.SetActive(false);
            bullets.Add(bul);
            return bul;
        }

        return null;
    }
    public void ClearPool()
    {
        for (int i = 0; i < bullets.Count; i++)
        {
            if (bullets[i] != null && bullets[i].activeInHierarchy)
                bullets[i].SetActive(false);
        }
    }
}
