using UnityEngine;

public class EnemyWalker : MonoBehaviour
{
    public float velocidade = 2f;
    public string groundLayerName = "Ground";

    private Rigidbody2D rb;
    private SpriteRenderer sr;

    private Transform sensorDir;
    private Transform sensorEsq;

    private int direcao = -1; // começa para esquerda
    private int groundLayer;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        // pega os sensores pelo nome
        sensorDir = transform.Find("dir");
        sensorEsq = transform.Find("esq");

        groundLayer = LayerMask.NameToLayer(groundLayerName);

        if (!sensorDir || !sensorEsq)
            Debug.LogError("Faltando sensores 'dir' e 'esq' como children do inimigo.");

        Debug.Log("[EnemyWalker] Awake OK. Direção inicial: " + direcao);
    }

    void Update()
    {
        rb.linearVelocity = new Vector2(direcao * velocidade, rb.linearVelocityY);

        sr.flipX = direcao < 0;

        ChecarSensores();
    }

    void ChecarSensores()
    {
        // Raycast curtinho para frente de cada sensor
        RaycastHit2D hitDir = Physics2D.Raycast(sensorDir.position, Vector2.right, 0.05f, 1 << groundLayer);
        RaycastHit2D hitEsq = Physics2D.Raycast(sensorEsq.position, Vector2.left, 0.05f, 1 << groundLayer);

        if (hitDir.collider != null)
        {
            Debug.Log("[EnemyWalker] Sensor Direita detectou Ground. Virando para esquerda.");
            direcao = -1;
        }

        if (hitEsq.collider != null)
        {
            Debug.Log("[EnemyWalker] Sensor Esquerda detectou Ground. Virando para direita.");
            direcao = 1;
        }
    }

    void OnDrawGizmos()
    {
        // desenha gizmos no editor para debug
        if (Application.isPlaying && sensorDir && sensorEsq)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(sensorDir.position, sensorDir.position + Vector3.right * 0.05f);
            Gizmos.DrawLine(sensorEsq.position, sensorEsq.position + Vector3.left * 0.05f);
        }
    }
}
