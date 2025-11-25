using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instancia;

    [Header("Estado do Jogo (somente leitura via GameManager)")]
    [SerializeField] private int vidaMax;
    [SerializeField] private int vidaAtual;
    [SerializeField] private int documento;
    [SerializeField] private int inimigosTotais;
    [SerializeField] private int inimigosVivos;

    void Awake()
    {
        if (instancia == null)
        {
            instancia = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // inicializa/atualiza os dados de player (chamado pelo Player.Awake)
    public void ConfigurarPlayer(int vidaMax, int vidaAtual, int documento)
    {
        this.vidaMax = vidaMax;
        this.vidaAtual = vidaAtual;
        this.documento = documento;
    }

    public void AtualizarVida(int novaVida)
    {
        vidaAtual = novaVida;
    }

    public void AtualizarDocumento(int novoValor)
    {
        documento = novoValor;
    }

    public int GetVidaAtual() => vidaAtual;
    public int GetVidaMax() => vidaMax;
    public int GetDocumento() => documento;

    public void RegistrarInimigo()
    {
        inimigosTotais++;
        inimigosVivos++;
    }

    public void InimigoConvertido()
    {
        inimigosVivos = Mathf.Max(0, inimigosVivos - 1);
        // quando zerar inimigos, aqui você pode tratar vitória
        if (inimigosVivos <= 0)
        {
            // exemplo: carregar cena de vitória (ajuste nome)
            // SceneManager.LoadScene("Vitoria");
            Debug.Log("[GM] Todos inimigos convertidos - vitória!");
        }
    }

    public int GetInimigosVivos() => inimigosVivos;
    public int GetInimigosTotais() => inimigosTotais;
}
