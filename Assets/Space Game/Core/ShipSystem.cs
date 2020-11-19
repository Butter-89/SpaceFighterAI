using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipSystem : MonoBehaviour {

	[SerializeField] protected Spaceship ship = null;
	public Spaceship Ship {
		get { return ship; }
	}

	private void Start() {
		if(ship == null) {
			ship = GetComponentInParent<Spaceship>();
		}
	}

}
