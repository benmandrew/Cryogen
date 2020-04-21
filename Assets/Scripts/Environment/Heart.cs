using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heart : MonoBehaviour
{
    public float maxHealth;
    public GameObject destroyedCryopod;
    
    private float health;
    
    void Start()
    {
        health = maxHealth;
    }

    public void damage(float damage) {
        health -= damage;
        
        PlayerUIController.instance.SetHeartHealthPercent(health / maxHealth);
        
        if (health <= 0.0f)
        {
            Die();
        }
    }

    private void Die()
    {
        GameManager.instance.heartDead = true;
        
        Instantiate(destroyedCryopod, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
