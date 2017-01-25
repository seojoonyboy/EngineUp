using UnityEngine;
using System.Collections;
using System;

public class FriendsViewController : MonoBehaviour {
    public int childNum = 5;
    public GameObject container;

    private GameObject[] itemArr;

    private UIGrid 
        myFriendGrid,
        sendFriendReqGrid,
        receiveFrienReqGrid;

    private UIInput input;
    private GameManager gameManager;

    public Friends friendsStore;
    public GameObject 
        modal,
        friendProfilePanel;

    public void OnFriendsStoreListener() {
        Debug.Log(friendsStore.eventType);
        if(friendsStore.eventType == ActionTypes.COMMUNITY_INITIALIZE) {
            makeMyFriendList();
            makeFriendReqList();
        }
        if(friendsStore.eventType == ActionTypes.COMMUNITY_SEARCH) {
            if(friendsStore.searchResult == true) {
                search();
            }
            onSearchFeedbackMsg(friendsStore.msg);
        }
        if(friendsStore.eventType == ActionTypes.COMMUNITY_DELETE) {
            Debug.Log("친구 삭제");
            if(friendsStore.targetItem != null) {
                delete(friendsStore.targetItem);
            }
        }

        if(friendsStore.eventType == ActionTypes.ADD_FRIEND) {
            addFriendPref(friendsStore.keyword);
        }
    }

    void Start() {
        input = gameObject.transform.Find("FindFriendPanel/Input").GetComponent<UIInput>();
        input.activeTextColor = Color.black;
        gameManager = GameManager.Instance;
        
        sendFriendReqGrid = gameObject.transform.Find("SendReqListPanel/ScrollView/Grid").GetComponent<UIGrid>();
    }

    //내 친구 목록 생성
    public void makeMyFriendList() {
        myFriendGrid = gameObject.transform.Find("MyFriendListPanel/ScrollView/Grid").GetComponent<UIGrid>();
        if (friendsStore.myFriends == null) {
            Debug.Log("no friend");
            return;
        }
        removeAllList(myFriendGrid);

        for (int i = 0; i < friendsStore.myFriends.Count; i++) {
            itemArr[i] = Instantiate(container);
            containerInit(itemArr[i], myFriendGrid);
            GameObject tmp = itemArr[i].transform.Find("RemoveButton").gameObject;
            //tmp.GetComponent<ButtonIndex>().index = i;

            EventDelegate delEvent = new EventDelegate(this, "delFriendReq");

            EventDelegate.Parameter param = new EventDelegate.Parameter();
            param.obj = itemArr[i];
            param.field = "index";
            delEvent.parameters[0] = param;

            EventDelegate.Add(tmp.GetComponent<UIButton>().onClick, delEvent);

            tmp = itemArr[i];
            EventDelegate friendProfileEvent = new EventDelegate(this, "onFriendPanel");
            EventDelegate.Add(tmp.GetComponent<UIButton>().onClick, friendProfileEvent);
        }
    }

    //수락 대기 목록 생성
    public void makeStandByAcceptList() {
        removeAllList(receiveFrienReqGrid);
    }

    //친구 신청 목록 생성
    public void makeFriendReqList() {
        removeAllList(sendFriendReqGrid);
        for(int i=0; i<friendsStore.friendReqLists.Count; i++) {
            Debug.Log("make Friend Req List");
            //addFriendPref()
        }
    }

    void removeAllList(UIGrid grid) {
        
    }

    public void search() {
        CommunitySearchAction action = ActionCreator.createAction(ActionTypes.COMMUNITY_SEARCH) as CommunitySearchAction;
        action.type = CommunitySearchAction.searchType.FRIEND;
        action.keyword = input.value;
        gameManager.gameDispatcher.dispatch(action);
    }

    public void onSearchFeedbackMsg(string msg) {
        modal.SetActive(true);
        modal.transform.Find("ResponseModal/MsgLabel").GetComponent<UILabel>().text = msg;
    }

    //Server에 친구 추가 요청
    private void addFriend() {
        Debug.Log("친구 추가 요청");
        AddFriendAction addFriendAct = ActionCreator.createAction(ActionTypes.ADD_FRIEND) as AddFriendAction;
        addFriendAct.toUserId = friendsStore.toUserId;
        gameManager.gameDispatcher.dispatch(addFriendAct);
    }

    //친구 추가 요청시 UI Prefab 생성
    public void addFriendPref(string keyword) {
        GameObject item = Instantiate(container);
        containerInit(item, sendFriendReqGrid);

        GameObject tmp = item.transform.Find("RemoveButton").gameObject;
        EventDelegate delEvent = new EventDelegate(this, "delFriendReq");

        EventDelegate.Parameter param = new EventDelegate.Parameter();
        param.obj = item;
        param.field = "index";
        delEvent.parameters[0] = param;

        EventDelegate.Add(tmp.GetComponent<UIButton>().onClick, delEvent);

        GameObject additionalMsg = item.transform.Find("AdditionalMsg").gameObject;
        item.transform.Find("Name").GetComponent<UILabel>().text = keyword;
        additionalMsg.SetActive(true);
    }

    //if someone request friend, then exec
    public void addFriendReq() {        
        GameObject item = Instantiate(container);

        containerInit(item, receiveFrienReqGrid);

        GameObject btn = item.transform.Find("AcceptButton").gameObject;
        btn.SetActive(true);

        EventDelegate acceptEvent = new EventDelegate(this, "acceptFriendReq");
        EventDelegate.Parameter param = new EventDelegate.Parameter();
        param.obj = item;
        param.field = "index";
        acceptEvent.parameters[0] = param;
        EventDelegate.Add(btn.GetComponent<UIButton>().onClick, acceptEvent);

        EventDelegate denyEvent = new EventDelegate(this, "delFriendReq");
        denyEvent.parameters[0] = param;
        btn = item.transform.Find("RejectButton").gameObject;
        btn.SetActive(true);        
        EventDelegate.Add(btn.GetComponent<UIButton>().onClick, denyEvent);
        
        btn = item.transform.Find("RemoveButton").gameObject;
        btn.SetActive(false);
    }

    private void acceptFriendReq(GameObject obj) {
        Debug.Log("Accept Friend Req");
        Debug.Log(obj.name);
    }

    //친구 삭제 버튼 클릭시
    private void delFriendReq(GameObject obj) {
        Debug.Log(obj.name);
        CommunityDeleteAction action = ActionCreator.createAction(ActionTypes.COMMUNITY_DELETE) as CommunityDeleteAction;
        action.type = CommunityDeleteAction.deleteType.FRIEND;
        action.targetGameObj = obj;
        gameManager.gameDispatcher.dispatch(action);
    }
    
    public void delete(GameObject obj) {
        Destroy(obj.gameObject);
        UIGrid grid = obj.transform.parent.GetComponent<UIGrid>();
        grid.repositionNow = true;
        grid.Reposition();

        //grid.repositionNow = true;
        //DeleteCommunityAction action = ActionCreator.createAction(ActionTypes.DELETE_COMMUNITY_DATA) as DeleteCommunityAction;
        //action.key_id = index;
        //gameManager.gameDispatcher.dispatch(action);
    }

    private void containerInit(GameObject obj, UIGrid grid) {
        obj.transform.SetParent(grid.transform);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localScale = Vector3.one;

        grid.Reposition();
    }

    private void onFriendPanel() {
        friendProfilePanel.SetActive(true);
    }
}
