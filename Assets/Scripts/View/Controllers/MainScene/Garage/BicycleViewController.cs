using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BicycleViewController : MonoBehaviour {
    private GameManager gm;
    public BicycleItem_Inventory bicycleItemStore;
    public Char_Inventory charItemStore;
    public User userStore;

    //판매 버튼 클릭시
    private bool 
        isSellMode = false,
        isLockMode = false,
        isSingleSellOrLock = false;

    public GameObject 
        slotItem,
        sideBar,
        selectedItem;

    public GameObject
        sellButton,
        lockButton,
        bicycle;

    public GameObject 
        sellingModal,
        lockingModal,
        detailModal,
        notifyModal;

    public UIGrid[] 
        framePageGrids,
        wheelPageGrids,
        enginePageGrids;

    public int pagePerSlotCount;

    public UILabel[] spects;
    public UIScrollView[] scrollview;

    public int[] equipedItemIndex;
    public UIAtlas 
        atlas,
        bicycleAtlas;

    List<int> lockIdList = new List<int>();
    List<int> unlockList = new List<int>();
    List<Info> sellList = new List<Info>();

    public UILabel lvLavel;

    void Awake() {
        gm = GameManager.Instance;
    }

    public void onBicycleItemStoreListener() {
        ActionTypes bicycleItemStoreEventType = bicycleItemStore.eventType;

        if (bicycleItemStoreEventType == ActionTypes.GARAGE_ITEM_INIT) {
            if (bicycleItemStore.storeStatus == storeStatus.NORMAL) {
                if(gameObject.activeSelf) {
                    makeList();
                }
            }
        }

        else if(bicycleItemStoreEventType == ActionTypes.GARAGE_ITEM_EQUIP) {
            if(bicycleItemStore.storeStatus == storeStatus.NORMAL) {
                makeList();
            }
        }
    }

    public void onCharStoreListener() {
        
    }

    public void onUserStoreListener() {
        ActionTypes userStoreEventType = userStore.eventType;
        lvLavel.text = "Lv. " + userStore.myData.status.rank.ToString();
    }

    void OnEnable() {
        itemInitAct();
    }

    void Start() {
        
    }

    void OnDisable() {
        selectedItem = null;
        detailModal.SetActive(false);
    }

    //판매 버튼 클릭시
    public void setSellingMode() {
        bool isOn = sellButton.GetComponent<boolIndex>().isOn;

        if (lockButton.GetComponent<boolIndex>().isOn) {
            lockingModal.SetActive(true);
            return;
        }

        if (isOn) {
            //최종 판매
            Debug.Log("최종 판매");
            sellButton.transform.Find("Label").GetComponent<UILabel>().text = "판매";
            sellingModal.SetActive(true);
        }
        else {
            sellButton.transform.Find("Label").GetComponent<UILabel>().text = "최종 판매";
        }
        isSellMode = !isOn;
        sellButton.GetComponent<boolIndex>().isOn = isSellMode;
    }

    //잠금 버튼 클릭시
    public void setLockingMode() {
        bool isOn = lockButton.GetComponent<boolIndex>().isOn;

        if(sellButton.GetComponent<boolIndex>().isOn) {
            sellingModal.SetActive(true);
            return;
        }

        if(isOn) {
            Debug.Log("최종 잠금");
            lockingModal.SetActive(true);
            lockButton.transform.Find("Label").GetComponent<UILabel>().text = "잠금";
        }
        else {
            lockButton.transform.Find("Label").GetComponent<UILabel>().text = "최종 잠금";
        }
        isLockMode = !isOn;
        lockButton.GetComponent<boolIndex>().isOn = isLockMode;
    }

    public void selected(GameObject obj) {
        Info info = obj.GetComponent<Info>();
        //판매모드인 경우
        if (isSellMode) {
            if(obj.transform.Find("TypeTag").tag == "locked") {
                return;
            }
            
            GameObject tmp = obj.transform.Find("Selected").gameObject;
            tmp.SetActive(!tmp.activeSelf);

            if(tmp.activeSelf) {
                obj.tag = "selected";
                sellList.Add(info);
            }
            else {
                obj.tag = "unselected";
            }
        }
        //잠금모드인 경우
        else if (isLockMode) {
            GameObject tmp = obj.transform.Find("LockIcon").gameObject;
            tmp.SetActive(!tmp.activeSelf);
            if(tmp.activeSelf) {
                obj.tag = "locked";
                //int 리스트
                //리스트에 id값(int)을 담는다. (button index 말고 실제 아이템 id)
                lockIdList.Add(info.id);
                if (unlockList.Contains(info.id)) {
                    unlockList.Remove(info.id);
                }
            }
            else {
                obj.tag = "unselected";
                //리스트에 담겨 있으면 제외시킨다.
                unlockList.Add(info.id);
                if (lockIdList.Contains(info.id)) {
                    lockIdList.Remove(info.id);
                }
            }
        }
        else if(isLockMode == false && isSellMode == false) {
            //Debug.Log(obj.GetComponent<ButtonIndex>().index);
            selectedItem = obj;
            onDetailModal();
        }
    }

    //아이템 상세보기 Modal
    public void onDetailModal() {
        detailModal.SetActive(true);
        isSingleSellOrLock = true;
        GameObject modal = detailModal.transform.Find("Modal").gameObject;
        Info info = selectedItem.GetComponent<Info>();

        modal.transform.Find("Name").GetComponent<UILabel>().text = info.name;
        modal.transform.Find("Desc").GetComponent<UILabel>().text = info.desc;
        modal.transform.Find("limitLv").GetComponent<UILabel>().text = "제한 레벨 : " + info.limit_rank;
        UISprite img = modal.transform.Find("Image").GetComponent<UISprite>();
        img.atlas = bicycleAtlas;
        string spriteName = info.imageId + "-1";
        img.spriteName = spriteName;
        img.MakePixelPerfect();

        //현재 장착중인 아이템인 경우
        //모달 내 해제하기 버튼 활성화
        if (info.is_equiped) {
            detailModal.transform.Find("Modal/PutOffButton").gameObject.SetActive(true);
            detailModal.transform.Find("Modal/PutOnButton").gameObject.SetActive(false);
        }
        //장착중인 아이템이 아닌 경우
        //모달 내 장착하기 버튼 활성화
        else {
            detailModal.transform.Find("Modal/PutOffButton").gameObject.SetActive(false);
            detailModal.transform.Find("Modal/PutOnButton").gameObject.SetActive(true);
        }
    }

    //아이템 장착
    public void equip() {
        if(selectedItem == null) {
            return;
        }

        //아이템을 장착할 수 있는 등급인지
        int myRank = userStore.myData.status.rank;
        Info info = selectedItem.GetComponent<Info>();
        int index = info.id;
        int itemRank = info.limit_rank;
        if (myRank >= itemRank) {
            equip_act act = ActionCreator.createAction(ActionTypes.GARAGE_ITEM_EQUIP) as equip_act;
            act._type = equip_act.type.ITEM;
            act.id = index;
            gm.gameDispatcher.dispatch(act);
        }
        else {
            notifyModal.SetActive(true);
            notifyModal.transform.Find("Modal/Label").GetComponent<UILabel>().text = "등급이 낮아 아이템을 장착할 수 없습니다.";
        }
    }

    //아이템 해제
    public void unequip() {
        if(selectedItem == null) {
            return;
        }

        unequip_act act = ActionCreator.createAction(ActionTypes.GARAGE_ITEM_UNEQUIP) as unequip_act;
        int tmp = selectedItem.GetComponent<ButtonIndex>().index;
        Info info = selectedItem.GetComponent<Info>();
        int index = info.id;
        act.id = index;
        gm.gameDispatcher.dispatch(act);
    }

    public void makeList() {
        removeList();
        //slot 생성
        BicycleItem[] items;
        init();
        UISprite sprite = null;
        //프레임 리스트 생성
        int cnt = 0;
        items = gm.bicycleInventStore.frameItems.ToArray(typeof(BicycleItem)) as BicycleItem[];
        for (int i = 0; i < framePageGrids.Length; i++) {
            for (int j = 0; j < pagePerSlotCount; j++) {
                cnt++;
                if (cnt > items.Length) {
                    break;
                }
                GameObject item = Instantiate(slotItem);
                item.name = "item" + cnt;
                Transform parent = framePageGrids[i].GetChild(j).transform;
                item.transform.SetParent(parent);
                item.transform.localScale = Vector3.one;
                item.transform.localPosition = Vector3.zero;
                item.GetComponent<ButtonIndex>().index = cnt - 1;

                Info info = item.AddComponent<Info>();
                item.AddComponent<UIDragScrollView>().scrollView = scrollview[0];
                info.id = items[cnt - 1].id;
                info.parts = items[cnt - 1].item.parts;
                info.name = items[cnt - 1].item.name;
                info.desc = items[cnt - 1].item.desc;
                info.imageId = items[cnt - 1].item.id;
                info.limit_rank = items[cnt - 1].item.limit_rank;
                info.gear = items[cnt - 1].item.gear;

                if (items[cnt - 1].is_equiped == "true") {
                    Debug.Log("FR Equiped");
                    info.is_equiped = true;
                    item.transform.Find("Equiped").gameObject.SetActive(true);
                    equipedItemIndex[1] = items[cnt - 1].item.id;

                    sprite = bicycle.transform.Find("Frame").GetComponent<UISprite>();
                    sprite.atlas = bicycleAtlas;
                    sprite.spriteName = info.imageId.ToString();
                }
                else {
                    info.is_equiped = false;
                }

                if(items[cnt - 1].is_locked == "true") {
                    info.is_locked = true;
                    item.transform.Find("LockIcon").gameObject.SetActive(true);
                    item.transform.Find("TypeTag").gameObject.tag = "locked";
                }
                else {
                    info.is_locked = false;
                }

                EventDelegate.Parameter param = new EventDelegate.Parameter();
                EventDelegate onClick = new EventDelegate(this, "selected");
                param.obj = item;
                onClick.parameters[0] = param;
                EventDelegate.Add(item.GetComponent<UIButton>().onClick, onClick);

                sprite = item.GetComponent<UISprite>();
                sprite.atlas = bicycleAtlas;
                //image의 id값
                string spriteName = items[cnt - 1].item.id + "-1";
                sprite.spriteName = spriteName;
            }
        }

        //바퀴 리스트 생성
        cnt = 0;
        items = gm.bicycleInventStore.wheelItems.ToArray(typeof(BicycleItem)) as BicycleItem[];
        for (int i = 0; i < wheelPageGrids.Length; i++) {
            for (int j = 0; j < pagePerSlotCount; j++) {
                cnt++;
                if (cnt > items.Length) {
                    break;
                }

                GameObject item = Instantiate(slotItem);
                item.name = "item" + cnt;

                Transform parent = wheelPageGrids[i].GetChild(j).transform;
                item.transform.SetParent(parent);
                item.transform.localScale = Vector3.one;
                item.transform.localPosition = Vector3.zero;
                item.GetComponent<ButtonIndex>().index = cnt - 1;

                Info info = item.AddComponent<Info>();
                item.AddComponent<UIDragScrollView>().scrollView = scrollview[1];
                info.id = items[cnt - 1].id;
                info.parts = items[cnt - 1].item.parts;
                info.name = items[cnt - 1].item.name;
                info.desc = items[cnt - 1].item.desc;
                info.imageId = items[cnt - 1].item.id;
                info.limit_rank = items[cnt - 1].item.limit_rank;
                info.gear = items[cnt - 1].item.gear;

                if (items[cnt - 1].is_equiped == "true") {
                    Debug.Log("WH Equiped");
                    info.is_equiped = true;
                    item.transform.Find("Equiped").gameObject.SetActive(true);
                    equipedItemIndex[0] = items[cnt - 1].item.id;

                    sprite = bicycle.transform.Find("Wheel").GetComponent<UISprite>();
                    sprite.atlas = bicycleAtlas;
                    sprite.spriteName = info.imageId.ToString();
                }
                else {
                    info.is_equiped = false;
                }

                if (items[cnt - 1].is_locked == "true") {
                    info.is_locked = true;
                    item.transform.Find("LockIcon").gameObject.SetActive(true);
                    item.transform.Find("TypeTag").gameObject.tag = "locked";
                }
                else {
                    info.is_locked = false;
                }

                EventDelegate.Parameter param = new EventDelegate.Parameter();
                EventDelegate onClick = new EventDelegate(this, "selected");
                param.obj = item;
                onClick.parameters[0] = param;
                EventDelegate.Add(item.GetComponent<UIButton>().onClick, onClick);

                sprite = item.GetComponent<UISprite>();
                sprite.atlas = bicycleAtlas;
                string spriteName = items[cnt - 1].item.id + "-1";
                sprite.spriteName = spriteName;
            }
        }

        //구동계 리스트 생성
        cnt = 0;
        items = gm.bicycleInventStore.engineItems.ToArray(typeof(BicycleItem)) as BicycleItem[];
        for (int i = 0; i < enginePageGrids.Length; i++) {
            for (int j = 0; j < pagePerSlotCount; j++) {
                cnt++;
                if (cnt > items.Length) {
                    break;
                }
                GameObject item = Instantiate(slotItem);
                item.name = "item" + cnt;

                Transform parent = enginePageGrids[i].GetChild(j).transform;
                item.transform.SetParent(parent);
                item.transform.localScale = Vector3.one;
                item.transform.localPosition = Vector3.zero;
                item.GetComponent<ButtonIndex>().index = cnt - 1;

                Info info = item.AddComponent<Info>();
                item.AddComponent<UIDragScrollView>().scrollView = scrollview[2];
                info.id = items[cnt - 1].id;
                info.parts = items[cnt - 1].item.parts;
                info.name = items[cnt - 1].item.name;
                info.desc = items[cnt - 1].item.desc;
                info.imageId = items[cnt - 1].item.id;
                info.limit_rank = items[cnt - 1].item.limit_rank;
                info.gear = items[cnt - 1].item.gear;

                if (items[cnt - 1].is_equiped == "true") {
                    Debug.Log("EG Equiped");
                    info.is_equiped = true;
                    item.transform.Find("Equiped").gameObject.SetActive(true);
                    equipedItemIndex[2] = items[cnt - 1].item.id;

                    sprite = bicycle.transform.Find("Engine").GetComponent<UISprite>();
                    sprite.atlas = bicycleAtlas;
                    sprite.spriteName = info.imageId.ToString();
                }
                else {
                    info.is_equiped = false;
                }

                if (items[cnt - 1].is_locked == "true") {
                    info.is_locked = true;
                    item.transform.Find("LockIcon").gameObject.SetActive(true);
                    item.transform.Find("TypeTag").gameObject.tag = "locked";
                }
                else {
                    info.is_locked = false;
                }

                EventDelegate.Parameter param = new EventDelegate.Parameter();
                EventDelegate onClick = new EventDelegate(this, "selected");
                param.obj = item;
                onClick.parameters[0] = param;
                EventDelegate.Add(item.GetComponent<UIButton>().onClick, onClick);

                sprite = item.GetComponent<UISprite>();
                sprite.atlas = bicycleAtlas;
                string spriteName = items[cnt - 1].item.id + "-1";
                sprite.spriteName = spriteName;
            }
        }

        //SideBar 갱신
        for (int i=0; i < equipedItemIndex.Length; i++) {
            switch (i) {
                case 0:
                    sprite = sideBar.transform.Find("FrameSlot/Item").GetComponent<UISprite>();
                    sideBar.transform.Find("FrameSlot/Item/Label").gameObject.SetActive(false);
                    break;
                case 1:
                    sprite = sideBar.transform.Find("WheelSlot/Item").GetComponent<UISprite>();
                    sideBar.transform.Find("WheelSlot/Item/Label").gameObject.SetActive(false);
                    break;
                case 2:
                    sprite = sideBar.transform.Find("EngineSlot/Item").GetComponent<UISprite>();
                    sideBar.transform.Find("EngineSlot/Item/Label").gameObject.SetActive(false);
                    break;
            }
            sprite.atlas = bicycleAtlas;
            if (equipedItemIndex[i] == -1) {
                sprite.spriteName = "default";
            }
            else {
                int index = equipedItemIndex[i];
                string spriteName = index + "-1";
                sprite.spriteName = spriteName;
            }
        }
    }

    private void removeList() {
        for(int i=0; i< framePageGrids.Length; i++) {
            foreach(Transform slot in framePageGrids[i].transform) {
                slot.DestroyChildren();
            }
        }
        for (int i = 0; i < wheelPageGrids.Length; i++) {
            foreach (Transform slot in wheelPageGrids[i].transform) {
                slot.DestroyChildren();
            }
        }
        for (int i = 0; i < enginePageGrids.Length; i++) {
            foreach (Transform slot in enginePageGrids[i].transform) {
                slot.DestroyChildren();
            }
        }
    }

    private void initGrid() {
        
    }

    private void init() {
        equipedItemIndex[0] = -1;
        equipedItemIndex[1] = -1;
        equipedItemIndex[2] = -1;
        lockIdList.Clear();
        sellList.Clear();
    }

    public void locking() {
        garage_lock_act act = ActionCreator.createAction(ActionTypes.GARAGE_LOCK) as garage_lock_act;
        act.type = "lock";
        //단일 잠금
        if (isSingleSellOrLock) {
            act.id = selectedItem.GetComponent<Info>().id;
            gm.gameDispatcher.dispatch(act);
        }
        //다중 잠금
        else {
            foreach (int id in lockIdList) {
                act.id = id;
                gm.gameDispatcher.dispatch(act);
            }
            act.type = "unlock";
            foreach (int id in unlockList) {
                act.id = id;
                gm.gameDispatcher.dispatch(act);
            }
        }
        isSingleSellOrLock = false;
    }

    public void selling() {
        garage_sell_act act = ActionCreator.createAction(ActionTypes.GARAGE_SELL) as garage_sell_act;
        int gears = 0;
        //단일 판매
        Debug.Log(isSingleSellOrLock);
        if (isSingleSellOrLock) {
            Debug.Log("단일 판매");
            Info info = selectedItem.GetComponent<Info>();
            if (info.is_locked) {
                //잠금이 되어 있어 팔지 못함 Modal 표시
                return;
            }
            act.id = info.id;
            gm.gameDispatcher.dispatch(act);
            gears += info.gear;
        }
        //다중 판매
        else {
            foreach (Info info in sellList) {
                act.id = info.id;
                gm.gameDispatcher.dispatch(act);
                gears += info.gear;
            }
        }
        isSingleSellOrLock = false;
        notifyModal.SetActive(true);
        notifyModal.transform.Find("Modal/Label").GetComponent<UILabel>().text = "총 " + gears + "개의 기어를 획득하였습니다.";
    }

    public void offSellingModal() {
        sellingModal.SetActive(false);
        sellButton.GetComponent<boolIndex>().isOn = false;
        isSellMode = false;
        itemInitAct();
    }

    public void offLockingModal() {
        lockingModal.SetActive(false);
        lockButton.GetComponent<boolIndex>().isOn = false;
        isLockMode = false;
        itemInitAct();
    }

    public void offNotfiyModal() {
        notifyModal.SetActive(false);
    }

    private void clearList() {
        sellList.Clear();
        lockIdList.Clear();
        unlockList.Clear();
    }

    public void offDetailModal() {
        detailModal.SetActive(false);
    }

    private void itemInitAct() {
        Debug.Log("Init");
        getItems_act act = ActionCreator.createAction(ActionTypes.GARAGE_ITEM_INIT) as getItems_act;
        act._type = equip_act.type.ITEM;
        gm.gameDispatcher.dispatch(act);
    }

    private class Info : MonoBehaviour {
        public int id;
        public bool is_equiped;
        public bool is_locked;
        public int imageId;

        public string name;
        public string desc;
        public int grade;
        public int gear;
        public string parts;
        public int limit_rank;
    }

    public void offPanel() {
        gameObject.SetActive(false);
    }
}
