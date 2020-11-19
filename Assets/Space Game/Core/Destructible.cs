using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructible : MonoBehaviour, IResetable {

	[SerializeField] private float health = 100f;
    public float Health => health;
    [SerializeField] private float maxHealth = 100f;
    public float MaxHealth => maxHealth;

	[SerializeField] private GameObject deathVfxPrefab = null;

    public bool IsAlive => health > 0f;
    public bool IsDead => health <= 0f;

	public void DoDamage(float amount) {
		if (IsDead)
			return;

		float actualDamage = Mathf.Min(health, amount);

        if (actualDamage <= 0)
            return;

		health -= actualDamage;
        OnDamaged?.Invoke(this, actualDamage);

		if (IsDead) {
			OnDeath();
		}
	}

    public System.Action<Destructible> OnDied;
    public System.Action<Destructible, float> OnDamaged;

	void OnDeath() {
		gameObject.SetActive(false);
		if (deathVfxPrefab != null)
			Instantiate(deathVfxPrefab, transform.position, transform.rotation, null);

        OnDied?.Invoke(this);
	}

    public void Reset() {
        health = maxHealth;
    }

}
