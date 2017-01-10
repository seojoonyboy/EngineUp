using Flux;
using UnityEngine;
using System.Collections;

public class User : Store<Actions> {
    // prop
    public string nickName = null;
    NetworkManager networkManager = NetworkManager.Instance;
    // end of prop
    public User(Dispatcher<Actions> _dispatcher) : base(_dispatcher){}

    NetworkCallbackExtention ncExt = new NetworkCallbackExtention();
    public ActionTypes eventType;

    Friends[] myFriends;

    Group[] myGroup;

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

    void getFeeds(GetCommunityAction act) {
        switch (act.status) {
            case NetworkAction.statusTypes.REQUEST:
            var strBuilder = GameManager.Instance.sb;
            strBuilder.Remove(0, strBuilder.Length);
            //Feed Data 전체를 요청
            //strBuilder.Append(networkManager.baseUrl)
            //    .Append("users/")
            //    .Append(GameManager.Instance.deviceId);
            //networkManager.request("GET", strBuilder.ToString(), ncExt.networkCallback(dispatcher, payload));
            break;
            case NetworkAction.statusTypes.SUCCESS: // Community Data 가져오기 성공
            //_emitChange();
            break;
            case NetworkAction.statusTypes.FAIL:    // Community Data 가져오기 실패
            break;
        }
    }

    void getFriends(GetCommunityAction act) {
        switch (act.status) {
            case NetworkAction.statusTypes.REQUEST:
            var strBuilder = GameManager.Instance.sb;
            strBuilder.Remove(0, strBuilder.Length);
            if (string.IsNullOrEmpty(act.keyword)) {
                //Friends Data 전체를 요청
                //strBuilder.Append(networkManager.baseUrl)
                //    .Append("users/")
                //    .Append(GameManager.Instance.deviceId);
            }
            else {
                //Friends Data를 일부를 요청
                //strBuilder.Append(networkManager.baseUrl)
                //    .Append("users/")
                //    .Append(GameManager.Instance.deviceId);
            }
            //networkManager.request("GET", strBuilder.ToString(), ncExt.networkCallback(dispatcher, payload));
            break;
            case NetworkAction.statusTypes.SUCCESS: // Friends Data 가져오기 성공
            myFriends = JsonHelper.getJsonArray<Friends>(act.response.data);
            //_emitChange();
            break;
            case NetworkAction.statusTypes.FAIL:    // Friends Data 가져오기 실패
            break;
        }
    }

    void getGroup(GetCommunityAction act) {
        switch (act.status) {
            case NetworkAction.statusTypes.REQUEST:
            var strBuilder = GameManager.Instance.sb;
            strBuilder.Remove(0, strBuilder.Length);
            if (string.IsNullOrEmpty(act.keyword)) {
                //Group Data 전체를 요청
                //strBuilder.Append(networkManager.baseUrl)
                //    .Append("users/")
                //    .Append(GameManager.Instance.deviceId);
            }
            else {
                //Group Data를 일부를 요청
                //strBuilder.Append(networkManager.baseUrl)
                //    .Append("users/")
                //    .Append(GameManager.Instance.deviceId);
            }
            //networkManager.request("GET", strBuilder.ToString(), ncExt.networkCallback(dispatcher, payload));
            break;
            case NetworkAction.statusTypes.SUCCESS: // getGroup Data 가져오기 성공
            //_emitChange();
            myGroup = JsonHelper.getJsonArray<Group>(act.response.data);
            break;
            case NetworkAction.statusTypes.FAIL:    // getGroup Data 가져오기 실패
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
            var _act = action as GameStartAction;
            getUserData(_act);
            break;
        case ActionTypes.EDIT_NICKNAME:
            //nickName = (action as EditNickNameAction).nickname;
            break;
        case ActionTypes.USER_CREATE:
            userCreate(action as UserCreateAction);
            break;
        case ActionTypes.GET_COMMUNITY_DATA:
            GetCommunityAction act = action as GetCommunityAction;
            if(act.type == GetCommunityAction.requestType.ALL) {
                Debug.Log("LOAD ALL COMMUNITY DATA");
                getFeeds(action as GetCommunityAction);
                getFriends(action as GetCommunityAction);
                getGroup(action as GetCommunityAction);
            }

            if(act.type == GetCommunityAction.requestType.FRIENDS) {
                Debug.Log("Get Friends Data");
                Debug.Log("Keyword : " + act.keyword);
                getFriends(action as GetCommunityAction);
            }

            if(act.type == GetCommunityAction.requestType.GROUP) {
                getGroup(action as GetCommunityAction);
            }
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

[System.Serializable]
class Feeds {
    public string date;
    public string header;
    public string contents;

    public static Feeds fromJSON(string json) {
        return JsonUtility.FromJson<Feeds>(json);
    }
}

[System.Serializable]
class Friends {
    public int lv;
    public string nickName;
    public string name;

    public static Friends fromJSON(string json) {
        return JsonUtility.FromJson<Friends>(json);
    }
}

[System.Serializable]
class Group {
    public string groupName;
    public int memberNum;
    public string location;

    public static Group fromJSON(string json) {
        return JsonUtility.FromJson<Group>(json);
    }
}