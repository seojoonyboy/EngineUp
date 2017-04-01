using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BicycleViewController : MonoBehaviour {
    //판매 버튼 클릭시
    private bool 
        isSellMode = false,
        isLockMode = false;

    public GameObject 
        test,
        test2;
    public GameObject slotItem;

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

    void OnEnable() {
        makeList();
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
            Debug.Log("일반 모드");
            detailModal.SetActive(true);
        }
    }

    private void makeList() {
        removeList();
        for(int i=0; i<pageGrids.Length; i++) {
            for (int j = 0; j < pagePerSlotCount; j++) {
                GameObject item = Instantiate(slotItem);
                Transform parent = pageGrids[i].GetChild(j).transform;
                item.transform.SetParent(parent);
                item.transform.localScale = Vector3.one;
                item.transform.localPosition = Vector3.zero;
                item.name = "item";

                item.GetComponent<ButtonIndex>().index = j;
                item.AddComponent<UIDragScrollView>().scrollView = scrollview;
                //store에 접근하여 item에 반영한다.
                EventDelegate.Parameter param = new EventDelegate.Parameter();
                EventDelegate onClick = new EventDelegate(this, "selected");
                param.obj = item;
                onClick.parameters[0] = param;
                EventDelegate.Add(item.GetComponent<UIButton>().onClick, onClick);
            }
        }
        
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
                }
            }
        }
    }

    public void offSellingModal() {
        sellingModal.SetActive(false);
        sellButton.GetComponent<boolIndex>().isOn = false;
        isSellMode = false;
        test.SetActive(false);
        makeList();
    }

    public void offLockingModal() {
        lockingModal.SetActive(false);
        lockButton.GetComponent<boolIndex>().isOn = false;
        isLockMode = false;
        test2.SetActive(false);
        makeList();
    }

    public void offDetailModal() {
        detailModal.SetActive(false);
    }
}
