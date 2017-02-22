using UnityEngine;
using System.Collections;

public class GroupViewController : MonoBehaviour {
    public GameObject[] subPanels;
    public Groups groupStore;
    public Locations locationStore;
    private GameManager gm;

    public UIInput searchInput;

    public GroupAddViewController addViewCtrler;

    public GameObject container;
    public UIGrid grid;

    void Awake() {
        gm = GameManager.Instance;
    }

    void OnEnable() {
        Group_myGroups getMyGroupAct = ActionCreator.createAction(ActionTypes.GROUP_MY_GROUPS) as Group_myGroups;
        getMyGroupAct.id = 0;
        gm.gameDispatcher.dispatch(getMyGroupAct);
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
                Group_detail getGroupMemberAct = ActionCreator.createAction(ActionTypes.GROUP_DETAIL) as Group_detail;
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
            //그룹추가
            case 8:
                GetDistrictsData getDistDataAct = ActionCreator.createAction(ActionTypes.GET_DISTRICT_DATA) as GetDistrictsData;
                gm.gameDispatcher.dispatch(getDistDataAct);
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
        }

        if(groupStore.eventType == ActionTypes.GROUP_ADD) {
            if (groupStore.addResult) {
                addViewCtrler.offPanel();
                Group_myGroups getMyGroupAct = ActionCreator.createAction(ActionTypes.GROUP_MY_GROUPS) as Group_myGroups;
                getMyGroupAct.id = 0;
                gm.gameDispatcher.dispatch(getMyGroupAct);
            }
            else {

            }
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
}
