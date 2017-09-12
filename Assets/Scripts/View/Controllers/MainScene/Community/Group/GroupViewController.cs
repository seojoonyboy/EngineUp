using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GroupViewController : MonoBehaviour {
    public GameObject[] subPanels;
    public Groups groupStore;
    public Locations locationStore;
    private GameManager gm;

    public InputField searchInput;

    public GroupAddViewController addViewCtrler;
    public GroupDetailViewController detailView;
    public GroupSearchView searchView;
    public GroupDelView groupDelView;

    public GameObject container;
    public UIGrid grid;

    public GameObject modal;

    void Awake() {
        gm = GameManager.Instance;
    }

    void OnEnable() {
        //내 그룹 목록을 불러온다.
        //Group_myGroups getMyGroupAct = ActionCreator.createAction(ActionTypes.MY_GROUP_PANEL) as Group_myGroups;
        //gm.gameDispatcher.dispatch(getMyGroupAct);
    }

    void OnDisable() {
        //for(int i=0; i<subPanels.Length; i++) {
        //    subPanels[i].SetActive(false);
        //}
    }

    public void onPanel(GameObject obj) {
        int index = obj.GetComponent<GroupSceneIndex>().index;
        sendReq(index, obj);
    }

    public void sendReq(int sceneIndex, GameObject obj) {
        Debug.Log("Scene Index : " + sceneIndex);
        switch (sceneIndex) {
            //그룹찾기
            case 1:
                break;
            //그룹 설정 메인
            case 2:
                break;
            //그룹 설정 변경
            case 4:
            //그룹추가
            case 8:
                break;
            //그룹원 관리
            //그룹원 보기
            case 0:
            case 5:
                break;
            case 6:
                break;
            case 7:
            //그룹 상세보기
                int id = obj.transform.parent.GetComponent<GroupIndex>().id;
                Group_detail getGroupDetailAct = ActionCreator.createAction(ActionTypes.GROUP_DETAIL) as Group_detail;
                getGroupDetailAct.id = id;
                gm.gameDispatcher.dispatch(getGroupDetailAct);
                subPanels[sceneIndex].SetActive(true);
                //Server에게 index를 이용한 리스트 요청 액션을 작성한다.
                break;
        }
        subPanels[sceneIndex].SetActive(true);
    }

    public void onGroupStoreListener() {
        detailView.onGroupStoreListener();
        addViewCtrler.onGroupStoreListener();
        searchView.onGroupStoreListener();

        ActionTypes groupStoreEventType = groupStore.eventType;
        if (groupStoreEventType == ActionTypes.MY_GROUP_PANEL || groupStoreEventType == ActionTypes.GROUP_ADD || groupStoreEventType == ActionTypes.GROUP_DESTROY) {
            makeList(groupStore.myGroups);
        }
    }

    private void makeList(Group[] myGroups) {
        Debug.Log("내 그룹 목록 갱신");
        removeAllList();
        if(myGroups == null) {
            return;
        }
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

    void showDetail(GameObject obj) {
        sendReq(7, obj);
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

    public void offModal() {
        modal.SetActive(false);
    }
}
