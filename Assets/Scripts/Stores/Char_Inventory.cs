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
    public Dictionary<int, all_characters> all_characters = new Dictionary<int, all_characters>();
    public ActionTypes eventType;

    //현재 보유하지 않은 캐릭터 역시 파트너룸에서 보여주기 위한 임시 배열
    public charStat[] allStats;
    public character_inventory repCharacter;

    protected override void _onDispatch(Actions action) {
        switch (action.type) {
            case ActionTypes.CHAR_OPEN:
                unlock(action as garage_unlock_char);
                break;
            case ActionTypes.GARAGE_ITEM_EQUIP:
                equip(action as equip_act);
                break;
            case ActionTypes.ITEM_INIT:
                item_init itemInitAct = action as item_init;
                if (itemInitAct._type == equip_act.type.CHAR) {
                    getMyChar(itemInitAct);
                }
                break;
            case ActionTypes.BOX_OPEN:
                item_init act = ActionCreator.createAction(ActionTypes.ITEM_INIT) as item_init;
                act._type = equip_act.type.CHAR;
                dispatcher.dispatch(act);
                break;
        }
        eventType = action.type;
    }

    //내 캐릭터 목록 가져오기
    private void getMyChar(item_init payload) {
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
                //Debug.Log(payload.response.data);
                message = "캐릭터 아이템을 성공적으로 불러왔습니다.";
                callbackGetchar callback = callbackGetchar.fromJSON(payload.response.data);
                all_characters.Clear();
                all_characters[] tmp = callback.all_characters;
                for(int i=0; i<tmp.Length; i++) {
                    all_characters.Add(tmp[i].id, tmp[i]);
                }

                my_characters = callback.character_inventory;
                int repCharId = userStore.myData.represent_character.character_inventory.character;

                foreach(character_inventory charInven in my_characters) {
                    if(charInven.character == repCharId) {
                        var status = charInven.status;
                        userStore.itemSpects.Char_endurance = status.endurance;
                        userStore.itemSpects.Char_regeneration = status.regeneration;
                        userStore.itemSpects.Char_speed = status.speed;
                        userStore.itemSpects.Char_strength = status.strength;

                        //대표 캐릭터 정보 저장
                        repCharacter = charInven;
                    }
                }
                _emitChange();
                break;
            case NetworkAction.statusTypes.FAIL:
                storeStatus = storeStatus.ERROR;
                Debug.Log(payload.response.data);
                message = "캐릭터 정보를 불러오는 과정에서 문제가 발생하였습니다.";
                _emitChange();
                break;
        }
    }

    //캐릭터 장착
    private void equip(equip_act payload) {
        if (payload._type != equip_act.type.CHAR) {
            return;
        }
        switch (payload.status) {
            case NetworkAction.statusTypes.REQUEST:
                var strBuilder = GameManager.Instance.sb;
                strBuilder.Remove(0, strBuilder.Length);
                strBuilder.Append(networkManager.baseUrl)
                    .Append("inventory/characters/")
                    .Append(payload.id)
                    .Append("/equip");
                WWWForm form = new WWWForm();
                networkManager.request("POST", strBuilder.ToString(), form, ncExt.networkCallback(dispatcher, payload));
                break;
            case NetworkAction.statusTypes.SUCCESS:
                storeStatus = storeStatus.NORMAL;
                Debug.Log("캐릭터 장착 완료");

                item_init act = ActionCreator.createAction(ActionTypes.ITEM_INIT) as item_init;
                act._type = equip_act.type.CHAR;
                dispatcher.dispatch(act);

                MyInfo myInfoAct = ActionCreator.createAction(ActionTypes.MYINFO) as MyInfo;
                dispatcher.dispatch(myInfoAct);

                break;
            case NetworkAction.statusTypes.FAIL:
                storeStatus = storeStatus.ERROR;
                _emitChange();
                break;
        }
    }

    //캐릭터 해금
    private void unlock(garage_unlock_char payload) {
        switch (payload.status) {
            case NetworkAction.statusTypes.REQUEST:
                var strBuilder = GameManager.Instance.sb;
                strBuilder.Remove(0, strBuilder.Length);
                strBuilder.Append(networkManager.baseUrl)
                    .Append("inventory/characters/")
                    .Append(payload.id)
                    .Append("/get");
                WWWForm form = new WWWForm();
                networkManager.request("POST", strBuilder.ToString(), form, ncExt.networkCallback(dispatcher, payload));
                break;
            case NetworkAction.statusTypes.SUCCESS:
                storeStatus = storeStatus.NORMAL;
                Debug.Log("캐릭터 해금 완료");

                item_init act = ActionCreator.createAction(ActionTypes.ITEM_INIT) as item_init;
                act._type = equip_act.type.CHAR;
                dispatcher.dispatch(act);

                break;
            case NetworkAction.statusTypes.FAIL:
                storeStatus = storeStatus.ERROR;
                _emitChange();
                break;
        }
    }
}

[System.Serializable]
public class character_inventory {
    public int id;
    public int character;
    public int paid;
    public int lv;
    public int exp;
    public string has_character;
    public charStat status;

}

[System.Serializable]
public class charStat {
    public int strength;
    public int speed;
    public int endurance;
    public int regeneration;
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