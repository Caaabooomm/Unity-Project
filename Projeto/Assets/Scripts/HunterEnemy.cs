using UnityEngine;

public class HunterEnemy : EnemyBase
{
    [Header("Hunter")]
    public float chaseSpeed = 3.5f;
    public float chaseRange = 5f;
    public float maxChaseTime = 10f;

    // NOVO: Checagem de Borda
    public Transform edgeCheck;
    public float edgeCheckDistance = 0.5f; // Distância à frente para checar

    private Transform player;
    private bool chasing = false;
    private float chaseTimer = 0f;

    // NOVO: Adicionado para evitar que a velocidade de patrulha pise na detecção de borda a cada frame
    private bool patrolling = false;

    protected override void Start()
    {
        base.Start();
        maxHealth = 2;
        currentHealth = maxHealth;
    }

    protected override void Update()
    {
        // Detectar Player
        if (player == null)
        {
            GameObject pObj = GameObject.FindWithTag("Player");
            if (pObj != null) player = pObj.transform;
        }

        base.Update();

        // --- Lógica de Chase Detection e Timer ---
        if (!stunned)
        {
            if (player != null)
            {
                float dist = Vector2.Distance(transform.position, player.position);
                if (dist <= chaseRange)
                {
                    if (!chasing)
                    {
                        chasing = true;
                        chaseTimer = maxChaseTime;
                    }
                }
            }

            if (chasing)
            {
                chaseTimer -= Time.deltaTime;
                if (chaseTimer <= 0)
                {
                    chasing = false;
                }
            }
        }

        // --- Lógica de Movimento ---
        if (!stunned)
        {
            if (chasing && player != null)
            {
                patrolling = false; // Parar a lógica de patrulha

                // Perseguir o jogador
                float dirf = Mathf.Sign(player.position.x - transform.position.x);
                if ((dirf > 0 && !facingRight) || (dirf < 0 && facingRight))
                    Flip();

                float dir = facingRight ? 1f : -1f;
                // CORREÇÃO 2: rb.velocity
                rb.linearVelocity = new Vector2(dir * chaseSpeed, rb.linearVelocity.y);
            }
            else // Patrulha
            {
                patrolling = true;

                // NOVO: Checagem de Borda e de Parede
                if (ShouldTurn())
                {
                    Flip();
                }

                // Patrulhar
                float dir = facingRight ? 1f : -1f;
                // CORREÇÃO 3: rb.velocity
                rb.linearVelocity = new Vector2(dir * moveSpeed, rb.linearVelocity.y);
            }
        }
        else // Atordoado (Stunned)
        {
            rb.linearVelocity = Vector2.zero;
            patrolling = false;
        }
    }

    // NOVO: Função para checar se deve virar (Parede ou Borda)
    private bool ShouldTurn()
    {
        // 1. Checa por Parede (usando raycast no sentido do movimento)
        Vector2 direction = facingRight ? Vector2.right : Vector2.left;
        RaycastHit2D wallCheck = Physics2D.Raycast(transform.position, direction, edgeCheckDistance, groundLayer);

        // 2. Checa por Borda
        // (Verifica se há chão ligeiramente à frente, e se não houver, deve virar)
        bool noEdge = !Physics2D.Raycast(edgeCheck.position, Vector2.down, edgeCheckDistance, groundLayer);

        // Deve virar se: Atingiu uma parede OU não há chão (borda)
        return wallCheck.collider != null || noEdge;
    }

    // Se o hunter colidir com o player, causa dano e interrompe a perseguição
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 1. LÓGICA DE DANO
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController playerScript = collision.gameObject.GetComponent<PlayerController>();

            if (playerScript != null)
            {
                playerScript.TakeDamage(1); // Causa 1 de dano
            }

            // Interrompe a perseguição após contato
            chasing = false;
        }

        // 2. LÓGICA DE INVERSÃO DE PATRULHA
        if (patrolling)
        {
            if (!collision.gameObject.CompareTag("Player") && !collision.gameObject.GetComponent<Collider2D>().isTrigger)
            {
                Flip();
            }
        }
    }
}