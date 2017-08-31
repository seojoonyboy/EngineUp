using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FR_SendingsView : MonoBehaviour {
    public GameObject container;
    public FriendsViewController parent;

    public GameManager gameManager;
    public Friends friendsStore;

    //요청 대기중 목록 생성
    public void makeMyFriendList() {
        removeList();
        if (friendsStore.friendReqLists == null || friendsStore.friendReqLists.Count == 0) {
            gameObject.SetActive(false);
            return;
        }
        else {
            gameObject.SetActive(true);
        }

        Friend[] lists = friendsStore.friendReqLists.ToArray(typeof(Friend)) as Friend[];

        for (int i = 0; i < lists.Length; i++) {
            GameObject item = Instantiate(container);
            item.transform.SetParent(transform, false);
            item.GetComponent<ButtonIndex>().index = lists[i].id;
            item.transform.Find("Name").GetComponent<Text>().text = lists[i].toUser.nickName;
            //containerInit(item, myFriendGrid);
            Button cancelBtn = item.transform.Find("CancelButton").GetComponent<Button>();
            cancelBtn.onClick.AddListener(() => cancelReq(item));

            item.GetComponent<FriendIndex>().nickName = lists[i].toUser.nickName;
            item.GetComponent<Button>().onClick.AddListener(() => showProfile(item));
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(parent.content.GetComponent<RectTransform>());
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

    private void cancelReq(GameObject obj) {
        CommunityDeleteAction action = ActionCreator.createAction(ActionTypes.COMMUNITY_DELETE) as CommunityDeleteAction;
        action._type = CommunityDeleteAction.deleteType.FRIEND;
        action._detailType = CommunityDeleteAction.detailType.SENDING;
        action.id = obj.GetComponent<ButtonIndex>().index;
        gameManager.gameDispatcher.dispatch(action);

        string msg = "친구요청을 취소합니다.";
        parent.onNotifyModal(msg);
    }

    //친구 프로필 보기
    private void showProfile(GameObject obj) {
        string nickName = obj.GetComponent<FriendIndex>().nickName;

        GetFriendInfoAction act = ActionCreator.createAction(ActionTypes.GET_FR_INFO) as GetFriendInfoAction;
        act.nickName = nickName;
        gameManager.gameDispatcher.dispatch(act);
    }
}
