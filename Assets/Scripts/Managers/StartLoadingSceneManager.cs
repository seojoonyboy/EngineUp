using UnityEngine;
using UnityEngine.SceneManagement;
public class StartLoadingSceneManager : fbl_SceneManager {
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
        //SceneManager.LoadScene("Main");
    }

    void addListener() {
        userStore.addListener(userListener);
    }

    void userListener() {
        if(!string.IsNullOrEmpty(userStore.nickName)) {
            Debug.Log("Go to Main Scene");
            loadMainScene();
        }
    }
}