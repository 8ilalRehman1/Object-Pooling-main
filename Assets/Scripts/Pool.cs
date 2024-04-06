using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Pool : MonoBehaviour
{

    [Serializable]
    private struct PooledObject
    {
        public Projectile prefab;
        public int numToSpawn;
    }

    [SerializeField] private PooledObject[] pools;

    private static readonly Dictionary<string, Queue<Projectile>> pooledObjects =
        new Dictionary<string, Queue<Projectile>>();

    private void Awake()
    {
        pooledObjects.Clear();
        updatingProjectiles.Clear();
        foreach (PooledObject pool in pools)
        {
            string name = pool.prefab.name;
            Transform parent = new GameObject(name).transform;
            parent.SetParent(transform);
            Queue<Projectile> objectsToSpawn = new(pool.numToSpawn);
            for (int i = 0; i < pool.numToSpawn; ++i)
            {
                Projectile rb = Instantiate(pool.prefab, parent);
                rb.gameObject.SetActive(false);
                objectsToSpawn.Enqueue(rb);
            }
            pooledObjects.Add(name, objectsToSpawn);
        }
    }


    public static readonly List<Projectile> updatingProjectiles = new();
    private void Update()
    {
        float dt = Time.deltaTime;
        for (int i = updatingProjectiles.Count - 1; i >= 0; i--)
        {
            Projectile p = updatingProjectiles[i];
            p.lifeTime -= dt;
            if (p.lifeTime < 0)
            {
                updatingProjectiles.RemoveAt(i);
                p.gameObject.SetActive(false);
            }
        }
    }

    public static Projectile Shoot(string name, Vector3 location, Quaternion rotation)
    {
        if (!pooledObjects.ContainsKey(name))
        {
            Debug.LogAssertion("Pool does not contain key:  " + name);
            return null;
        }

        Projectile rb = pooledObjects[name].Dequeue();
        pooledObjects[name].Enqueue(rb);

        rb.transform.SetPositionAndRotation(location, rotation);
        rb.gameObject.SetActive(true);
        return rb;
    }

}
