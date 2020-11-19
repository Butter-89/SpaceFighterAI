using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PA_6_3_EnemyAgent : MonoBehaviour, IResetable
{
    public Spaceship ship;
    public BasicEngine engine;
    public Vector3 waypoint;
    public enum State {
        MoveToWaypoint,
        Idle
    }
    public State state = State.Idle;
    public float timeInState = 0;

    public void Reset() {
        if (ship == null)
            ship = GetComponent<Spaceship>();
        engine = ship.GetSystem<BasicEngine>();
        engine.turnThrottle = 0;
        engine.forwardThrottle = 0;
        state = State.Idle;
        timeInState = 0;
        RollNewState();
    }

    void Update()
    {
        timeInState += Time.deltaTime;

        if(state == State.Idle && timeInState >= 3f) {
            RollNewState();
        }
        else if(state == State.MoveToWaypoint) {
            float dist = MoveTo(waypoint);
            if (dist < 5f) {
                RollNewState();
            }
        }
    }

    void RollNewState() {
        int roll = Random.Range(0, 2);
        if(roll == 0) {
            state = State.Idle;
            engine.forwardThrottle = -1;
        } else {
            state = State.MoveToWaypoint;
            waypoint = Random.insideUnitSphere * 200f;
            waypoint.y = 0;
        }

        timeInState = 0;
    }

    float MoveTo(Vector3 point) {
        Vector3 delta = transform.position - point;
        Vector3 dir = delta.normalized;
        float dist = delta.magnitude;
        float dot = Vector3.Dot(transform.right, dir);
        float throttle = 0;
        if (dot < 0)
            throttle = 1f;
        if (dot > 0)
            throttle = -1f;
        else if (dot == 0)
            throttle = 0;

        throttle = Mathf.Clamp(throttle, -1f, 1f);
        engine.turnThrottle = throttle;

        engine.forwardThrottle = 1f;

        return dist;
    }
}
