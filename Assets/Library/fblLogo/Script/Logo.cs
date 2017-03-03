using UnityEngine;
using UnityEngine.SceneManagement;

public class Logo : MonoBehaviour {
    [SerializeField]
    private string sceneName;

    public void nextScene() {
        SceneManager.LoadScene(sceneName);
    }
}
