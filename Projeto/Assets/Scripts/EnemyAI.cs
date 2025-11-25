using UnityEngine;
using System.Collections;

public class EnemyAI : MonoBehaviour
{
    public enum TipoInimigo { Patrulheiro, Algoz }
    public TipoInimigo tipo = TipoInimigo.Patrulheiro;

    [Header("Movimento Geral")]
    public float velocidade = 2f;
    public Transform sensorChao;
    public Transform sensorParede;
    public float distanciaSensor = 0.25f;
    private int direcao = 1;

    [Header("Algoz")]
    public float raioDeteccao = 5f;
    public float velocidadePerseguicao = 3.5f;
    public float tempoMaximoPerseguicao = 10f;
    private float tempoRestante;
    private bool perseguindo = false;
    private Transform alvoPlayer;

    [Header("Vida")]
    public float vidaMax = 20f;
    private float vidaAtual;
    public bool ativo = true;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Animator anim;
    private int layerGround;
    private int layerPlayer;
    private bool morto = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        layerGround = LayerMask.GetMask("Ground");
        layerPlayer = LayerMask.GetMask("Player");

        vidaAtual = vidaMax;

        if (GameManager.instancia != null)
            GameManager.instancia.RegistrarInimigo();
        else
            Debug.LogError("[EnemyAI] GameManager não encontrado.");

        tempoRestante = tempoMaximoPerseguicao;
    }

    void Update()
    {
        if (!ativo || morto) return;

        switch (tipo)
        {
            case TipoInimigo.Patrulheiro:
                Patrulhar();
                break;

            case TipoInimigo.Algoz:
                AlgozUpdate();
                break;
        }
    }

    void Patrulhar()
    {
        bool semChao = !Physics2D.Raycast(sensorChao.position, Vector2.down, distanciaSensor, layerGround);
        bool parede = Physics2D.Raycast(sensorParede.position, Vector2.right * direcao, distanciaSensor, layerGround);

        if (semChao || parede)
            Virar();

        rb.linearVelocity = new Vector2(direcao * velocidade, rb.linearVelocity.y);

        AtualizarAnimacoes(rb.linearVelocity.x);
    }

    void AlgozUpdate()
    {
        if (!perseguindo)
        {
            DetectarPlayer();
            if (!perseguindo)
            {
                Patrulhar();
                return;
            }
        }

        Perseguir();
    }

    void DetectarPlayer()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, raioDeteccao, layerPlayer);

        if (hit != null)
        {
            alvoPlayer = hit.transform;
            perseguindo = true;
            tempoRestante = tempoMaximoPerseguicao;

            anim.SetTrigger("DetectPlayer");
        }
    }

    void Perseguir()
    {
        if (alvoPlayer == null)
        {
            ResetarParaPatrulha();
            return;
        }

        tempoRestante -= Time.deltaTime;
        if (tempoRestante <= 0f)
        {
            ResetarParaPatrulha();
            return;
        }

        float dirX = Mathf.Sign(alvoPlayer.position.x - transform.position.x);
        direcao = (int)dirX;

        rb.linearVelocity = new Vector2(dirX * velocidadePerseguicao, rb.linearVelocity.y);

        AtualizarAnimacoes(rb.linearVelocity.x);
    }

    void ResetarParaPatrulha()
    {
        perseguindo = false;
        alvoPlayer = null;
        anim.SetTrigger("LosePlayer");
    }

    void AtualizarAnimacoes(float velocidadeX)
    {
        bool andando = Mathf.Abs(velocidadeX) > 0.1f;

        anim.SetBool("Walk", andando);
        anim.SetBool("Idle", !andando);
    }

    public void LevarTiro(float dano)
    {
        if (!ativo || morto) return;

        vidaAtual -= dano;

        if (vidaAtual <= 0)
            StartCoroutine(MorrerEnemy());
    }

    void Virar()
    {
        direcao *= -1;
        Vector3 esc = transform.localScale;
        esc.x *= -1;
        transform.localScale = esc;
    }

    IEnumerator MorrerEnemy()
    {
        ativo = false;
        morto = true;

        anim.SetTrigger("Die");

        yield return new WaitForSeconds(1f);

        GameManager.instancia.InimigoConvertido();
        gameObject.SetActive(false);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (!ativo || morto) return;

        if (col.collider.CompareTag("Player"))
            anim.SetBool("Attack", true);
    }

    void OnCollisionExit2D(Collision2D col)
    {
        if (col.collider.CompareTag("Player"))
            anim.SetBool("Attack", false);
    }

}
