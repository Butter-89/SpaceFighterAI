using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetingSystem : ShipSystem {
    
	[SerializeField] private Targetable target = null;
    public Targetable CurrentTarget {
        get {
            if(target != null) {
                Destructible destructible = target.GetComponent<Destructible>();
                if (destructible != null && destructible.IsDead)
                    return null;
            }

            return target;
        }
    }

    [SerializeField] private List<float> scanAngles = new List<float>() { 0 };
    [SerializeField] private float scanRange = 100;
    [SerializeField] private float scanSize = 5f;
    [SerializeField] private float scanRadius = 50f;

    public Targetable TargetNearestEnemy() {
        List<Targetable> scan = Scan();
        Targetable nearest = null;
        float dist = float.MaxValue;
        foreach(Targetable target in scan) {
            if(target.type == Targetable.TargetType.Ship && target.gameObject != ship.gameObject) {
                float curDist = (target.Position - ship.transform.position).magnitude;
                if(curDist < dist) {
                    dist = curDist;
                    nearest = target;
                }
            }
        }
        target = nearest;
        return nearest;
    }

    public void ClearTarget() {
        target = null;
    }

    public Targetable ForwardScan() {
        Targetable target = null;
        Vector3 dir = transform.forward;
        Ray ray = new Ray(transform.position, dir);
        RaycastHit raycastHit;
        bool hit = Physics.SphereCast(ray, scanSize, out raycastHit, scanRange);
        if (hit) {
            target = raycastHit.rigidbody.GetComponentInParent<Targetable>();
        }

        return target;
    }

    public Targetable DirectionScan(Vector3 dir) {
        Targetable target = null;
        Ray ray = new Ray(transform.position, dir);
        RaycastHit raycastHit;
        bool hit = Physics.SphereCast(ray, scanSize, out raycastHit, scanRange);
        if (hit) {
            target = raycastHit.rigidbody.GetComponentInParent<Targetable>();
        }

        return target;
    }

    public List<Targetable> Scan() {
        List<Targetable> targets = new List<Targetable>();

        foreach(float angle in scanAngles) {
            Vector3 dir = Quaternion.AngleAxis(angle, Vector3.up) * transform.forward;
            Ray ray = new Ray(transform.position, dir);
            RaycastHit raycastHit;
            bool hit = Physics.SphereCast(ray, scanSize, out raycastHit, scanRange);
            if(hit) {
                Targetable target = raycastHit.rigidbody.GetComponentInParent<Targetable>();
                if (target != null)
                    targets.Add(target);
            }
        }

        Collider[] colliders = Physics.OverlapSphere(ship.transform.position, scanRadius);
        foreach(Collider collider in colliders) {
            Targetable targetable = collider.GetComponentInParent<Targetable>();
            if (targetable == null || targetable.gameObject == ship.gameObject)
                continue;

            if (!targets.Contains(targetable))
                targets.Add(targetable);
        }

        targets.RemoveAll(t => t == null);

        return targets;
    }
}
