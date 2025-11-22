using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instancia;
    public int inimigosNoMapa;
    public int inimigosConvertidos;

    void Awake()
    {
        if (instancia == null)
        {
            instancia = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("[GM] GameManager iniciado.");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void RegistrarInimigo()
    {
        inimigosNoMapa++;
        Debug.Log("[GM] Inimigos no mapa: " + inimigosNoMapa);
    }

    public void InimigoConvertido()
    {
        inimigosConvertidos++;
        Debug.Log("[GM] Inimigos convertidos: " + inimigosConvertidos);
    }

}
