using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class FriendsViewController : MonoBehaviour {
    public GameManager gameManager;
    public Friends friendsStore;

    public GameObject[] subPanels;
    private GameObject notifyModal;

    public GameObject content;
    public GameObject label;
    public InputField input;

    void Awake() {
        notifyModal = transform.parent.GetComponent<CommunityVC>().notifyModal;
        subPanels[2].GetComponent<FR_SendingsView>().gameManager = gameManager;
        subPanels[2].GetComponent<FR_SendingsView>().friendsStore = friendsStore;

        subPanels[0].GetComponent<FR_ReceivesView>().gameManager = gameManager;
        subPanels[0].GetComponent<FR_ReceivesView>().friendsStore = friendsStore;
    }

    void OnEnable() {
        init();
    }

    public void OnFriendsStoreListener() {
        if((friendsStore.myFriends == null || friendsStore.myFriends.Count == 0) && (friendsStore.waitingAcceptLists == null || friendsStore.waitingAcceptLists.Count == 0) && (friendsStore.friendReqLists == null || friendsStore.friendReqLists.Count == 0)) {
            label.SetActive(true);
        }
        else {
            label.SetActive(false);
        }

        content.GetComponent<ContentSizeFitter>().enabled = false;
        if (friendsStore.eventType == ActionTypes.GET_MY_FRIEND_LIST) {
            if(friendsStore.storeStatus == storeStatus.NORMAL) {
                if(friendsStore.getReqType == GetMyFriendListAction.type.FRIEND) {
                    subPanels[1].GetComponent<FR_MyListView>().makeMyFriendList();
                }
                else if(friendsStore.getReqType == GetMyFriendListAction.type.WAITING) {
                    subPanels[2].GetComponent<FR_SendingsView>().makeMyFriendList();
                }
            }
        }

        if(friendsStore.eventType == ActionTypes.GET_WAITING_FRIEND_ACCEPT_LIST) {
            if (friendsStore.storeStatus == storeStatus.NORMAL) {
                subPanels[0].GetComponent<FR_ReceivesView>().makeMyFriendList();
            }
        }

        if(friendsStore.eventType == ActionTypes.COMMUNITY_SEARCH) {
            if(friendsStore.storeStatus == storeStatus.NORMAL || friendsStore.storeStatus == storeStatus.ERROR) {
                onNotifyModal(friendsStore.msg);
            }
        }

        if(friendsStore.eventType == ActionTypes.ADD_FRIEND) {
            if(friendsStore.storeStatus == storeStatus.NORMAL || friendsStore.storeStatus == storeStatus.ERROR) {
                onNotifyModal(friendsStore.msg);
            }
        }

        if(friendsStore.eventType == ActionTypes.COMMUNITY_DELETE) {
            if(friendsStore.storeStatus == storeStatus.NORMAL || friendsStore.storeStatus == storeStatus.ERROR) {
                onNotifyModal(friendsStore.msg);
            }
        }
    }

    public void init() {
        var act = ActionCreator.createAction(ActionTypes.GET_MY_FRIEND_LIST) as GetMyFriendListAction;
        act._type = GetMyFriendListAction.type.FRIEND;
        gameManager.gameDispatcher.dispatch(act);

        var _act = ActionCreator.createAction(ActionTypes.GET_WAITING_FRIEND_ACCEPT_LIST) as GetAcceptWaitingListAction;
        gameManager.gameDispatcher.dispatch(_act);

        var __act = ActionCreator.createAction(ActionTypes.GET_MY_FRIEND_LIST) as GetMyFriendListAction;
        __act._type = GetMyFriendListAction.type.WAITING;
        gameManager.gameDispatcher.dispatch(__act);
    }

    public void search() {
        CommunitySearchAction action = ActionCreator.createAction(ActionTypes.COMMUNITY_SEARCH) as CommunitySearchAction;
        action._type = CommunitySearchAction.searchType.FRIEND;
        action.keyword = input.text;
        gameManager.gameDispatcher.dispatch(action);

        input.text = null;
    }

    public void onNotifyModal(string msg) {
        notifyModal.SetActive(true);
        notifyModal.transform.Find("InnerModal/Text").GetComponent<Text>().text = msg;
    }
}
