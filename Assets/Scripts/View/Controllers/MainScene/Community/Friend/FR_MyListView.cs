using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FR_MyListView : MonoBehaviour {
    public GameObject container;
    public FriendsViewController parent;

    private GameManager gameManager;
    private Friends friendsStore;
    
    void Awake() {
        gameManager = parent.gameManager;
        friendsStore = parent.friendsStore;
    }

    //내 친구 목록 생성
    public void makeMyFriendList() {
        removeList();
        if (friendsStore.myFriends == null || friendsStore.myFriends.Count == 0) {
            gameObject.SetActive(false);
            return;
        }
        else {
            gameObject.SetActive(true);
        }

        Friend[] lists = friendsStore.myFriends.ToArray(typeof(Friend)) as Friend[];

        for (int i = 0; i < lists.Length; i++) {
            GameObject item = Instantiate(container);
            item.transform.SetParent(transform, false);

            item.GetComponent<ButtonIndex>().index = lists[i].id;
            item.transform.Find("Name").GetComponent<Text>().text = lists[i].toUser.nickName;
            //containerInit(item, myFriendGrid);
            Button delBtn = item.transform.Find("DeleteButton").GetComponent<Button>();
            delBtn.onClick.AddListener(() => delFriend(item));

            item.GetComponent<Button>().onClick.AddListener(() => showProfile(item));
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(parent.content.GetComponent<RectTransform>());
    }

    private void delFriend(GameObject obj) {
        CommunityDeleteAction action = ActionCreator.createAction(ActionTypes.COMMUNITY_DELETE) as CommunityDeleteAction;
        action._type = CommunityDeleteAction.deleteType.FRIEND;
        action._detailType = CommunityDeleteAction.detailType.MYLIST;
        action.id = obj.GetComponent<ButtonIndex>().index;
        gameManager.gameDispatcher.dispatch(action);
    }

    public void removeList() {
        foreach (Transform item in transform) {
            if (item.tag == "locked") {
                continue;
            }
            else {
                Destroy(item.gameObject);
            }
        }
    }

    //친구 프로필 보기
    private void showProfile(GameObject obj) {
        int id = obj.GetComponent<ButtonIndex>().index;
        GetFriendInfoAction act = ActionCreator.createAction(ActionTypes.GET_FR_INFO) as GetFriendInfoAction;
        act.id = id;
        gameManager.gameDispatcher.dispatch(act);
    }
}
