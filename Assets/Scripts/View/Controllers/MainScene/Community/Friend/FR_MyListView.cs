using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UIWidgets;

public class FR_MyListView : MonoBehaviour {
    public GameObject container;
    public FriendsViewController parent;

    private GameManager gameManager;
    private Friends friendsStore;

    public MainViewController mV;

    void Awake() {
        gameManager = parent.gameManager;
        friendsStore = parent.friendsStore;
    }

    //내 친구 목록 생성
    public void makeMyFriendList() {
        removeList();
        if (friendsStore.myFriends == null || friendsStore.myFriends.Count == 0) {
            gameObject.SetActive(false);
            parent.subHeaders[1].SetActive(false);
            return;
        }
        else {
            gameObject.SetActive(true);
            parent.subHeaders[1].SetActive(true);
        }

        Friend[] lists = friendsStore.myFriends.ToArray(typeof(Friend)) as Friend[];

        for (int i = 0; i < lists.Length; i++) {
            GameObject item = Instantiate(container);
            item.transform.SetParent(transform, false);

            GameObject innerContainer = item.transform.Find("InnerContainer").gameObject;

            item.GetComponent<ButtonIndex>().index = lists[i].id;
            innerContainer.transform.Find("Name").GetComponent<Text>().text = lists[i].toUser.nickName;

            item.transform.Find("SideMenu").GetComponent<Sidebar>().Content = innerContainer.GetComponent<RectTransform>();
            item.transform.Find("SideMenu").GetComponent<Sidebar>().OptionalHandle = innerContainer;

            //containerInit(item, myFriendGrid);
            Button delBtn = item.transform.Find("SideMenu/DeleteButton").GetComponent<Button>();
            delBtn.onClick.AddListener(() => delFriend(item));

            item.GetComponent<Button>().onClick.AddListener(() => showProfile(item));

            Image rankImg = innerContainer.transform.Find("Rank").GetComponent<Image>();

            int rank = lists[i].toUser.rank;

            int iconRank = (int)Mathf.Ceil((float)rank / 5);

            if (iconRank == 0) {
                rankImg.sprite = mV.ranks[0];
            }
            else {
                rankImg.sprite = mV.ranks[iconRank - 1];
            }
            rankImg.transform.Find("Text").GetComponent<Text>().text = "랭크 " + rank;
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
        act._type = GetFriendInfoAction.type.MYFRIEND;
        gameManager.gameDispatcher.dispatch(act);
    }
}
