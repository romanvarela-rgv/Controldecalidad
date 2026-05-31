using System.Collections.Generic;
using UnityEngine;

public class MeteoritoPool : MonoBehaviour
{
    [SerializeField] private GameObject meteoritoPrefab;
    [SerializeField] private int initialPoolSize = 5;
    [SerializeField] private Transform poolParent;

    private readonly Queue<GameObject> availableMeteoritos = new Queue<GameObject>();
    private readonly List<GameObject> allMeteoritos = new List<GameObject>();

    private void Awake()
    {
        if (meteoritoPrefab == null)
        {
            Debug.LogError("[MeteoritoPool] No se ha asignado un prefab de meteorito.", this);
            return;
        }

        for (int i = 0; i < initialPoolSize; i++)
        {
            CreateMeteorito();
        }
    }

    private GameObject CreateMeteorito()
    {
        GameObject meteorito = Instantiate(meteoritoPrefab, poolParent);
        meteorito.SetActive(false);
        availableMeteoritos.Enqueue(meteorito);
        allMeteoritos.Add(meteorito);
        return meteorito;
    }

    public GameObject GetMeteorito()
    {
        GameObject meteorito;
        if (availableMeteoritos.Count > 0)
        {
            meteorito = availableMeteoritos.Dequeue();
        }
        else
        {
            meteorito = CreateMeteorito();
        }
        meteorito.transform.SetParent(null, false);
        meteorito.SetActive(true);
        return meteorito;
    }

    public void ReturnMeteorito(GameObject meteorito)
    {
        if (meteorito == null) return;
        meteorito.SetActive(false);
        meteorito.transform.SetParent(poolParent, false);
        availableMeteoritos.Enqueue(meteorito);
    }
}
