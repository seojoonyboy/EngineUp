using Flux;
using UnityEngine;
using UnityEngine.Networking;

public class User : Store<Actions> {
    // prop
    public string nickName;
    NetworkManager networkManager = NetworkManager.Instance;
    // end of prop
    public User(Dispatcher<Actions> _dispatcher) : base(_dispatcher){}

    void gameStart(){
        var strBuilder = GameManager.Instance.sb;
        strBuilder.Remove(0,strBuilder.Length);
        strBuilder.Append(networkManager.baseUrl)
            .Append("users/")
            .Append(GameManager.Instance.deviceId).Append("/");
        networkManager.request("GET",strBuilder.ToString(), getUserData);
    }

    void getUserData(HttpResponse response){
        if(response.isError){
            Debug.Log(response.errorMessage);
        } else if(response.responseCode>=200 && response.responseCode < 300) {    //유저있음 닉네임 받고 화면 전환 처리
            Debug.Log(response.data);
            UserData data = UserData.fromJSON(response.data);
            nickName = data.nickName;
            //_emitChange();
        } else {
            if(response.responseCode == 404) return;    //해당유저 없음
        }
    }

    void userCreateCallback(HttpResponse response) {
        //Debug.Log("USER CREATE CALLBACK");
        //Debug.Log(response.data);
        if(response.isError) {
            Debug.Log(response.errorMessage);
        }
        else if(response.responseCode >= 200 && response.responseCode < 300) {    //유저있음 닉네임 받고 화면 전환 처리
            Debug.Log(response.data);
            UserData data = UserData.fromJSON(response.data);
            nickName = data.nickName;
        }
        else {
            if(response.responseCode == 404) return;    //해당유저 없음
        }
    }

    void userCreate(string nickName, string deviceId) {
        var strBuilder = GameManager.Instance.sb;
        strBuilder.Remove(0,strBuilder.Length);
        strBuilder.Append(networkManager.baseUrl)
            .Append("users/");
        WWWForm form = new WWWForm();

        form.AddField("nickName",nickName);
        form.AddField("deviceId",deviceId);

        networkManager.request("POST",strBuilder.ToString(),form,userCreateCallback);
    }

    protected override void _onDispatch(Actions action){
        switch(action.type){
        case ActionTypes.GAME_START:
            gameStart();
            break;
        case ActionTypes.EDIT_NICKNAME:
            //nickName = (action as EditNickNameAction).nickname;
            break;
        case ActionTypes.USER_CREATE:
            //Debug.Log("USER CREATE ACTION");
            nickName = (action as UserCreateAction).nickname;
            string deviceId = (action as UserCreateAction).deviceId; 
            userCreate(nickName, deviceId);
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