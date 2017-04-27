using Flux;
using UnityEngine;
using System;
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
    public string userTitle;

    public character[] basicCharacters;
    public represent_character myCharacters;
    public UserbicycleType[] userBicycleTypes;

    NetworkManager networkManager = NetworkManager.Instance;

    private GameManager gm = GameManager.Instance;
    // end of prop
    public User(QueueDispatcher<Actions> _dispatcher) : base(_dispatcher){}

    public UserData myData;

    NetworkCallbackExtention ncExt = new NetworkCallbackExtention();
    public ActionTypes eventType;
    public SignupAction.loginType loginType;
    //최신 내 정보 불러오기
    void myInfo(MyInfo payload) {
        switch (payload.status) {
            case NetworkAction.statusTypes.REQUEST:
                storeStatus = storeStatus.WAITING_REQ;
                setMessage(1);

                var strBuilder = GameManager.Instance.sb;
                WWWForm form = new WWWForm();
                strBuilder.Remove(0, strBuilder.Length);
                strBuilder.Append(networkManager.baseUrl)
                    .Append("me");
                networkManager.request("GET", strBuilder.ToString(), form, ncExt.networkCallback(dispatcher, payload));
                break;
            case NetworkAction.statusTypes.SUCCESS:
                storeStatus = storeStatus.NORMAL;
                setMessage(2);
                myData = UserData.fromJSON(payload.response.data);
                setUserTitle(myData.status.rank);
                Debug.Log(payload.response.data);
                _emitChange();
                break;
            case NetworkAction.statusTypes.FAIL:
                storeStatus = storeStatus.ERROR;
                _emitChange();
                break;
        }
    }

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
                switch (act.login_type) {
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
                form.AddField("represent_character", act.charIndex);

                strBuilder.Append(networkManager.baseUrl)
                    .Append("signup");
                networkManager.request("POST", strBuilder.ToString(), form, ncExt.networkCallback(dispatcher, act), false);
                break;
            case NetworkAction.statusTypes.SUCCESS:
                storeStatus = storeStatus.NORMAL;
                setMessage(2);

                Debug.Log("회원가입 Callback : " + act.response.data);

                LoginCallbackData callbackData = LoginCallbackData.fromJSON(act.response.data);
                this.nickName = callbackData.user.nickName;
                userTokenId = callbackData.key;
                userId = callbackData.user.id;
                myCharacters = callbackData.user.represent_character;
                GameStartAction startAct = ActionCreator.createAction(ActionTypes.GAME_START) as GameStartAction;
                dispatcher.dispatch(startAct);
                break;
            case NetworkAction.statusTypes.FAIL:
                Debug.Log(act.response.data);
                storeStatus = storeStatus.ERROR;
                //SignUpError msg = SignUpError.fromJSON(act.response.data);
                var msg = GameManager.Instance.sb;
                msg.Remove(0, msg.Length);
                SignUpError errorJson = SignUpError.fromJSON(act.response.data);
                //Debug.Log(act.response.data);
                string[] nickName = errorJson.nickName;
                if (errorJson.deviceId != null) {
                    msg.Append("이미 이전에 회원가입을 하였습니다.");
                }
                else if(nickName != null) {
                    foreach(string str in nickName) {
                        if(str.Contains("banned")) {
                            msg.Append("\n 욕설은 포함할 수 없습니다.");
                        }
                        else if (str.Contains("korean,alphabet,number")) {
                            msg.Append("\n 오직 한글,알파벳,숫자만이 허용됩니다.");
                        }
                        else if(str.Contains("more")) {
                            msg.Append("\n 8글자 이내에만 허용됩니다.");
                        }
                        if(str.Contains("unique")) {
                            msg.Append("\n 중복되는 닉네임입니다.");
                        }
                        if(str.Contains("least")) {
                            msg.Append("\n 2글자 이상을 입력해주세요.");
                        }
                    }
                }
                message = msg.ToString();
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
                switch (act.login_type) {
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
                loginType = act.login_type;
                networkManager.request("POST", strBuilder.ToString(), form, ncExt.networkCallback(dispatcher, act), false);
                break;
            case NetworkAction.statusTypes.SUCCESS:

                storeStatus = storeStatus.NORMAL;
                setMessage(2);

                LoginCallbackData callbackData = LoginCallbackData.fromJSON(act.response.data);
                //Debug.Log("sign in에 대한 callback : " + act.response.data);
                //Debug.Log(callbackData.user);
                userTokenId = callbackData.key;
                nickName = callbackData.user.nickName;
                userId = callbackData.user.id;
                myCharacters = callbackData.user.represent_character;

                //Debug.Log("Nickname : " + nickName);
                GameStartAction startAct = ActionCreator.createAction(ActionTypes.GAME_START) as GameStartAction;
                dispatcher.dispatch(startAct);

                _emitChange();
                break;
            case NetworkAction.statusTypes.FAIL:
                storeStatus = storeStatus.ERROR;
                message = "User 정보가 없습니다. 회원가입으로 넘어갑니다.";
                Debug.Log(act.response.data);
                _emitChange();
                break;
        }
    }

    void getCharInfo(GetDefaultCharInfo payload) {
        switch (payload.status)
        {
            case NetworkAction.statusTypes.REQUEST:

                storeStatus = storeStatus.WAITING_REQ;
                setMessage(1);

                var strBuilder = GameManager.Instance.sb;
                strBuilder.Remove(0, strBuilder.Length);
                strBuilder.Append(networkManager.baseUrl)
                    .Append("starting_characters");
                
                networkManager.request("GET", strBuilder.ToString(), ncExt.networkCallback(dispatcher, payload), false);
                break;
            case NetworkAction.statusTypes.SUCCESS:
                storeStatus = storeStatus.NORMAL;
                basicCharacters = JsonHelper.getJsonArray<character>(payload.response.data);
                _emitChange();
                break;
            case NetworkAction.statusTypes.FAIL:
                storeStatus = storeStatus.ERROR;
                message = "캐릭터 정보를 불러오는데 문제가 발생하였습니다.";
                Debug.Log(payload.response.data);
                _emitChange();
                break;
        }
    }

    private void editProfile(EditProfileAction payload) {
        switch (payload.status) {
            case NetworkAction.statusTypes.REQUEST:
                var strBuilder = GameManager.Instance.sb;
                strBuilder.Remove(0, strBuilder.Length);
                strBuilder.Append(networkManager.baseUrl)
                    .Append("me");
                WWWForm form = new WWWForm();
                if (payload.type == EditProfileAction.profileType.DISTRICT) {
                    string val = (string)payload.value;
                    form.AddField("district", val);
                    form.AddField("country", "대한민국");
                }
                else if(payload.type == EditProfileAction.profileType.BICYCLE) {
                    string val = (string)payload.value;
                    form.AddField("bicycle", val);
                }
                else if(payload.type == EditProfileAction.profileType.WEIGHT) {
                    int val = Convert.ToInt32(payload.value);
                    form.AddField("weight", val);
                }
                else if(payload.type == EditProfileAction.profileType.HEIGHT) {
                    int val = Convert.ToInt32(payload.value);
                    form.AddField("height", val);
                }
                else if(payload.type == EditProfileAction.profileType.GENDER) {
                    string val = null;
                    if((string)payload.value == "woman") {
                        val = "W";
                    }
                    else if((string)payload.value == "man") {
                        val = "M";
                    }
                    form.AddField("gender", val);
                }
                else if(payload.type == EditProfileAction.profileType.BIRTHDAY) {
                    string val = (string)payload.value;
                    form.AddField("birthday", val);
                }
                networkManager.request("PUT", strBuilder.ToString(), form, ncExt.networkCallback(dispatcher, payload));
                storeStatus = storeStatus.WAITING_REQ;
                setMessage(1);
                
                break;
            case NetworkAction.statusTypes.SUCCESS:
                storeStatus = storeStatus.NORMAL;
                MyInfo act = ActionCreator.createAction(ActionTypes.MYINFO) as MyInfo;
                dispatcher.dispatch(act);
                _emitChange();
                break;
            case NetworkAction.statusTypes.FAIL:
                storeStatus = storeStatus.ERROR;

                message = "캐릭터 정보를 불러오는데 문제가 발생하였습니다.";
                Debug.Log(payload.response.data);
                _emitChange();
                break;
        }
    }

    private void getBicycleTypes(GetBicycleTypes payload) {
        switch (payload.status) {
            case NetworkAction.statusTypes.REQUEST:

                storeStatus = storeStatus.WAITING_REQ;
                setMessage(1);

                var strBuilder = GameManager.Instance.sb;
                strBuilder.Remove(0, strBuilder.Length);
                strBuilder.Append(networkManager.baseUrl)
                    .Append("bicycles");

                networkManager.request("GET", strBuilder.ToString(), ncExt.networkCallback(dispatcher, payload), false);
                break;
            case NetworkAction.statusTypes.SUCCESS:
                storeStatus = storeStatus.NORMAL;
                Debug.Log(payload.response.data);
                userBicycleTypes = JsonHelper.getJsonArray<UserbicycleType>(payload.response.data);
                _emitChange();
                break;
            case NetworkAction.statusTypes.FAIL:
                Debug.Log(payload.response.data);
                storeStatus = storeStatus.ERROR;
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
            case ActionTypes.GET_DEFAULT_CHAR_INFO:
                getCharInfo(action as GetDefaultCharInfo);
                break;
            case ActionTypes.MYINFO:
                myInfo(action as MyInfo);
                break;
            case ActionTypes.GAME_START:
                MyInfo act = ActionCreator.createAction(ActionTypes.MYINFO) as MyInfo;
                dispatcher.dispatch(act);
                _emitChange();
                break;
            case ActionTypes.EDIT_PROFILE:
                Debug.Log("?!");
                editProfile(action as EditProfileAction);
                break;
            case ActionTypes.USER_BICYCLETYPES:
                getBicycleTypes(action as GetBicycleTypes);
                break;
        }
        eventType = action.type;
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

    //유저 호칭 설정
    private void setUserTitle(int lv) {
        if(lv == 0) {
            userTitle = "초보자";
        }
        else if(lv >= 1 && lv <= 9) {
            userTitle = "동호회급";
        }
        else if(lv >= 10 && lv <= 19) {
            userTitle = "클럽급";
        }
        else if(lv >= 20 && lv <= 29) {
            userTitle = "지역대표급";
        }
        else if (lv >= 30 && lv <= 44) {
            userTitle = "국가대표급";
        }
        else if (lv >= 45 && lv <= 59) {
            userTitle = "UCI 컨티낸탈급";
        }
        else if (lv >= 60 && lv <= 74) {
            userTitle = "UCI 프로 컨티낸탈급";
        }
        else if (lv >= 75 && lv <= 99) {
            userTitle = "지역대표급";
        }
        else if(lv >= 100) {
            userTitle = "지존급";
        }
    }
}

public class UserData {
    public int id;
    public string nickName;
    public int gears;
    public int boxes;
    public represent_character represent_character;
    public status status;
    public string country;
    public string district;
    public string bicycle;
    public string birthday;
    public string weight;
    public string height;
    public string gender;

    public static UserData fromJSON(string json){
        return JsonUtility.FromJson<UserData>(json);
    }
}

[System.Serializable]
class SignUpError {
    public string[] represent_character;
    public string[] deviceId;
    public string[] nickName;

    public static SignUpError fromJSON(string json) {
        return JsonUtility.FromJson<SignUpError>(json);
    }
}

[System.Serializable]
class LoginCallbackData {
    public string key = null;
    //public string createDate = null;
    public SubLoginCallBack user = null;
    public static LoginCallbackData fromJSON(string json) {
        return JsonUtility.FromJson<LoginCallbackData>(json);
    }
}

[System.Serializable]
class SubLoginCallBack {
    public int id = -1;
    public string nickName = null;
    public represent_character represent_character;
}

[System.Serializable]
public class represent_character {
    public int user;
    public int selectedLv;
    public character_inventory character_inventory;
    public status status;
}

[System.Serializable]
public class character {
    public int id;
    public string name;
    public int cost;
    public lvup_exps[] lvup_exps;
}

[System.Serializable]
public class lvup_exps {
    public int num1;
    public int num2;
}

[System.Serializable]
public class status {
    public int user;
    public int rank;
    public int exp;
    public int strength;
    public int speed;
    public int endurance;
    public int regeneration;
}

[System.Serializable]
public class UserbicycleType {
    public int id;
    public string name;
}