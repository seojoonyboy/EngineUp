using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FR_MyListView : MonoBehaviour {
    public GameObject container;
    public GameObject sendFriendEmptyLabel;
    public Transform content;
    public FriendsViewController parent;

    private GameManager gameManager;
    private Friends friendsStore;
    
    void Awake() {
        gameManager = parent.gameManager;
        friendsStore = parent.friendsStore;
    }

    void OnEnable() {
        //친구 목록 요청
        var act = ActionCreator.createAction(ActionTypes.GET_MY_FRIEND_LIST) as GetMyFriendListAction;
        act._type = GetMyFriendListAction.type.FRIEND;
        gameManager.gameDispatcher.dispatch(act);
    }

    void OnDisable() {
        sendFriendEmptyLabel.SetActive(false);
    }

    //내 친구 목록 생성
    public void makeMyFriendList() {
        removeList();
        if (friendsStore.myFriends == null || friendsStore.myFriends.Count == 0) {
            sendFriendEmptyLabel.SetActive(true);
            return;
        }

        Friend[] lists = friendsStore.myFriends.ToArray(typeof(Friend)) as Friend[];

        for (int i = 0; i < lists.Length; i++) {
            GameObject item = Instantiate(container);
            item.transform.SetParent(content.transform, false);

            item.GetComponent<ButtonIndex>().index = lists[i].id;
            item.transform.Find("Name").GetComponent<Text>().text = lists[i].toUser.nickName;
            //containerInit(item, myFriendGrid);
            Button delBtn = item.transform.Find("DeleteButton").GetComponent<Button>();
            delBtn.onClick.AddListener(() => delFriend(item));
        }
    }

    private void delFriend(GameObject obj) {
        CommunityDeleteAction action = ActionCreator.createAction(ActionTypes.COMMUNITY_DELETE) as CommunityDeleteAction;
        action._type = CommunityDeleteAction.deleteType.FRIEND;
        action._detailType = CommunityDeleteAction.detailType.MYLIST;
        action.id = obj.GetComponent<ButtonIndex>().index;
        gameManager.gameDispatcher.dispatch(action);
    }

    public void removeList() {
        foreach(Transform item in content) {
            Destroy(item.gameObject);
        }
    }
}
