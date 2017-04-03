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

    public UIGrid[] pageGrids;
    public int pagePerSlotCount;

    public UILabel[] spects;
    public UIScrollView scrollview;

    public int[] equiptedItemIndex;
    public UIAtlas atlas;
    void OnEnable() {
        gm = GameManager.Instance;
        itemInitAct();
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
        //판매모드인 경우
        if(isSellMode) {
            if(obj.tag == "locked") {
                return;
            }
            int index = obj.GetComponent<ButtonIndex>().index;
            GameObject tmp = obj.transform.Find("Selected").gameObject;
            tmp.SetActive(!tmp.activeSelf);

            if(tmp.activeSelf) {
                obj.tag = "selected";
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
            }
            else {
                obj.tag = "unselected";
            }
        }
        else if(isLockMode == false && isSellMode == false) {
            //Debug.Log(obj.GetComponent<ButtonIndex>().index);
            selectedItem = obj;
            onDetailModal();
        }
    }

    public void onDetailModal() {
        detailModal.SetActive(true);
        GameObject modal = detailModal.transform.Find("Modal").gameObject;
        int index = selectedItem.GetComponent<ButtonIndex>().index;
        BicycleItem item = gm.bicycleInventStore.items[index];
        Item innerItem = item.item;

        modal.transform.Find("Name").GetComponent<UILabel>().text = innerItem.name;
        modal.transform.Find("Desc").GetComponent<UILabel>().text = innerItem.desc;

        //현재 장착중인 아이템인 경우
        //모달 내 해제하기 버튼 활성화
        if (item.is_equiped == "true") {
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
        int tmp = selectedItem.GetComponent<ButtonIndex>().index;
        int index = gm.bicycleInventStore.items[tmp].id;
        act.id = index;
        gm.gameDispatcher.dispatch(act);

        GameObject target = null;
        switch(selectedItem.transform.Find("TypeTag").tag) {
            case "FM":
                target = sideBar.transform.Find("FrameSlot").gameObject;
                break;
            case "WH":
                target = sideBar.transform.Find("WheelSlot").gameObject;
                break;
            case "DS":
                target = sideBar.transform.Find("EngineSlot").gameObject;
                break;
        }
        UISprite targetSprite = target.GetComponent<UISprite>();
        //targetSprite.atlas = "UI";
    }

    //아이템 해제
    public void unequip() {
        if(selectedItem == null) {
            return;
        }

        unequip_act act = ActionCreator.createAction(ActionTypes.GARAGE_ITEM_EQUIP) as unequip_act;
        int tmp = selectedItem.GetComponent<ButtonIndex>().index;
        int index = gm.bicycleInventStore.items[tmp].id;
        act.id = index;
        gm.gameDispatcher.dispatch(act);
    }

    public void makeList() {
        removeList();
        //slot 생성
        BicycleItem[] items = gm.bicycleInventStore.items;
        //Debug.Log(items.Length);

        //프레임 리스트 생성
        int cnt = 0;
        for (int i = 0; i<pageGrids.Length; i++) {
            for (int j = 0; j<pagePerSlotCount; j++) {
                cnt++;
                if (cnt > items.Length) {
                    break;
                }
                GameObject item = Instantiate(slotItem);
                Transform parent = pageGrids[i].GetChild(j).transform;
                item.transform.SetParent(parent);
                item.transform.localScale = Vector3.one;
                item.transform.localPosition = Vector3.zero;
                item.name = "item";
                item.GetComponent<ButtonIndex>().index = cnt - 1;
                GameObject tag = item.transform.Find("TypeTag").gameObject;
                Item bItem = items[cnt - 1].item;
                if (bItem.parts == "FR") {
                    tag.tag = "FR";
                }
                else if(bItem.parts == "WH") {
                    tag.tag = "WH";
                }
                else if(bItem.parts == "DS") {
                    tag.tag = "DS";
                }

                if(items[cnt - 1].is_equiped == "true") {
                    item.transform.Find("Equiped").gameObject.SetActive(true);
                    //장착중인 아이템
                    //따로 배열에 담는다.
                    if(tag.tag == "FR") {
                        equiptedItemIndex[0] = cnt - 1;
                    }
                    else if(tag.tag == "WH") {
                        equiptedItemIndex[1] = cnt - 1;
                    }
                    else if(tag.tag == "DS") {
                        equiptedItemIndex[2] = cnt - 1;
                    }
                }

                EventDelegate.Parameter param = new EventDelegate.Parameter();
                EventDelegate onClick = new EventDelegate(this, "selected");
                param.obj = item;
                onClick.parameters[0] = param;
                EventDelegate.Add(item.GetComponent<UIButton>().onClick, onClick);
            }
        }
        //SideBar 갱신
        BicycleItem[] tmp = gm.bicycleInventStore.items;
        UISprite sprite = null;
        for (int i=0; i<equiptedItemIndex.Length; i++) {
            int index = tmp[equiptedItemIndex[i]].id;
            switch (i) {
                case 1:
                    sprite = sideBar.transform.Find("FrameSlot").GetComponent<UISprite>();
                    break;
                case 2:
                    sprite = sideBar.transform.Find("WheelSlot").GetComponent<UISprite>();
                    break;
                case 3:
                    sprite = sideBar.transform.Find("EngineSlot").GetComponent<UISprite>();
                    break;
            }
            //sprite.atlas = atlas;
            //sprite.name = index.ToString();
        }
        //바퀴 리스트 생성

        //구동계 리스트 생성

    }

    private void removeList() {
        for(int i=0; i<pageGrids.Length; i++) {
            foreach(Transform slot in pageGrids[i].transform) {
                slot.DestroyChildren();
            }
        }
    }

    private void initGrid() {
        
    }

    private void selling() {
        for(int i=0; i<pageGrids.Length; i++) {
            for(int j=0; j<pagePerSlotCount; j++) {
                GameObject item = pageGrids[i].GetChild(j).FindChild("item").gameObject;
                if (item.tag == "selected") {
                    //Destroy(item);
                    //삭제 Action 실행
                    //삭제 성공시 makeList함수 호출
                    itemInitAct();
                }
            }
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

    public void offDetailModal() {
        detailModal.SetActive(false);
    }

    private void itemInitAct() {
        Debug.Log("Init");
        getItems_act act = ActionCreator.createAction(ActionTypes.GARAGE_ITEM_INIT) as getItems_act;
        gm.gameDispatcher.dispatch(act);
    }
}
