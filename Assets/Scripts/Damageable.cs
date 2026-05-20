using UnityEngine;
using MoreMountains.Feedbacks;


public class Damageable : MonoBehaviour
{
    public int health = 30;

    public MMFeedbacks takeDamageFeedbacks;

    public void TakeDamage(int amount)
    {
        health -= amount;

        takeDamageFeedbacks.PlayFeedbacks();

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Destroy(gameObject);
    }
}