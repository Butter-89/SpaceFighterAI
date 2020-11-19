using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyAgent : BaseAgent
{
    public BasicEngine engine;
    public TargetingSystem targetingSystem;
    public IFireable weapon;

    public override void Reset() {
        base.Reset();
        engine = ship.GetSystem<BasicEngine>();
        targetingSystem = ship.GetSystem<TargetingSystem>();
        weapon = ship.GetComponentInChildren<IFireable>();
    }

    /*
     * 6.1: Move to the target point and stop on it
     * You gain 1 point for every target you 1,
     * and 1 point for coming to a complete stop on the target
     * before the timer runs out.
     */
    public override void Run_6_1(Targetable target) {
        // HINT: control the turnThrottle and forwardThrottle on your engine to move
    }

    /*
     * 6.2: Avoid enemy fire
     * Enemies will fire on you for 30 seconds.
     * You start with 10 points and lose 1 point every time you are hit.
     */
    public override void Run_6_2() {
        // HINT: use your targeting system to identify any projectiles that might hit you
    }

    /*
     * 6.3: Shoot enemies
     * You gain 1 point for each enemy you successfully hit.
     */
    public override void Run_6_3(Targetable target) {
        // HINT: aim where the target is moving to, not where it's currently at
    }

    /*
     * 6.4: Move to target, avoid colliding with asteroids
     * You gain 1 point for reaching each target,
     * and gain an additional point if you didn't collide with any asteroids to get there.
     */
    public override void Run_6_4(Targetable target) {
        
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
