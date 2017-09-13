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
    public Char_Inventory charItemStore;
    public User userStore;
    public ScrollSnapRect[] sR;
    public MainViewController mV;
    public SpritesManager spriteManager;
    public BicycleListViewController childPanel;

    public GameObject changeSpecViewButton;
    public Text specHeader;
    public int 
        per_str,
        per_speed,
        per_endurance,
        per_recovery;

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
                if (bicycleItemStore.storeStatus == storeStatus.NORMAL) {
                    //Bicycle Item Store에서 Item_init 처리시, User Store에 장착 Spec 전달함
                    setStat();
                    setMainStageImage();
                    setSideBar();
                }
            }
            if(bicycleItemStore.eventType == ActionTypes.GARAGE_ITEM_EQUIP) {
                if(bicycleItemStore.storeStatus == storeStatus.WAITING_REQ) {
                    //mV.loadingModal.SetActive(true);
                }
            }
        }

        if(bicycleItemStore.eventType == ActionTypes.GARAGE_ITEM_SORT) {
            if(bicycleItemStore.storeStatus == storeStatus.NORMAL) {
                childPanel.makeList();
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
        setStat();
        setMainStageImage();
        setSideBar();
    }

    void OnDisable() {
        //init();
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
        per_endurance = char_end + item_end;
        per_speed = char_speed + item_speed;
        per_str = char_str + item_str;
        per_recovery = char_reg + item_reg;

        spects[0].text = (per_str).ToString();
        spects[1].text = (per_endurance).ToString();
        spects[2].text = (per_speed).ToString();
        spects[3].text = (per_recovery).ToString();

        specHeader.text = "자전거 능력치 %";

        changeSpecViewButton.GetComponent<boolIndex>().isOn = false;
    }

    public void changeSpecViewType() {
        bool isOn = changeSpecViewButton.GetComponent<boolIndex>().isOn;
        var mySpec = userStore.myData.status;

        if (isOn) {
            setStat();
            specHeader.text = "자전거 능력치 %";
        }
        else {
            spects[0].text = ((int)(per_str * mySpec.strength / 100)).ToString();
            spects[1].text = ((int)(per_endurance * mySpec.endurance / 100)).ToString();
            spects[2].text = ((int)(per_speed * mySpec.speed / 100)).ToString();
            spects[3].text = ((int)(per_recovery * mySpec.regeneration / 100)).ToString();

            specHeader.text = "자전거 능력치";
        }

        changeSpecViewButton.GetComponent<boolIndex>().isOn = !isOn;
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
            sideBar.transform.Find("WheelSlot/Name").GetComponent<Text>().text = _item.name;
        }
        else {
            sprite.sprite = spriteManager.stage_items[53];
            sideBar.transform.Find("WheelSlot/Name").GetComponent<Text>().text = "바퀴";
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
            sideBar.transform.Find("FrameSlot/Name").GetComponent<Text>().text = _item.name;
        }
        else {
            sprite.sprite = spriteManager.stage_items[0];
            sideBar.transform.Find("FrameSlot/Name").GetComponent<Text>().text = "프레임";
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
            sideBar.transform.Find("EngineSlot/Name").GetComponent<Text>().text = _item.name;
        }
        else {
            sprite.sprite = spriteManager.stage_items[85];
            sideBar.transform.Find("EngineSlot/Name").GetComponent<Text>().text = "구동계";
        }
        //mV.loadingModal.SetActive(false);
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
            sideSlot.transform.Find("Grade").GetComponent<Image>().sprite = spriteManager.grade_items[sideBarInfo.grade - 1];
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
            sideSlot.transform.Find("Grade").GetComponent<Image>().sprite = spriteManager.grade_items[sideBarInfo.grade - 1];
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
            sideSlot.transform.Find("Grade").GetComponent<Image>().sprite = spriteManager.grade_items[sideBarInfo.grade - 1];
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

        setStat();

        specHeader.text = "자전거 능력치 %";

        changeSpecViewButton.GetComponent<boolIndex>().isOn = false;
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

    public void onClickSlot() {
        GameObject effect = Instantiate(mV.touchEffectPref);
        effect.transform.SetParent(transform, false);
        Vector3 screenPoint = Input.mousePosition;
        Vector3 resultPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10.0f);
        effect.transform.position = Camera.main.ScreenToWorldPoint(resultPos);
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
