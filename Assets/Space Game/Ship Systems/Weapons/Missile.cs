using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour {

	[SerializeField] private Rigidbody rb = null;

	[SerializeField] private float maxSpeed = 100f;
	public float MaxSpeed {
		get { return maxSpeed; }
	}
	[SerializeField] private float maxTurnSpeed = 360f;
	public float MaxTurnSpeed {
		get { return maxTurnSpeed; }
	}

	public float forwardThrottle = 1f;
	public float turnThrottle = 0f;

	[SerializeField] private bool exploded = false;
	public bool Exploded {
		get { return exploded; }
	}
	[SerializeField] private float explosionRange = 20f;
	[SerializeField] private float explosionDamage = 50f;
	[SerializeField] private GameObject explosionPrefab = null;

	[SerializeField] private Rigidbody launcher = null;

	public bool Launched {
		get { return enabled; }
	}

	private void Start() {
		if(rb == null) {
			rb = GetComponent<Rigidbody>();
		}
	}

	private void Update() {
		rb.velocity = transform.forward * (forwardThrottle * maxSpeed);
		rb.MoveRotation(Quaternion.AngleAxis(maxTurnSpeed * turnThrottle * Time.deltaTime, transform.up) * rb.rotation);
	}

	public void Launch(Rigidbody launcher) {
		this.launcher = launcher;
		enabled = true;
	}

	private void OnCollisionEnter(Collision collision) {
		if(collision.rigidbody != launcher)
			Explode();
	}

	void Explode() {
		if (exploded)
			return;

		exploded = true;

		Collider[] collisions = Physics.OverlapSphere(transform.position, explosionRange);

		HashSet<Destructible> destructiblesInRange = new HashSet<Destructible>();
		foreach(Collider col in collisions) {
			if (col.attachedRigidbody == null)
				continue;
			Destructible destructible = col.attachedRigidbody.GetComponent<Destructible>();
			if(destructible != null) {
				destructiblesInRange.Add(destructible);
			}
		}

		foreach(Destructible destructible in destructiblesInRange) {
			destructible.DoDamage(explosionDamage);
		}

		if(explosionPrefab != null) {
			Instantiate(explosionPrefab, transform.position, transform.rotation, null);
		}

		Destroy(gameObject);
	}

}
