using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 2f;
    public Transform groundCheck; // child placed in front-bottom
    public Transform wallCheck;   // child placed in front
    public float checkRadius = 0.1f;
    public LayerMask groundLayer;

    [Header("Stats")]
    public int maxHealth = 1;

    [Header("Stun")]
    public float stunDuration = 0.5f;

    protected int currentHealth;
    protected Rigidbody2D rb;
    protected SpriteRenderer sr;
    protected bool facingRight = true;

    protected bool stunned = false;
    protected float stunTimer = 0f;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        currentHealth = maxHealth;
    }

    protected virtual void Update()
    {
        if (stunned)
        {
            stunTimer -= Time.deltaTime;
            if (stunTimer <= 0) stunned = false;
            return;
        }

        PatrolChecks();
    }

    protected virtual void PatrolChecks()
    {
        bool groundAhead = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);
        bool wallAhead = Physics2D.OverlapCircle(wallCheck.position, checkRadius, groundLayer) || Physics2D.OverlapCircle(wallCheck.position, checkRadius, LayerMask.GetMask("Enemy"));

        if (!groundAhead || wallAhead)
        {
            Flip();
        }
    }

    protected void Flip()
    {
        facingRight = !facingRight;
        Vector3 s = transform.localScale;
        s.x *= -1;
        transform.localScale = s;

        // SpriteRenderer flip is optional since we flip scale, but keep it consistent
        if (sr != null) sr.flipX = !sr.flipX;
    }

    public virtual void TakeDamage(int damage)
    {
        if (stunned) return; // brief invulnerability during stun

        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
            return;
        }

        // apply stun
        stunned = true;
        stunTimer = stunDuration;
    }

    protected virtual void Die()
    {
        GameManager.instance.CleanEnemy();
        Destroy(gameObject);
    }
}