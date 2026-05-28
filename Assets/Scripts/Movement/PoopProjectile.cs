using UnityEngine;

public class PoopProjectile : MonoBehaviour
{
    [SerializeField] private int damage = 5;

    private void Start()
    {
        Destroy(gameObject, 5f); 
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Poop hit: " + collision.gameObject.name); 

        
        Damageable damageable = collision.gameObject.GetComponentInParent<Damageable>();

        if (damageable != null)
        {
            damageable.TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}