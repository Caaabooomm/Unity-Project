using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [Header("Movimento")]
    [SerializeField] private float speed = 3f;
    [SerializeField] private float jumpForce = 7.25f;
    [SerializeField] private LayerMask layerGround;
    [SerializeField] private Transform sensorChao;
    [SerializeField] private float distanciaSensor = 0.12f;

    private bool grounded;
    private bool groundedLastFrame;
    private bool canDoubleJump;

    [Header("Vida")]
    [SerializeField] private int vidaMax = 3;
    private int vidaAtual;

    [Header("Invencibilidade")]
    [SerializeField] private float tempoInvencivel = 3f; // segundos
    private bool invencivel;

    [Header("Tiro")]
    [SerializeField] private GameObject prefabBullet;
    [SerializeField] private float forcaTiroNormal = 10f;
    [SerializeField] private float forcaTiroCarregado = 5f;
    [SerializeField] private float tempoNecessarioCarregar = 1f;
    private int documento;

    [Header("Animações / Componentes")]
    [SerializeField] private Animator animator;

    [Header("Boost (removido - reserva)")]
    [SerializeField, HideInInspector] private float speedOriginal;

    // estado privado
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Collider2D colPlayer;
    private Camera cam;
    private bool morto;

    // carregamento de tiro
    private bool carregandoTiro;
    private float tempoCarregando;

    // inspector defaults set above

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        colPlayer = GetComponent<Collider2D>();
        cam = Camera.main;

        if (animator == null)
            animator = GetComponent<Animator>();

        if (sensorChao == null)
            sensorChao = transform.Find("GroundCheck");

        vidaAtual = vidaMax;
        speedOriginal = speed;

        // inicializar GameManager (se existir) com valores iniciais
        if (GameManager.instancia != null)
            GameManager.instancia.ConfigurarPlayer(vidaMax, vidaAtual, documento);
    }

    void Update()
    {
        if (morto) return;

        AtualizarGroundCheck();
        Movimento();
        Pulo();

        HandleTiroInput();

        AtualizarAnimacoes();

        groundedLastFrame = grounded;
    }

    private void AtualizarAnimacoes()
    {
        if (animator == null) return;

        animator.SetBool("Idle", grounded && Mathf.Abs(rb.linearVelocity.x) < 0.05f);
        animator.SetBool("Walk", grounded && Mathf.Abs(rb.linearVelocity.x) >= 0.1f);
        animator.SetBool("Jump", !grounded);
        animator.SetBool("Invincible", invencivel);
    }

    private void AtualizarGroundCheck()
    {
        grounded = Physics2D.Raycast(sensorChao.position, Vector2.down, distanciaSensor, layerGround);

        if (grounded)
            canDoubleJump = true;
    }

    private void Movimento()
    {
        float eixo = Input.GetAxis("Horizontal");
        rb.linearVelocity = new Vector2(eixo * speed, rb.linearVelocity.y);

        if (eixo != 0)
            sr.flipX = eixo < 0;
    }

    private void Pulo()
    {
        if (!Input.GetButtonDown("Jump")) return;

        if (grounded)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            return;
        }

        if (canDoubleJump && documento > 0)
        {
            documento--;
            AtualizarDocumentoNoGameManager();
            canDoubleJump = false;
            if (animator != null) animator.SetTrigger("DoubleJump");
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    private void HandleTiroInput()
    {
        // Inicia carregamento
        if (Input.GetMouseButtonDown(0))
        {
            carregandoTiro = true;
            tempoCarregando = 0f;
        }

        // Mantendo carregando
        if (carregandoTiro && Input.GetMouseButton(0))
        {
            tempoCarregando += Time.deltaTime;
        }

        // Soltou o botão
        if (carregandoTiro && Input.GetMouseButtonUp(0))
        {
            carregandoTiro = false;

            if (tempoCarregando >= tempoNecessarioCarregar && documento > 0)
            {
                TiroCarregado();
            }
            else
            {
                AtirarNormal();
                if (animator != null) animator.SetTrigger("Shoot");
            }
        }
    }

    private void AtirarNormal()
    {
        if (documento <= 0) return;
        documento--;
        AtualizarDocumentoNoGameManager();

        Vector3 mp = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dir = (mp - transform.position).normalized;

        GameObject b = Instantiate(prefabBullet, transform.position, Quaternion.identity);
        // damage 1, velocity forcaTiroNormal, escala 1
        b.GetComponent<Bullet>().ConfigurarDirecao(dir, 1f, forcaTiroNormal, 1f);
    }

    private void TiroCarregado()
    {
        if (documento <= 0) return;
        documento--;
        AtualizarDocumentoNoGameManager();

        Vector3 mp = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dir = (mp - transform.position).normalized;

        GameObject b = Instantiate(prefabBullet, transform.position, Quaternion.identity);
        // damage 2, slower velocity, bigger scale
        b.GetComponent<Bullet>().ConfigurarDirecao(dir, 2f, forcaTiroCarregado, 1.5f);
        if (animator != null) animator.SetTrigger("Shoot");
    }

    public void AdicionarDocumento(int qtd)
    {
        documento += qtd;
        AtualizarDocumentoNoGameManager();
    }

    private void AtualizarDocumentoNoGameManager()
    {
        if (GameManager.instancia != null)
            GameManager.instancia.AtualizarDocumento(documento);
    }

    public void TomarDano(int dano)
    {
        if (morto || invencivel) return;

        vidaAtual -= dano;
        if (GameManager.instancia != null)
            GameManager.instancia.AtualizarVida(vidaAtual);

        if (vidaAtual <= 0)
        {
            Morrer();
            return;
        }

        StartCoroutine(InvencivelRoutine());
    }

    private IEnumerator InvencivelRoutine()
    {
        invencivel = true;

        // Ignore colisão com todos os inimigos
        EnemyAI[] inimigos = Object.FindObjectsByType<EnemyAI>(FindObjectsSortMode.None);
        foreach (var enemy in inimigos)
        {
            Collider2D colEnemy = enemy.GetComponent<Collider2D>();
            if (colEnemy != null)
                Physics2D.IgnoreCollision(colPlayer, colEnemy, true);
        }

        yield return new WaitForSeconds(tempoInvencivel);

        invencivel = false;

        // Reabilita colisões
        inimigos = Object.FindObjectsByType<EnemyAI>(FindObjectsSortMode.None);
        foreach (var enemy in inimigos)
        {
            Collider2D colEnemy = enemy.GetComponent<Collider2D>();
            if (colEnemy != null)
                Physics2D.IgnoreCollision(colPlayer, colEnemy, false);
        }

        // Se estiver sobre um inimigo, morre
        Collider2D overlap = Physics2D.OverlapCircle(transform.position, 0.2f);
        if (overlap != null && overlap.CompareTag("Enemy"))
        {
            Morrer();
        }
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (!invencivel)
        {
            EnemyAI e = col.collider.GetComponent<EnemyAI>();
            if (e != null && e.gameObject.activeSelf)
                TomarDano(1);
        }
    }

    private void Morrer()
    {
        morto = true;
        if (animator != null) animator.SetTrigger("Die");
        StartCoroutine(DelayMorte());
    }

    private IEnumerator DelayMorte()
    {
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // getters utilitários (se quiser expor leitura)
    public int GetVidaAtual() => vidaAtual;
    public int GetVidaMax() => vidaMax;
    public int Getdocumento() => documento;
}
