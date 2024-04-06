using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{

    [SerializeField] private Projectile pooledProjectile;
    [SerializeField] private Rigidbody projectile;
    [SerializeField] private float force;
    [SerializeField] private float lifeTime;
    [SerializeField] private float timeBetweenShots;
    [SerializeField] private int numToSpawn;
    [SerializeField] private bool isPooled;

    private float curTime;

    private string name;
    // Start is called before the first frame update
    void Start()
    {
        curTime = timeBetweenShots;
        name = pooledProjectile.name;

    }

    // Update is called once per frame
    void Update()
    {
        curTime -= Time.deltaTime;
        if (curTime > 0) return;
        curTime = timeBetweenShots;
        if(isPooled) PooledShoot();
        else Shoot();
    }

    void Shoot()
    {
        for (int i = 0; i < numToSpawn; ++i)
        {
            Rigidbody go = Instantiate(projectile, transform.position, Quaternion.identity);
            go.AddForce(Random.insideUnitSphere * force, ForceMode.Impulse);
            Destroy(go, lifeTime);
        }
    }

    void PooledShoot()
    {
        for (int i = 0; i < numToSpawn; ++i)
        {
            Projectile p = Pool.Shoot(pooledProjectile.name, transform.position, Quaternion.identity);
            p.Init(Random.insideUnitSphere * force, lifeTime);
        }
    }

}
