using UnityEngine;

public class StartLoadingSceneManager : SceneManager {
    public GameObject gameManager;

    void Awake() {
        if(GameManager.Instance == null){
            Instantiate(gameManager);
        }
    }

    void Start() {
        
    }

    public void loadMainScene() {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Main");
    }
}
