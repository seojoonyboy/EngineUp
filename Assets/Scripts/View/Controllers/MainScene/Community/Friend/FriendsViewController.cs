using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class FriendsViewController : MonoBehaviour {
    public int childNum = 5;
    public GameObject container;

    public InputField input;
    public GameManager gameManager;
    public Friends friendsStore;

    public GameObject 
        modal,
        friendProfilePanel,
        myFriendEmptyLabel,
        sendFriendEmptyLabel,
        receiveFriendEmptyLabel;

    public GameObject[] subPanels;
    private GameObject notifyModal;

    void Awake() {
        notifyModal = transform.parent.GetComponent<CommunityVC>().notifyModal;
        subPanels[2].GetComponent<FR_SendingsView>().gameManager = gameManager;
        subPanels[2].GetComponent<FR_SendingsView>().friendsStore = friendsStore;

        subPanels[1].GetComponent<FR_ReceivesView>().gameManager = gameManager;
        subPanels[1].GetComponent<FR_ReceivesView>().friendsStore = friendsStore;
    }

    void OnDisable() {
        initToggle();
    }

    public void OnFriendsStoreListener() {
        if(friendsStore.eventType == ActionTypes.GET_MY_FRIEND_LIST) {
            if(friendsStore.storeStatus == storeStatus.NORMAL) {
                if(friendsStore.getReqType == GetMyFriendListAction.type.FRIEND) {
                    subPanels[0].GetComponent<FR_MyListView>().makeMyFriendList();
                }
                else if(friendsStore.getReqType == GetMyFriendListAction.type.WAITING) {
                    subPanels[2].GetComponent<FR_SendingsView>().makeMyFriendList();
                }
            }
        }

        if(friendsStore.eventType == ActionTypes.GET_WAITING_FRIEND_ACCEPT_LIST) {
            if (friendsStore.storeStatus == storeStatus.NORMAL) {
                subPanels[1].GetComponent<FR_ReceivesView>().makeMyFriendList();
            }
        }

        if(friendsStore.eventType == ActionTypes.COMMUNITY_SEARCH) {
            if(friendsStore.storeStatus == storeStatus.NORMAL || friendsStore.storeStatus == storeStatus.ERROR) {
                onNotifyModal(friendsStore.msg);
            }
        }

        if(friendsStore.eventType == ActionTypes.ADD_FRIEND) {
            if(friendsStore.storeStatus == storeStatus.ERROR) {
                onNotifyModal(friendsStore.msg);
            }
        }

        //if (friendsStore.eventType == ActionTypes.GET_WAITING_FRIEND_ACCEPT_LIST) {
        //    makeMyFriendList();
        //    //makeStandByAcceptList();
        //    //makeFriendReqList();
        //}

        //if(friendsStore.eventType == ActionTypes.COMMUNITY_SEARCH) {
        //    if (!friendsStore.searchResult) {
        //        onSearchFeedbackMsg(friendsStore.msg);
        //    }
        //}

        //if(friendsStore.eventType == ActionTypes.ADD_FRIEND) {
        //    if (!friendsStore.addResult) {
        //        onSearchFeedbackMsg(friendsStore.msg);
        //    }
        //}

        //if(friendsStore.eventType == ActionTypes.ADD_COMMUNITY_FRIEND_PREFAB) {
        //    if (friendsStore.addResult) {
        //        onSearchFeedbackMsg(friendsStore.msg);
        //        if (friendsStore.searchedFriend != null) {
        //            Debug.Log("친구 검색을 통한 프리팹 생성");
        //            addFriendPref(friendsStore.searchedFriend[0], friendsStore.addFriendType);
        //        }
        //        //수락을 통한 친구 추가인 경우
        //        else {
        //            Debug.Log("수락을 통한 프리팹 생성");
        //            addFriendPref(friendsStore.addedFriend, friendsStore.addFriendType);
        //            friendsStore.addedFriend = null;
        //        }
        //        friendsStore.addResult = false;
        //    }
        //}

        //if(friendsStore.eventType == ActionTypes.DELETE_COMMUNITY_FRIEND_PREFAB) {
        //    deletePref(friendsStore.targetObj);
        //    friendsStore.targetObj = null;
        //}
    }

    private void onNotifyModal(string msg) {
        notifyModal.SetActive(true);
        notifyModal.transform.Find("InnerModal/Text").GetComponent<Text>().text = msg;
    }

    //내 친구 목록 생성
    //public void makeMyFriendList() {
    //    //removeAllList(myFriendGrid);
    //    if (friendsStore.myFriends.Length == 0) {
    //        Debug.Log("no friend");
    //        myFriendEmptyLabel.SetActive(true);
    //        return;
    //    }

    //    for (int i = 0; i < friendsStore.myFriends.Length; i++) {
    //        GameObject item = Instantiate(container);

    //        item.GetComponent<ButtonIndex>().index = friendsStore.myFriends[i].id;

    //        item.transform.Find("Name").GetComponent<UILabel>().text = friendsStore.myFriends[i].toUser.nickName;
    //        //containerInit(item, myFriendGrid);
    //        GameObject tmp = item.transform.Find("RemoveButton").gameObject;

    //        EventDelegate delEvent = new EventDelegate(this, "delFriend");

    //        EventDelegate.Parameter param = new EventDelegate.Parameter();
    //        param.obj = item;
    //        param.field = "index";
    //        delEvent.parameters[0] = param;

    //        EventDelegate.Add(tmp.GetComponent<UIButton>().onClick, delEvent);

    //        tmp = item;
    //        EventDelegate friendProfileEvent = new EventDelegate(this, "onFriendDetailPanel");
    //        friendProfileEvent.parameters[0] = param;
    //        EventDelegate.Add(tmp.GetComponent<UIButton>().onClick, friendProfileEvent);

    //        //내친구 상세 정보 할당
    //        tmp.GetComponent<FriendIndex>().id = friendsStore.myFriends[i].toUser.id;
    //        tmp.GetComponent<FriendIndex>().nickName = friendsStore.myFriends[i].toUser.nickName;
    //    }
    //}

    ////수락 대기 목록 생성
    //public void makeStandByAcceptList() {
    //    //removeAllList(receiveFrienReqGrid);
    //    if (friendsStore.waitingAcceptLists.Length == 0) {
    //        Debug.Log("수락 대기 없음");
    //        receiveFriendEmptyLabel.SetActive(true);
    //        return;
    //    }
    //    for (int i = 0; i < friendsStore.waitingAcceptLists.Length; i++) {
    //        Debug.Log("수락대기 목록");
    //        GameObject item = Instantiate(container);
    //        item.GetComponent<ButtonIndex>().index = friendsStore.waitingAcceptLists[i].id;
    //        item.GetComponent<ButtonIndex>().fromUserId = friendsStore.waitingAcceptLists[i].fromUser.id;
    //        EventDelegate.Parameter param = new EventDelegate.Parameter();
    //        param.obj = item;
    //        param.field = "index";

    //        item.transform.Find("Name").GetComponent<UILabel>().text = friendsStore.waitingAcceptLists[i].fromUser.nickName;
    //        //containerInit(item, receiveFrienReqGrid);
    //        item.transform.Find("RemoveButton").gameObject.SetActive(false);

    //        GameObject tmp = item.transform.Find("AcceptButton").gameObject;
    //        tmp.SetActive(true);

    //        EventDelegate acceptEvent = new EventDelegate(this, "acceptFriendReq");
    //        acceptEvent.parameters[0] = param;
    //        EventDelegate.Add(tmp.GetComponent<UIButton>().onClick, acceptEvent);

    //        tmp = item.transform.Find("RejectButton").gameObject;
    //        tmp.SetActive(true);

    //        EventDelegate delEvent = new EventDelegate(this, "rejectReq");
    //        delEvent.parameters[0] = param;
    //        EventDelegate.Add(tmp.GetComponent<UIButton>().onClick, delEvent);

    //        tmp = item;
    //        EventDelegate friendProfileEvent = new EventDelegate(this, "onFriendDetailPanel");
    //        friendProfileEvent.parameters[0] = param;
    //        EventDelegate.Add(tmp.GetComponent<UIButton>().onClick, friendProfileEvent);

    //        //내친구 상세 정보 할당
    //        //tmp.GetComponent<FriendIndex>().id = friendsStore.waitingAcceptLists[i].fromUser.id;
    //        //tmp.GetComponent<FriendIndex>().nickName = friendsStore.waitingAcceptLists[i].fromUser.nickName;
    //    }
    //}

    ////친구 신청 목록 생성
    //public void makeFriendReqList() {
    //    //removeAllList(sendFriendReqGrid);
    //    Friend[] friends = friendsStore.friendReqLists;
    //    if(friends.Length == 0) {
    //        sendFriendEmptyLabel.SetActive(true);
    //    }
    //    for(int i=0; i< friends.Length; i++) {
    //        addFriendPref(friends[i]);
    //    }
    //}

    //void removeAllList(UIGrid grid) {
    //    grid.transform.DestroyChildren();
    //}

    public void search() {
        CommunitySearchAction action = ActionCreator.createAction(ActionTypes.COMMUNITY_SEARCH) as CommunitySearchAction;
        action._type = CommunitySearchAction.searchType.FRIEND;
        action.keyword = input.text;
        gameManager.gameDispatcher.dispatch(action);
    }

    public void onSearchFeedbackMsg(string msg) {
        modal.SetActive(true);
        modal.transform.Find("ResponseModal/MsgLabel").GetComponent<UILabel>().text = msg;
    }

    //overloading
    public void addFriendPref(Friend friend) {
        GameObject item = Instantiate(container);
        //containerInit(item, sendFriendReqGrid);
        item.GetComponent<ButtonIndex>().index = friend.id;
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

        tmp = item;

        EventDelegate friendProfileEvent = new EventDelegate(this, "onFriendDetailPanel");
        friendProfileEvent.parameters[0] = param;
        EventDelegate.Add(tmp.GetComponent<UIButton>().onClick, friendProfileEvent);

        //내친구 상세 정보 할당
        tmp.GetComponent<FriendIndex>().id = friend.toUser.id;
        tmp.GetComponent<FriendIndex>().nickName = friend.toUser.nickName;
    }

    //친구 삭제 버튼 클릭시
    private void delFriend(GameObject obj) {
        //Debug.Log(obj.name);
        //Destroy(obj);
        Debug.Log("친구 삭제");
        CommunityDeleteAction action = ActionCreator.createAction(ActionTypes.COMMUNITY_DELETE) as CommunityDeleteAction;
        action._type = CommunityDeleteAction.deleteType.FRIEND;
        action.id = obj.GetComponent<ButtonIndex>().index;
        gameManager.gameDispatcher.dispatch(action);
    }

    //친구 요청 거절시
    private void rejectReq(GameObject obj) {
        Debug.Log("요청 거절");
        CommunityDeleteAction action = ActionCreator.createAction(ActionTypes.COMMUNITY_DELETE) as CommunityDeleteAction;
        action._type = CommunityDeleteAction.deleteType.FRIEND;
        int index = obj.GetComponent<ButtonIndex>().index;
        action.id = index;
        gameManager.gameDispatcher.dispatch(action);

        //containerInit(obj, receiveFrienReqGrid);

        //if (receiveFrienReqGrid.transform.childCount <= 1) {
        //    receiveFriendEmptyLabel.SetActive(true);
        //}
    }

    //친구 요청 취소
    private void cancelReq(GameObject obj) {
        Debug.Log("신청 취소");
        CommunityDeleteAction action = ActionCreator.createAction(ActionTypes.COMMUNITY_DELETE) as CommunityDeleteAction;
        action._type = CommunityDeleteAction.deleteType.FRIEND;
        int index = obj.GetComponent<FriendIndex>().queryId;
        action.id = index;
        gameManager.gameDispatcher.dispatch(action);
    }

    public void deletePref(GameObject obj) {
        Destroy(obj);
        UIGrid grid = obj.transform.parent.GetComponent<UIGrid>();
        grid.repositionNow = true;
        grid.Reposition();
        checkEmpty(obj);
    }

    private void checkEmpty(GameObject obj) {
        Transform parent = obj.transform.parent;
        //Debug.Log("child count : " + parent.childCount);
        if (parent.childCount <= 1) {
            Debug.Log("Count zero");
            parent.parent.parent.Find("Background/Message").gameObject.SetActive(true);
        }
    }

    private void containerInit(GameObject obj, UIGrid grid) {
        obj.transform.SetParent(grid.transform);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localScale = Vector3.one;

        grid.repositionNow = true;
        grid.Reposition();
        grid.transform.parent.parent.Find("Background/Message").gameObject.SetActive(false);
    }

    private void onFriendDetailPanel(GameObject obj) {
        friendProfilePanel.SetActive(true);
        friendProfilePanel.GetComponent<FriendsProfileViewController>().setInfo(obj);
        //Debug.Log(obj.GetComponent<FriendIndex>().nickName);
    }

    public void OnToggle(GameObject obj) {
        bool isOn = obj.GetComponent<Toggle>().isOn;

        //obj.transform.Find("DeactiveImg").gameObject.SetActive(!isOn);
        obj.transform.Find("DeactiveLabel").gameObject.SetActive(!isOn);
        //obj.transform.Find("ActiveImg").gameObject.SetActive(isOn);
        obj.transform.Find("ActiveLabel").gameObject.SetActive(isOn);
    }

    private void initToggle() {
        foreach (GameObject panel in subPanels) {
            panel.SetActive(false);
        }

        var tmp = transform.Find("TogglePanel/MyFriends").gameObject;
        tmp.GetComponent<Toggle>().isOn = true;
        OnToggle(tmp);

        tmp = transform.Find("TogglePanel/Receives").gameObject;
        tmp.GetComponent<Toggle>().isOn = false;
        OnToggle(tmp);

        tmp = transform.Find("TogglePanel/Sendings").gameObject;
        tmp.GetComponent<Toggle>().isOn = false;
        OnToggle(tmp);
    }
}
