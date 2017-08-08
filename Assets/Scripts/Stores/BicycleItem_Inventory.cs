using Flux;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System;

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

    public RespGetItems[] allItems;
    public ArrayList
        wheelItems = new ArrayList(),
        frameItems = new ArrayList(),
        engineItems = new ArrayList();
    public RespGetItems[] equipedItemIndex = new RespGetItems[3];

    protected override void _onDispatch(Actions action) {
        switch (action.type) {
            case ActionTypes.GARAGE_ITEM_EQUIP:
                equip_act equipAct = action as equip_act;
                if(equipAct._type == equip_act.type.ITEM) {
                    equip(action as equip_act);
                }
                break;
            case ActionTypes.GARAGE_ITEM_UNEQUIP:
                unequip(action as unequip_act);
                break;
            case ActionTypes.GARAGE_LOCK:
                _lock(action as garage_lock_act);
                break;
            case ActionTypes.GARAGE_SELL:
                sell(action as garage_sell_act);
                break;
            case ActionTypes.ITEM_INIT:
                item_init itemInitAct = action as item_init;
                if(itemInitAct._type == equip_act.type.ITEM) {
                    getItems(itemInitAct);
                }
                break;
            case ActionTypes.GARAGE_ITEM_SORT:
                Sorting.itemSort(wheelItems, 0);
                Sorting.itemSort(frameItems, 0);
                Sorting.itemSort(engineItems, 0);

                _emitChange();
                break;
            case ActionTypes.BOX_OPEN:
                item_init act = ActionCreator.createAction(ActionTypes.ITEM_INIT) as item_init;
                act._type = equip_act.type.ITEM;
                dispatcher.dispatch(act);
                break;
        }
        eventType = action.type;
    }

    //내 아이템 목록 불러오기
    private void getItems(item_init payload) {
        switch (payload.status) {
            case NetworkAction.statusTypes.REQUEST:
                storeStatus = storeStatus.WAITING_REQ;
                var strBuilder = GameManager.Instance.sb;
                strBuilder.Remove(0, strBuilder.Length);
                strBuilder.Append(networkManager.baseUrl)
                    .Append("inventory/items");
                networkManager.request("GET", strBuilder.ToString(), ncExt.networkCallback(dispatcher, payload));
                break;
            case NetworkAction.statusTypes.SUCCESS:
                storeStatus = storeStatus.NORMAL;
                //Debug.Log("아이템 목록 가져오기 : " + payload.response.data);
                message = "아이템을 성공적으로 불러왔습니다.";
                allItems = JsonHelper.getJsonArray<RespGetItems>(payload.response.data);

                init();
                itemCategorization(allItems);

                Sorting.itemSort(wheelItems, 0);
                Sorting.itemSort(frameItems, 0);
                Sorting.itemSort(engineItems, 0);

                _emitChange();
                break;
            case NetworkAction.statusTypes.FAIL:
                storeStatus = storeStatus.ERROR;
                message = "아이템 목록을 불러오는 과정에서 문제가 발생하였습니다.";
                Debug.Log("아이템 목록 가져오기 : " + payload.response.data);
                _emitChange();
                break;
        }
    }

    //자전거 아이템 종류별로 분류
    private void itemCategorization(RespGetItems[] item) {
        for (int i = 0; i < item.Length; i++) {
            BicycleItem _item = item[i].item;
            if (_item.parts == "WH") {
                wheelItems.Add(item[i]);
                if(item[i].is_equiped == "true") {
                    equipedItemIndex[0] = item[i];
                }
            }
            else if (_item.parts == "FR") {
                frameItems.Add(item[i]);
                if (item[i].is_equiped == "true") {
                    equipedItemIndex[1] = item[i];
                }
            }
            else if (_item.parts == "DS") {
                engineItems.Add(item[i]);
                if (item[i].is_equiped == "true") {
                    equipedItemIndex[2] = item[i];
                }
            }
        }
        itemEffect();
        
        //itemSort act = ActionCreator.createAction(ActionTypes.GARAGE_ITEM_SORT) as itemSort;
        //dispatcher.dispatch(act);
    }

    //아이템 장착에 의한 total spec 계산
    private void itemEffect() {
        int totalStr = 0;
        int totalSpeed = 0;
        int totalEndurance = 0;
        int totalRecovery = 0;
        for(int i=0; i<equipedItemIndex.Length; i++) {
            if(equipedItemIndex[i] != null) {
                var item = equipedItemIndex[i].item;
                totalStr += item.strength;
                totalSpeed += item.speed;
                totalRecovery += item.regeneration;
                totalEndurance += item.endurance;
            }
        }
        userStore.itemSpects.Item_strength = totalStr;
        userStore.itemSpects.Item_speed = totalSpeed;
        userStore.itemSpects.Item_regeneration = totalRecovery;
        userStore.itemSpects.Item_endurance = totalEndurance;
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

                //Debug.Log("아이템 장착 완료");

                MyInfo myInfoAct = ActionCreator.createAction(ActionTypes.MYINFO) as MyInfo;
                gm.gameDispatcher.dispatch(myInfoAct);

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

                MyInfo myInfoAct = ActionCreator.createAction(ActionTypes.MYINFO) as MyInfo;
                gm.gameDispatcher.dispatch(myInfoAct);

                break;
            case NetworkAction.statusTypes.FAIL:
                storeStatus = storeStatus.ERROR;
                _emitChange();
                break;
        }
    }

    //아이템 잠금 및 해제
    private void _lock(garage_lock_act payload) {
        switch (payload.status) {
            case NetworkAction.statusTypes.REQUEST:
                var strBuilder = GameManager.Instance.sb;
                strBuilder.Remove(0, strBuilder.Length);
                if (payload.type == "lock") {
                    strBuilder.Append(networkManager.baseUrl)
                    .Append("inventory/items/")
                    .Append(payload.id)
                    .Append("/lock");

                    Debug.Log("Lock ID :" + payload.id);
                }
                else if (payload.type == "unlock") {
                    strBuilder.Append(networkManager.baseUrl)
                    .Append("inventory/items/")
                    .Append(payload.id)
                    .Append("/unlock");

                    Debug.Log("UNLock ID :" + payload.id);
                }

                WWWForm form = new WWWForm();
                networkManager.request("POST", strBuilder.ToString(), form, ncExt.networkCallback(dispatcher, payload));
                break;
            case NetworkAction.statusTypes.SUCCESS:
                storeStatus = storeStatus.NORMAL;

                item_init act = ActionCreator.createAction(ActionTypes.ITEM_INIT) as item_init;
                act._type = equip_act.type.ITEM;
                dispatcher.dispatch(act);

                break;
            case NetworkAction.statusTypes.FAIL:
                storeStatus = storeStatus.ERROR;
                _emitChange();
                break;
        }
    }

    //아이템 판매
    private void sell(garage_sell_act payload) {
        switch (payload.status) {
            case NetworkAction.statusTypes.REQUEST:
                var strBuilder = GameManager.Instance.sb;
                //var parmStrBuilder = new StringBuilder();
                strBuilder.Remove(0, strBuilder.Length);
                //parmStrBuilder.Remove(0, parmStrBuilder.Length);
                //List<int> lists = payload.lists;
                //int cnt = lists.Count;
                //foreach(int id in lists) {
                //    parmStrBuilder.Append(id);
                //    if (cnt > 1) {
                //        parmStrBuilder.Append(",");
                //    }
                //    cnt--;
                //}
                WWWForm form = new WWWForm();
                //form.AddField("ids", parmStrBuilder.ToString());

                strBuilder.Append(networkManager.baseUrl)
                    .Append("inventory/items/")
                    .Append(payload.id)
                    .Append("/sell");
                networkManager.request("POST", strBuilder.ToString(), form, ncExt.networkCallback(dispatcher, payload));
                break;
            case NetworkAction.statusTypes.SUCCESS:
                storeStatus = storeStatus.NORMAL;

                Debug.Log("아이템 판매 완료");

                MyInfo myInfoAct = ActionCreator.createAction(ActionTypes.MYINFO) as MyInfo;
                gm.gameDispatcher.dispatch(myInfoAct);

                break;
            case NetworkAction.statusTypes.FAIL:
                storeStatus = storeStatus.ERROR;
                Debug.Log(payload.response.data);
                _emitChange();
                break;
        }
    }

    private void init() {
        wheelItems.Clear();
        frameItems.Clear();
        engineItems.Clear();

        equipedItemIndex[0] = null;
        equipedItemIndex[1] = null;
        equipedItemIndex[2] = null;
    }
}

[System.Serializable]
public class RespGetItems {
    public int id;
    public RespItem item;
    public string is_equiped;
    public string is_locked;

    public static RespGetItems fromJSON(string json) {
        return JsonUtility.FromJson<RespGetItems>(json);
    }
}

[System.Serializable]
public class RespItem : BicycleItem { }