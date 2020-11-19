using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFireable
{
    bool CanFire();
    void Fire();
    Transform GetLaunchPoint();
}
