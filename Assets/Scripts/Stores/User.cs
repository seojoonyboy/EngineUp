using Flux;
using UnityEngine;

public class User : Store<Actions> {
    // prop
    public string nickName;

    // end of prop
    public User(Dispatcher<Actions> _dispatcher) : base(_dispatcher){}

    void gameStart(){
        var networkManager = NetworkManager.Instance;
        var strBuffer = GameManager.Instance.sb;
        strBuffer.Remove(0,strBuffer.Length);
        strBuffer.Append(networkManager.baseUrl)
            .Append("usesrs/")
            .Append(GameManager.Instance.deviceId).Append("/");
        networkManager.request("GET", strBuffer.ToString(), getUserData);
    }

    void getUserData(HttpResponse response){
        if(response.isError){
            Debug.Log(response.errorMessage);
            if(response.responseCode == 404) return;    //해당유저 없음
        } else {    //유저있음 닉네임 받고 화면 전환 처리
            Debug.Log(response.data);
            UserData data = UserData.fromJSON(response.data);
            nickName = data.nickName;
            _emitChange();
        }
    }

    protected override void _onDispatch(Actions action){
        switch(action.type){
        case ActionTypes.GAME_START:
            gameStart();
            break;
        case ActionTypes.EDIT_NICKNAME:
            nickName = (action as EditNickNameAction).nickname;
            break;
        }
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