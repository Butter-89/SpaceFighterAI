using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : ShipSystem, IResetable, IFireable
{
    [SerializeField] private float cooldown = 1f;
    [SerializeField] private float timer = 0f;

    public System.Action<Cannon,Projectile> OnFire;

    [SerializeField] private Projectile projectilePrefab;
    [SerializeField] private List<Transform> launchPoints;
    [SerializeField] private int curLauncher = -1;

    public bool CanFire() {
        return timer <= 0;
    }

    public Transform GetLaunchPoint() {
        int index = (curLauncher + 1) % launchPoints.Count;
        return launchPoints[index];
    }

    public void Fire() {
        if (timer > 0)
            return; // on cooldown

        timer = cooldown;

        curLauncher = (curLauncher + 1) % launchPoints.Count;

        Projectile projectile = Instantiate(projectilePrefab);
        projectile.launcher = this;
        projectile.transform.position = launchPoints[curLauncher].transform.position;
        projectile.transform.rotation = launchPoints[curLauncher].transform.rotation;
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        rb.velocity = projectile.transform.forward * projectile.speed;

        OnFire?.Invoke(this, projectile);
    }

    void Update() {
        timer -= Time.deltaTime;
    }

    public void Reset() {
        timer = 0;
    }
}
