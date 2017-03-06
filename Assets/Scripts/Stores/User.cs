using Flux;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class User : AjwStore {
    //store status
    public storeStatus storeStatus = storeStatus.NORMAL;
    //store message
    public string message;
    // prop
    public string userTokenId = null;
    public string nickName = null;

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

                storeStatus = storeStatus.WAITING_REQ;
                setMessage(1);

                if (act.nickName.Length == 0) {
                    //UserCreateError noneNickNameError = ActionCreator.createAction(ActionTypes.USER_CREATE_ERROR) as UserCreateError;
                    //noneNickNameError.msg = "닉네임을 입력하세요.";
                    //dispatcher.dispatch(noneNickNameError);
                    storeStatus = storeStatus.ERROR;
                    message = "닉네임을 입력하세요.";
                    _emitChange();
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

                storeStatus = storeStatus.NORMAL;
                setMessage(2);

                Debug.Log("회원가입 Callback");

                LoginCallbackData callbackData = LoginCallbackData.fromJSON(act.response.data);
                nickName = callbackData.user.nickName;
                userTokenId = callbackData.key;
                userId = callbackData.user.id;
                GameStartAction startAct = ActionCreator.createAction(ActionTypes.GAME_START) as GameStartAction;
                dispatcher.dispatch(startAct);
                break;
            case NetworkAction.statusTypes.FAIL:

                storeStatus = storeStatus.ERROR;
                //setMessage(3);

                SignUpError msg = SignUpError.fromJSON(act.response.data);
                Debug.Log("Detail 필드 : " + msg.detail);
                if (UImsg.Contains("exsist")) {
                    //Debug.Log("닉네임 중복");
                    message = "이미 존재하는 닉네임입니다.";
                }
                _emitChange();
                break;
        }
    }

    void signin(SigninAction act) {
        switch (act.status) {
            case NetworkAction.statusTypes.REQUEST:

                storeStatus = storeStatus.WAITING_REQ;
                setMessage(1);

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

                storeStatus = storeStatus.NORMAL;
                setMessage(2);

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

                storeStatus = storeStatus.ERROR;
                message = "User 정보가 없습니다. 회원가입으로 넘어갑니다.";
                Debug.Log("User 정보 없음.");
                _emitChange();
                break;
        }
    }

    protected override void _onDispatch(Actions action){
        switch(action.type){
        case ActionTypes.SIGNUP:
            signup(action as SignupAction);
            break;
        case ActionTypes.SIGNIN:
            signin(action as SigninAction);
            break;
        }
        eventType = action.type;
        _emitChange();
    }

    private void setMessage(int type) {
        switch (type) {
            //서버 요청중
            case 1 :
                message = "서버 요청중입니다. 잠시만 기다려 주세요.";
                break;
            case 2 :
                message = "서버 요청에 성공하였습니다.";
                break;
            case 3 :
                message = "서버 요청간에 문제가 발생하였습니다.";
                break;
        }
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