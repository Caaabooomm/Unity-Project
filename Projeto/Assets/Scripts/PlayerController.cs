// PlayerController.cs
using UnityEngine;
// Adicionar este namespace para controle de cena
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpForce = 12f;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Documents")]
    public int documents = 0;

    [Header("Stats")]
    public int lives = 3;

    [Header("Abilities")]
    public Transform shootPoint;
    public GameObject bulletPrefab;
    public GameObject chargedBulletPrefab;
    public float shootCooldown = 1f;
    public float chargeTime = 1f;

    private Rigidbody2D rb;
    private SpriteRenderer sr;

    private bool isGrounded;
    private bool canDoubleJump = true;

    private float shootTimer;
    private float chargeTimer;
    private bool charging = false;
    private bool atirando = false;

    // NOVO: Flag para evitar dano múltiplo em um único toque
    private bool canTakeDamage = true;
    private float damageCooldown = 1.0f; // Tempo de invencibilidade após o dano

    private Vector3 originalShootPointLocalPosition;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        if (shootPoint != null)
        {
            originalShootPointLocalPosition = shootPoint.localPosition;
        }
    }

    void Update()
    {
        CheckGround();
        Move();
        Jump();
        HandleShooting();
    }

    // ... (CheckGround, Move, Jump, HandleShooting, GetBulletRotation permanecem, mas com correção de rb.velocity)

    void CheckGround()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        if (isGrounded) canDoubleJump = true;
    }

    void Move()
    {
        if (atirando) return;

        float x = Input.GetAxisRaw("Horizontal");
        // CORRIGIDO: rb.velocity
        rb.linearVelocity = new Vector2(x * moveSpeed, rb.linearVelocity.y);

        if (x != 0)
        {
            sr.flipX = x < 0;
        }

        if (shootPoint != null)
        {
            Vector3 newPos = originalShootPointLocalPosition;

            if (sr.flipX)
            {
                newPos.x *= -1;
            }

            shootPoint.localPosition = newPos;
        }
    }

    void Jump()
    {
        if (atirando) return;

        if (Input.GetButtonDown("Jump"))
        {
            if (isGrounded)
            {
                // CORRIGIDO: rb.velocity
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            }
            else if (canDoubleJump && documents > 0)
            {
                documents--;
                canDoubleJump = false;
                // CORRIGIDO: rb.velocity
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            }
        }
    }

    void HandleShooting()
    {
        shootTimer -= Time.deltaTime;

        if (atirando && shootTimer > 0) return;
        if (atirando && shootTimer <= 0 && !charging) return;


        if (Input.GetKeyDown(KeyCode.X) && shootTimer <= 0 && documents > 0)
        {
            charging = true;
            chargeTimer = 0;
        }

        if (charging)
        {
            chargeTimer += Time.deltaTime;

            if (Input.GetKeyUp(KeyCode.X))
            {
                charging = false;

                if (chargeTimer >= chargeTime && documents >= 3)
                {
                    ShootCharged();
                }
                else
                {
                    ShootNormal();
                }

                chargeTimer = 0;
            }
        }
    }

    Quaternion GetBulletRotation()
    {
        if (sr.flipX)
        {
            return Quaternion.Euler(0, 0, 180f);
        }
        return shootPoint.rotation;
    }

    void ShootNormal()
    {
        documents--;
        shootTimer = shootCooldown;
        atirando = true;

        // CORRIGIDO: rb.velocity
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;

        Instantiate(bulletPrefab, shootPoint.position, GetBulletRotation());
        Invoke(nameof(RestoreMovement), 0.1f);
    }

    void ShootCharged()
    {
        documents -= 3;
        shootTimer = shootCooldown;
        atirando = true;

        // CORRIGIDO: rb.velocity
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;

        Instantiate(chargedBulletPrefab, shootPoint.position, GetBulletRotation());
        Invoke(nameof(RestoreMovement), 0.5f);
    }

    void RestoreMovement()
    {
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 2.5f;
        atirando = false;
    }

    // =======================================================
    // NOVO: FUNÇÕES DE DANO
    // =======================================================

    // Função pública para outros scripts (inimigos) chamarem
    public void TakeDamage(int damage)
    {
        if (!canTakeDamage) return; // Ignora se estiver em cooldown de dano

        lives -= damage;
        Debug.Log("Player sofreu dano! Vidas restantes: " + lives);

        if (lives <= 0)
        {
            Die();
        }
        else
        {
            // Aplica cooldown de invencibilidade após levar dano
            canTakeDamage = false;
            // Opcional: Adicione um visual de piscada aqui (ex: InvokeRepeating no SpriteRenderer)
            Invoke(nameof(ResetDamageCooldown), damageCooldown);
        }
    }

    void ResetDamageCooldown()
    {
        canTakeDamage = true;
        // Opcional: Pare o visual de piscada aqui
    }

    void Die()
    {
        Debug.Log("Player morreu! Reiniciando cena...");

        // Obtém o índice da cena atual
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        // Reinicia a cena
        SceneManager.LoadScene(currentSceneIndex);

        // Opcional: Destruir o objeto Player se você quiser atrasar o load da cena.
        // Destroy(gameObject); 
    }
}