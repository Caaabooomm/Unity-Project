// PlayerController.cs
using UnityEngine;

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

    void CheckGround()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        if (isGrounded) canDoubleJump = true;
    }

    void Move()
    {
        if (atirando) return;

        float x = Input.GetAxisRaw("Horizontal");
        // CORREÇÃO: rb.linearVelocity não existe em Rigidbody2D. O correto é rb.velocity.
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
                // CORREÇÃO: rb.linearVelocity não existe. O correto é rb.velocity.
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            }
            else if (canDoubleJump && documents > 0)
            {
                documents--;
                canDoubleJump = false;
                // CORREÇÃO: rb.linearVelocity não existe. O correto é rb.velocity.
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

    // Função auxiliar para calcular a rotação do projétil
    Quaternion GetBulletRotation()
    {
        // Se flipX for true (virado para a esquerda), rotaciona 180 graus (espalha no eixo Y).
        if (sr.flipX)
        {
            return Quaternion.Euler(0, 0, 180f);
        }
        // Se flipX for false (virado para a direita), usa a rotação padrão (geralmente Quaternion.identity ou shootPoint.rotation se for 0).
        return shootPoint.rotation;
    }

    void ShootNormal()
    {
        documents--;
        shootTimer = shootCooldown;
        atirando = true;

        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;

        // MUDANÇA: Usa a rotação calculada para garantir que a bala aponte para a direção correta.
        Instantiate(bulletPrefab, shootPoint.position, GetBulletRotation());
        Invoke(nameof(RestoreMovement), 0.1f);
    }

    void ShootCharged()
    {
        documents -= 3;
        shootTimer = shootCooldown;
        atirando = true;

        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;

        // MUDANÇA: Usa a rotação calculada para garantir que a bala aponte para a direção correta.
        Instantiate(chargedBulletPrefab, shootPoint.position, GetBulletRotation());
        Invoke(nameof(RestoreMovement), 0.5f);
    }

    void RestoreMovement()
    {
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 2.5f;
        atirando = false;
    }
}