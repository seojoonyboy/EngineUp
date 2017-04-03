using Flux;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class BicycleItem_Inventory : AjwStore {
    //store status
    public storeStatus storeStatus = storeStatus.NORMAL;
    //store message
    public string message;

    public BicycleItem_Inventory(QueueDispatcher<Actions> _dispatcher) : base(_dispatcher) { }

    NetworkManager networkManager = NetworkManager.Instance;
    NetworkCallbackExtention ncExt = new NetworkCallbackExtention();

    public represent_character myCharacters;

    private GameManager gm = GameManager.Instance;
    private User userStore = GameManager.Instance.userStore;

    public ActionTypes eventType;

    public BicycleItem[] items;
    protected override void _onDispatch(Actions action) {
        switch (action.type) {
            case ActionTypes.GARAGE_ITEM_INIT:
                getItems(action as getItems_act);
                break;
            case ActionTypes.GARAGE_ITEM_EQUIP:
                equip(action as equip_act);
                break;
            case ActionTypes.GARAGE_ITEM_UNEQUIP:
                unequip(action as unequip_act);
                break;
        }
        eventType = action.type;
    }

    //내 아이템 목록 불러오기
    private void getItems(getItems_act payload) {
        switch (payload.status) {
            case NetworkAction.statusTypes.REQUEST:
                var strBuilder = GameManager.Instance.sb;
                strBuilder.Remove(0, strBuilder.Length);
                strBuilder.Append(networkManager.baseUrl)
                    .Append("inventory/items");
                networkManager.request("GET", strBuilder.ToString(), ncExt.networkCallback(dispatcher, payload));
                break;
            case NetworkAction.statusTypes.SUCCESS:
                storeStatus = storeStatus.NORMAL;
                message = "아이템을 성공적으로 불러왔습니다.";
                Debug.Log(payload.response.data);
                items = JsonHelper.getJsonArray<BicycleItem>(payload.response.data);
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

    //아이템 장착
    private void equip(equip_act payload) {
        switch (payload.status) {
            case NetworkAction.statusTypes.REQUEST:
                var strBuilder = GameManager.Instance.sb;
                strBuilder.Remove(0, strBuilder.Length);
                strBuilder.Append(networkManager.baseUrl)
                    .Append("inventory/items/")
                    .Append(payload.id)
                    .Append("/equip");
                WWWForm form = new WWWForm();
                networkManager.request("POST", strBuilder.ToString(), form, ncExt.networkCallback(dispatcher, payload));
                break;
            case NetworkAction.statusTypes.SUCCESS:
                storeStatus = storeStatus.NORMAL;
                Debug.Log("아이템 장착 완료");
                getItems_act act = ActionCreator.createAction(ActionTypes.GARAGE_ITEM_INIT) as getItems_act;
                dispatcher.dispatch(act);

                break;
            case NetworkAction.statusTypes.FAIL:
                storeStatus = storeStatus.ERROR;
                _emitChange();
                break;
        }
    }

    //아이템 해제
    private void unequip(unequip_act payload) {
        switch (payload.status) {
            case NetworkAction.statusTypes.REQUEST:
                var strBuilder = GameManager.Instance.sb;
                strBuilder.Remove(0, strBuilder.Length);
                strBuilder.Append(networkManager.baseUrl)
                    .Append("inventory/items/")
                    .Append(payload.id)
                    .Append("/unequip");
                WWWForm form = new WWWForm();
                networkManager.request("POST", strBuilder.ToString(), form, ncExt.networkCallback(dispatcher, payload));
                break;
            case NetworkAction.statusTypes.SUCCESS:
                storeStatus = storeStatus.NORMAL;
                Debug.Log("아이템 해제 완료");
                getItems_act act = ActionCreator.createAction(ActionTypes.GARAGE_ITEM_INIT) as getItems_act;
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
public class BicycleItem {
    public int id;
    public Item item;
    public string is_equiped;

    public static BicycleItem fromJSON(string json) {
        return JsonUtility.FromJson<BicycleItem>(json);
    }
}

[System.Serializable]
public class Item {
    public string name;
    public string desc;
    public int grade;
    public int gear;
    public string parts;
    public int limit_rank;
}