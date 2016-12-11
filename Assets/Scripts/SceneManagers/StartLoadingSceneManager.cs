using UnityEngine;

public class StartLoadingSceneManager : SceneManager {
    // public GameObject gameManager;
    // public NetworkManager networkManager;
    public GameObject[] managers;

    void Awake() {
        for(var i=0; i<managers.Length; i++){
            GameObject obj = managers[i];
            Instantiate(obj);
        }
        var _gameManager = GameManager.Instance;
        _gameManager.userStore = new User(_gameManager.gameDispatcher);
        _gameManager.deviceId = SystemInfo.deviceUniqueIdentifier;
    }

    void Start() {
        GameStartAction act = ActionCreator.createAction(ActionTypes.GAME_START) as GameStartAction;
        GameManager.Instance.gameDispatcher.dispatch(act);
    }

    public void loadMainScene() {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Main");
    }
}
