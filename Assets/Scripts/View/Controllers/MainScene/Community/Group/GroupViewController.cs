using UnityEngine;
using System.Collections;
using System;

public class GroupViewController : MonoBehaviour {
    public GameObject[] subPanels;
    public Groups groupStore;
    private GameManager gm;

    void Start() {
        gm = GameManager.Instance;
    }

    public void onPanel(GameObject obj) {
        int sceneIndex = obj.GetComponent<GroupSceneIndex>().index;
        sendReq(sceneIndex, obj);
        //subPanels[index].SetActive(true);
    }

    private void sendReq(int sceneIndex, GameObject obj) {
        switch (sceneIndex) {
            //그룹 상세보기
            case 0:
                int id = obj.transform.parent.GetComponent<GroupIndex>().id;
                Group_getMemberAction getGroupMemberAct = ActionCreator.createAction(ActionTypes.GROUP_GET_MEMBERS) as Group_getMemberAction;
                getGroupMemberAct.id = id;
                gm.gameDispatcher.dispatch(getGroupMemberAct);
                //Server에게 index를 이용한 리스트 요청 액션을 작성한다.
                break;
        }
    }

    public void onGroupStoreListener() {

    }
}
