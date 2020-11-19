using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseAgent : MonoBehaviour, IResetable
{
    public Spaceship ship;

    public virtual string GetStatus() { return ""; }
    public virtual string GetMessage() { return ""; }

    public virtual void Reset() {
        if (ship == null)
            ship = GetComponent<Spaceship>();
    }

    public virtual void Run_6_1(Targetable target) { throw new System.NotImplementedException(); }

    public virtual void Run_6_2() { throw new System.NotImplementedException(); }

    public virtual void Run_6_3(Targetable target) { throw new System.NotImplementedException(); }

    public virtual void Run_6_4(Targetable target) { throw new System.NotImplementedException(); }

    public virtual void Run_6_5() { throw new System.NotImplementedException(); }
}
