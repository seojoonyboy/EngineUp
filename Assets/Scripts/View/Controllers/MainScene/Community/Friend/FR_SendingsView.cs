using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FR_SendingsView : MonoBehaviour {
    public GameObject container;
    public GameObject sendFriendEmptyLabel;
    public GameObject content;
    public FriendsViewController parent;

    public GameManager gameManager;
    public Friends friendsStore;

    void OnEnable() {
        //요청 목록
        var act = ActionCreator.createAction(ActionTypes.GET_MY_FRIEND_LIST) as GetMyFriendListAction;
        act._type = GetMyFriendListAction.type.WAITING;
        gameManager.gameDispatcher.dispatch(act);
    }

    void OnDisable() {
        sendFriendEmptyLabel.SetActive(false);
    }

    //요청 대기중 목록 생성
    public void makeMyFriendList() {
        removeList();
        if (friendsStore.friendReqLists == null || friendsStore.friendReqLists.Count == 0) {
            sendFriendEmptyLabel.SetActive(true);
            return;
        }

        Friend[] lists = friendsStore.friendReqLists.ToArray(typeof(Friend)) as Friend[];

        for (int i = 0; i < lists.Length; i++) {
            GameObject item = Instantiate(container);
            item.transform.SetParent(content.transform, false);
            item.GetComponent<ButtonIndex>().index = lists[i].id;
            item.transform.Find("Name").GetComponent<Text>().text = lists[i].toUser.nickName;
            //containerInit(item, myFriendGrid);
            Button cancelBtn = item.transform.Find("CancelButton").GetComponent<Button>();
            cancelBtn.onClick.AddListener(() => cancelReq(item));
        }
    }

    public void removeList() {
        foreach (Transform item in content.transform) {
            Destroy(item.gameObject);
        }
    }

    private void cancelReq(GameObject obj) {
        CommunityDeleteAction action = ActionCreator.createAction(ActionTypes.COMMUNITY_DELETE) as CommunityDeleteAction;
        action._type = CommunityDeleteAction.deleteType.FRIEND;
        action._detailType = CommunityDeleteAction.detailType.SENDING;
        action.id = obj.GetComponent<ButtonIndex>().index;
        gameManager.gameDispatcher.dispatch(action);
    }
}
