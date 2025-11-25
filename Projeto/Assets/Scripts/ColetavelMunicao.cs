using UnityEngine;

public class DocumentoCol : MonoBehaviour
{
    public int quantidade = 1;

    void OnTriggerEnter2D(Collider2D col)
    {
        Player p = col.GetComponent<Player>();
        if (p != null)
        {
            p.AdicionarDocumento(quantidade);
            Destroy(gameObject);
        }
    }
}
