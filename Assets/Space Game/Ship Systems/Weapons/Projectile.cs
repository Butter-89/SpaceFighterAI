using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public ShipSystem launcher;
    public float speed;
    public float impactDamage;

    public float lifetime = 0;
    public float maxLifetime = 10f;

    public GameObject impactVfxPrefab;

    public System.Action<Projectile, Destructible> OnImpact;

    private void Update() {
        lifetime += Time.deltaTime;
        if (lifetime >= maxLifetime)
            Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision) {
        Destructible destructible = collision.gameObject.GetComponent<Destructible>();
        if(destructible != null) {
            destructible.DoDamage(impactDamage);
            OnImpact?.Invoke(this, destructible);

            if (impactVfxPrefab != null) {
                GameObject impactVfx = Instantiate(impactVfxPrefab);
                impactVfx.transform.position = transform.position;
            }
        }

        Destroy(gameObject);
    }
}
