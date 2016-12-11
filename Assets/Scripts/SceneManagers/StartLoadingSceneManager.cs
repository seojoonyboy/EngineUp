using UnityEngine;

public class StartLoadingSceneManager : SceneManager {

    void Awake() {

    }

    void Start() {
        GameStartAction act = ActionCreator.createAction(ActionTypes.GAME_START) as GameStartAction;
        GameManager.Instance.gameDispatcher.dispatch(act);
    }

    public void loadMainScene() {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Main");
    }
}
