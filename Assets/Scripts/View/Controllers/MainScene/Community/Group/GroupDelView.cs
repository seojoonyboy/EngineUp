using UnityEngine;
using System.Collections;

public class GroupDelView : MonoBehaviour {
    public GroupViewController controller;
    GameManager gm;

    public GameObject modal;
    public bool isDestroyReq = false;

    void Awake() {
        gm = GameManager.Instance;
    }

    public void onDestroyButton() {
        getMembers();
    }

    public void offPanel() {
        gameObject.SetActive(false);
    }

    public void offModal() {
        gameObject.SetActive(false);
        modal.SetActive(false);
        modal.transform.Find("SuccessModal").gameObject.SetActive(false);
        modal.transform.Find("FailModal").gameObject.SetActive(false);
    }

    private void getMembers() {
        //그룹장을 제외한 그룹원이 존재하는지 확인
        Group_getMemberAction act = ActionCreator.createAction(ActionTypes.GROUP_GET_MEMBERS) as Group_getMemberAction;
        act.id = controller.detailView.id;
        gm.gameDispatcher.dispatch(act);

        isDestroyReq = true;
    }

    private bool checkMemberExist() {
        Member[] members = controller.groupStore.groupMembers;
        for (int i = 0; i < members.Length; i++) {
            if (members[i].memberState != "MB") {
                modal.SetActive(true);
                modal.transform.Find("Modal/Label").GetComponent<UILabel>().text = "가입된 그룹원이 있어 해체할 수 없습니다.";
                return false;
            }
        }
        return true;
    }

    public void destoyReq() {
        //Debug.Log("Destroy req");
        Group_del delAct = ActionCreator.createAction(ActionTypes.GROUP_DESTROY) as Group_del;
        delAct.id = controller.detailView.id;
        gm.gameDispatcher.dispatch(delAct);
    }

    public void onGroupStoreListener() {
        Groups groupStore = controller.groupStore;
        ActionTypes groupStoreEventType = groupStore.eventType;
        if (groupStoreEventType == ActionTypes.GROUP_DESTROY) {
            if(groupStore.storeStatus == storeStatus.NORMAL) {
                modal.SetActive(true);
                modal.transform.Find("Modal/Label").GetComponent<UILabel>().text = controller.groupStore.message;
                gameObject.SetActive(false);
                controller.subPanels[2].SetActive(false);
                controller.subPanels[7].SetActive(false);

                Group_myGroups getMyGroupAct = ActionCreator.createAction(ActionTypes.MY_GROUP_PANEL) as Group_myGroups;
                gm.gameDispatcher.dispatch(getMyGroupAct);
            }
        }

        if(groupStoreEventType == ActionTypes.GROUP_GET_MEMBERS) {
            if (groupStore.storeStatus == storeStatus.NORMAL) {
                if (isDestroyReq && checkMemberExist()) {
                    destoyReq();
                    isDestroyReq = false;
                }
            }
        }
    }
}
