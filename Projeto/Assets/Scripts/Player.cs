using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;


public class Player : MonoBehaviour
{
    [Header("Atributos")]
    public float velocidade = 3f;
    public float forcaDePulo = 7.25f;

    public int vidaMax = 3;
    public int vidaAtual;

    public int municao = 0;

    [Header("Invencibilidade")]
    public float tempoInvencivel = 2f;
    private bool invencivel = false;

    [Header("Tiro")]
    public GameObject prefabBala;
    public float forcaTiro = 400f;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Camera cam;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        cam = Camera.main;

        vidaAtual = vidaMax;
        Debug.Log("[Player] Iniciado com " + vidaAtual + " vidas.");
    }

    void Update()
    {
        Movimento();
        Pulo();

        if (Input.GetMouseButtonDown(0))
            Atirar();
    }

    void Movimento()
    {
        float eixo = Input.GetAxis("Horizontal");
        rb.linearVelocity = new Vector2(eixo * velocidade, rb.linearVelocityY);

        if (eixo != 0)
            sr.flipX = eixo < 0;
    }

    void Pulo()
    {
        if (Input.GetButtonDown("Jump"))
        {
            rb.AddForce(Vector2.up * forcaDePulo, ForceMode2D.Impulse);
            Debug.Log("[Player] Pulou.");
        }
    }

    void Atirar()
    {
        if (municao <= 0)
        {
            Debug.Log("[Player] Tentou atirar sem munição.");
            return;
        }

        municao--;

        Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direcao = mousePos - transform.position;
        direcao.Normalize();

        GameObject b = Instantiate(prefabBala, transform.position, Quaternion.identity);

        Bala bala = b.GetComponent<Bala>();
        bala.ConfigurarDirecao(direcao);

        Rigidbody2D rbBala = b.GetComponent<Rigidbody2D>();
        rbBala.linearVelocity = direcao * forcaTiro;

        Debug.Log("[Player] Disparou. Munição restante: " + municao);
    }



    public void AdicionarMunicao(int qtd)
    {
        municao += qtd;
        Debug.Log("[Player] + " + qtd + " munição. Total: " + municao);
    }

    public void TomarDano(int dano)
    {
        if (invencivel)
        {
            Debug.Log("[Player] Tentou tomar dano invencível.");
            return;
        }

        vidaAtual -= dano;
        Debug.Log("[Player] Tomou " + dano + " dano. Vidas: " + vidaAtual);

        if (vidaAtual <= 0)
        {
            Debug.Log("[Player] Morreu. Reiniciando fase.");
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            return;
        }

        StartCoroutine(RoutineInvencivel());
    }

    IEnumerator RoutineInvencivel()
    {
        invencivel = true;
        Debug.Log("[Player] Invencível por " + tempoInvencivel);

        yield return new WaitForSeconds(tempoInvencivel);

        invencivel = false;
        Debug.Log("[Player] Invencibilidade terminou.");
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        EnemyAI e = col.collider.GetComponent<EnemyAI>();

        if (e != null && e.isEvil)
        {
            Debug.Log("[Player] Tocou inimigo mal.");
            TomarDano(1);
        }
    }
}
