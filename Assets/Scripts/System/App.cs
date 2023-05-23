using UnityEngine;
using UnityEngine.SceneManagement;

public class App : MonoBehaviour
{
    public string sceneToLoad;
    void Awake() {
        //Cursor.SetCursor(standardCursor, Vector2.zero, CursorMode.ForceSoftware);
        SceneManager.LoadScene(sceneName: sceneToLoad);
    }
}
