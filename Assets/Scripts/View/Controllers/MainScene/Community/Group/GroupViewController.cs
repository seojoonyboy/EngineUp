using UnityEngine;
using System.Collections;
using System;

public class GroupViewController : MonoBehaviour {
    public GameObject[] subPanels;
    public Groups groupStore;
    public Locations locationStore;
    private GameManager gm;

    public UIInput searchInput;

    public GroupAddViewController addViewCtrler;

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
                //Debug.Log("그룹 상세 보기");
                int id = obj.transform.parent.GetComponent<GroupIndex>().id;
                Group_getMemberAction getGroupMemberAct = ActionCreator.createAction(ActionTypes.GROUP_GET_MEMBERS) as Group_getMemberAction;
                getGroupMemberAct.id = id;
                gm.gameDispatcher.dispatch(getGroupMemberAct);
                //Server에게 index를 이용한 리스트 요청 액션을 작성한다.
                break;
            //그룹찾기
            case 1:
                Group_search searchAct = ActionCreator.createAction(ActionTypes.GROUP_SEARCH) as Group_search;
                searchAct.keyword = searchInput.value;
                gm.gameDispatcher.dispatch(searchAct);
                break;
        }
    }

    public void onGroupStoreListener() {
        if (groupStore.eventType == ActionTypes.GROUP_ON_PANEL) {
            int index = groupStore.sceneIndex;
            if(index != -1) {
                subPanels[index].SetActive(true);
            }
        }
        if(locationStore.eventType == ActionTypes.GET_CITY_DATA) {
            addViewCtrler.setCityList();
        }

        if(groupStore.eventType == ActionTypes.GROUP_ADD) {
            if (groupStore.addResult) {
                addViewCtrler.offPanel();
            }
            else {

            }
        }
    }
}
