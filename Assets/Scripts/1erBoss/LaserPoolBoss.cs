using System.Collections.Generic;
using UnityEngine;

public class LaserPoolBoss : MonoBehaviour
{
    public static LaserPoolBoss Instance;

    [Header("Prefab & Pool")]
    [SerializeField] private GameObject pooledLaser;   
    [SerializeField] private int initialSize = 10;     
    [SerializeField] private bool canExpand = true;    

    private readonly List<GameObject> lasers = new List<GameObject>();

    private void Awake()
    {
        Instance = this;
        Prewarm();
    }

    private void Prewarm()
    {
        if (pooledLaser == null)
        {
            Debug.LogError("[LaserPoolBoss] Asigná el 'pooledLaser' en el inspector.");
            return;
        }

        for (int i = 0; i < initialSize; i++)
        {
            var go = Instantiate(pooledLaser, transform);
            go.SetActive(false);
            lasers.Add(go);
        }
    }

    public GameObject GetLaser()
    {
        for (int i = 0; i < lasers.Count; i++)
        {
            if (!lasers[i].activeInHierarchy)
                return lasers[i];
        }

        if (canExpand && pooledLaser != null)
        {
            var go = Instantiate(pooledLaser, transform);
            go.SetActive(false);
            lasers.Add(go);
            return go;
        }

        return null;
    }

    public void ClearPool()
    {
        for (int i = 0; i < lasers.Count; i++)
        {
            if (lasers[i] != null && lasers[i].activeInHierarchy)
                lasers[i].SetActive(false);
        }
    }


    public void ReturnToPool(GameObject laser)
    {
        if (laser == null) return;
        if (!lasers.Contains(laser))
        {
            laser.transform.SetParent(transform);
            lasers.Add(laser);
        }
        laser.SetActive(false);
    }
}
