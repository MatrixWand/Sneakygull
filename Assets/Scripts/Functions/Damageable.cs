

using UnityEngine;

public class Damageable : MonoBehaviour
{

    
    [SerializeField] private int health = 3;
    private bool isDead = false;
    public void TakeDamage(int damage)
    {
        if (isDead) return; 

        health -= damage;

        if (health <= 0)
        {
            isDead = true; 

            GameManager.Instance.TargetDestroyed();
            Destroy(gameObject);
        }
    }

    private void Die()
    {
        Debug.Log(gameObject.name + " destroyed");

        
        if (GameManager.Instance != null)
        {
            GameManager.Instance.TargetDestroyed();
        }

        Destroy(gameObject);
    }
}