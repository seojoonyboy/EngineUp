using Flux;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class User : AjwStore {
    // prop
    public string nickName = null;
    public bool isSearch = false;
    
    NetworkManager networkManager = NetworkManager.Instance;
    // end of prop
    public User(QueueDispatcher<Actions> _dispatcher) : base(_dispatcher){}

    NetworkCallbackExtention ncExt = new NetworkCallbackExtention();
    public ActionTypes eventType;

    void getUserData(GameStartAction payload){
        switch(payload.status){
        case NetworkAction.statusTypes.REQUEST:
            var strBuilder = GameManager.Instance.sb;
            strBuilder.Remove(0,strBuilder.Length);
            strBuilder.Append(networkManager.baseUrl)
                .Append("users/")
                .Append(GameManager.Instance.deviceId);
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

    void userCreate(UserCreateAction act) {
        switch(act.status){
        case NetworkAction.statusTypes.REQUEST:
            var strBuilder = GameManager.Instance.sb;
            strBuilder.Remove(0,strBuilder.Length);
            strBuilder.Append(networkManager.baseUrl)
                .Append("users");
            WWWForm form = new WWWForm();
            form.AddField("nickName",act.nickName);
            form.AddField("deviceId",act.deviceId);
            networkManager.request("POST",strBuilder.ToString(),form, ncExt.networkCallback(dispatcher, act));
            break;
        case NetworkAction.statusTypes.SUCCESS:
            UserData data = UserData.fromJSON(act.response.data);
            nickName = data.nickName;
            Debug.Log("user 생성 성공");
            _emitChange();
            break;
        case NetworkAction.statusTypes.FAIL:
            Debug.Log("user 생성 실패");
            Debug.Log("Error Msg : " + act.response.errorMessage);
            // create 실패
            break;
        }
    }

    protected override void _onDispatch(Actions action){
        switch(action.type){
        case ActionTypes.GAME_START:
            getUserData(action as GameStartAction);
            break;
        case ActionTypes.EDIT_NICKNAME:
            //nickName = (action as EditNickNameAction).nickname;
            break;
        case ActionTypes.USER_CREATE:
            userCreate(action as UserCreateAction);
            break;
        }
        eventType = action.type;
        _emitChange();
    }
}

class UserData {
    public int id;
    public string deviceId;
    public string nickName;

    public static UserData fromJSON(string json){
        return JsonUtility.FromJson<UserData>(json);
    }
}