using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed;
    public float jumpForce;
    public float climbSpeed = 5f;

    private bool isJumping;
    public bool isGrounded;
    public bool isClimbing;

    public Transform groundCheck;
    public float groundCheckRadius;
    public LayerMask collisionLayers;

    public Rigidbody2D rb;
    public Animator animator;
    public SpriteRenderer spriteRenderer;
    public CapsuleCollider2D playerCollider;
    private Vector3 velocity = Vector3.zero;

    public bool isKnockedBack = false;

    public float dashForce = 18f;
    public float dashDuration = 0.15f;
    public float dashCooldown = 2f;
    public float dashRechargeTime = 1f;
    public int maxDashCharges = 3;

    private int dashCharges = 3;
    private bool isDashing;
    private float dashTimer;
    private float dashCooldownTimer;
    private float rechargeTimer;

    float horizontalMovement;
    float verticalMovement;


    public Transform attackPoint;

    public static PlayerMovement instance;


    private void Awake()
        {
            if(instance != null)
            {
                Debug.LogWarning("Il y a plus d'une instance de PlayerMovement dans la scène");
                return;
            }
            instance = this;
        }

    void Update()
    {
        horizontalMovement = Input.GetAxis("Horizontal") * moveSpeed;
        verticalMovement = Input.GetAxis("Vertical") * climbSpeed; // plus de deltaTime ici

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            isJumping = true;
        }


        if (dashCharges < maxDashCharges)
        {
            rechargeTimer += Time.deltaTime;
            if (rechargeTimer >= dashRechargeTime)
            {
                dashCharges++;
                rechargeTimer = 0f;
            }
        }
        else
        {
            rechargeTimer = 0f;
        }

        if (Input.GetKeyDown(KeyCode.Q) && !isDashing && dashCooldownTimer <= 0f && dashCharges > 0)
        {
            StartDash();           
        }


        if (isDashing)
        {
            dashTimer -= Time.deltaTime;
            if (dashTimer <= 0f)
                isDashing = false;
        }

        if (dashCooldownTimer > 0f)
        {
            dashCooldownTimer -= Time.deltaTime;
        }


        DashBar.instance.UpdateDashBar(dashCharges, rechargeTimer / dashRechargeTime);
    }

    void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, collisionLayers);

        if (!isDashing)
            MovePlayer(horizontalMovement, verticalMovement);

        Flip(rb.linearVelocity.x);

        float characterVelocity = Mathf.Abs(rb.linearVelocity.x);
        animator.SetFloat("Speed", characterVelocity);
        animator.SetBool("IsClimbing", isClimbing);
    }

    void MovePlayer(float _horizontalMovement, float _verticalMovement)
    {
        if (!isClimbing)
        {
            rb.gravityScale = 1f;

            Vector2 targetVelocity = new Vector2(_horizontalMovement, rb.linearVelocity.y);
            rb.linearVelocity = Vector3.SmoothDamp(rb.linearVelocity, targetVelocity, ref velocity, .05f);

            if (isJumping)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                isJumping = false;
            }
        }
        else
        {
            rb.gravityScale = 0f;

            Vector2 targetVelocity = new Vector2(0f, _verticalMovement);
            rb.linearVelocity = Vector3.SmoothDamp(rb.linearVelocity, targetVelocity, ref velocity, .05f);
        }
    }

    void StartDash()
    {
        isDashing = true;
        dashTimer = dashDuration;
        dashCooldownTimer = dashCooldown;
        dashCharges--;

        float direction = spriteRenderer.flipX ? -1f : 1f;
        rb.linearVelocity = new Vector2(direction * dashForce, 0f);
    }

    void Flip(float _velocity)
    {
        if (isKnockedBack) return; 

        if (_velocity > 0.1f)
        {
            spriteRenderer.flipX = false;
            attackPoint.localPosition = new Vector3(Mathf.Abs(attackPoint.localPosition.x), attackPoint.localPosition.y, 0f);
            WeaponManager.instance.slashRenderer.flipX = false;
        }
        else if (_velocity < -0.1f)
        {
            spriteRenderer.flipX = true;
            attackPoint.localPosition = new Vector3(-Mathf.Abs(attackPoint.localPosition.x), attackPoint.localPosition.y, 0f);
            WeaponManager.instance.slashRenderer.flipX = true;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}