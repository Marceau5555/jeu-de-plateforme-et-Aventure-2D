using UnityEngine;
using System.Collections;

public class WeaponManager : MonoBehaviour
{
    public Item equippedWeapon;
    public Transform attackPoint;
    public LayerMask enemyLayers;
    public Animator slashAnimator;
    public SpriteRenderer slashRenderer;
    private float cooldownTimer = 0f;
    private bool isAttacking = false;

    public AudioClip sound;

    public static WeaponManager instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Il y a plus d'une instance de WeaponManager");
            return;
        }
        instance = this;
    }

    void Update()
    {
        if (cooldownTimer > 0f)
            cooldownTimer -= Time.deltaTime;

        if (Input.GetMouseButtonDown(1) && equippedWeapon != null && equippedWeapon.isWeapon && cooldownTimer <= 0f && !isAttacking)
            StartCoroutine(AttackRoutine());
    }

    IEnumerator AttackRoutine()
    {
        AudioManager.instance.PlayClipAt(sound, transform.position);
        isAttacking = true;
        cooldownTimer = equippedWeapon.weaponCooldown;

        // Active le slash et joue l'animation
        slashRenderer.enabled = true;
        slashAnimator.SetTrigger("Attack");

        yield return new WaitForSeconds(0.08f);

        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position, equippedWeapon.weaponRange, enemyLayers);

        foreach (Collider2D hit in hits)
        {
            EnemyHealth enemy = hit.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                Vector2 knockDir = (hit.transform.position - attackPoint.position).normalized;
                enemy.TakeDamage(equippedWeapon.weaponDamage, knockDir);
            }
        }

        yield return new WaitForSeconds(0.24f - 0.08f);

        // Désactive le slash à la fin
        slashRenderer.enabled = false;

        isAttacking = false;
    }

    private void OnDrawGizmos()
    {
        if (equippedWeapon == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(attackPoint.position, equippedWeapon.weaponRange);
    }
}