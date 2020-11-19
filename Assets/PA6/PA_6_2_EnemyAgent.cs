using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PA_6_2_EnemyAgent : MonoBehaviour
{
    public bool canFire = false;

    [SerializeField] private Spaceship ship = null;
    [SerializeField] private Targetable target = null;
    [SerializeField] private IFireable weapon = null;
    [SerializeField] private float projectileSpeed = 100;

    void Start()
    {
        weapon = ship.GetComponentInChildren<IFireable>();
    }

    void Update()
    {
        Transform launchPoint = weapon.GetLaunchPoint();
        Vector3 delta = launchPoint.position - target.Position;
        Vector3 dir = delta.normalized;
        float dist = delta.magnitude;
        BasicEngine engine = ship.GetSystem<BasicEngine>();
        float dot = Vector3.Dot(ship.transform.right, dir);
        if (dot < 0)
            engine.turnThrottle = 1;
        else if (dot > 0)
            engine.turnThrottle = -1;
        else
            engine.turnThrottle = 0;
        engine.forwardThrottle = -1;

        if(canFire)
            weapon.Fire();
    }
}
