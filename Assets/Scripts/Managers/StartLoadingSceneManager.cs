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
        policyModal,
        policyErrorModal,
        NickNameCheckResultModal,
        buttonGroup;

    public FacebookLogin facebooklogin;
    public NormalLogin normalLogin;

    public UIInput modalInput;
    public int charIndex;

    private string newNickName;

    public UIToggle
        mobileServiceCheckBox,
        privacyCollectCheckBox;

    void Awake() {
        //Debug.Log("StartLoadingScene Awake");
        gm = GameManager.Instance;
        userStore = gm.userStore;
        userStore.addListener(userListener);
    }

    void Start() {
        string str = PlayerPrefs.GetString("socialType");
        if (string.IsNullOrEmpty(str)) {
            Debug.Log("이전 로그인 기록 없음");
            buttonGroup.SetActive(true);
        }
        if(str == "FB") {
            Debug.Log("이전 Facebook 로그인했음");
            facebooklogin.FBlogin();
        }
        if(str == "NO") {
            Debug.Log("이전 Normal 로그인했음");
            normalLogin.onLoginButton();
        }
        //Debug.Log(str);
    }

    public void loadMainScene() {
        SceneManager.LoadScene("Main");
    }

    //생성하기 버튼 클릭시
    public void okInNicknameModal() {
        Debug.Log("type : " + userStore.loginType);
        Debug.Log("nickName : " + newNickName);
        newNickName = modalInput.value;

        SignupAction signUpAct = ActionCreator.createAction(ActionTypes.SIGNUP) as SignupAction;
        signUpAct.charIndex = charIndex;
        signUpAct.nickName = newNickName;
        signUpAct.type = userStore.loginType;

        gm.gameDispatcher.dispatch(signUpAct);
    }

    public void cancelInSignUpModal() {
        modal.SetActive(false);

        nicknameModal.SetActive(false);
        //charselectModal.SetActive(false);

        buttonGroup.SetActive(true);
    }

    public void checkBoxListener() {
        //Debug.Log(mobileServiceCheckBox.value);
    }

    //이용 약관 동의 화면
    private void onPolicyModal() {
        modal.SetActive(true);
        policyModal.SetActive(true);
    }

    //이용 약관 동의 버튼
    public void onAgreePolicy() {
        //캐릭터 선택 화면으로 넘어감
        if(mobileServiceCheckBox.value && privacyCollectCheckBox.value) {
            policyModal.SetActive(false);
            charselectModal.SetActive(true);
        }
        else {
            policyErrorModal.SetActive(true);
        }
    }

    //이용약관 에러 모달 닫기 버튼
    public void offpolicyErrorModal() {
        policyErrorModal.SetActive(false);
    }

    //이용 약관 거절 버튼
    public void onDisagreePolicy() {
        //다시 로그인 버튼 화면으로 돌아감
        modal.SetActive(false);
        policyModal.SetActive(false);
        buttonGroup.SetActive(true);
    }

    //캐릭터 선택시
    public void onCharSelect() {
        //닉네임 입력화면으로 넘어감
        nicknameModal.SetActive(true);
        charselectModal.SetActive(false);
    }

    void userListener() {
        if(userStore.eventType == ActionTypes.SIGNIN) {
            if(userStore.storeStatus == storeStatus.ERROR) {
                onPolicyModal();
            }
        }

        if(userStore.eventType == ActionTypes.SIGNUP) {
            if(userStore.storeStatus == storeStatus.ERROR && userStore.message.Contains("이미")) {
                Debug.Log("이미 존재하는 닉네임");
            }
        }

        if (userStore.eventType  == ActionTypes.GAME_START) {
            loadMainScene();
            if(userStore.loginType == SignupAction.loginType.FB) {
                PlayerPrefs.SetString("socialType", "FB");
            }
            if (userStore.loginType == SignupAction.loginType.NO) {
                PlayerPrefs.SetString("socialType", "NO");
            }
        }
    }

    public void offNickNameCheckResultModal() {
        NickNameCheckResultModal.SetActive(false);
    }

    public void offModal() {
        modal.SetActive(false);
    }

    public void setCharIndex(GameObject obj) {
        charIndex = obj.GetComponent<ButtonIndex>().index;
    }
}