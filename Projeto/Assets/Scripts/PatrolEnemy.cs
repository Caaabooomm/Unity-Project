using UnityEngine;

public class PatrolEnemy : EnemyBase
{
    protected override void Start()
    {
        base.Start();
        maxHealth = 1;
        currentHealth = maxHealth;
    }

    protected override void Update()
    {
        base.Update();

        if (!stunned)
        {
            float dir = facingRight ? 1f : -1f;
            // CORREÇÃO 1: Usar rb.velocity (não rb.linearVelocity)
            rb.linearVelocity = new Vector2(dir * moveSpeed, rb.linearVelocity.y);
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    // NOVO: Lógica de Colisão para Inverter
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Inverte a direção se colidir com algo que não seja um trigger e não seja o jogador.
        // Assumindo que inimigos e paredes têm o mesmo comportamento de colisão.
        if (!collision.gameObject.CompareTag("Player") && !collision.gameObject.GetComponent<Collider2D>().isTrigger)
        {
            // Inverte a direção
            Flip();
        }
    }
}