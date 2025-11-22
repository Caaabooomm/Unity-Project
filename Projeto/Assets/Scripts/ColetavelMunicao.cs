using UnityEngine;

public class ColetavelMunicao : MonoBehaviour
{
    public int quantidade = 1;

    void OnTriggerEnter2D(Collider2D col)
    {
        Player p = col.GetComponent<Player>();
        if (p != null)
        {
            p.AdicionarMunicao(quantidade);
            Debug.Log("[Coletável] Player coletou munição.");
            Destroy(gameObject);
        }
    }

}
