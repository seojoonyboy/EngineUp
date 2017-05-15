using System;
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
        bicycle,
        itemGrid;

    public GameObject 
        sellingModal,
        lockingModal,
        detailModal,
        notifyModal;

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
                //sidebar 갱신
                setSideBar();
            }
        }

        else if(bicycleItemStoreEventType == ActionTypes.GARAGE_ITEM_SORT) {
            makeList();
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
        for(int i=0; i<scrollview.Length; i++) {
            scrollview[i].transform.Find("Grid").GetComponent<UICenterOnChild>().nextPageThreshold = 4;
        }
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
            Debug.Log(obj.GetComponent<ButtonIndex>().index);
            selectedItem = obj;
            onDetailModal();
        }
    }

    public void selectedSiderBar(GameObject obj) {
        detailModal.SetActive(true);
        GameObject modal = detailModal.transform.Find("Modal").gameObject;
        Info info = obj.GetComponent<Info>();

        modal.transform.Find("Name").GetComponent<UILabel>().text = info.name;
        modal.transform.Find("Desc").GetComponent<UILabel>().text = info.desc;
        modal.transform.Find("limitLv").GetComponent<UILabel>().text = "제한 레벨 : " + info.limit_rank;
        UISprite img = modal.transform.Find("Image").GetComponent<UISprite>();
        img.atlas = bicycleAtlas;
        string spriteName = info.imageId + "-1";
        img.spriteName = spriteName;
        img.MakePixelPerfect();

        detailModal.transform.Find("Modal/PutOffButton").gameObject.SetActive(false);
        detailModal.transform.Find("Modal/PutOnButton").gameObject.SetActive(false);
        detailModal.transform.Find("Modal/SellingButton").gameObject.SetActive(false);
    }

    //아이템 상세보기 Modal
    public void onDetailModal() {
        detailModal.SetActive(true);
        detailModal.transform.Find("Modal/SellingButton").gameObject.SetActive(true);
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
        Debug.Log(info.is_equiped);
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
        ArrayList FI = bicycleItemStore.frameItems;
        ArrayList WI = bicycleItemStore.wheelItems;
        ArrayList EI = bicycleItemStore.engineItems;

        int frameItemCnt = FI.Count;
        int wheelItemCnt = WI.Count;
        int engineItemCnt = EI.Count;

        float num = (float)frameItemCnt / pagePerSlotCount;
        int GN = (int)Mathf.Ceil(num);
        int cnt = 0;
        for(int i = 0; i < GN; i++) {
            GameObject grid = Instantiate(itemGrid);
            grid.transform.SetParent(scrollview[0].transform.Find("Grid").transform);

            grid.transform.localScale = Vector3.one;
            grid.transform.localPosition = Vector3.zero;

            UIGrid uiGrid = grid.GetComponent<UIGrid>();
            
            for(int j = 0; j < pagePerSlotCount; j++) {
                if(cnt < frameItemCnt) {
                    Transform slot = uiGrid.GetChild(j).transform;
                    GameObject item = Instantiate(slotItem);

                    item.transform.SetParent(slot);
                    item.transform.localPosition = Vector3.zero;
                    item.transform.localScale = Vector3.one;

                    item.AddComponent<UIDragScrollView>().scrollView = scrollview[0];

                    Info info = item.AddComponent<Info>();
                    BicycleItem data = (BicycleItem)FI[cnt];
                    Item data_item = data.item;
                    info.id = data.id;
                    info.desc = data_item.desc;
                    info.name = data_item.name;
                    info.grade = data_item.grade;
                    info.limit_rank = data_item.limit_rank;
                    info.parts = data_item.parts;
                    info.gear = data_item.gear;
                    info.imageId = data_item.id;

                    if (data.is_equiped == "true") {
                        info.is_equiped = true;
                        equipedItemIndex[0] = info.imageId;
                    }

                    if (data.is_locked == "true") {
                        info.is_locked = true;
                    }

                    EventDelegate.Parameter parm = new EventDelegate.Parameter();
                    EventDelegate onClick = new EventDelegate(this, "selected");
                    parm.obj = item;
                    onClick.parameters[0] = parm;
                    EventDelegate.Add(item.GetComponent<UIButton>().onClick, onClick);

                    UISprite sprite = item.GetComponent<UISprite>();
                    sprite.atlas = bicycleAtlas;
                    string spriteName = info.imageId + "-1";
                    sprite.spriteName = spriteName;

                    cnt++;
                }
            }
        }

        cnt = 0;
        num = (float)wheelItemCnt / pagePerSlotCount;
        GN = (int)Mathf.Ceil(num);
        for (int i = 0; i < GN; i++) {
            GameObject grid = Instantiate(itemGrid);
            grid.transform.SetParent(scrollview[1].transform.Find("Grid").transform);

            grid.transform.localScale = Vector3.one;
            grid.transform.localPosition = Vector3.zero;

            UIGrid uiGrid = grid.GetComponent<UIGrid>();

            for (int j = 0; j < pagePerSlotCount; j++) {
                if(cnt < wheelItemCnt) {
                    Transform slot = uiGrid.GetChild(j).transform;
                    GameObject item = Instantiate(slotItem);

                    item.transform.SetParent(slot);
                    item.transform.localPosition = Vector3.zero;
                    item.transform.localScale = Vector3.one;

                    item.AddComponent<UIDragScrollView>().scrollView = scrollview[1];

                    Info info = item.AddComponent<Info>();
                    BicycleItem data = (BicycleItem)WI[cnt];
                    Item data_item = data.item;
                    info.id = data.id;
                    info.desc = data_item.desc;
                    info.name = data_item.name;
                    info.grade = data_item.grade;
                    info.limit_rank = data_item.limit_rank;
                    info.parts = data_item.parts;
                    info.gear = data_item.gear;
                    info.imageId = data_item.id;

                    if (data.is_equiped == "true") {
                        info.is_equiped = true;
                        equipedItemIndex[1] = info.imageId;
                    }

                    if (data.is_locked == "true") {
                        info.is_locked = true;
                    }

                    EventDelegate.Parameter parm = new EventDelegate.Parameter();
                    EventDelegate onClick = new EventDelegate(this, "selected");
                    parm.obj = item;
                    onClick.parameters[0] = parm;
                    EventDelegate.Add(item.GetComponent<UIButton>().onClick, onClick);

                    UISprite sprite = item.GetComponent<UISprite>();
                    sprite.atlas = bicycleAtlas;
                    string spriteName = info.imageId + "-1";
                    sprite.spriteName = spriteName;

                    cnt++;
                }
            }
        }

        cnt = 0;
        num = (float)engineItemCnt / pagePerSlotCount;
        GN = (int)Mathf.Ceil(num);
        for (int i = 0; i < GN; i++) {
            GameObject grid = Instantiate(itemGrid);
            grid.transform.SetParent(scrollview[2].transform.Find("Grid").transform);

            grid.transform.localScale = Vector3.one;
            grid.transform.localPosition = Vector3.zero;

            UIGrid uiGrid = grid.GetComponent<UIGrid>();

            for (int j = 0; j < pagePerSlotCount; j++) {
                if(cnt < engineItemCnt) {
                    Transform slot = uiGrid.GetChild(j).transform;
                    GameObject item = Instantiate(slotItem);

                    item.transform.SetParent(slot);
                    item.transform.localPosition = Vector3.zero;
                    item.transform.localScale = Vector3.one;

                    item.AddComponent<UIDragScrollView>().scrollView = scrollview[2];

                    Info info = item.AddComponent<Info>();
                    BicycleItem data = (BicycleItem)EI[cnt];
                    Item data_item = data.item;
                    info.id = data.id;
                    info.desc = data_item.desc;
                    info.name = data_item.name;
                    info.grade = data_item.grade;
                    info.limit_rank = data_item.limit_rank;
                    info.parts = data_item.parts;
                    info.gear = data_item.gear;
                    info.imageId = data_item.id;

                    if (data.is_equiped == "true") {
                        info.is_equiped = true;
                        equipedItemIndex[2] = info.imageId;
                    }

                    if (data.is_locked == "true") {
                        info.is_locked = true;
                    }

                    EventDelegate.Parameter parm = new EventDelegate.Parameter();
                    EventDelegate onClick = new EventDelegate(this, "selected");
                    parm.obj = item;
                    onClick.parameters[0] = parm;
                    EventDelegate.Add(item.GetComponent<UIButton>().onClick, onClick);

                    UISprite sprite = item.GetComponent<UISprite>();
                    sprite.atlas = bicycleAtlas;
                    string spriteName = info.imageId + "-1";
                    sprite.spriteName = spriteName;

                    cnt++;
                }
            }
        }
        initGrid();
        setSideBar();
    }

    private void setSideBar() {
        GameObject sideSlot = sideBar.transform.Find("WheelSlot/Item").gameObject;
        Info sideBarInfo = sideSlot.AddComponent<Info>();
        UISprite sideSprite;
        if (bicycleItemStore.equipedItemIndex[0] != null) {
            sideBarInfo.imageId = bicycleItemStore.equipedItemIndex[0].item.id;
            sideBarInfo.desc = bicycleItemStore.equipedItemIndex[0].item.desc;
            sideBarInfo.name = bicycleItemStore.equipedItemIndex[0].item.name;
            sideBarInfo.limit_rank = bicycleItemStore.equipedItemIndex[0].item.limit_rank;
            sideBarInfo.gear = bicycleItemStore.equipedItemIndex[0].item.gear;
            sideBarInfo.id = bicycleItemStore.equipedItemIndex[0].id;
            sideBarInfo.is_equiped = true;

            sideSprite = sideSlot.GetComponent<UISprite>();
            sideSprite.atlas = bicycleAtlas;
            sideSprite.spriteName = sideBarInfo.imageId + "-1";
        } 
        else {
            sideSlot.GetComponent<UISprite>().spriteName = "-1";
        }

        sideSlot = sideBar.transform.Find("FrameSlot/Item").gameObject;
        sideBarInfo = sideSlot.AddComponent<Info>();
        if(bicycleItemStore.equipedItemIndex[1] != null) {
            sideBarInfo.imageId = bicycleItemStore.equipedItemIndex[1].item.id;
            sideBarInfo.desc = bicycleItemStore.equipedItemIndex[1].item.desc;
            sideBarInfo.name = bicycleItemStore.equipedItemIndex[1].item.name;
            sideBarInfo.limit_rank = bicycleItemStore.equipedItemIndex[1].item.limit_rank;
            sideBarInfo.gear = bicycleItemStore.equipedItemIndex[1].item.gear;
            sideBarInfo.id = bicycleItemStore.equipedItemIndex[1].id;
            sideBarInfo.is_equiped = true;

            sideSprite = sideSlot.GetComponent<UISprite>();
            sideSprite.atlas = bicycleAtlas;
            sideSprite.spriteName = bicycleItemStore.equipedItemIndex[1].item.id + "-1";
        }
        else {
            sideSlot.GetComponent<UISprite>().spriteName = "-1";
        }

        sideSlot = sideBar.transform.Find("EngineSlot/Item").gameObject;
        sideBarInfo = sideSlot.AddComponent<Info>();
        if(bicycleItemStore.equipedItemIndex[2] != null) {
            sideBarInfo.imageId = bicycleItemStore.equipedItemIndex[2].item.id;
            sideBarInfo.desc = bicycleItemStore.equipedItemIndex[2].item.desc;
            sideBarInfo.name = bicycleItemStore.equipedItemIndex[2].item.name;
            sideBarInfo.limit_rank = bicycleItemStore.equipedItemIndex[2].item.limit_rank;
            sideBarInfo.gear = bicycleItemStore.equipedItemIndex[2].item.gear;
            sideBarInfo.id = bicycleItemStore.equipedItemIndex[2].id;
            sideBarInfo.is_equiped = true;

            sideSprite = sideSlot.GetComponent<UISprite>();
            sideSprite.atlas = bicycleAtlas;
            sideSprite.spriteName = bicycleItemStore.equipedItemIndex[2].item.id + "-1";
        }
        else {
            sideSlot.GetComponent<UISprite>().spriteName = "-1";
        }
    }

    private void removeList() {
        for(int i=0; i<scrollview.Length; i++) {
            Transform grid = scrollview[i].transform.Find("Grid").transform;
            grid.DestroyChildren();
        }
    }

    private void initGrid() {
        for(int i=0; i<scrollview.Length; i++) {
            UIGrid grid = scrollview[i].transform.Find("Grid").GetComponent<UIGrid>();
            grid.repositionNow = true;
            grid.Reposition();
        }
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
        getItems_act act = ActionCreator.createAction(ActionTypes.GARAGE_ITEM_INIT) as getItems_act;
        act._type = equip_act.type.ITEM;
        gm.gameDispatcher.dispatch(act);

        int index = PlayerPrefs.GetInt("Filter");

        filterSelected(index);
    }

    public void onFilterButton(GameObject obj) {
        GameObject menu = obj.transform.Find("DropMenu").gameObject;
        menu.SetActive(!menu.activeSelf);
    }

    public void filterSelected(object obj) {
        int index = 0;
        Type type = obj.GetType();

        if(type == typeof(int)) {
            index = (int)obj;
        }
        else if(type == typeof(GameObject)) {
            index = ((GameObject)obj).GetComponent<ButtonIndex>().index;
            ((GameObject)obj).transform.parent.parent.parent.gameObject.SetActive(false);
        }
        PlayerPrefs.SetInt("Filter", index);
        itemSort act = ActionCreator.createAction(ActionTypes.GARAGE_ITEM_SORT) as itemSort;
        switch (index) {
            case 1:
                Debug.Log("이름순 정렬");
                act._type = itemSort.type.NAME;
                break;
            case 2:
                Debug.Log("등급순 정렬");
                act._type = itemSort.type.GRADE;
                break;
        }
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
