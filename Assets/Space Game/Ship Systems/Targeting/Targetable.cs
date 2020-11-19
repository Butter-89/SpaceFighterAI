using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Targetable : MonoBehaviour {

    public enum TargetType {
        Ship,
        Projectile,
        Asteroid,
        Pickup,
        NavPoint
    }
    public TargetType type;

	public TargetableEvent OnTargeted = null;

    public Vector3 Position => transform.position;
    public Vector3 Velocity {
        get {
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb == null)
                return Vector3.zero;
            return rb.velocity;
        }
    }

	public void TargetedBy(Targetable targeter) {
		if(OnTargeted != null) {
			OnTargeted.Invoke(targeter);
		}
	}

    public Destructible Destructible => GetComponent<Destructible>();
}
