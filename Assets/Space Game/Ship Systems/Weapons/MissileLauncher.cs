using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileLauncher : ShipSystem {

	[SerializeField] private Missile missilePrefab = null;
	[SerializeField] private Transform launchPoint = null;

	[SerializeField] private int inventory = 10;

	[SerializeField] private float cooldown = 5f;
	[SerializeField] private float cooldownRemaining = 0f;

	private void Update() {
		cooldownRemaining -= Time.deltaTime;
	}

	public string FireStatus {
		get {
			if (inventory <= 0)
				return "Out of missiles";
			if (cooldownRemaining > 0)
				return "Missile is on cooldown";
			return null;
		}
	}

	public bool CanFire {
		get { return FireStatus == null; }
	}

	public Missile Fire() {
		if (!CanFire)
			return null;

		inventory--;
		cooldownRemaining = cooldown;

		Missile missile = Instantiate(missilePrefab, launchPoint.position, launchPoint.rotation, null);
		missile.gameObject.SetActive(true);
		missile.Launch(ship.Rigidbody);

		return missile;
	}
}
