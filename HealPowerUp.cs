using UnityEngine;

public class HealPowerUp : MonoBehaviour
{
    public int healthPoints;
    public AudioClip pickUpSound;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if(Playerhealth.instance.currentHealth != Playerhealth.instance.maxHealth)
            {
                AudioManager.instance.PlayClipAt(pickUpSound, transform.position);
                Playerhealth.instance.HealPlayer(healthPoints);
                Destroy(gameObject);
            }            
        }
    }
}
