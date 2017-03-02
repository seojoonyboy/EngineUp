using Flux;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class User : AjwStore {
    // prop
    public string userTokenId = null;
    public string nickName = null;

    public bool 
        isSearch = false,
        isCreated = false,
        isFirstTry = false;

    public string UImsg;
    //facebook token
    public string facebookToken;

    public int userId;

    public UserData[] findNickNameResult;

    NetworkManager networkManager = NetworkManager.Instance;
    // end of prop
    public User(QueueDispatcher<Actions> _dispatcher) : base(_dispatcher){}

    NetworkCallbackExtention ncExt = new NetworkCallbackExtention();
    public ActionTypes eventType;
    public SignupAction.loginType loginType;

    void signup(SignupAction act) {
        switch (act.status) {
            case NetworkAction.statusTypes.REQUEST:
                if(act.nickName.Length == 0) {
                    UserCreateError noneNickNameError = ActionCreator.createAction(ActionTypes.USER_CREATE_ERROR) as UserCreateError;
                    noneNickNameError.msg = "닉네임을 입력하세요.";
                    dispatcher.dispatch(noneNickNameError);
                    return;
                }
                var strBuilder = GameManager.Instance.sb;
                WWWForm form = new WWWForm();
                strBuilder.Remove(0, strBuilder.Length);
                switch (act.type) {
                    case SignupAction.loginType.FB:
                        form.AddField("type", "FB");
                        form.AddField("accessToken", facebookToken);
                        break;
                    case SignupAction.loginType.NO:
                        form.AddField("type", "NO");
                        form.AddField("deviceId", GameManager.Instance.deviceId);
                        break;
                }
                form.AddField("nickName", act.nickName);
                form.AddField("representative", act.charIndex);

                strBuilder.Append(networkManager.baseUrl)
                    .Append("signup");
                networkManager.request("POST", strBuilder.ToString(), form, ncExt.networkCallback(dispatcher, act), false);
                break;
            case NetworkAction.statusTypes.SUCCESS:
                Debug.Log("회원가입 Callback");
                LoginCallbackData callbackData = LoginCallbackData.fromJSON(act.response.data);
                nickName = callbackData.user.nickName;
                userTokenId = callbackData.key;
                GameStartAction startAct = ActionCreator.createAction(ActionTypes.GAME_START) as GameStartAction;
                dispatcher.dispatch(startAct);
                break;
            case NetworkAction.statusTypes.FAIL:
                Debug.Log("Sign up Error Message : " + act.response.data);
                UserCreateError errorAct = ActionCreator.createAction(ActionTypes.USER_CREATE_ERROR) as UserCreateError;
                SignUpError msg = SignUpError.fromJSON(act.response.data);
                Debug.Log("Detail 필드 : " + msg.detail);
                UImsg = msg.detail;
                if (UImsg.Contains("exsist")) {
                    Debug.Log("닉네임 중복");
                    errorAct.msg = "이미 존재하는 닉네임입니다.";
                    dispatcher.dispatch(errorAct);
                }
                break;
        }
    }

    void signin(SigninAction act) {
        switch (act.status) {
            case NetworkAction.statusTypes.REQUEST:
                var strBuilder = GameManager.Instance.sb;
                WWWForm form = new WWWForm();
                strBuilder.Remove(0, strBuilder.Length);
                switch (act.type) {
                    case SignupAction.loginType.FB:
                        facebookToken = act.token;
                        //Debug.Log("signIn Switch case FB");
                        form.AddField("type", "FB");
                        form.AddField("accessToken", facebookToken);
                        break;
                    case SignupAction.loginType.NO:
                        form.AddField("type", "NO");
                        form.AddField("deviceId", GameManager.Instance.deviceId);
                        break;
                }
                strBuilder.Append(networkManager.baseUrl)
                    .Append("signin");
                loginType = act.type;
                networkManager.request("POST", strBuilder.ToString(), form, ncExt.networkCallback(dispatcher, act), false);
                break;
            case NetworkAction.statusTypes.SUCCESS:
                LoginCallbackData callbackData = LoginCallbackData.fromJSON(act.response.data);
                //Debug.Log("sign in에 대한 callback : " + act.response.data);
                userTokenId = callbackData.key;
                nickName = callbackData.user.nickName;
                userId = callbackData.user.id;
                //Debug.Log("Nickname : " + nickName);
                GameStartAction startAct = ActionCreator.createAction(ActionTypes.GAME_START) as GameStartAction;
                dispatcher.dispatch(startAct);
                _emitChange();
                break;
            case NetworkAction.statusTypes.FAIL:
                Debug.Log("User 정보 없음.");
                //해당 user정보가 없음
                //회원가입 진행. 약관 동의 화면
                SignupModalAction signupModalAct = ActionCreator.createAction(ActionTypes.SIGNUPMODAL) as SignupModalAction;
                dispatcher.dispatch(signupModalAct);
                _emitChange();
                break;
        }
    }

    void getUserData(GameStartAction payload){
        switch(payload.status){
        case NetworkAction.statusTypes.REQUEST:
            var strBuilder = GameManager.Instance.sb;
            strBuilder.Remove(0,strBuilder.Length);
            strBuilder.Append(networkManager.baseUrl)
                .Append("users");
            networkManager.request("GET",strBuilder.ToString(), ncExt.networkCallback(dispatcher, payload));
            break;
        case NetworkAction.statusTypes.SUCCESS: // 유저 정보 있음!!!
            UserData data = UserData.fromJSON(payload.response.data);
            nickName = data.nickName;
            _emitChange();
            break;
        case NetworkAction.statusTypes.FAIL:    // 유저 정보 없음!!
            break;
        }
    }

    private void onError(UserCreateError act) {
        UImsg = act.msg;
        _emitChange();
    }

    protected override void _onDispatch(Actions action){
        switch(action.type){
        case ActionTypes.SIGNUP:
            signup(action as SignupAction);
            break;
        case ActionTypes.SIGNIN:
            signin(action as SigninAction);
            break;
        case ActionTypes.GAME_START:
            //getUserData(action as GameStartAction);
            break;
        case ActionTypes.USER_CREATE_ERROR:
            onError(action as UserCreateError);
            break;
        }
        eventType = action.type;
        _emitChange();
    }
}

public class UserData {
    public int id;
    //public string deviceId;
    public string nickName;

    public static UserData fromJSON(string json){
        return JsonUtility.FromJson<UserData>(json);
    }
}

[System.Serializable]
public class SignUpError {
    public string detail;

    public static SignUpError fromJSON(string json) {
        return JsonUtility.FromJson<SignUpError>(json);
    }
}

class LoginCallbackData {
    public string key;
    public string createDate;
    public SubLoginCallBack user;
    public static LoginCallbackData fromJSON(string json) {
        return JsonUtility.FromJson<LoginCallbackData>(json);
    }
}

[System.Serializable]
class SubLoginCallBack {
    public int id;
    public string nickName;
}