using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEngine : ShipSystem, IResetable {

	[SerializeField] private Rigidbody rb = null;

	[SerializeField] private float maxSpeed = 10f;
    public float MaxSpeed => maxSpeed;
    [SerializeField] private float curSpeed = 0;
	[SerializeField] private float turnSpeed = 60f;
    public float TurnSpeed => turnSpeed;

    public float Speed => rb.velocity.magnitude;
    public Vector3 Velocity => rb.velocity;

	public float forwardThrottle = 0;
	public float turnThrottle = 0;
    public float accel = 10f;
    public float decel = 5f;
    public float turnAccel = 60f;

	private void Awake() {
		if(rb == null) {
			rb = GetComponentInParent<Rigidbody>();
		}
    }

	private void Update() {
        forwardThrottle = Mathf.Clamp(forwardThrottle, -1f, 1f);
        turnThrottle = Mathf.Clamp(turnThrottle, -1f, 1f);

        curSpeed = rb.velocity.magnitude;
        float rate = forwardThrottle < 0 ? decel : accel;
        float newSpeed = Mathf.Clamp(curSpeed + forwardThrottle * rate * Time.deltaTime, 0, maxSpeed);
        float delta = newSpeed - curSpeed;

        //if(Mathf.Abs(delta) > 0.001f)
        //    rb.AddForce(transform.forward * delta, ForceMode.VelocityChange);

        rb.MoveRotation(Quaternion.AngleAxis(turnSpeed * turnThrottle * Time.deltaTime, transform.up) * rb.rotation);
        rb.velocity = transform.forward * newSpeed;
        curSpeed = newSpeed;
    }

    public void Reset() {
        rb.velocity = Vector3.zero;
        curSpeed = 0;
        forwardThrottle = 0;
        turnThrottle = 0;
    }
}
