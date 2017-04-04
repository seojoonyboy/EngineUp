using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BicycleViewController : MonoBehaviour {
    private GameManager gm;
    //판매 버튼 클릭시
    private bool 
        isSellMode = false,
        isLockMode = false;

    public GameObject 
        test,
        test2;
    public GameObject 
        slotItem,
        sideBar,
        selectedItem;

    public GameObject
        sellButton,
        lockButton;

    public GameObject 
        sellingModal,
        lockingModal,
        detailModal;

    public UIGrid[] 
        framePageGrids,
        wheelPageGrids,
        enginePageGrids;

    public int pagePerSlotCount;

    public UILabel[] spects;
    public UIScrollView scrollview;

    public int[] equipedItemIndex;
    public UIAtlas atlas;

    List<int> lockIdList = new List<int>();
    List<int> sellList = new List<int>();

    void OnEnable() {
        gm = GameManager.Instance;
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
            selling();
        }
        else {
            sellButton.transform.Find("Label").GetComponent<UILabel>().text = "최종 판매";
        }
        isSellMode = !isOn;
        test.SetActive(isSellMode);
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
        test2.SetActive(isLockMode);
        lockButton.GetComponent<boolIndex>().isOn = isLockMode;
    }

    public void selected(GameObject obj) {
        Info info = obj.GetComponent<Info>();
        //판매모드인 경우
        if (isSellMode) {
            if(obj.tag == "locked") {
                return;
            }
            
            GameObject tmp = obj.transform.Find("Selected").gameObject;
            tmp.SetActive(!tmp.activeSelf);

            if(tmp.activeSelf) {
                obj.tag = "selected";
                int id = info.id;
                sellList.Add(id);
            }
            else {
                obj.tag = "unselected";
                if (sellList.Contains(info.id)) {
                    sellList.Remove(info.id);
                }
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
                int id = info.id;
                lockIdList.Add(id);
            }
            else {
                obj.tag = "unselected";
                //리스트에 담겨 있으면 제외시킨다.
                if(lockIdList.Contains(info.id)) {
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
        GameObject modal = detailModal.transform.Find("Modal").gameObject;
        Info info = selectedItem.GetComponent<Info>();

        modal.transform.Find("Name").GetComponent<UILabel>().text = info.name;
        modal.transform.Find("Desc").GetComponent<UILabel>().text = info.desc;
        UISprite img = modal.transform.Find("Image").GetComponent<UISprite>();
        img.atlas = atlas;
        img.spriteName = info.id.ToString();
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
        
        equip_act act = ActionCreator.createAction(ActionTypes.GARAGE_ITEM_EQUIP) as equip_act;
        Info info = selectedItem.GetComponent<Info>();
        int index = info.id;
        act.id = index;
        gm.gameDispatcher.dispatch(act);
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
                info.id = items[cnt - 1].id;
                info.parts = items[cnt - 1].item.parts;
                info.name = items[cnt - 1].item.name;
                info.desc = items[cnt - 1].item.desc;

                if (items[cnt - 1].is_equiped == "true") {
                    info.is_equiped = true;
                    item.transform.Find("Equiped").gameObject.SetActive(true);
                    equipedItemIndex[1] = items[cnt - 1].id;
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
                sprite.atlas = atlas;
                Debug.Log(items[cnt - 1].id);
                sprite.spriteName = items[cnt - 1].id.ToString();
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
                info.id = items[cnt - 1].id;
                info.parts = items[cnt - 1].item.parts;
                info.name = items[cnt - 1].item.name;
                info.desc = items[cnt - 1].item.desc;

                if (items[cnt - 1].is_equiped == "true") {
                    info.is_equiped = true;
                    item.transform.Find("Equiped").gameObject.SetActive(true);
                    equipedItemIndex[0] = items[cnt - 1].id;
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
                sprite.atlas = atlas;
                sprite.spriteName = items[cnt - 1].id.ToString();
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
                info.id = items[cnt - 1].id;
                info.parts = items[cnt - 1].item.parts;
                info.name = items[cnt - 1].item.name;
                info.desc = items[cnt - 1].item.desc;

                if (items[cnt - 1].is_equiped == "true") {
                    info.is_equiped = true;
                    item.transform.Find("Equiped").gameObject.SetActive(true);
                    equipedItemIndex[2] = items[cnt - 1].id;
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
                sprite.atlas = atlas;
                sprite.spriteName = items[cnt - 1].id.ToString();
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
            sprite.atlas = atlas;
            if (equipedItemIndex[i] == -1) {
                sprite.spriteName = "default";
            }
            else {
                int index = equipedItemIndex[i];
                sprite.spriteName = index.ToString();
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
        //id가 담겨있는 int 리스트를 이용하여 Action 작성
        foreach (int id in lockIdList) {
            act.id = id;
            gm.gameDispatcher.dispatch(act);
        }
    }

    private void selling() {
        foreach(int id in sellList) {
            Debug.Log(id);
        }
    }

    public void offSellingModal() {
        sellingModal.SetActive(false);
        sellButton.GetComponent<boolIndex>().isOn = false;
        isSellMode = false;
        test.SetActive(false);
        itemInitAct();
    }

    public void offLockingModal() {
        lockingModal.SetActive(false);
        lockButton.GetComponent<boolIndex>().isOn = false;
        isLockMode = false;
        test2.SetActive(false);
        itemInitAct();
    }

    private void clearList() {
        sellList.Clear();
        lockIdList.Clear();
    }

    public void offDetailModal() {
        detailModal.SetActive(false);
    }

    private void itemInitAct() {
        Debug.Log("Init");
        getItems_act act = ActionCreator.createAction(ActionTypes.GARAGE_ITEM_INIT) as getItems_act;
        gm.gameDispatcher.dispatch(act);
    }

    private class Info : MonoBehaviour {
        public int id;
        public bool is_equiped;
        public bool is_locked;

        public string name;
        public string desc;
        public int grade;
        public int gear;
        public string parts;
        public int limit_rank;
    }
}
