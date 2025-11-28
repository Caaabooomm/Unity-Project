using UnityEngine;
using UnityEngine.SceneManagement;
public class MNCredits : MonoBehaviour
{
       public void OpenCredits()
    {
        SceneManager.LoadSceneAsync(2);
    }

}
