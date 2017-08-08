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
        privacyContainer,
        loadingModal;

    public FacebookLogin facebooklogin;
    public NormalLogin normalLogin;
    public InputField modalInput;
    private string newNickName;

    public Toggle
        mobileServiceCheckBox,
        privacyCollectCheckBox;

    private int selectIndex;
    public Sprite[] charPortraits;
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
            loadingModal.SetActive(false);
        }
        if(str == "FB") {
            Debug.Log("이전 Facebook 로그인했음");
            facebooklogin.FBlogin();
        }
        if(str == "NO") {
            Debug.Log("이전 Normal 로그인했음");
            normalLogin.onLoginButton(true);
        }
    }

    public void loadMainScene() {
        SceneManager.LoadScene("Main");
    }

    //생성하기 버튼 클릭시
    public void okInNicknameModal() {
        //Debug.Log("type : " + userStore.loginType);
        newNickName = modalInput.text;
        //Debug.Log("nickName : " + newNickName);
        int index = 0;
        if(selectIndex == 1) {
            index = 1;
        }
        else if(selectIndex == 0) {
            index = 2;
        }
        SignupAction signUpAct = ActionCreator.createAction(ActionTypes.SIGNUP) as SignupAction;
        signUpAct.charIndex = index;
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
                if(userStore.isAutoLogin) {
                    buttonGroup.SetActive(true);
                    loadingModal.SetActive(false);
                }
                else {
                    if(userStore.loginType == SignupAction.loginType.NO) {
                        onPolicyModal();
                    }
                }
            }
        }

        if(userStore.eventType == ActionTypes.SIGNUP) {
            if(userStore.storeStatus == storeStatus.ERROR) {
                modal.SetActive(true);
                errorModal.SetActive(true);
                errorModal.transform.Find("Modal/Text").GetComponent<Text>().text = userStore.message;

                nicknameModal.SetActive(true);
            }
        }

        if(userStore.eventType == ActionTypes.GET_DEFAULT_CHAR_INFO) {
            if(userStore.storeStatus == storeStatus.NORMAL) {
                selectIndex = userStore.basicCharacters[0].id - 1;
                setInitCharSelect();
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
        NickNameCheckResultModal.SetActive(false);
    }

    public void offModal() {
        modal.SetActive(false);
    }

    //캐릭터 선택 좌우 버튼
    public void charSelArrowClicked(int type) {
        int basicCharLength = userStore.basicCharacters.Length;
        //좌측 버튼 클릭시
        
        if (type == 0) {
            selectIndex--;
            if(selectIndex < 0) {
                selectIndex = basicCharLength - 1;
            }
        }
        else if(type == 1) {
            selectIndex++;
            if(selectIndex > basicCharLength - 1) {
                selectIndex = 0;
            }
        }
        Debug.Log(selectIndex);
        setInitCharSelect();
    }

    private void setInitCharSelect() {
        Image img = charselectModal.transform.Find("Image").GetComponent<Image>();
        img.sprite = charPortraits[selectIndex];

        Text name = charselectModal.transform.Find("SelectButton/Name").GetComponent<Text>();
        name.text = userStore.basicCharacters[selectIndex].name;
    }
}