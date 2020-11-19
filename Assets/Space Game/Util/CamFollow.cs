using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamFollow : MonoBehaviour {

	public Transform target = null;
	public Camera cam = null;

	private void Start() {
		if(cam == null) {
			cam = Camera.main;
		}
	}

	void Update() {
		if(cam != null && target != null) {
			Vector3 camPos = new Vector3(target.transform.position.x, cam.transform.position.y, target.transform.position.z);
			cam.transform.position = camPos;
		}
    }
}
