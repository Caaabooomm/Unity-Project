using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public int totalDocuments;
    public int documentsCollected;

    public int totalEnemies;
    public int enemiesCleansed;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public void AddDocument()
    {
        documentsCollected++;
    }

    public void CleanEnemy()
    {
        enemiesCleansed++;
    }
}