using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FR_ReceivesView : MonoBehaviour {
    public GameObject container;
    public FriendsViewController parent;

    public GameManager gameManager;
    public Friends friendsStore;

    //수락 대기 목록 생성
    public void makeMyFriendList() {
        removeList();
        if(friendsStore.waitingAcceptLists == null || friendsStore.waitingAcceptLists.Count == 0) {
            gameObject.SetActive(false);
            return;
        }
        else {
            gameObject.SetActive(true);
        }

        Friend[] lists = friendsStore.waitingAcceptLists.ToArray(typeof(Friend)) as Friend[];

        for (int i = 0; i < lists.Length; i++) {
            GameObject item = Instantiate(container);
            item.transform.SetParent(transform, false);

            item.GetComponent<ButtonIndex>().index = lists[i].id;
            item.GetComponent<ButtonIndex>().fromUserId = lists[i].fromUser.id;

            item.transform.Find("Name").GetComponent<Text>().text = lists[i].fromUser.nickName;
            //containerInit(item, myFriendGrid);
            Button delBtn = item.transform.Find("DenyButton").GetComponent<Button>();
            delBtn.onClick.AddListener(() => reject(item));

            Button acceptBtn = item.transform.Find("AcceptButton").GetComponent<Button>();
            acceptBtn.onClick.AddListener(() => accept(item));

            item.GetComponent<Button>().onClick.AddListener(() => showProfile(item));

            item.GetComponent<FriendIndex>().nickName = lists[i].fromUser.nickName;
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(parent.content.GetComponent<RectTransform>());
    }

    public void removeList() {
        foreach (Transform item in transform) {
            if(item.tag == "locked") {
                continue;
            }
            else {
                Destroy(item.gameObject);
            }
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

    //친구 프로필 보기
    private void showProfile(GameObject obj) {
        int id = obj.GetComponent<ButtonIndex>().index;
        GetFriendInfoAction act = ActionCreator.createAction(ActionTypes.GET_FR_INFO) as GetFriendInfoAction;
        act.nickname = obj.GetComponent<FriendIndex>().nickName;
        act._type = GetFriendInfoAction.type.WAITINGACCEPT;
        gameManager.gameDispatcher.dispatch(act);
    }
}
