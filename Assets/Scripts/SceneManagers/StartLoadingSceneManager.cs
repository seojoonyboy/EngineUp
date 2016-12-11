using UnityEngine;

public class StartLoadingSceneManager : SceneManager {
    GameManager gm;
    User userStore;
    bool isUserExist = false;
    void Awake() {
        gm = GameManager.Instance;
    }

    void Start() {
        userStore = gm.userStore;
        addListener();

        GameStartAction act = ActionCreator.createAction(ActionTypes.GAME_START) as GameStartAction;
        gm.gameDispatcher.dispatch(act);
    }

    public void loadMainScene() {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Main");
    }

    void addListener() {
        userStore.addListener(userListener);
    }

    void userListener() {
        isUserExist = userStore.isUserExist;
        if(isUserExist) {
            Debug.Log("Go to Main Scene");
            loadMainScene();
        }
    }
}
