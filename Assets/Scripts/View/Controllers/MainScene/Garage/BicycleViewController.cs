using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using UnityEngine.UI;

public enum SelectedType { FR, EG, WH }

public class BicycleViewController : MonoBehaviour {
    private GameManager gm;
    private SoundManager sm;

    public BicycleItem_Inventory bicycleItemStore;
    public TweenManager tM;
    public Char_Inventory charItemStore;
    public User userStore;
    public ScrollSnapRect[] sR;
    public MainViewController mV;
    public SpritesManager spriteManager;
    public BicycleListViewController childPanel;
    //판매 버튼 클릭시
    private bool 
        isSellMode = false,
        isLockMode = false,
        isSingleSellOrLock = false;

    public GameObject 
        slotItem,
        sideBar,
        selectedItem;

    public GameObject[] toggleBeetweenLines;

    public GameObject
        sellButton,
        lockButton,
        bicycle,
        itemGrid;

    public GameObject 
        sellingModal,
        lockingModal,
        detailModal,
        notifyModal,
        pagination_icon_pref;

    //차고지 리스트를 보여주는 패널
    public GameObject listPanel;

    public int pagePerSlotCount;

    public Text[] spects;

    public int[] equipedItemIndex;
    public AudioClip[] audioClip;

    List<int> lockIdList = new List<int>();
    List<int> unlockList = new List<int>();
    List<Info> sellList = new List<Info>();
    private Animator animator;

    public SelectedType selectedType;

    void Awake() {
        gm = GameManager.Instance;
        sm = SoundManager.Instance;
        animator = GetComponent<Animator>();
    }

    public void onBicycleItemStoreListener() {
        ActionTypes bicycleItemStoreEventType = bicycleItemStore.eventType;

        if(gameObject.activeSelf) {
            if(bicycleItemStore.eventType == ActionTypes.ITEM_INIT) {
                if(bicycleItemStore.storeStatus == storeStatus.NORMAL) {
                    //Bicycle Item Store에서 Item_init 처리시, User Store에 장착 Spec 전달함
                    setStat();
                    setMainStageImage();
                    setSideBar();
                }
            }
        }
    }

    public void onCharItemStoreListener() {
        ActionTypes bicycleItemStoreEventType = charItemStore.eventType;

        if (gameObject.activeSelf) {
            if (charItemStore.eventType == ActionTypes.ITEM_INIT) {
                if (charItemStore.storeStatus == storeStatus.NORMAL) {
                    //Character Item Store에서 Item_init 처리시, User Store에 장착(파트너) Spec 전달함
                    setStat();
                }
            }
        }
    }

    public void onUserStoreListener() {
        ActionTypes userStoreEventType = userStore.eventType;
        ////lvLavel.text = "Lv. " + userStore.myData.status.rank.ToString();
    }

    void OnEnable() {
        Invoke("playSlideIn", 0.2f);
    }

    void playSlideIn() {
        animator.Play("SlideIn");
    }

    public void slideFinished(AnimationEvent animationEvent) {
        int boolParm = animationEvent.intParameter;

        //slider in
        if (boolParm == 1) {
            setStat();
            setMainStageImage();
            setSideBar();
        }

        //slider out
        else if (boolParm == 0) {
            gameObject.SetActive(false);
        }
    }

    public void onBackButton() {
        animator.Play("SlideOut");
    }

    //void offPanel() {
    //    gameObject.SetActive(false);
    //    selectedItem = null;
    //    detailModal.SetActive(false);

    //    //tP.ResetToBeginning();
    //}

    public void tweenPos() {
        //bool isTweening = tM.isTweening;
        //if (isTweening) {
        //    return;
        //}
        //tM.isTweening = true;
        //if (!isReverse_tp) {
        //    tP.PlayForward();
        //}
        //else {
        //    sm.playEffectSound(0);
        //    //swap
        //    Vector3 tmp;
        //    tmp = tP.to;
        //    tP.to = tP.from;
        //    tP.from = tmp;

        //    tP.ResetToBeginning();
        //    tP.PlayForward();
        //}
    }

    public void tpFinished() {
        //tM.isTweening = false;

        //if (isReverse_tp) {
        //    offPanel();
        //    gameObject.transform.Find("TopPanel").gameObject.SetActive(false);
        //}

        //else {
        //    //itemInitAct();
        //    makeList();
        //    setStat();
        //    gameObject.transform.Find("TopPanel").gameObject.SetActive(true);
        //}

        //isReverse_tp = true;
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
            sellButton.transform.Find("Text").GetComponent<Text>().text = "판매";
            if(sellList.Count != 0) {
                sellingModal.SetActive(true);
                playEffectSound(1);
            }
        }
        else {
            sellButton.transform.Find("Text").GetComponent<Text>().text = "최종 판매";
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
            lockButton.transform.Find("Text").GetComponent<Text>().text = "잠금";
            if (lockIdList.Count != 0 || unlockList.Count != 0) {
                lockingModal.SetActive(true);
                playEffectSound(1);
            }
        }
        else {
            lockButton.transform.Find("Text").GetComponent<Text>().text = "최종 잠금";
        }
        isLockMode = !isOn;
        lockButton.GetComponent<boolIndex>().isOn = isLockMode;
    }

    public void selected(GameObject obj) {
        int index = obj.GetComponent<ButtonIndex>().index;
        switch(index) {
            case 0:
                selectedType = SelectedType.WH;
                break;
            case 1:
                selectedType = SelectedType.FR;
                break;
            case 2:
                selectedType = SelectedType.EG;
                break;
        }
        childPanel.gameObject.SetActive(true);
        //Info info = obj.GetComponent<Info>();
        //if(info == null) {
        //    return;
        //}
        //if(info.id == 0) {
        //    return;
        //}


        ////판매모드인 경우
        //if (isSellMode) {
        //    if (info.is_locked) { return; }

        //    GameObject tmp = obj.transform.Find("Selected").gameObject;
        //    tmp.SetActive(!tmp.activeSelf);

        //    if(tmp.activeSelf) {
        //        obj.tag = "selected";
        //        sellList.Add(info);
        //    }
        //    else {
        //        obj.tag = "unselected";
        //    }
        //}
        ////잠금모드인 경우
        //else if (isLockMode) {
        //    GameObject tmp = obj.transform.Find("LockIcon").gameObject;
        //    tmp.SetActive(!tmp.activeSelf);
        //    if(tmp.activeSelf) {
        //        info.is_locked = true;
        //        //int 리스트
        //        //리스트에 id값(int)을 담는다. (button index 말고 실제 아이템 id)
        //        lockIdList.Add(info.id);
        //        if (unlockList.Contains(info.id)) {
        //            unlockList.Remove(info.id);
        //        }
        //    }
        //    else {
        //        info.is_locked = false;
        //        //리스트에 담겨 있으면 제외시킨다.
        //        unlockList.Add(info.id);
        //        if (lockIdList.Contains(info.id)) {
        //            lockIdList.Remove(info.id);
        //        }
        //    }
        //}
        //else if(isLockMode == false && isSellMode == false) {
        //    selectedItem = obj;
        //    onDetailModal();
        //    playEffectSound(1);
        //}
    }

    //아이템 상세보기 Modal
    public void onDetailModal() {
        detailModal.SetActive(true);

        sellList.Clear();
        sellList.Add(selectedItem.GetComponent<Info>());

        isSingleSellOrLock = true;

        GameObject modal = detailModal.transform.Find("InnerModal").gameObject;
        modal.transform.Find("SellButton").gameObject.SetActive(true);

        Info info = selectedItem.GetComponent<Info>();

        List<string> valList = new List<string>();
        int val = info.strength;
        if(val != 0) {
            string str = "근력 + " + val;
            valList.Add(str);
        }
        val = info.endurance;
        if(val != 0) {
            string str = "지구력 + " + val;
            valList.Add(str);
        }
        val = info.speed;
        if (val != 0) {
            string str = "스피드 + " + val;
            valList.Add(str);
        }
        val = info.recovery;
        if (val != 0) {
            string str = "회복력 + " + val;
            valList.Add(str);
        }

        int count = 0;
        StringBuilder sb = new StringBuilder();
        foreach(String str in valList) {
            if(count == 1) {
                sb.Append("    ");
                sb.Append(str);
            }
            else if(count == 2) {
                sb.Append("\n");
                sb.Append(str);
                sb.Append("    ");
            }
            else if(count == 0 || count == 3) {
                sb.Append(str);
            }
            count++;
        }

        modal.transform.Find("Spec").GetComponent<Text>().text = sb.ToString();
        modal.transform.Find("Name").GetComponent<Text>().text = info.name;
        modal.transform.Find("Desc").GetComponent<Text>().text = info.desc;
        modal.transform.Find("LimitLv").GetComponent<Text>().text = "제한 레벨 : " + info.limit_rank;

        Image img = modal.transform.Find("Image").GetComponent<Image>();

        var tmp = spriteManager.stage_items[info.imageId - 1];
        if (tmp != null) {
            img.sprite = spriteManager.stage_items[info.imageId - 1];
        }
        else {
            img.sprite = spriteManager.default_slots[info.grade - 1];
        }

        //현재 장착중인 아이템인 경우
        //모달 내 해제하기 버튼 활성화
        if (info.is_equiped) {
            modal.transform.Find("UnequipButton").gameObject.SetActive(true);
            modal.transform.Find("EquipButton").gameObject.SetActive(false);
        }
        //장착중인 아이템이 아닌 경우
        //모달 내 장착하기 버튼 활성화
        else {
            modal.transform.Find("UnequipButton").gameObject.SetActive(false);
            modal.transform.Find("EquipButton").gameObject.SetActive(true);
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
            playEffectSound(1);
            notifyModal.transform.Find("InnerModal/Text").GetComponent<Text>().text = "등급이 낮아 아이템을 장착할 수 없습니다.";
            sm.playEffectSound(0);
            //notifyModal.GetComponent<UIPlaySound>().audioClip = audioClip[0];
            //notifyModal.GetComponent<UIPlaySound>().Play();
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
        foreach (ScrollSnapRect sR in sR) {
            sR.enabled = false;
        }
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

        if(GN == 0) {
            GameObject grid = Instantiate(itemGrid);
            GameObject _content = sR[0].transform.Find("Content").gameObject;

            grid.transform.SetParent(_content.transform, false);
            GameObject pageIcon = Instantiate(pagination_icon_pref);
            pageIcon.name = "Icon";

            pageIcon.transform.SetParent(sR[0].transform.parent.Find("PaginationIcons").transform, false);
        }
        for(int i = 0; i < GN; i++) {
            GameObject grid = Instantiate(itemGrid);
            GameObject _content = sR[0].transform.Find("Content").gameObject;
            grid.transform.SetParent(_content.transform, false);
            GameObject pageIcon = Instantiate(pagination_icon_pref);
            pageIcon.name = "Icon";
            pageIcon.transform.SetParent(sR[0].transform.parent.Find("PaginationIcons").transform, false);
            for(int j = 0; j < pagePerSlotCount; j++) {
                if(cnt < frameItemCnt) {
                    Transform slot = grid.transform.GetChild(j).transform;
                    GameObject item = Instantiate(slotItem);

                    item.transform.SetParent(slot, false);
                    item.transform.localPosition = Vector3.zero;

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
                        item.transform.Find("Equip_icon").gameObject.SetActive(true);
                    }

                    if (data.is_locked == "true") {
                        info.is_locked = true;
                        item.transform.Find("LockIcon").gameObject.SetActive(true);
                    }

                    Image sprite = item.GetComponent<Image>();
                    var tmp = spriteManager.stage_items[info.imageId - 1];
                    if(tmp != null) {
                        sprite.sprite = spriteManager.stage_items[info.imageId - 1];
                    }
                    else {
                        sprite.sprite = spriteManager.default_slots[info.grade - 1];
                    }
                    
                    item.GetComponent<Button>().onClick.AddListener(() => selected(item));
                    cnt++;
                }
            }
        }

        cnt = 0;
        num = (float)wheelItemCnt / pagePerSlotCount;
        GN = (int)Mathf.Ceil(num);
        for (int i = 0; i < GN; i++) {
            GameObject grid = Instantiate(itemGrid);
            GameObject _content = sR[1].transform.Find("Content").gameObject;
            grid.transform.SetParent(_content.transform, false);
            GameObject pageIcon = Instantiate(pagination_icon_pref);
            pageIcon.name = "Icon";
            pageIcon.transform.SetParent(sR[1].transform.parent.Find("PaginationIcons").transform, false);

            for (int j = 0; j < pagePerSlotCount; j++) {
                if(cnt < wheelItemCnt) {
                    Transform slot = grid.transform.GetChild(j).transform;
                    GameObject item = Instantiate(slotItem);

                    item.transform.SetParent(slot, false);
                    item.transform.localPosition = Vector3.zero;

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
                        item.transform.Find("Equip_icon").gameObject.SetActive(true);
                    }

                    if (data.is_locked == "true") {
                        info.is_locked = true;
                        item.transform.Find("LockIcon").gameObject.SetActive(true);
                    }

                    Image sprite = item.GetComponent<Image>();
                    var tmp = spriteManager.stage_items[info.imageId - 1];
                    if (tmp != null) {
                        sprite.sprite = spriteManager.stage_items[info.imageId - 1];
                    }
                    else {
                        sprite.sprite = spriteManager.default_slots[info.grade - 1];
                    }
                    item.GetComponent<Button>().onClick.AddListener(() => selected(item));

                    cnt++;
                }
            }
        }

        cnt = 0;
        num = (float)engineItemCnt / pagePerSlotCount;
        GN = (int)Mathf.Ceil(num);
        for (int i = 0; i < GN; i++) {
            GameObject grid = Instantiate(itemGrid);
            GameObject _content = sR[2].transform.Find("Content").gameObject;
            grid.transform.SetParent(_content.transform, false);
            GameObject pageIcon = Instantiate(pagination_icon_pref);
            pageIcon.name = "Icon";
            pageIcon.transform.SetParent(sR[2].transform.parent.Find("PaginationIcons").transform, false);

            for (int j = 0; j < pagePerSlotCount; j++) {
                if(cnt < engineItemCnt) {
                    Transform slot = grid.transform.GetChild(j).transform;
                    GameObject item = Instantiate(slotItem);

                    item.transform.SetParent(slot, false);
                    item.transform.localPosition = Vector3.zero;

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
                        item.transform.Find("Equip_icon").gameObject.SetActive(true);
                    }

                    if (data.is_locked == "true") {
                        info.is_locked = true;
                        item.transform.Find("LockIcon").gameObject.SetActive(true);
                    }

                    Image sprite = item.GetComponent<Image>();
                    var tmp = spriteManager.stage_items[info.imageId - 1];
                    if (tmp != null) {
                        sprite.sprite = spriteManager.stage_items[info.imageId - 1];
                    }
                    else {
                        sprite.sprite = spriteManager.default_slots[info.grade - 1];
                    }
                    item.GetComponent<Button>().onClick.AddListener(() => selected(item));

                    cnt++;
                }
            }
        }

        foreach(ScrollSnapRect sR in sR) {
            sR.enabled = true;
        }

        setMainStageImage();
        setSideBar();
    }

    private void setStat() {
        //아이템 장착, 파트너 장착에 따른 Spec 변화
        var itemSpects = userStore.itemSpects;

        //파트너 장착 효과
        int char_str = itemSpects.Char_strength;
        int char_end = itemSpects.Char_endurance;
        int char_reg = itemSpects.Char_regeneration;
        int char_speed = itemSpects.Char_speed;

        //아이템 장착 효과
        int item_str = itemSpects.Item_strength;
        int item_end = itemSpects.Item_endurance;
        int item_speed = itemSpects.Item_speed;
        int item_reg = itemSpects.Item_regeneration;

        //파트너 + 아이템 장착효과
        spects[0].text = (char_str + item_str).ToString();
        spects[1].text = (char_end + item_end).ToString();
        spects[2].text = (char_speed + item_speed).ToString();
        spects[3].text = (char_reg + item_reg).ToString();
    }

    private void setMainStageImage() {
        Image sprite;

        RespGetItems equipedItem = bicycleItemStore.equipedItemIndex[0];
        sprite = bicycle.transform.Find("Wheel").GetComponent<Image>();
        if (equipedItem != null) {
            RespItem _item = equipedItem.item;
            var tmp = spriteManager.stage_items[_item.id - 1];
            if(tmp == null) {
                sprite.sprite = spriteManager.stage_items[53];
            }
            else {
                sprite.sprite = spriteManager.stage_items[_item.id - 1];
            }
        }
        else {
            sprite.sprite = spriteManager.stage_items[53];
        }

        equipedItem = bicycleItemStore.equipedItemIndex[1];
        sprite = bicycle.transform.Find("Frame").GetComponent<Image>();
        if (equipedItem != null) {
            RespItem _item = equipedItem.item;
            var tmp = spriteManager.stage_items[_item.id - 1];
            if (tmp == null) {
                sprite.sprite = spriteManager.stage_items[0];
            }
            else {
                sprite.sprite = spriteManager.stage_items[_item.id - 1];
            }
        }
        else {
            sprite.sprite = spriteManager.stage_items[0];
        }

        equipedItem = bicycleItemStore.equipedItemIndex[2];
        sprite = bicycle.transform.Find("Engine").GetComponent<Image>();
        if (equipedItem != null) {
            RespItem _item = equipedItem.item;
            var tmp = spriteManager.stage_items[_item.id - 1];
            if (tmp == null) {
                sprite.sprite = spriteManager.stage_items[85];
            }
            else {
                sprite.sprite = spriteManager.stage_items[_item.id - 1];
            }
        }
        else {
            sprite.sprite = spriteManager.stage_items[85];
        }
    }

    private void setSideBar() {
        GameObject sideSlot = sideBar.transform.Find("WheelSlot").gameObject;
        Info sideBarInfo = sideSlot.GetComponent<Info>();
        if(sideBarInfo == null) {
            sideBarInfo = sideSlot.AddComponent<Info>();
        }
        Image sideSprite;
        RespGetItems equipedItem = bicycleItemStore.equipedItemIndex[0];
        sideSprite = sideSlot.transform.Find("Mask/Image").GetComponent<Image>();
        if (equipedItem != null) {
            sideSprite.enabled = true;
            RespItem _item = equipedItem.item;
            sideBarInfo.imageId = _item.id;
            sideBarInfo.desc = _item.desc;
            sideBarInfo.name = _item.name;
            sideBarInfo.limit_rank = _item.limit_rank;
            sideBarInfo.gear = _item.gear;
            sideBarInfo.speed = _item.speed;
            sideBarInfo.strength = _item.strength;
            sideBarInfo.endurance = _item.endurance;
            sideBarInfo.grade = _item.grade;
            sideBarInfo.recovery = _item.regeneration;

            sideBarInfo.id = equipedItem.id;
            sideBarInfo.is_equiped = true;

            var tmp = spriteManager.stage_items[sideBarInfo.imageId - 1];
            if (tmp != null) {
                sideSprite.sprite = spriteManager.stage_items[sideBarInfo.imageId - 1];
            }
            else {
                sideSprite.sprite = spriteManager.default_slots[sideBarInfo.grade - 1];
            }

            sideSlot.transform.Find("Plus").gameObject.SetActive(false);
            sideSlot.transform.Find("Grade").GetComponent<Image>().sprite = spriteManager.grade_items[sideBarInfo.grade];
        }
        else {
            Info tmp = sideSlot.GetComponent<Info>();
            Destroy(tmp);
            sideSprite.enabled = false;

            sideSlot.transform.Find("Plus").gameObject.SetActive(true);
            sideSlot.transform.Find("Grade").GetComponent<Image>().sprite = null;
        }

        sideSlot = sideBar.transform.Find("FrameSlot").gameObject;
        sideBarInfo = sideSlot.GetComponent<Info>();
        if (sideBarInfo == null) {
            sideBarInfo = sideSlot.AddComponent<Info>();
        }

        equipedItem = bicycleItemStore.equipedItemIndex[1];

        sideSprite = sideSlot.transform.Find("Image").GetComponent<Image>();

        if (equipedItem != null) {
            sideSprite.enabled = true;
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
            sideBarInfo.grade = _item.grade;
            sideBarInfo.id = equipedItem.id;
            sideBarInfo.is_equiped = true;

            var tmp = spriteManager.stage_items[sideBarInfo.imageId - 1];
            if (tmp != null) {
                sideSprite.sprite = spriteManager.stage_items[sideBarInfo.imageId - 1];
            }
            else {
                sideSprite.sprite = spriteManager.default_slots[sideBarInfo.grade - 1];
            }

            sideSlot.transform.Find("Plus").gameObject.SetActive(false);
            sideSlot.transform.Find("Grade").GetComponent<Image>().sprite = spriteManager.grade_items[sideBarInfo.grade];
        }
        else {
            Info tmp = sideSlot.GetComponent<Info>();
            Destroy(tmp);
            sideSprite.enabled = false;

            sideSlot.transform.Find("Plus").gameObject.SetActive(true);
            sideSlot.transform.Find("Grade").GetComponent<Image>().sprite = null;
        }

        sideSlot = sideBar.transform.Find("EngineSlot").gameObject;
        sideBarInfo = sideSlot.GetComponent<Info>();
        if (sideBarInfo == null) {
            sideBarInfo = sideSlot.AddComponent<Info>();
        }

        equipedItem = bicycleItemStore.equipedItemIndex[2];

        sideSprite = sideSlot.transform.Find("Image").GetComponent<Image>();

        if (equipedItem != null) {
            sideSprite.enabled = true;
            RespItem _item = equipedItem.item;
            sideBarInfo.imageId = _item.id;
            sideBarInfo.desc = _item.desc;
            sideBarInfo.name = _item.name;
            sideBarInfo.limit_rank = _item.limit_rank;
            sideBarInfo.gear = _item.gear;
            sideBarInfo.speed = _item.speed;
            sideBarInfo.strength = _item.strength;
            sideBarInfo.endurance = _item.endurance;
            sideBarInfo.grade = _item.grade;
            sideBarInfo.recovery = _item.regeneration;

            sideBarInfo.id = equipedItem.id;
            sideBarInfo.is_equiped = true;

            var tmp = spriteManager.stage_items[sideBarInfo.imageId - 1];
            if (tmp != null) {
                sideSprite.sprite = spriteManager.stage_items[sideBarInfo.imageId - 1];
            }
            else {
                sideSprite.sprite = spriteManager.default_slots[sideBarInfo.grade - 1];
            }

            sideSlot.transform.Find("Plus").gameObject.SetActive(false);
            sideSlot.transform.Find("Grade").GetComponent<Image>().sprite = spriteManager.grade_items[sideBarInfo.grade];
        }
        else {
            Info tmp = sideSlot.GetComponent<Info>();
            Destroy(tmp);
            sideSprite.enabled = false;

            sideSlot.transform.Find("Plus").gameObject.SetActive(true);
            sideSlot.transform.Find("Grade").GetComponent<Image>().sprite = null;
        }
    }

    private void removeList() {
        for(int i=0; i<sR.Length; i++) {
            sR[i].transform.Find("Content").DestroyChildren();
            sR[i].transform.parent.Find("PaginationIcons").DestroyChildren();
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
        lockIdList.Clear();
        unlockList.Clear();
    }

    public void selling() {
        //garage_sell_act act = ActionCreator.createAction(ActionTypes.GARAGE_SELL) as garage_sell_act;
        //int gears = 0;
        
        //List<int> idLists = new List<int>();
        
        //foreach (Info info in sellList) {
        //    idLists.Add(info.id);
        //    gears += info.gear;
        //    Debug.Log(info.id);
        //}
        //act.lists = idLists;
        //gm.gameDispatcher.dispatch(act);

        //isSingleSellOrLock = false;
        //notifyModal.SetActive(true);
        //notifyModal.transform.Find("InnerModal/Text").GetComponent<Text>().text = "총 " + gears + "개의 기어를 획득하였습니다.";
        //sm.playEffectSound(1);
        //sellList.Clear();
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
        item_init bicycleInfo = ActionCreator.createAction(ActionTypes.ITEM_INIT) as item_init;
        bicycleInfo._type = equip_act.type.ITEM;
        gm.gameDispatcher.dispatch(bicycleInfo);

        //int index = PlayerPrefs.GetInt("Filter");
        //Debug.Log("Item Init");
        //filterSelected(index);
    }

    public void onFilterButton(GameObject obj) {
        obj.SetActive(!obj.activeSelf);
    }

    public void playClickSound() {
        sm.playEffectSound(0);
    }

    public void playEffectSound(int index) {
        sm.playEffectSound(index);
    }

    public void filterSelected(int index) {
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
}
