using Flux;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Char_Inventory : AjwStore {
    //store status
    public storeStatus storeStatus = storeStatus.NORMAL;
    //store message
    public string message;

    public Char_Inventory(QueueDispatcher<Actions> _dispatcher) : base(_dispatcher) { }
    public represent_character representChar;

    private GameManager gm = GameManager.Instance;
    private User userStore = GameManager.Instance.userStore;

    NetworkManager networkManager = NetworkManager.Instance;
    NetworkCallbackExtention ncExt = new NetworkCallbackExtention();

    public character_inventory[] my_characters;
    public all_characters[] all_characters;
    public ActionTypes eventType;

    protected override void _onDispatch(Actions action) {
        switch (action.type) {
            case ActionTypes.SIGNIN:
                string[] ids = new string[1];
                ids[0] = userStore.dispatchToken;
                gm.gameDispatcher.waitFor(ids);
                representChar = userStore.myCharacters;
                break;
            case ActionTypes.GARAGE_CHAR_INIT:
                getMyChar(action as getCharacters_act);
                break;
        }
        eventType = action.type;
    }

    private void getMyChar(getCharacters_act payload) {
        switch (payload.status) {
            case NetworkAction.statusTypes.REQUEST:
                var strBuilder = GameManager.Instance.sb;
                strBuilder.Remove(0, strBuilder.Length);
                strBuilder.Append(networkManager.baseUrl)
                    .Append("inventory/characters");
                networkManager.request("GET", strBuilder.ToString(), ncExt.networkCallback(dispatcher, payload));
                break;
            case NetworkAction.statusTypes.SUCCESS:
                storeStatus = storeStatus.NORMAL;
                message = "아이템을 성공적으로 불러왔습니다.";
                callbackGetchar callback = callbackGetchar.fromJSON(payload.response.data);
                all_characters = callback.all_characters;
                my_characters = callback.character_inventory;
                _emitChange();
                break;
            case NetworkAction.statusTypes.FAIL:
                storeStatus = storeStatus.ERROR;
                Debug.Log(payload.response.data);
                message = "아이템 목록을 불러오는 과정에서 문제가 발생하였습니다.";
                _emitChange();
                break;
        }
    }
}

[System.Serializable]
public class character_inventory {
    public int id;
    public int paid;
    public int lv;
    public int exp;
    public int user;
    public int character;
}

[System.Serializable]
public class all_characters {
    public int id;
    public string name;
    public string desc;
    public int cost;
    public int[] lvup_exps;
}

[System.Serializable]
public class callbackGetchar {
    public all_characters[] all_characters;
    public character_inventory[] character_inventory;

    public static callbackGetchar fromJSON(string json) {
        return JsonUtility.FromJson<callbackGetchar>(json);
    }
}