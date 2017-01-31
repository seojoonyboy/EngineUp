using UnityEngine;
using System.Collections;
using System;

public class FriendsViewController : MonoBehaviour {
    public int childNum = 5;
    public GameObject container;

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
        //if(friendsStore.eventType == ActionTypes.COMMUNITY_INITIALIZE) {
        //    makeMyFriendList();
        //    makeFriendReqList();
        //    makeStandByAcceptList();
        //}
        if(friendsStore.eventType == ActionTypes.GET_MY_FRIEND_LIST) {
            makeMyFriendList();

            GetAcceptWaitingListAction action = ActionCreator.createAction(ActionTypes.GET_WAITING_FRIEND_ACCEPT_LIST) as GetAcceptWaitingListAction;
            GameManager.Instance.gameDispatcher.dispatch(action);
        }

        if(friendsStore.eventType == ActionTypes.GET_WAITING_FRIEND_ACCEPT_LIST) {
            makeStandByAcceptList();
        }

        if(friendsStore.eventType == ActionTypes.COMMUNITY_SEARCH) {
            if(friendsStore.searchResult) {
                addFriend(friendsStore.newFriend.id);
                friendsStore.searchResult = false;
            }
            onSearchFeedbackMsg(friendsStore.msg);
        }
        if(friendsStore.eventType == ActionTypes.COMMUNITY_DELETE) {
            Debug.Log("Delete Success");
            if(friendsStore.deleteResult) {
                if (friendsStore.targetObj) {
                    deletePref(friendsStore.targetObj);
                    friendsStore.targetObj = null;
                    friendsStore.deleteResult = false;
                }
            }
        }

        if(friendsStore.eventType == ActionTypes.ADD_FRIEND) {
            if (friendsStore.addResult) {
                if (friendsStore.needNewPref) {
                    addFriendPref(friendsStore.newFriend);
                    friendsStore.needNewPref = false;
                }
                friendsStore.addResult = false;
            }
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

        for (int i = 0; i < friendsStore.myFriends.Length; i++) {
            GameObject item = Instantiate(container);

            item.GetComponent<ButtonIndex>().index = friendsStore.myFriends[i].id;

            item.transform.Find("Name").GetComponent<UILabel>().text = friendsStore.myFriends[i].toUser.nickName;
            containerInit(item, myFriendGrid);
            GameObject tmp = item.transform.Find("RemoveButton").gameObject;

            EventDelegate delEvent = new EventDelegate(this, "delFriend");

            EventDelegate.Parameter param = new EventDelegate.Parameter();
            param.obj = item;
            param.field = "index";
            delEvent.parameters[0] = param;

            EventDelegate.Add(tmp.GetComponent<UIButton>().onClick, delEvent);

            tmp = item;
            EventDelegate friendProfileEvent = new EventDelegate(this, "onFriendPanel");
            EventDelegate.Add(tmp.GetComponent<UIButton>().onClick, friendProfileEvent);
        }
    }

    //수락 대기 목록 생성
    public void makeStandByAcceptList() {
        receiveFrienReqGrid = gameObject.transform.Find("ReceiveReqListPanel/ScrollView/Grid").GetComponent<UIGrid>();
        removeAllList(receiveFrienReqGrid);
        if (friendsStore.waitingAcceptLists == null) {
            Debug.Log("수락 대기 없음");
            return;
        }
        for (int i = 0; i < friendsStore.waitingAcceptLists.Length; i++) {
            GameObject item = Instantiate(container);
            item.GetComponent<ButtonIndex>().index = friendsStore.waitingAcceptLists[i].fromUser.id;

            EventDelegate.Parameter param = new EventDelegate.Parameter();
            param.obj = item;
            param.field = "index";

            item.transform.Find("Name").GetComponent<UILabel>().text = friendsStore.waitingAcceptLists[i].fromUser.nickName;
            containerInit(item, receiveFrienReqGrid);
            item.transform.Find("RemoveButton").gameObject.SetActive(false);

            GameObject tmp = item.transform.Find("AcceptButton").gameObject;
            tmp.SetActive(true);

            EventDelegate acceptEvent = new EventDelegate(this, "acceptFriendReq");
            acceptEvent.parameters[0] = param;
            EventDelegate.Add(tmp.GetComponent<UIButton>().onClick, acceptEvent);

            tmp = item.transform.Find("RejectButton").gameObject;
            tmp.SetActive(true);

            EventDelegate delEvent = new EventDelegate(this, "delFriend");
            delEvent.parameters[0] = param;
            EventDelegate.Add(tmp.GetComponent<UIButton>().onClick, delEvent);

            tmp = item;

            EventDelegate friendProfileEvent = new EventDelegate(this, "onFriendPanel");
            EventDelegate.Add(tmp.GetComponent<UIButton>().onClick, friendProfileEvent);
        }
    }

    //친구 신청 목록 생성
    public void makeFriendReqList() {
        removeAllList(sendFriendReqGrid);
        Friend[] friends = friendsStore.friendReqLists;
        for(int i=0; i< friends.Length; i++) {
            addFriendPref(friends[i]);
        }
    }

    void removeAllList(UIGrid grid) {
        grid.transform.DestroyChildren();
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
    private void addFriend(int id) {
        Debug.Log("친구 추가 요청");
        AddFriendAction addFriendAct = ActionCreator.createAction(ActionTypes.ADD_FRIEND) as AddFriendAction;
        addFriendAct.id = id;
        gameManager.gameDispatcher.dispatch(addFriendAct);
    }

    //친구 추가 요청시 UI Prefab 생성
    public void addFriendPref(SearchedFriend data) {
        GameObject item = Instantiate(container);
        containerInit(item, sendFriendReqGrid);
        
        GameObject tmp = item.transform.Find("RemoveButton").gameObject;
        //요청 거절 시 이벤트 동적 할당
        EventDelegate delEvent = new EventDelegate(this, "rejectReq");
        item.transform.Find("Name").GetComponent<UILabel>().text = data.nickName;

        EventDelegate.Parameter param = new EventDelegate.Parameter();
        param.obj = item;
        param.field = "index";
        delEvent.parameters[0] = param;

        EventDelegate.Add(tmp.GetComponent<UIButton>().onClick, delEvent);

        GameObject additionalMsg = item.transform.Find("AdditionalMsg").gameObject;
        additionalMsg.SetActive(true);
    }

    //overloading
    public void addFriendPref(Friend friend) {
        GameObject item = Instantiate(container);
        containerInit(item, sendFriendReqGrid);

        GameObject tmp = item.transform.Find("RemoveButton").gameObject;
        EventDelegate delEvent = new EventDelegate(this, "delFriend");
        item.transform.Find("Name").GetComponent<UILabel>().text = friend.toUser.nickName;

        EventDelegate.Parameter param = new EventDelegate.Parameter();
        param.obj = item;
        param.field = "index";
        delEvent.parameters[0] = param;

        EventDelegate.Add(tmp.GetComponent<UIButton>().onClick, delEvent);

        GameObject additionalMsg = item.transform.Find("AdditionalMsg").gameObject;
        additionalMsg.SetActive(true);
    }

    private void acceptFriendReq(GameObject obj) {
        Debug.Log("Accept Friend Req");
        int index = obj.GetComponent<ButtonIndex>().index;
        addFriend(index);
        Destroy(obj);
    }

    //친구 삭제 버튼 클릭시
    private void delFriend(GameObject obj) {
        //Debug.Log(obj.name);
        //Destroy(obj);
        CommunityDeleteAction action = ActionCreator.createAction(ActionTypes.COMMUNITY_DELETE) as CommunityDeleteAction;
        action.type = CommunityDeleteAction.deleteType.FRIEND;
        action.targetGameObj = obj;
        action.id = obj.GetComponent<ButtonIndex>().index;
        gameManager.gameDispatcher.dispatch(action);
    }

    //친구 요청 거절시
    private void rejectReq(GameObject obj) {

    }

    public void deletePref(GameObject obj) {
        Destroy(obj);
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
