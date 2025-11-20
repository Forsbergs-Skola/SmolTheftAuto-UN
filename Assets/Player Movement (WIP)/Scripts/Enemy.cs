using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float maxHealth;
    private float health;

    public float Health
    {
        get 
        {
            return health;
        }
        set
        {
            health = value;
            Debug.Log("Health: " + health);
            if (health <= 0)
                Destroy(gameObject);
        }
    }
    void Start() => health = maxHealth;
}
