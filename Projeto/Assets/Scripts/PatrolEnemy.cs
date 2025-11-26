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


    // Lógica de Colisão para Inverter e Causar Dano
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
        }

        // 2. LÓGICA DE INVERSÃO
        // Inverte a direção se colidir com algo que não seja um trigger
        if (!collision.gameObject.GetComponent<Collider2D>().isTrigger)
        {
            Flip();
        }
    }
}