using UnityEngine;
using System.Collections;

public class GroupViewController : MonoBehaviour {
    public GameObject[] subPanels;
    public Groups groupStore;
    public Locations locationStore;
    private GameManager gm;

    public UIInput searchInput;

    public GroupAddViewController addViewCtrler;
    public GroupDetailView detailView;
    public GroupMemberManageView memberManageView;
    public GroupSettingChangeView settingChangeView;

    public GameObject container;
    public UIGrid grid;

    public GameObject modal;

    void Awake() {
        gm = GameManager.Instance;
    }

    void OnEnable() {
        //내 그룹 목록을 불러온다.
        Group_myGroups getMyGroupAct = ActionCreator.createAction(ActionTypes.GROUP_MY_GROUPS) as Group_myGroups;
        getMyGroupAct.id = 0;
        gm.gameDispatcher.dispatch(getMyGroupAct);
    }

    void OnDisable() {
        for(int i=0; i<subPanels.Length; i++) {
            subPanels[i].SetActive(false);
        }
    }

    public void onPanel(GameObject obj) {
        int sceneIndex = obj.GetComponent<GroupSceneIndex>().index;
        sendReq(sceneIndex, obj);
        //subPanels[index].SetActive(true);
    }

    private void sendReq(int sceneIndex, GameObject obj) {
        switch (sceneIndex) {
            //그룹원 보기
            case 0:
                Group_getMemberAction getMembersAct = ActionCreator.createAction(ActionTypes.GROUP_GET_MEMBERS) as Group_getMemberAction;
                getMembersAct.id = detailView.id;
                gm.gameDispatcher.dispatch(getMembersAct);
                break;
            //그룹찾기
            case 1:
                Group_search searchAct = ActionCreator.createAction(ActionTypes.GROUP_SEARCH) as Group_search;
                searchAct.keyword = searchInput.value;
                gm.gameDispatcher.dispatch(searchAct);
                break;
            //그룹 설정 메인
            case 2:
                subPanels[sceneIndex].SetActive(true);
                break;
            //그룹 설정 변경
            //그룹추가
            case 4:
            case 8:
                GetDistrictsData getDistDataAct = ActionCreator.createAction(ActionTypes.GET_DISTRICT_DATA) as GetDistrictsData;
                getDistDataAct.id = sceneIndex;
                gm.gameDispatcher.dispatch(getDistDataAct);
                break;
            //그룹원 관리
            case 5:
                Group_getMemberAction _getMembersAct = ActionCreator.createAction(ActionTypes.GROUP_GET_MEMBERS) as Group_getMemberAction;
                _getMembersAct.id = detailView.id;
                _getMembersAct.forMemberManage = true;
                gm.gameDispatcher.dispatch(_getMembersAct);
                break;
            //그룹 상세보기
            case 7:
                int id = obj.transform.parent.GetComponent<GroupIndex>().id;
                Group_detail getGroupDetailAct = ActionCreator.createAction(ActionTypes.GROUP_DETAIL) as Group_detail;
                getGroupDetailAct.id = id;
                gm.gameDispatcher.dispatch(getGroupDetailAct);
                //Server에게 index를 이용한 리스트 요청 액션을 작성한다.
                break;
        }
    }

    public void onGroupStoreListener() {
        if(groupStore.eventType == ActionTypes.GROUP_MY_GROUPS) {
            makeList(groupStore.myGroups);
        }

        if (groupStore.eventType == ActionTypes.GROUP_ON_PANEL) {
            int index = groupStore.sceneIndex;
            if(index != -1) {
                subPanels[index].SetActive(true);
            }
        }
        if(locationStore.eventType == ActionTypes.GET_CITY_DATA) {
            addViewCtrler.setCityList();
            settingChangeView.setCityList();
        }

        if(groupStore.eventType == ActionTypes.GROUP_ADD) {
            if (groupStore.addResult) {
                addViewCtrler.offPanel();
                //내 그룹 목록을 불러온다.
                Group_myGroups getMyGroupAct = ActionCreator.createAction(ActionTypes.GROUP_MY_GROUPS) as Group_myGroups;
                getMyGroupAct.id = 0;
                gm.gameDispatcher.dispatch(getMyGroupAct);
            }
            else {

            }
        }

        if(groupStore.eventType == ActionTypes.GROUP_CHECK_MY_STATUS) {
            if (groupStore.isGroupMember) {
                Debug.Log("그룹 멤버임");
                if(groupStore.myInfoInGroup[0].memberGrade == "GO") {
                    Debug.Log("그룹장임");
                    detailView.setViewMode("OWNER");
                }
                else if(groupStore.myInfoInGroup[0].memberGrade == "GM") {
                    detailView.setViewMode("MEMBER");
                }
            }
            else {
                detailView.setViewMode("VISITOR");
                Debug.Log("그룹 멤버가 아님");
            }
        }

        if(groupStore.eventType == ActionTypes.GROUP_JOIN) {
            detailView.setViewMode("MEMBER");

            modal.SetActive(true);

            modal.transform.Find("ResponseModal/MsgLabel").GetComponent<UILabel>().text = "회원가입 신청을 완료하였습니다.";
            //내 그룹 목록 갱신
            Group_myGroups getMyGroupAct = ActionCreator.createAction(ActionTypes.GROUP_MY_GROUPS) as Group_myGroups;
            getMyGroupAct.id = 0;
            gm.gameDispatcher.dispatch(getMyGroupAct);
        }

        if(groupStore.eventType == ActionTypes.GROUP_MEMBER_ACCEPT) {
            modal.SetActive(true);

            //해당 그룹 멤버 목록 갱신
            Group_getMemberAction _getMembersAct = ActionCreator.createAction(ActionTypes.GROUP_GET_MEMBERS) as Group_getMemberAction;
            _getMembersAct.id = detailView.id;
            _getMembersAct.forMemberManage = true;
            gm.gameDispatcher.dispatch(_getMembersAct);

            modal.transform.Find("ResponseModal/MsgLabel").GetComponent<UILabel>().text = "멤버요청을 수락하였습니다.";
        }

        if(groupStore.eventType == ActionTypes.GROUP_BAN) {
            modal.SetActive(true);

            //해당 그룹 멤버 목록 갱신
            Group_getMemberAction _getMembersAct = ActionCreator.createAction(ActionTypes.GROUP_GET_MEMBERS) as Group_getMemberAction;
            _getMembersAct.id = detailView.id;
            _getMembersAct.forMemberManage = true;
            gm.gameDispatcher.dispatch(_getMembersAct);

            modal.transform.Find("ResponseModal/MsgLabel").GetComponent<UILabel>().text = "멤버를 강퇴시켰습니다.";
        }

        if(groupStore.eventType == ActionTypes.GROUP_GET_MEMBERS) {
            memberManageView.makeList();
        }
    }

    public void checkCanAdd(GameObject obj) {
        //조건에 만족하면
        onPanel(obj);
    }

    private void makeList(Group[] myGroups) {
        Debug.Log("내 그룹 목록 갱신");
        removeAllList();
        for (int i = 0; i < myGroups.Length; i++) {
            GameObject item = Instantiate(container);

            item.transform.SetParent(grid.transform);
            item.transform.localPosition = Vector3.zero;
            item.transform.localScale = Vector3.one;

            item.GetComponent<GroupIndex>().id = myGroups[i].id;
            item.transform.Find("GroupNameLabel").GetComponent<UILabel>().text = myGroups[i].name;
            item.transform.Find("LocationLabel").GetComponent<UILabel>().text = myGroups[i].locationDistrict + " " + myGroups[i].locationCity;
            item.transform.Find("MemberCountLabel").GetComponent<UILabel>().text = "멤버 " + myGroups[i].membersCount + "명";

            EventDelegate addEvent = new EventDelegate(this, "showDetail");
            GameObject detailBtn = item.transform.Find("GotoDetailBtn").gameObject;
            addEvent.parameters[0] = MakeParameter(detailBtn, typeof(GameObject));
            EventDelegate.Add(detailBtn.GetComponent<UIButton>().onClick, addEvent);
        }
        containerInit();
    }

    private void removeAllList() {
        foreach(Transform child in grid.transform) {
            if(child.tag == "AddBtn") {
                continue;
            }
            else {
                Destroy(child.gameObject);
            }
        }
    }

    private void containerInit() {
        grid.repositionNow = true;
        grid.Reposition();
    }

    private EventDelegate.Parameter MakeParameter(Object _value, System.Type _type) {
        EventDelegate.Parameter param = new EventDelegate.Parameter();  // 이벤트 parameter 생성.
        param.obj = _value;   // 이벤트 함수에 전달하고 싶은 값.
        param.expectedType = _type;    // 값의 타입.

        return param;
    }

    void showDetail(GameObject _obj) {
        onPanel(_obj);
    }

    public void offModal() {
        modal.SetActive(false);
    }
}
