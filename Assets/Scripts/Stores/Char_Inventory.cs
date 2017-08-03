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

    private GameManager gm = GameManager.Instance;
    private User userStore = GameManager.Instance.userStore;

    NetworkManager networkManager = NetworkManager.Instance;
    NetworkCallbackExtention ncExt = new NetworkCallbackExtention();

    //public character_inventory[] my_characters;
    //public Dictionary<int, all_characters> all_characters = new Dictionary<int, all_characters>();
    public ArrayList myCharacters = new ArrayList();
    public ActionTypes eventType;

    //현재 보유하지 않은 캐릭터 역시 파트너룸에서 보여주기 위한 임시 배열
    public charStat[] allStats;
    public Character_inventory repCharacter;

    protected override void _onDispatch(Actions action) {
        switch (action.type) {
            case ActionTypes.CHAR_OPEN:
                unlock(action as garage_unlock_char);
                break;
            case ActionTypes.GARAGE_ITEM_EQUIP:
                equip_act equipAct = action as equip_act;
                if (equipAct._type == equip_act.type.CHAR) {
                    equip(action as equip_act);
                }
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
            case ActionTypes.CHAR_SORT:
                Sorting.itemSort(myCharacters, 1);

                _emitChange();
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

                myCharacters.Clear();

                callbackGetchar callback = callbackGetchar.fromJSON(payload.response.data);
                callback_allChars[] allCharacters = callback.all_characters;
                callback_CharInventory[] callback_myCharacters = callback.character_inventory;
                int repId = userStore.myData.represent_character.character_inventory.character;
                for(int i=0; i<allCharacters.Length; i++) {
                    Character_inventory _myCharacter = new Character_inventory();
                    foreach (callback_CharInventory myCharater in callback_myCharacters) {
                        if(allCharacters[i].id == myCharater.character) {
                            _myCharacter.imageId = allCharacters[i].id;
                            _myCharacter.desc = allCharacters[i].desc;
                            _myCharacter.lvup_exps = allCharacters[i].lvup_exps;
                            _myCharacter.name = allCharacters[i].name;
                            _myCharacter.cost = allCharacters[i].cost;

                            _myCharacter.id = myCharater.id;
                            _myCharacter.paid = myCharater.paid;
                            _myCharacter.exp = myCharater.exp;
                            _myCharacter.lv = myCharater.lv;

                            _myCharacter.strength = myCharater.status.strength;
                            _myCharacter.speed = myCharater.status.speed;
                            _myCharacter.regeneration = myCharater.status.regeneration;
                            _myCharacter.endurance = myCharater.status.endurance;
                            _myCharacter.hasCharacter = true;
                        }

                        if(repId == _myCharacter.imageId) {
                            repCharacter = _myCharacter;

                            userStore.itemSpects.Char_strength = _myCharacter.strength;
                            userStore.itemSpects.Char_speed = _myCharacter.speed;
                            userStore.itemSpects.Char_regeneration = _myCharacter.regeneration;
                            userStore.itemSpects.Char_endurance = _myCharacter.endurance;
                        }
                    }

                    if(_myCharacter.id == 0) {
                        _myCharacter.imageId = allCharacters[i].id;
                        _myCharacter.name = allCharacters[i].name;
                        _myCharacter.desc = allCharacters[i].desc;
                        _myCharacter.lvup_exps = allCharacters[i].lvup_exps;
                        _myCharacter.hasCharacter = false;
                    }
                    myCharacters.Add(_myCharacter);
                    //if (_myCharacter.id == 0) {
                    //    Debug.Log("해당 캐릭터 없음");
                    //}
                }
                Sorting.itemSort(myCharacters, 1);
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

                MyInfo myInfoAct = ActionCreator.createAction(ActionTypes.MYINFO) as MyInfo;
                gm.gameDispatcher.dispatch(myInfoAct);

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
public class callback_CharInventory {
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
public class callback_allChars {
    public int id;
    public string name;
    public string desc;
    public int cost;
    public int[] lvup_exps;
}

[System.Serializable]
public class callbackGetchar {
    public callback_allChars[] all_characters;
    public callback_CharInventory[] character_inventory;

    public static callbackGetchar fromJSON(string json) {
        return JsonUtility.FromJson<callbackGetchar>(json);
    }
}

public class Character_inventory {
    public int id;              //고유식별 번호
    public string name;         //이름
    public string desc;         //설명
    public int cost;            //필요 조각 갯수
    public int[] lvup_exps;     //레벨업 필요 경험치
    public int lv;              //현재 레벨
    public int imageId;         //해당 이미지 id
    public int paid;            //보유 조각 갯수
    public int exp;             //현재 경험치

    public int strength;        //능력치
    public int speed;
    public int endurance;
    public int regeneration;

    public bool hasCharacter;
}