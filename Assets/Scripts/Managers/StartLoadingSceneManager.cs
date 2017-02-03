using UnityEngine;
using UnityEngine.SceneManagement;
public class StartLoadingSceneManager : fbl_SceneManager {
    GameManager gm;
    User userStore;
    bool isUserExist = false;
    public GameObject modal;

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
        SceneManager.LoadScene("Main");
    }

    void addListener() {
        userStore.addListener(userListener);
    }

    void userListener() {
        Debug.Log(userStore.eventType);
        if(userStore.eventType  == ActionTypes.GAME_START) {
            if (userStore.nickName != null) {
                Debug.Log("!!");
                loadMainScene();
            }
        }

        if (userStore.eventType == ActionTypes.USER_CREATE) {
            if (userStore.isCreated) {
                loadMainScene();
                userStore.isCreated = false;
            }
        }

        if (userStore.eventType == ActionTypes.USER_CREATE_ERROR) {
            modal.SetActive(true);
            modal.transform.Find("MessagePanel/Label").GetComponent<UILabel>().text = userStore.UImsg;
        }

        
    }

    public void offModal() {
        modal.SetActive(false);
    }
}