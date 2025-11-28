using UnityEngine;
using UnityEngine.SceneManagement;
public class MNReturn : MonoBehaviour

{
       public void OpenMenu()
    {
        SceneManager.LoadSceneAsync(0);
    }

}
