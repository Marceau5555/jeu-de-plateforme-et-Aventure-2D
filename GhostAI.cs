using UnityEngine;
using System.Collections;

/*



 A LIRE AVANT DE REGARDER LE CODE !!!!!!!

 Ce code n'est pas intéressant à inspecter, il y a été à 95% réalisé par IA. 
 Dans une optique d'apprentissage et pour faire profiter les copains d'un monstre avec une difficulté supplémentaire.





*/


public class GhostAI : MonoBehaviour
{
    [Header("Patrouille")]
    public float wanderSpeed = 2f;
    public float wanderRadiusX = 4f;
    public float wanderRadiusY = 2f;
    public float waitTimeMin = 1f;
    public float waitTimeMax = 3f;

    [Header("Détection et poursuite")]
    public float detectionRadiusX = 8f;
    public float detectionRadiusY = 4f;
    public float chaseMaxDistance = 10f; // distance max avant d'abandonner
    public float chaseSpeed = 4f;
    public float attackRange = 1.5f;
    public int damageOnContact = 10;
    public float attackCooldown = 1.5f;
    private Vector2 startPosition;
    private Vector2 targetPosition;
    private bool isWaiting = false;
    private float waitTimer = 0f;
    private bool isChasing = false;
    private bool isAttacking = false;
    private bool isStunned = false;

    private Rigidbody2D rb;
    public SpriteRenderer spriteRenderer;
    private Transform player;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;

        startPosition = transform.position;
        PickNewTarget();

        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        
        if (isAttacking || isStunned)
        {
            return;
        }


        if (isAttacking) return;

        Vector2 diff = (Vector2)player.position - startPosition;
        float distanceToPlayer = Vector2.Distance(startPosition, player.position);

        bool wasChasing = isChasing;

        if (Mathf.Abs(diff.x) <= detectionRadiusX && Mathf.Abs(diff.y) <= detectionRadiusY)
            isChasing = true;

        if (distanceToPlayer > chaseMaxDistance)
            isChasing = false;

        // Reset complet quand il arrête de chasser
        if (wasChasing && !isChasing)
        {
            rb.linearVelocity = Vector2.zero;
            isWaiting = true;
            waitTimer = Random.Range(waitTimeMin, waitTimeMax);
            PickNewTarget();
        }

        if (isChasing)
        {
            if (Vector2.Distance(transform.position, player.position) <= attackRange)
            {
                rb.linearVelocity = Vector2.zero;
                StartCoroutine(Attack());
            }
            else
            {
                ChasePlayer();
            }
        }
        else
        {
            Wander();
        }
    }
    IEnumerator Attack()
    {
        isAttacking = true;
        rb.linearVelocity = Vector2.zero;

        // Applique les dégâts
        Playerhealth playerHealth = player.GetComponent<Playerhealth>();
        if (playerHealth != null)
            playerHealth.TakeDamage(damageOnContact);

        // Knockback uniquement horizontal
        Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
        if (playerRb != null)
        {
            PlayerMovement.instance.isKnockedBack = true;
            Vector2 knockDir = new Vector2(Mathf.Sign(player.position.x - transform.position.x), 0f);
            playerRb.AddForce(knockDir * 5f, ForceMode2D.Impulse);
            yield return new WaitForSeconds(0.2f);
            PlayerMovement.instance.isKnockedBack = false;
        }

        yield return new WaitForSeconds(attackCooldown);
        isAttacking = false;
    }
        void ChasePlayer()
        {
            Vector2 currentPos = transform.position;
            Vector2 playerPos = player.position;
            
            // Recalcule la direction complète à chaque frame
            Vector2 dir = (playerPos - currentPos).normalized;
            
            // Applique directement la vélocité sans Lerp
            rb.linearVelocity = dir * chaseSpeed;

            if (dir.x > 0.1f)
                spriteRenderer.flipX = false;
            else if (dir.x < -0.1f)
                spriteRenderer.flipX = true;
        }

    void Wander()
    {
        if (isWaiting)
        {
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0f)
            {
                isWaiting = false;
                PickNewTarget();
            }
            return;
        }

        Vector2 dir = (targetPosition - (Vector2)transform.position).normalized;
        rb.linearVelocity = dir * wanderSpeed;

        if (dir.x > 0.1f)
            spriteRenderer.flipX = false;
        else if (dir.x < -0.1f)
            spriteRenderer.flipX = true;

        if (Vector2.Distance(transform.position, targetPosition) < 0.3f)
        {
            rb.linearVelocity = Vector2.zero;
            isWaiting = true;
            waitTimer = Random.Range(waitTimeMin, waitTimeMax);
        }
    }

    void PickNewTarget()
    {
        float randomX = Random.Range(-wanderRadiusX, wanderRadiusX);
        float randomY = Random.Range(-wanderRadiusY, wanderRadiusY);
        targetPosition = startPosition + new Vector2(randomX, randomY);
    }


    public IEnumerator Stun(Vector2 hitDirection)
    {
        isStunned = true;
        
        // Passe en Dynamic pour recevoir le knockback
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 0.1f;
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(hitDirection * 2f, ForceMode2D.Impulse);
        
        yield return new WaitForSeconds(0.5f);
        
        // Remet en Kinematic après
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;
        isStunned = false;
    }
    private void OnDrawGizmos()
    {
        Vector3 origin = Application.isPlaying ? (Vector3)startPosition : transform.position;

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(origin, new Vector3(wanderRadiusX * 2, wanderRadiusY * 2, 0f));
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(origin, new Vector3(detectionRadiusX * 2, detectionRadiusY * 2, 0f));
    }
}