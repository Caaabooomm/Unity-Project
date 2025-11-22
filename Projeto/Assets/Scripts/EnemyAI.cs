using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public enum TipoInimigo { Basico, Caçador }
    public TipoInimigo tipo = TipoInimigo.Basico;

    [Header("Movimentação")]
    public float velocidade = 2f;
    public float forcaPulo = 8f;
    public float distanciaSensor = 0.2f;

    [Header("Perseguição (apenas Caçador)")]
    public float alcanceDeteccao = 6f;
    private Transform player;

    [Header("Sensores")]
    public Transform sensorParede;
    public Transform sensorChao;
    public Transform sensorAlto;

    [Header("Conversão")]
    public int balasNecessarias = 3;
    private int balasRecebidas = 0;
    public bool isEvil = true;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private int direcao = 1;
    private bool grounded;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (!isEvil) return;

        AtualizarGrounded();

        bool parede = Physics2D.Raycast(sensorParede.position, Vector2.right * direcao, distanciaSensor, LayerMask.GetMask("Ground"));
        bool semChao = !Physics2D.Raycast(sensorChao.position, Vector2.down, distanciaSensor, LayerMask.GetMask("Ground"));

        if (tipo == TipoInimigo.Basico)
        {
            MovimentarBasico(parede, semChao);
        }
        else
        {
            MovimentarCacador(parede, semChao);
        }

        sr.flipX = direcao < 0;
    }

    void MovimentarBasico(bool parede, bool semChao)
    {
        if (parede || semChao)
            direcao *= -1;

        rb.linearVelocity = new Vector2(direcao * velocidade, rb.linearVelocity.y);
    }

    void MovimentarCacador(bool parede, bool semChao)
    {
        float distPlayer = Vector2.Distance(transform.position, player.position);

        if (distPlayer <= alcanceDeteccao)
        {
            direcao = player.position.x > transform.position.x ? 1 : -1;
        }
        else if (parede || semChao)
        {
            direcao *= -1;
        }

        rb.linearVelocity = new Vector2(direcao * velocidade, rb.linearVelocity.y);

        if (parede)
            TentarPular();
    }

    void TentarPular()
    {
        if (!grounded) return;

        bool espacoLivre = !Physics2D.Raycast(sensorAlto.position, Vector2.right * direcao, distanciaSensor, LayerMask.GetMask("Ground"));

        if (espacoLivre)
        {
            rb.AddForce(Vector2.up * forcaPulo, ForceMode2D.Impulse);
        }
    }

    void AtualizarGrounded()
    {
        grounded = Physics2D.Raycast(transform.position, Vector2.down, 0.1f, LayerMask.GetMask("Ground"));
    }

    public void LevarTiro()
    {
        balasRecebidas++;
        if (balasRecebidas >= balasNecessarias)
        {
            isEvil = false;
            gameObject.SetActive(false);
        }
    }
}
