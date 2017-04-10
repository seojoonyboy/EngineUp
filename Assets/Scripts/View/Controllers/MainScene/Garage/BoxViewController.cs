using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxViewController : MonoBehaviour {
    public UILabel numOfBoxLabel;

    private GameManager gm;
    private Box_Inventory boxStore;
    public User userStore;

    public GameObject 
        loadingModal,
        notifyModal,
        boxOpenModal;

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
                numOfBoxLabel.text = userStore.myData.boxes.ToString();
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
                UILabel name = boxOpenModal.transform.Find("Modal/Name").GetComponent<UILabel>();
                var openedItem = boxStore.openedItem;
                string type = openedItem[0].type;
                if (type == "item") {
                    name.text = openedItem[0].item.name;
                }
                else if (type == "character") {
                    name.text = openedItem[0].character.name;
                }
            }
        }
    }

    //공구함 열기 버튼 클릭시
    //박스 열기 Action 이전 Animation 발생시킴
    //Animation 마지막에 open 함수 실행
    public void startAnim() {
        open();
    }

    //박스 열기 Action 전송
    public void open() {
        Debug.Log("Open");
        int boxNum = userStore.myData.boxes;
        //더 이상 열 수 있는 박스가 없는 경우 Modal 활성화
        if(boxNum > 0) {
            garage_box_open act = ActionCreator.createAction(ActionTypes.BOX_OPEN) as garage_box_open;
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
        boxOpenModal.SetActive(false);
    }
}
