using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxViewController : MonoBehaviour {
    public UILabel numOfBoxLabel;

    private GameManager gm;
    private Box_Inventory boxStore;

    public GameObject 
        loadingModal,
        notifyModal;

    void Awake() {
        gm = GameManager.Instance;
        boxStore = gm.boxInvenStore;
    }

    void OnEnable() {
        garage_getBox_act act = ActionCreator.createAction(ActionTypes.BOX_INIT) as garage_getBox_act;
        gm.gameDispatcher.dispatch(act);
    }

    public void onStoreListener() {
        if(boxStore.eventType == ActionTypes.BOX_INIT) {
            if (boxStore.storeStatus == storeStatus.WAITING_REQ) {
                loadingModal.SetActive(true);
            }

            else if (boxStore.storeStatus == storeStatus.NORMAL) {
                loadingModal.SetActive(false);
            }
        }

        if(boxStore.eventType == ActionTypes.BOX_OPEN) {
            if (boxStore.storeStatus == storeStatus.WAITING_REQ) {
                loadingModal.SetActive(true);
            }

            else if (boxStore.storeStatus == storeStatus.NORMAL) {
                loadingModal.SetActive(false);
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
        //더 이상 열 수 있는 박스가 없는 경우 Modal 활성화
        garage_box_open act = ActionCreator.createAction(ActionTypes.BOX_OPEN) as garage_box_open;
        gm.gameDispatcher.dispatch(act);
    }

    //박스가 없는 경우
    public void onNotifyModal() {
        notifyModal.SetActive(true);
    }

    public void offNotifyModal() {
        notifyModal.SetActive(false);
    }
}
