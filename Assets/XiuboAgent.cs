using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XiuboAgent : BaseAgent
{
    public BasicEngine engine;
    public TargetingSystem targetingSystem;
    public IFireable weapon;

    private float prevSpeed = 0f;
    private bool bStopping = false;
    [SerializeField] List<Targetable> targetables = new List<Targetable>();
    private float steering = 1f;
    private bool escaping = true;
    private float timer = 0f;
    private bool turningComplete = false;
    public override void Reset() {
        base.Reset();
        engine = ship.GetSystem<BasicEngine>();
        targetingSystem = ship.GetSystem<TargetingSystem>();
        weapon = ship.GetComponentInChildren<IFireable>();
        prevSpeed = 0f;
        timer = 0f;
        turningComplete = false;
        escaping = true;
    }

    /*
     * 6.1: Move to the target point and stop on it
     * You gain 1 point for every target you 1,
     * and 1 point for coming to a complete stop on the target
     * before the timer runs out.
     */
    public override void Run_6_1(Targetable target) {
        // HINT: control the turnThrottle and forwardThrottle on your engine to move
        
        Vector3 toTarget = target.Position - transform.position;
        Vector3 targetDirection = toTarget.normalized;
        Vector3 relativeDirection = targetDirection - transform.forward;
        float dotProduct = Vector3.Dot(relativeDirection, transform.right);
        if (dotProduct <= 0)
            engine.turnThrottle = -1;
        else if (dotProduct > 0)
            engine.turnThrottle = 1;
        else
            engine.turnThrottle = 0;

        float distance = toTarget.magnitude;
        float decelDistance = GetStopDistance(bStopping);
        bStopping = CanReachTarget(bStopping, distance);
        float forwardRate = Mathf.Clamp (1 - distance / 10, 0.5f, 1);

        if(bStopping)
            engine.forwardThrottle = -1;
        else
            engine.forwardThrottle = forwardRate;

        prevSpeed = engine.Speed;
    }

    private float GetStopDistance(bool bStopping)
    {
        float stopDistance = 0;
        float rate = bStopping ? engine.decel : engine.accel;
        float nextSpeed = Mathf.Clamp(prevSpeed + rate * Time.deltaTime, 0, engine.MaxSpeed);
        stopDistance = (prevSpeed + nextSpeed) * Time.deltaTime / 2 + (prevSpeed + nextSpeed) * (prevSpeed + nextSpeed) / 2 / engine.decel;
        return stopDistance;
    }

    private bool CanReachTarget(bool bStopping, float remainDistance)
    {
        // The distance when speed reaches 0
        float stopDistance = GetStopDistance(bStopping);
        if (stopDistance < remainDistance)
            return false;
        else
            return true;
    }

    /*
     * 6.2: Avoid enemy fire
     * Enemies will fire on you for 30 seconds.
     * You start with 10 points and lose 1 point every time you are hit.
     */
    public override void Run_6_2() {
        // HINT: use your targeting system to identify any projectiles that might hit you
        targetables.Clear();
        targetables = targetingSystem.Scan();
        
        engine.turnThrottle = steering;
        engine.forwardThrottle = 1;

        if (escaping)
        {
            steering = Mathf.Clamp((float)(steering - 0.2 * Time.deltaTime), 0, 1);
            if (steering == 0) escaping = false;
        }
        else
        {
            steering = 0.1f;
        }
    }

    /*
     * 6.3: Shoot enemies
     * You gain 1 point for each enemy you successfully hit.
     */
    public override void Run_6_3(Targetable target) {
        // HINT: aim where the target is moving to, not where it's currently at
        BasicEngine enemyEngine = target.GetComponentInChildren<BasicEngine>();
        Vector3 interception = target.Position + target.Velocity.normalized * enemyEngine.Speed * Time.deltaTime;

        Vector3 toTarget = interception - transform.position;
        Vector3 targetDirection = toTarget.normalized;
        Vector3 relativeDirection = targetDirection - transform.forward;
        float dotProduct = Vector3.Dot(relativeDirection, transform.right);
        if (dotProduct <= 0)
            engine.turnThrottle = -1;
        else if (dotProduct > 0)
            engine.turnThrottle = 1;
        else
            engine.turnThrottle = 0;

        // Fire when possible
        if (toTarget.magnitude < 30)
            weapon.Fire();

        engine.forwardThrottle = 1;
    }

    /*
     * 6.4: Move to target, avoid colliding with asteroids
     * You gain 1 point for reaching each target,
     * and gain an additional point if you didn't collide with any asteroids to get there.
     */
    public override void Run_6_4(Targetable target) {
        engine.turnThrottle = GetTurningDir(false, target.Position);
        Vector3 toTarget = target.Position - transform.position;
        if (!turningComplete)
        {
            if(Vector3.Angle(transform.forward, toTarget.normalized) > 10)
                return;
            else
            {
                turningComplete = true;
            }
        }
        engine.forwardThrottle = 1f;

        // Detect if astroids ahead
        Targetable obstacle = targetingSystem.ForwardScan();
        if (obstacle)
        {
            // Detect side astroids
            Targetable sideObstacle = GetCloserSideObstacle();
            if (sideObstacle)
            {
                engine.turnThrottle = GetTurningDir(true, obstacle.Position);
                engine.forwardThrottle = 0.5f;
            }
            engine.turnThrottle = GetTurningDir(true, obstacle.Position);
            engine.forwardThrottle = 0.5f;
        }
        

        prevSpeed = engine.Speed;
    }

    private Targetable GetCloserSideObstacle()
    {
        Vector3 scanDirection = engine.turnThrottle > 0 ? transform.right : -transform.right;
        Targetable obstacle = targetingSystem.DirectionScan(transform.right);
        if (obstacle == null)
            return null;

        if (Vector3.Distance(obstacle.Position, transform.position) > 10)
            return null;
        else
            return obstacle;
    }

    private float GetTurningDir(bool isAvoiding, Vector3 targetPos)
    {
        Vector3 toTarget = targetPos - transform.position;
        Vector3 targetDirection = toTarget.normalized;
        Vector3 relativeDirection = targetDirection - transform.forward;
        float dotProduct = Vector3.Dot(relativeDirection, transform.right);

        float turn = 0;
        if(isAvoiding)
        {
            if (dotProduct <= 0)
                turn = 1;
            else if (dotProduct > 0)
                turn = -1;
            else
                turn = 0;
        }
        else
        {
            if (dotProduct <= 0)
                turn = -1;
            else if (dotProduct > 0)
                turn = 1;
            else
                turn = 0;
        }
        return turn;
    }

    /*
     * 6.5: In-class challenge
     * Destroy an enemy ship: +100 points
     * Hit an enemy ship with your cannon: +10 points
     * Die: -100 points
     * Shoot your cannon: -1 point
     * Your ship explodes if you leave the arena (radius of 1000 units from (0,0,0))
     */
    public override void Run_6_5() {
        
    }
}
