using UnityEngine;
using UnityEngine.SceneManagement;
public class StartLoadingSceneManager : fbl_SceneManager {
    GameManager gm;
    User userStore;
    bool isUserExist = false;
    public GameObject 
        modal,
        nicknameModal,
        charselectModal,
        buttonGroup;

    public FacebookLogin facebooklogin;
    public NormalLogin normalLogin;

    public UIInput modalInput;
    public int charIndex;

    private string newNickName;

    void Awake() {
        //Debug.Log("StartLoadingScene Awake");
        gm = GameManager.Instance;
        userStore = gm.userStore;
        userStore.addListener(userListener);
    }

    void Start() {
        string str = PlayerPrefs.GetString("socialType");
        if (string.IsNullOrEmpty(str)) {
            //Debug.Log("이전 로그인 기록 없음");
            buttonGroup.SetActive(true);
        }
        if(str == "FB") {
            //Debug.Log("이전 Facebook 로그인했음");
            facebooklogin.FBlogin();
        }
        if(str == "NO") {
            //Debug.Log("이전 Normal 로그인했음");
            normalLogin.onLoginButton();
        }
        //Debug.Log(str);
    }

    public void loadMainScene() {
        SceneManager.LoadScene("Main");
    }

    void onNicknameModal() {
        Debug.Log("닉네임 입력창을 띄웁니다!!");
        modal.SetActive(true);
        nicknameModal.SetActive(true);
    }

    public void okInNicknameModal() {
        //SignupAction signupAct = ActionCreator.createAction(ActionTypes.SIGNUP) as SignupAction;
        //signupAct.type = userStore.loginType;
        //signupAct.nickName = modalInput.value;
        //GameManager.Instance.gameDispatcher.dispatch(signupAct);
        newNickName = modalInput.value;
        //modal.SetActive(false);
        nicknameModal.SetActive(false);
        charselectModal.SetActive(true);
    }
    
    public void okInCharSelectModal() {
        modal.SetActive(false);

        SignupAction signupAct = ActionCreator.createAction(ActionTypes.SIGNUP) as SignupAction;
        signupAct.type = userStore.loginType;
        signupAct.nickName = modalInput.value;
        signupAct.charIndex = charIndex;
        GameManager.Instance.gameDispatcher.dispatch(signupAct);
    }

    public void cancelInSignUpModal() {
        modal.SetActive(false);

        nicknameModal.SetActive(false);
        charselectModal.SetActive(false);

        buttonGroup.SetActive(true);
    }

    void userListener() {
        //Debug.Log(userStore.eventType);
        if(userStore.eventType == ActionTypes.SIGNUPMODAL) {
            onNicknameModal();
        }

        if(userStore.eventType  == ActionTypes.GAME_START) {
            loadMainScene();
            if(userStore.loginType == SignupAction.loginType.FB) {
                PlayerPrefs.SetString("socialType", "FB");
            }
            if (userStore.loginType == SignupAction.loginType.NO) {
                PlayerPrefs.SetString("socialType", "NO");
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

    public void setCharIndex(GameObject obj) {
        charIndex = obj.GetComponent<ButtonIndex>().index;
    }
}