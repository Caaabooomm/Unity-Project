using UnityEngine;

public class Bala : MonoBehaviour
{
    public float vidaDaBala = 5f;
    public int ricochetesRestantes = 2;
    public float velocidade = 6f;

    public LayerMask layerInimigo;
    public LayerMask layerChao;

    private Rigidbody2D rb;
    private float tempoVivo = 0;
    private Vector2 direcao;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void ConfigurarDirecao(Vector2 d)
    {
        direcao = d.normalized;
    }

    void Update()
    {
        tempoVivo += Time.deltaTime;
        if (tempoVivo >= vidaDaBala)
            Destroy(gameObject);
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + direcao * velocidade * Time.fixedDeltaTime);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        int layer = col.collider.gameObject.layer;

        // Colisão com inimigo
        if (((1 << layer) & layerInimigo) != 0)
        {
            EnemyAI inimigo = col.collider.GetComponent<EnemyAI>();

            if (inimigo != null)
                inimigo.LevarTiro();

            Destroy(gameObject);
            return;
        }

        // Colisão com chão
        if (((1 << layer) & layerChao) != 0)
        {
            if (ricochetesRestantes > 0)
            {
                ricochetesRestantes--;

                Vector2 normal = col.contacts[0].normal;
                direcao = Vector2.Reflect(direcao, normal).normalized;
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

}
