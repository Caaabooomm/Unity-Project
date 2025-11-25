using UnityEngine;
using UnityEngine.SceneManagement;


public class Bullet : MonoBehaviour
{
    public float vidaDaBala = 5f;

    private float velocidade;
    private float dano;
    private Vector2 direcao;

    private float tempoVivo;

    void Awake()
    {

    }

    public void ConfigurarDirecao(Vector2 d, float dano, float vel, float escala)
    {
        direcao = d.normalized;
        this.dano = dano;
        velocidade = vel;

        transform.localScale *= escala;
    }

    void Update()
    {
        tempoVivo += Time.deltaTime;
        if (tempoVivo >= vidaDaBala)
            Destroy(gameObject);
    }

    void FixedUpdate()
    {
        transform.position += (Vector3)(direcao * velocidade * Time.fixedDeltaTime);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        EnemyAI e = col.GetComponent<EnemyAI>();
        if (e != null)
        {
            e.LevarTiro(dano);
            Destroy(gameObject);
        }

        if (col.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Destroy(gameObject);
        }
    }
}
