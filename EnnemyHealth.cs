using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 30;
    private int currentHealth;

    public float knockbackForce = 5f;
    public float knockbackDuration = 0.15f;

    private Rigidbody2D rb;
    private bool isKnockedBack = false;
    public AudioClip sound;

    void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();

        // Kinematic par défaut, la physique ne l'affecte pas
        rb.bodyType = RigidbodyType2D.Kinematic;
    }

    public void TakeDamage(int damage, Vector2 hitDirection)
    {
        if (isKnockedBack) return;

        currentHealth -= damage;
        GhostAI ghostAI = GetComponent<GhostAI>();
        if (ghostAI != null)
        {
            StartCoroutine(ghostAI.Stun(hitDirection));
        }

        else
        {
            StartCoroutine(ApplyKnockback(hitDirection));
        }
            
        if (currentHealth <= 0)
            Die();
    }

    private IEnumerator ApplyKnockback(Vector2 hitDirection)
    {
        isKnockedBack = true;

        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionY;
        rb.gravityScale = 0.3f;
        rb.linearVelocity = Vector2.zero;
        
        // Force uniquement sur l'axe horizontal
        rb.AddForce(new Vector2(hitDirection.x * knockbackForce, 0f), ForceMode2D.Impulse);

        yield return new WaitForSeconds(knockbackDuration);

        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 0f;
        rb.bodyType = RigidbodyType2D.Kinematic;

        isKnockedBack = false;
    }

    private void Die()
    {
        AudioManager.instance.PlayClipAt(sound, transform.position);
        // Pour l'instant on détruit simplement l'ennemi
        // On pourra y ajouter une animation de mort plus tard
        Destroy(gameObject);
    }
}