using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxViewController : MonoBehaviour {
    public UILabel 
        numOfBoxLabel,
        singleModalBoxNum,
        multiModalBoxNum;

    private GameManager gm;
    private Box_Inventory boxStore;
    public User userStore;

    public GameObject 
        loadingModal,
        notifyModal,
        boxOpenModal;

    public GameObject _openEffect;

    void Awake() {
        gm = GameManager.Instance;
        boxStore = gm.boxInvenStore;
    }

    void OnEnable() {
        MyInfo act = ActionCreator.createAction(ActionTypes.MYINFO) as MyInfo;
        gm.gameDispatcher.dispatch(act);
    }

    public void onUserStoreListener() {
        if(userStore.eventType == ActionTypes.MYINFO) {
            if(userStore.storeStatus == storeStatus.NORMAL) {
                string boxNum = userStore.myData.boxes.ToString();
                numOfBoxLabel.text = boxNum;
                singleModalBoxNum.text = "x " + boxNum;
                multiModalBoxNum.text = "x " + boxNum;
                Debug.Log("박스 갯수 갱신");
            }
        }
    }

    public void onBoxStoreListener() {
        if (boxStore.eventType == ActionTypes.BOX_OPEN) {
            if (boxStore.storeStatus == storeStatus.WAITING_REQ) {
                loadingModal.SetActive(true);
            }

            else if (boxStore.storeStatus == storeStatus.NORMAL) {
                //박스 열기 정상 동작시
                loadingModal.SetActive(false);
                boxOpenModal.SetActive(true);

                var items = boxStore.openedItem;
                int itemCount = items.Length;

                if(itemCount == 1) {
                    GameObject modal = boxOpenModal.transform.Find("SingleModal").gameObject;
                    modal.SetActive(true);
                    UILabel name = modal.transform.Find("Name").GetComponent<UILabel>();
                    var openedItem = boxStore.openedItem;
                    string type = openedItem[0].type;
                    if (type == "item") {
                        name.text = openedItem[0].item.name;
                    }
                    else if (type == "character") {
                        name.text = openedItem[0].character.name;
                    }
                }
                
                else {
                    GameObject modal = boxOpenModal.transform.Find("MultipleModal").gameObject;
                    modal.SetActive(true);
                    UITable table = modal.transform.Find("Table").GetComponent<UITable>();
                    List<Transform> list = table.GetChildList();

                    foreach(Transform item in list) {
                        item.Find("Name").GetComponent<UILabel>().text = "";
                    }

                    StartCoroutine(openEffect(list, itemCount, items, modal));
                }
            }
        }
    }

    //공구함 열기 버튼 클릭시
    //박스 열기 Action 이전 Animation 발생시킴
    //Animation 마지막에 open 함수 실행
    public void startAnim(GameObject obj) {
        offBoxOpenModal();
        int index = obj.GetComponent<ButtonIndex>().index;
        open(index);
    }

    //박스 열기 Action 전송
    public void open(int index) {
        //button index 판별
        int boxNum = userStore.myData.boxes;
        int openNum = 0;
        garage_box_open act = ActionCreator.createAction(ActionTypes.BOX_OPEN) as garage_box_open;
        switch (index) {
            case 0:
                openNum = 1;
                break;
            case 1:
                openNum = 10;
                break;
        }
        //더 이상 열 수 있는 박스가 없는 경우 Modal 활성화
        if(boxNum >= openNum) {
            act.num = openNum;
            gm.gameDispatcher.dispatch(act);
        }
        else {
            onNotifyModal();
        }
    }

    //박스가 없는 경우
    public void onNotifyModal() {
        notifyModal.SetActive(true);
    }

    public void offNotifyModal() {
        notifyModal.SetActive(false);
    }

    public void offBoxOpenModal() {
        boxOpenModal.transform.Find("MultipleModal").gameObject.SetActive(false);
        boxOpenModal.transform.Find("SingleModal").gameObject.SetActive(false);

        boxOpenModal.SetActive(false);
    }

    IEnumerator openEffect(List<Transform> list, int itemCount, Box_Inventory.boxOpenCallback[] items, GameObject modal) {
        int cnt = 0;
        modal.transform.Find("ConfirmButton").gameObject.GetComponent<UIButton>().enabled = false;
        modal.transform.Find("CancelButton").gameObject.GetComponent<UIButton>().enabled = false;
        foreach (Transform item in list) {
            if (cnt < itemCount) {
                //effect
                GameObject effect = Instantiate(_openEffect);
                effect.transform.SetParent(item.transform);
                effect.transform.localPosition = new Vector3(0, 48, 0);
                effect.transform.localScale = Vector3.one;

                item.gameObject.SetActive(true);
                //setUI
                string type = items[cnt].type;
                UILabel label = item.Find("Name").GetComponent<UILabel>();
                if (type == "item") {
                    label.text = items[cnt].item.name;
                }
                else if (type == "character") {
                    label.text = items[cnt].character.name;
                }
                cnt++;
            }
            yield return new WaitForSeconds(1.0f);
        }
        modal.transform.Find("ConfirmButton").gameObject.GetComponent<UIButton>().enabled = true;
        modal.transform.Find("CancelButton").gameObject.GetComponent<UIButton>().enabled = true;
    }

    public void offPanel() {
        gameObject.SetActive(false);
        UIToggle toggle = UIToggle.GetActiveToggle(2);
        toggle.value = false;
    }
}
