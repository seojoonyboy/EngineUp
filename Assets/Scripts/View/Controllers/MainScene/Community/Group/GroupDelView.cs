using UnityEngine;
using System.Collections;

public class GroupDelView : MonoBehaviour {
    public GroupViewController controller;
    GameManager gm;

    public GameObject modal;

    void Awake() {
        gm = GameManager.Instance;
    }

    public void onDestroyButton() {
        getMembers();
    }

    public void onModal(string result) {
        modal.SetActive(true);
        switch (result) {
            case "SUCCESS":
                modal.transform.Find("SuccessModal").gameObject.SetActive(true);
                break;
            case "FAIL":
                modal.transform.Find("FailModal").gameObject.SetActive(true);
                break;
        }
    }

    public void offPanel() {
        gameObject.SetActive(false);
    }

    public void offModal() {
        modal.SetActive(false);
        modal.transform.Find("SuccessModal").gameObject.SetActive(false);
        modal.transform.Find("FailModal").gameObject.SetActive(false);
    }

    private void getMembers() {
        //그룹장을 제외한 그룹원이 존재하는지 확인
        Group_getMemberAction getMemberAct = ActionCreator.createAction(ActionTypes.GROUP_GET_MEMBERS) as Group_getMemberAction;
        getMemberAct.id = controller.detailView.id;
        getMemberAct.forDestroyManage = true;
        gm.gameDispatcher.dispatch(getMemberAct);
    }

    public void getMemberCallback() {
        Member[] members = controller.groupStore.groupMembers;
        int memberCnt = 0;
        for (int i = 0; i < members.Length; i++) {
            if (members[i].memberState == "MB") {
                if (members[i].memberGrade != "GO") {
                    memberCnt++;
                }
            }
        }
        if (memberCnt == 0) {
            destoyReq();
        }
        else {
            onModal("FAIL");
        }
    }

    public void destoyReq() {
        Group_del delAct = ActionCreator.createAction(ActionTypes.GROUP_DESTROY) as Group_del;
        delAct.id = controller.detailView.id;
        gm.gameDispatcher.dispatch(delAct);
    }
}
