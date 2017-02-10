using UnityEngine;
using UnityEngine.SceneManagement;
public class StartLoadingSceneManager : fbl_SceneManager {
    GameManager gm;
    User userStore;
    bool isUserExist = false;
    public GameObject modal;
    public UIInput modalInput;

    void Awake() {
        Debug.Log("StartLoadingScene Awake");
        gm = GameManager.Instance;
        userStore = gm.userStore;
        userStore.addListener(userListener);
    }

    public void loadMainScene() {
        SceneManager.LoadScene("Main");
    }

    void onSignUpModal() {
        Debug.Log("Modal창을 띄웁니다!!");
        modal.SetActive(true);
    }

    public void okInSignUpModal() {
        SignupAction signupAct = ActionCreator.createAction(ActionTypes.SIGNUP) as SignupAction;
        signupAct.type = userStore.loginType;
        signupAct.nickName = modalInput.value;
        GameManager.Instance.gameDispatcher.dispatch(signupAct);

        modal.SetActive(false);
    }

    public void cancelInSignUpModal() {
        modal.SetActive(false);
    }

    void userListener() {
        Debug.Log(userStore.eventType);
        if(userStore.eventType == ActionTypes.SIGNUPMODAL) {
            onSignUpModal();
        }

        if(userStore.eventType  == ActionTypes.GAME_START) {
            loadMainScene();
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