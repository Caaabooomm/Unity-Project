using UnityEngine;
using UnityEngine.SceneManagement;

public class MN : MonoBehaviour
{
       public void PlayGame()
    {
        SceneManager.LoadSceneAsync(1);
    }

}
