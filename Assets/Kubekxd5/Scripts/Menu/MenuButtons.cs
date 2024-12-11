using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtons : MonoBehaviour
{
    public int sceneId;
    
    public void LoadNewScene()
    {
        SceneManager.LoadScene(sceneId);
    }

    public void ClearShip()
    {
        GameManager.Instance.ClearSelectedShip();
    }
}
