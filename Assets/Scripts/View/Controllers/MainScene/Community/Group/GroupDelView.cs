using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GroupDelView : MonoBehaviour {
    public GroupViewController controller;
    private Animator animator;

    GameManager gm;

    public GameObject modal;
    public bool isDestroyReq = false;

    void Awake() {
        animator = GetComponent<Animator>();
        gm = GameManager.Instance;
        modal = controller.modal;
    }

    void OnEnable() {
        Invoke("playSlideIn", 0.2f);
    }

    void playSlideIn() {
        animator.Play("SlideIn");
    }

    public void onBackButton() {
        animator.Play("SlideOut");
    }

    public void slideFinished(AnimationEvent animationEvent) {
        int boolParm = animationEvent.intParameter;

        //slider in
        if (boolParm == 1) {

        }

        //slider out
        else if (boolParm == 0) {
            gameObject.SetActive(false);
        }
    }

    public void onDestroyButton() {
        if (!checkMemberExist()) {
            destoyReq();
        }
        else {
            string msg = "가입된 그룹원이 있어 해체할 수 없습니다.";
            controller.onModal(msg);
        }
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
                return true;
            }
        }
        return false;
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
