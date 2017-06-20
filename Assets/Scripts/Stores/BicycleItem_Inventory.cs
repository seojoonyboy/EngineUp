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
            case ActionTypes.GARAGE_ITEM_INIT:
                getItems_act getItemsAct = action as getItems_act;
                if(getItemsAct._type == equip_act.type.ITEM) {
                    getItems(action as getItems_act);
                }
                break;
            case ActionTypes.GARAGE_ITEM_EQUIP:
                equip(action as equip_act);
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
            case ActionTypes.GARAGE_ITEM_SORT:
                itemSort(action as itemSort);
                break;
        }
        eventType = action.type;
    }

    //내 아이템 목록 불러오기
    private void getItems(getItems_act payload) {
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
                message = "아이템을 성공적으로 불러왔습니다.";
                allItems = JsonHelper.getJsonArray<RespGetItems>(payload.response.data);
                init();
                itemCategorization(allItems);

                MyInfo act = ActionCreator.createAction(ActionTypes.MYINFO) as MyInfo;
                dispatcher.dispatch(act);

                _emitChange();
                break;
            case NetworkAction.statusTypes.FAIL:
                storeStatus = storeStatus.ERROR;
                message = "아이템 목록을 불러오는 과정에서 문제가 발생하였습니다.";
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
        itemSort act = ActionCreator.createAction(ActionTypes.GARAGE_ITEM_SORT) as itemSort;
        dispatcher.dispatch(act);
    }

    //아이템 정렬
    private void itemSort(itemSort act) {
        int index = PlayerPrefs.GetInt("Filter");
        //Debug.Log("Sorting index : " + index);
        switch (index) {
            case 1:
                wheelItems.Sort(new SortByName());
                frameItems.Sort(new SortByName());
                engineItems.Sort(new SortByName());
                break;
            case 2:
                wheelItems.Sort(new SortByGrade());
                frameItems.Sort(new SortByGrade());
                engineItems.Sort(new SortByGrade());
                break;
        }
        _emitChange();
    }

    //아이템 장착
    private void equip(equip_act payload) {
        if(payload._type != equip_act.type.ITEM) {
            return;
        }
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
                getItems_act act = ActionCreator.createAction(ActionTypes.GARAGE_ITEM_INIT) as getItems_act;
                act._type = equip_act.type.ITEM;
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
                act._type = equip_act.type.ITEM;
                dispatcher.dispatch(act);

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

                getItems_act act = ActionCreator.createAction(ActionTypes.GARAGE_ITEM_INIT) as getItems_act;
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
                var parmStrBuilder = new StringBuilder();
                strBuilder.Remove(0, strBuilder.Length);
                parmStrBuilder.Remove(0, parmStrBuilder.Length);
                List<int> lists = payload.lists;
                int cnt = lists.Count;
                foreach(int id in lists) {
                    parmStrBuilder.Append(id);
                    if (cnt > 1) {
                        parmStrBuilder.Append(",");
                    }
                    cnt--;
                }
                WWWForm form = new WWWForm();
                form.AddField("ids", parmStrBuilder.ToString());

                strBuilder.Append(networkManager.baseUrl)
                    .Append("inventory/items/sell");
                networkManager.request("POST", strBuilder.ToString(), form, ncExt.networkCallback(dispatcher, payload));
                break;
            case NetworkAction.statusTypes.SUCCESS:
                storeStatus = storeStatus.NORMAL;
                Debug.Log("아이템 판매 완료");

                getItems_act act = ActionCreator.createAction(ActionTypes.GARAGE_ITEM_INIT) as getItems_act;
                act._type = equip_act.type.ITEM;
                dispatcher.dispatch(act);

                break;
            case NetworkAction.statusTypes.FAIL:
                storeStatus = storeStatus.ERROR;
                _emitChange();
                break;
        }
    }

    private class SortByGrade : IComparer, IComparer<RespGetItems> {
        public int Compare(RespGetItems x, RespGetItems y) {
            //throw new NotImplementedException();
            int xGrade = x.item.grade;
            int yGrade = y.item.grade;

            if(xGrade == yGrade) {
                return x.id.CompareTo(y.id);
            }
            else {
                return xGrade.CompareTo(yGrade);
            }
        }

        public int Compare(object x, object y) {
            //throw new NotImplementedException();
            RespGetItems _x = x as RespGetItems;
            RespGetItems _y = y as RespGetItems;

            if(_x.id == _y.id) {
                return _x.id.CompareTo(_y.id);
            }
            return Compare(_x, _y);
        }
    }

    private class SortByName : IComparer, IComparer<RespGetItems> {
        public int Compare(RespGetItems x, RespGetItems y) {
            string xName = x.item.name;
            string yName = y.item.name;

            if(xName == yName) {
                return x.id.CompareTo(y.id);
            }
            else {
                return x.item.name.CompareTo(y.item.name);
            }
        }

        public int Compare(object x, object y) {
            RespGetItems _x = x as RespGetItems;
            RespGetItems _y = y as RespGetItems;

            if (_x.id == _y.id) {
                return _x.id.CompareTo(_y.id);
            }
            return Compare(_x, _y);
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