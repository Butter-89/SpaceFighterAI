using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Spaceship : MonoBehaviour {

    [SerializeField] private Player owner = null;
    public Player Owner { get => owner; set => owner = value; }

	[SerializeField] private Rigidbody rb = null;
	public Rigidbody Rigidbody {
		get { return rb; }
	}

	public Vector3 Velocity {
		get { return rb.velocity; }
	}

	[SerializeField] private List<ShipSystem> systems = null;
	public T GetSystem<T>() where T : ShipSystem {
		LoadSystems();
		return systems.Find(sys => sys is T) as T;
	}
	public List<T> GetSystems<T>() where T : ShipSystem {
		LoadSystems();
		return systems.FindAll(sys => sys is T).ConvertAll<T>(sys => sys as T);
	}

	private void LoadSystems() {
		if (systems == null || systems.Count == 0) {
			systems = new List<ShipSystem>(GetComponentsInChildren<ShipSystem>());
		}
	}

	private void Start() {
		if(rb == null) {
			rb = GetComponent<Rigidbody>();
		}
		LoadSystems();
	}

	[SerializeField] private Destructible destructible = null;
	public Destructible Destructible {
		get { return destructible; }
	}

    public void Reset() {
        IResetable[] resets = GetComponentsInChildren<IResetable>();
        foreach (IResetable reset in resets)
            reset.Reset();
    }

    public System.Action<Spaceship, Collision> OnCollision;

    private void OnCollisionEnter(Collision collision) {
        float damage = 0;
        Targetable other = collision.rigidbody.GetComponent<Targetable>();
        if(other != null && (other.type == Targetable.TargetType.Asteroid || other.type == Targetable.TargetType.Ship))
        damage = Mathf.Clamp(collision.impulse.magnitude, 1f, 100f);
        if(damage > 0) {
            destructible.DoDamage(damage);
        }
        OnCollision?.Invoke(this, collision);
    }

    private void OnCollisionStay(Collision collision) {
        float damage = 0;
        Targetable other = collision.rigidbody.GetComponent<Targetable>();
        if (other != null && (other.type == Targetable.TargetType.Asteroid || other.type == Targetable.TargetType.Ship))
            damage = Mathf.Clamp(collision.impulse.magnitude, 1f, 100f);
        if (damage > 0) {
            destructible.DoDamage(damage);
        }
    }
}
