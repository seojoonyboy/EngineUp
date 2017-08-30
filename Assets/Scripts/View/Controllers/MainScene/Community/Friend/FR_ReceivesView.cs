using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FR_ReceivesView : MonoBehaviour {
    public GameObject container;
    public GameObject sendFriendEmptyLabel;
    public Transform content;
    public FriendsViewController parent;

    public GameManager gameManager;
    public Friends friendsStore;

    void OnEnable() {
        //친구 목록 요청
        var act = ActionCreator.createAction(ActionTypes.GET_WAITING_FRIEND_ACCEPT_LIST);
        gameManager.gameDispatcher.dispatch(act);
    }

    void OnDisable() {
        sendFriendEmptyLabel.SetActive(false);
    }

    //수락 대기 목록 생성
    public void makeMyFriendList() {
        removeList();
        if(friendsStore.waitingAcceptLists == null || friendsStore.waitingAcceptLists.Count == 0) {
            sendFriendEmptyLabel.SetActive(true);
            return;
        }

        Friend[] lists = friendsStore.waitingAcceptLists.ToArray(typeof(Friend)) as Friend[];

        for (int i = 0; i < lists.Length; i++) {
            GameObject item = Instantiate(container);
            item.transform.SetParent(content, false);

            item.GetComponent<ButtonIndex>().index = lists[i].id;
            item.GetComponent<ButtonIndex>().fromUserId = lists[i].fromUser.id;

            item.transform.Find("Name").GetComponent<Text>().text = lists[i].toUser.nickName;
            //containerInit(item, myFriendGrid);
            Button delBtn = item.transform.Find("DenyButton").GetComponent<Button>();
            delBtn.onClick.AddListener(() => reject(item));

            Button acceptBtn = item.transform.Find("AcceptButton").GetComponent<Button>();
            acceptBtn.onClick.AddListener(() => accept(item));
        }
    }

    public void removeList() {
        foreach (Transform item in content) {
            Destroy(item.gameObject);
        }
    }

    private void reject(GameObject obj) {
        CommunityDeleteAction action = ActionCreator.createAction(ActionTypes.COMMUNITY_DELETE) as CommunityDeleteAction;
        action._type = CommunityDeleteAction.deleteType.FRIEND;
        int index = obj.GetComponent<ButtonIndex>().index;
        action.id = index;
        gameManager.gameDispatcher.dispatch(action);
    }

    private void accept(GameObject obj) {
        int index = obj.GetComponent<ButtonIndex>().fromUserId;

        AddFriendAction addFriendAct = ActionCreator.createAction(ActionTypes.ADD_FRIEND) as AddFriendAction;
        addFriendAct.id = index;
        addFriendAct._type = AddFriendAction.friendType.ACCEPT;

        gameManager.gameDispatcher.dispatch(addFriendAct);
    }
}
