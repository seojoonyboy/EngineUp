using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartLoadingSceneManager : fbl_SceneManager {
    GameManager gm;
    User userStore;

    public GameObject 
        modal,
        nicknameModal,
        charselectModal,
        policyModal,
        errorModal,
        policyContextModal,
        NickNameCheckResultModal,
        buttonGroup,
        mobileServiceContainer,
        privacyContainer;

    public FacebookLogin facebooklogin;
    public NormalLogin normalLogin;

    public InputField modalInput;
    public int charIndex;

    private string newNickName;

    public Toggle
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
        newNickName = modalInput.text;
        Debug.Log("nickName : " + newNickName);

        SignupAction signUpAct = ActionCreator.createAction(ActionTypes.SIGNUP) as SignupAction;
        signUpAct.charIndex = charIndex;
        signUpAct.nickName = newNickName;
        signUpAct.login_type = userStore.loginType;

        gm.gameDispatcher.dispatch(signUpAct);

        nicknameModal.SetActive(false);
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

    //이용 약관 보기 버튼
    public void showPolicyModal(GameObject obj) {
        int index = obj.GetComponent<ButtonIndex>().index;

        switch (index) {
            //모바일 서비스 이용약관
            case 0:
                mobileServiceContainer.SetActive(true);
                break;
            //개인정보 수집 및 이용안내
            case 1:
                privacyContainer.SetActive(true);
                break;
        }
    }

    //이용 약관 닫기 버튼
    public void offPolicyModal() {
        mobileServiceContainer.SetActive(false);
        privacyContainer.SetActive(false);
    }

    //이용 약관 동의 버튼
    public void onAgreePolicy() {
        if (mobileServiceCheckBox.isOn && privacyCollectCheckBox.isOn) {
            policyModal.SetActive(false);
            charselectModal.SetActive(true);

            GetDefaultCharInfo getCharAct = ActionCreator.createAction(ActionTypes.GET_DEFAULT_CHAR_INFO) as GetDefaultCharInfo;
            gm.gameDispatcher.dispatch(getCharAct);
        }
        else {
            errorModal.SetActive(true);
            errorModal.transform.Find("Modal/Text").GetComponent<Text>().text = "약관을 체크해 주세요";
        }
    }

    //이용약관 에러 모달 닫기 버튼
    public void offErrorModal() {
        errorModal.SetActive(false);
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
            if(userStore.storeStatus == storeStatus.ERROR) {
                Debug.Log("이미 존재하는 닉네임");
                modal.SetActive(true);
                NickNameCheckResultModal.SetActive(true);
                NickNameCheckResultModal.transform.Find("Background/Label").GetComponent<UILabel>().text = userStore.message;
            }
        }

        if(userStore.eventType == ActionTypes.GET_DEFAULT_CHAR_INFO) {
            if(userStore.storeStatus == storeStatus.NORMAL) {
                //charselectModal.transform.Find("LoadingPanel").gameObject.SetActive(false);
                charselectModal.transform.Find("LeftBox/Button/Text").GetComponent<Text>().text = "회사원 " + userStore.basicCharacters[0].name;
                charselectModal.transform.Find("RightBox/Button/Text").GetComponent<Text>().text = "회사원 " + userStore.basicCharacters[1].name;

                GameObject charBtn = charselectModal.transform.Find("LeftBox").gameObject;
                charBtn.GetComponent<ButtonIndex>().index = userStore.basicCharacters[0].id;

                charBtn = charselectModal.transform.Find("RightBox").gameObject;
                charBtn.GetComponent<ButtonIndex>().index = userStore.basicCharacters[1].id;

                charIndex = userStore.basicCharacters[0].id;

                Text label = charselectModal.transform.Find("Description/Text").GetComponent<Text>();
                label.text = userStore.basicCharacters[0].desc;

                Text header = charselectModal.transform.Find("DescHeader").GetComponent<Text>();
                header.text = userStore.basicCharacters[0].name + "의 이야기";
                //charselectModal.transform.Find("Desc/Description").GetComponent<UILabel>().text = 
            }
        }

        if (userStore.eventType  == ActionTypes.GAME_START) {
            loadMainScene();
            if (userStore.loginType == SignupAction.loginType.FB) {
                PlayerPrefs.SetString("socialType", "FB");
            }
            if (userStore.loginType == SignupAction.loginType.NO) {
                PlayerPrefs.SetString("socialType", "NO");
            }
        }
    }

    public void offNickNameCheckResultModal() {
        //modal.SetActive(false);
        NickNameCheckResultModal.SetActive(false);
    }

    public void offModal() {
        modal.SetActive(false);
    }

    public void toggleGroupListener() {
        IEnumerable<Toggle> activeToggles = charselectModal.GetComponent<ToggleGroup>().ActiveToggles();
        foreach (Toggle tg in activeToggles) {
            //Debug.Log("active toggle" + tg.name);
            charIndex = tg.gameObject.GetComponent<ButtonIndex>().index;

            Text label = charselectModal.transform.Find("Description/Text").GetComponent<Text>();
            Text header = charselectModal.transform.Find("DescHeader").GetComponent<Text>();
            if (charIndex == 1) {
                label.text = userStore.basicCharacters[0].desc;
                header.text = userStore.basicCharacters[0].name + "의 이야기";
            }
            else if (charIndex == 2) {
                label.text = userStore.basicCharacters[1].desc;
                header.text = userStore.basicCharacters[1].name + "의 이야기";
            }
        }
    }
}