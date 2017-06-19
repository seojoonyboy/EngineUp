using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
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
        itemGrid,
        blockingCollPanel;

    public GameObject 
        sellingModal,
        lockingModal,
        detailModal,
        notifyModal;

    public int pagePerSlotCount;

    public UILabel[] 
        spects,
        incSpects;

    public UIScrollView[] scrollview;

    public int[] equipedItemIndex;
    public UIAtlas 
        atlas,
        bicycleAtlas;
    public AudioClip[] audioClip;

    List<int> lockIdList = new List<int>();
    List<int> unlockList = new List<int>();
    List<Info> sellList = new List<Info>();

    public UILabel lvLavel;

    private TweenPosition tP;
    private bool 
        isReverse_tp,
        isTweening = false;

    void Awake() {
        gm = GameManager.Instance;
        tP = gameObject.transform.Find("Background").GetComponent<TweenPosition>();
    }

    public void onBicycleItemStoreListener() {
        ActionTypes bicycleItemStoreEventType = bicycleItemStore.eventType;

        if (bicycleItemStoreEventType == ActionTypes.GARAGE_ITEM_SORT) {
            makeList();
        }

        if(bicycleItemStoreEventType == ActionTypes.GARAGE_ITEM_INIT) {
            if(bicycleItemStore.storeStatus == storeStatus.NORMAL) {
                setStat();
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

        tweenPos();
        blockingCollPanel.SetActive(true);
        isReverse_tp = false;
        tP.ResetToBeginning();
    }

    public void tweenPos() {
        if (isTweening) {
            return;
        }
        isTweening = true;
        blockingCollPanel.SetActive(true);
        if (!isReverse_tp) {
            tP.PlayForward();
        }
        else {
            //swap
            Vector3 tmp;
            tmp = tP.to;
            tP.to = tP.from;
            tP.from = tmp;

            tP.ResetToBeginning();
            tP.PlayForward();
        }
    }

    void Start() {
        for(int i=0; i<scrollview.Length; i++) {
            scrollview[i].transform.Find("Grid").GetComponent<UICenterOnChild>().nextPageThreshold = 4;
        }
    }

    public void tpFinished() {
        isTweening = false;
        blockingCollPanel.SetActive(false);

        if (isReverse_tp) {
            gameObject.SetActive(false);
            gameObject.transform.Find("TopPanel").gameObject.SetActive(false);
        }

        else {
            gameObject.transform.Find("TopPanel").gameObject.SetActive(true);
        }

        isReverse_tp = true;
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
            sellButton.transform.Find("Label").GetComponent<UILabel>().text = "판매";
            if(sellList.Count != 0) {
                sellingModal.SetActive(true);
            }
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
            lockButton.transform.Find("Label").GetComponent<UILabel>().text = "잠금";
            if (lockIdList.Count != 0) {
                lockingModal.SetActive(true);
            }
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
            if (obj.tag == "locked") { return; }
            if (obj.transform.Find("TypeTag").tag == "locked") {
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
            if (obj.tag == "locked") { return; }
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
            selectedItem = obj;
            onDetailModal();
        }
    }

    //아이템 상세보기 Modal
    public void onDetailModal() {
        detailModal.SetActive(true);
        detailModal.transform.Find("Modal/SellingButton").gameObject.SetActive(true);
        isSingleSellOrLock = true;
        GameObject modal = detailModal.transform.Find("Modal").gameObject;
        Info info = selectedItem.GetComponent<Info>();

        StringBuilder sb = new StringBuilder();
        int val = info.strength;
        if(val != 0) {
            string str = "근력 + " + val + "\n";
            sb.Append(str);
        }
        val = info.endurance;
        if(val != 0) {
            string str = "지구력 + " + val + "\n";
            sb.Append(str);
        }
        val = info.speed;
        if (val != 0) {
            string str = "스피드 + " + val + "\n";
            sb.Append(str);
        }
        val = info.recovery;
        if (val != 0) {
            string str = "회복력 + " + val + "\n";
            sb.Append(str);
        }

        modal.transform.Find("Spec").GetComponent<UILabel>().text = sb.ToString();
        modal.transform.Find("Name").GetComponent<UILabel>().text = info.name;
        modal.transform.Find("Desc").GetComponent<UILabel>().text = info.desc;
        modal.transform.Find("limitLv").GetComponent<UILabel>().text = "제한 레벨 : " + info.limit_rank;
        UISprite img = modal.transform.Find("Image").GetComponent<UISprite>();
        img.atlas = bicycleAtlas;
        string spriteName = info.imageId + "-1";
        img.spriteName = spriteName;
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
            notifyModal.GetComponent<UIPlaySound>().audioClip = audioClip[0];
            notifyModal.GetComponent<UIPlaySound>().Play();
        }
    }

    //아이템 해제
    public void unequip() {
        if(selectedItem == null) {
            return;
        }

        unequip_act act = ActionCreator.createAction(ActionTypes.GARAGE_ITEM_UNEQUIP) as unequip_act;
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
                    RespGetItems data = (RespGetItems)FI[cnt];
                    RespItem _item = data.item;
                    info.id = data.id;
                    info.desc = _item.desc;
                    info.name = _item.name;
                    info.grade = _item.grade;
                    info.limit_rank = _item.limit_rank;
                    info.parts = _item.parts;
                    info.gear = _item.gear;
                    info.imageId = _item.id;
                    info.speed = _item.speed;
                    info.endurance = _item.endurance;
                    info.strength = _item.strength;
                    info.recovery = _item.regeneration;

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
                    RespGetItems data = (RespGetItems)WI[cnt];
                    RespItem data_item = data.item;
                    info.id = data.id;
                    info.desc = data_item.desc;
                    info.name = data_item.name;
                    info.grade = data_item.grade;
                    info.limit_rank = data_item.limit_rank;
                    info.parts = data_item.parts;
                    info.gear = data_item.gear;
                    info.imageId = data_item.id;
                    info.speed = data_item.speed;
                    info.endurance = data_item.endurance;
                    info.strength = data_item.strength;
                    info.recovery = data_item.regeneration;

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
                    RespGetItems data = (RespGetItems)EI[cnt];
                    RespItem data_item = data.item;
                    info.id = data.id;
                    info.desc = data_item.desc;
                    info.name = data_item.name;
                    info.grade = data_item.grade;
                    info.limit_rank = data_item.limit_rank;
                    info.parts = data_item.parts;
                    info.gear = data_item.gear;
                    info.imageId = data_item.id;
                    info.speed = data_item.speed;
                    info.endurance = data_item.endurance;
                    info.strength = data_item.strength;
                    info.recovery = data_item.regeneration;

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
        setMainStageImage();
        setSideBar();
    }

    private void initStat() {
        for(int i=0; i<incSpects.Length; i++) {
            incSpects[i].gameObject.SetActive(true);
        }
    }

    private void setStat() {
        initStat();

        status status = userStore.myData.status;

        int endurance = status.endurance;
        int speed = status.speed;
        int recovery = status.regeneration;
        int strength = status.strength;

        spects[0].text = strength.ToString();
        spects[1].text = speed.ToString();
        spects[2].text = endurance.ToString();
        spects[3].text = recovery.ToString();

        int incEnd = 0;
        int incSpeed = 0;
        int incRecovery = 0;
        int incStrength = 0;

        RespGetItems equipedItem;
        for(int i=0; i<bicycleItemStore.equipedItemIndex.Length; i++) {
            equipedItem = bicycleItemStore.equipedItemIndex[i];
            if (equipedItem != null) {
                incEnd += equipedItem.item.strength;
                incSpeed += equipedItem.item.speed;
                incRecovery += equipedItem.item.regeneration;
                incStrength += equipedItem.item.strength;
            }
        }

        if(incStrength == 0) {
            incSpects[0].gameObject.SetActive(false);
        } else {
            incSpects[0].text = "+ " + incStrength;
        }

        if(incSpeed == 0) {
            incSpects[1].gameObject.SetActive(false);
        } else {
            incSpects[1].text = "+ " + incSpeed;
        }

        if (incRecovery == 0) {
            incSpects[2].gameObject.SetActive(false);
        } else {
            incSpects[2].text = "+ " + incRecovery;
        }

        if (incEnd == 0) {
            incSpects[3].gameObject.SetActive(false);
        } else {
            incSpects[3].text = "+ " + incEnd;
        }
    }

    private void setMainStageImage() {
        UISprite sprite;

        RespGetItems equipedItem = bicycleItemStore.equipedItemIndex[0];
        sprite = bicycle.transform.Find("Wheel").GetComponent<UISprite>();
        sprite.atlas = bicycleAtlas;
        if (equipedItem != null) {
            RespItem _item = equipedItem.item;
            sprite.spriteName = _item.id.ToString();
        }
        else {
            sprite.spriteName = "6";
        }

        equipedItem = bicycleItemStore.equipedItemIndex[1];
        sprite = bicycle.transform.Find("Frame").GetComponent<UISprite>();
        if (equipedItem != null) {
            RespItem _item = equipedItem.item;
            sprite.spriteName = _item.id.ToString();
        }
        else {
            sprite.spriteName = "3";
        }

        equipedItem = bicycleItemStore.equipedItemIndex[2];
        sprite = bicycle.transform.Find("Engine").GetComponent<UISprite>();
        if (equipedItem != null) {
            RespItem _item = equipedItem.item;
            sprite.spriteName = _item.id.ToString();
        }
        else {
            sprite.spriteName = "9";
        }
    }

    private void setSideBar() {
        GameObject sideSlot = sideBar.transform.Find("WheelSlot/Item").gameObject;
        Info sideBarInfo = sideSlot.GetComponent<Info>();
        if(sideBarInfo == null) {
            sideBarInfo = sideSlot.AddComponent<Info>();
        }
        UISprite sideSprite;
        RespGetItems equipedItem = bicycleItemStore.equipedItemIndex[0];
        sideSprite = sideSlot.GetComponent<UISprite>();
        if (equipedItem != null) {
            RespItem _item = equipedItem.item;
            sideBarInfo.imageId = _item.id;
            sideBarInfo.desc = _item.desc;
            sideBarInfo.name = _item.name;
            sideBarInfo.limit_rank = _item.limit_rank;
            sideBarInfo.gear = _item.gear;
            sideBarInfo.speed = _item.speed;
            sideBarInfo.strength = _item.strength;
            sideBarInfo.endurance = _item.endurance;
            sideBarInfo.recovery = _item.regeneration;

            sideBarInfo.id = equipedItem.id;
            sideBarInfo.is_equiped = true;
            sideSprite.atlas = bicycleAtlas;
            sideSprite.spriteName = sideBarInfo.imageId + "-1";
        }
        else {
            sideSprite.spriteName = "-1";
        }

        sideSlot = sideBar.transform.Find("FrameSlot/Item").gameObject;
        sideBarInfo = sideSlot.GetComponent<Info>();
        if (sideBarInfo == null) {
            sideBarInfo = sideSlot.AddComponent<Info>();
        }

        equipedItem = bicycleItemStore.equipedItemIndex[1];
        sideSprite = sideSlot.GetComponent<UISprite>();
        if (equipedItem != null) {
            RespItem _item = equipedItem.item;
            sideBarInfo.imageId = _item.id;
            sideBarInfo.desc = _item.desc;
            sideBarInfo.name = _item.name;
            sideBarInfo.limit_rank = _item.limit_rank;
            sideBarInfo.gear = _item.gear;
            sideBarInfo.speed = _item.speed;
            sideBarInfo.strength = _item.strength;
            sideBarInfo.endurance = _item.endurance;
            sideBarInfo.recovery = _item.regeneration;

            sideBarInfo.id = equipedItem.id;
            sideBarInfo.is_equiped = true;

            sideSprite.atlas = bicycleAtlas;
            sideSprite.spriteName = sideBarInfo.imageId + "-1";
        }
        else {
            sideSprite.spriteName = "-1";
        }

        sideSlot = sideBar.transform.Find("EngineSlot/Item").gameObject;
        sideBarInfo = sideSlot.GetComponent<Info>();
        if (sideBarInfo == null) {
            sideBarInfo = sideSlot.AddComponent<Info>();
        }

        equipedItem = bicycleItemStore.equipedItemIndex[2];
        sideSprite = sideSlot.GetComponent<UISprite>();
        if (equipedItem != null) {
            RespItem _item = equipedItem.item;
            sideBarInfo.imageId = _item.id;
            sideBarInfo.desc = _item.desc;
            sideBarInfo.name = _item.name;
            sideBarInfo.limit_rank = _item.limit_rank;
            sideBarInfo.gear = _item.gear;
            sideBarInfo.speed = _item.speed;
            sideBarInfo.strength = _item.strength;
            sideBarInfo.endurance = _item.endurance;
            sideBarInfo.recovery = _item.regeneration;

            sideBarInfo.id = equipedItem.id;
            sideBarInfo.is_equiped = true;

            sideSprite.atlas = bicycleAtlas;
            sideSprite.spriteName = sideBarInfo.imageId + "-1";
        }
        else {
            sideSprite.spriteName = "-1";
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
        
        List<int> idLists = new List<int>();
        
        foreach (Info info in sellList) {
            idLists.Add(info.id);
            gears += info.gear;
        }
        act.lists = idLists;
        gm.gameDispatcher.dispatch(act);

        isSingleSellOrLock = false;
        notifyModal.SetActive(true);
        notifyModal.transform.Find("Modal/Label").GetComponent<UILabel>().text = "총 " + gears + "개의 기어를 획득하였습니다.";
        notifyModal.GetComponent<UIPlaySound>().audioClip = audioClip[1];
        notifyModal.GetComponent<UIPlaySound>().Play();
        sellList.Clear();
    }

    public void singleSelling() {
        sellList.Clear();

        Info info = selectedItem.GetComponent<Info>();
        sellList.Add(info);
        selling();
    }

    public void offSellingModal() {
        sellingModal.SetActive(false);
        sellButton.GetComponent<boolIndex>().isOn = false;
        isSellMode = false;
        sellList.Clear();
        itemInitAct();
    }

    public void offLockingModal() {
        lockingModal.SetActive(false);
        lockButton.GetComponent<boolIndex>().isOn = false;
        isLockMode = false;
        lockIdList.Clear();
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

        public int speed;
        public int recovery;
        public int strength;
        public int endurance;
    }

    public void offPanel() {
        gameObject.SetActive(false);
    }
}
