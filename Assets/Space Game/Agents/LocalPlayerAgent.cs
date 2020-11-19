using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalPlayerAgent : BaseAgent {

	//[SerializeField] private Spaceship ship = null;
	[SerializeField] private BasicEngine engine = null;
    [SerializeField] private List<IFireable> weapons;

	public override void Reset() {
        if(ship == null) {
            ship = GetComponent<Spaceship>();
        }
		if(engine == null) {
			engine = ship.GetSystem<BasicEngine>();
		}
		if(weapons == null || weapons.Count == 0) {
			weapons = new List<IFireable>(ship.GetComponentsInChildren<IFireable>());
		}
	}

    public override void Run_6_1(Targetable target) {
        UpdateAgent();
    }

    public override void Run_6_2() {
        UpdateAgent();
    }

    public override void Run_6_3(Targetable target) {
        UpdateAgent();
    }

    public override void Run_6_4(Targetable target) {
        UpdateAgent();
    }

    public override void Run_6_5() {
        UpdateAgent();
    }

    void UpdateAgent() {

		if (engine != null) {
            if (Input.GetKey(KeyCode.W)) {
                engine.forwardThrottle = 1f;
            } else if (Input.GetKey(KeyCode.S)) {
                engine.forwardThrottle = -1;
            } else {
				engine.forwardThrottle = 0f;
			}

			if (Input.GetKey(KeyCode.A)) {
				engine.turnThrottle = -1f;
			} else if (Input.GetKey(KeyCode.D)) {
				engine.turnThrottle = 1f;
			} else {
				engine.turnThrottle = 0f;
			}
		}

		if(weapons.Count > 0 && Input.GetKey(KeyCode.Space)) {
			weapons[0].Fire();
		}
    }
}
