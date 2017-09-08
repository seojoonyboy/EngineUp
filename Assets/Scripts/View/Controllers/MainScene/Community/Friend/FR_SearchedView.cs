using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FR_SearchedView : MonoBehaviour {
    public GameObject 
        container,
        content;
    public FriendsViewController parent;

    public GameManager gameManager;
    public Friends friendsStore;
    public GameObject label;
    public MainViewController mV;

    private TweenPosition tP;
    public bool
        isReverse_tp = false,
        isTweening = false;

    void Awake() {
        tP = GetComponent<TweenPosition>();
    }

    void OnEnable() {
        tweenPos();
    }

    void OnDisable() {
        removeList();
        isReverse_tp = false;
    }

    public void tweenPos() {
        if (isTweening) {
            return;
        }
        isTweening = true;

        if (!isReverse_tp) {
            tP.ResetToBeginning();
            tP.PlayForward();
        }
        else {
            //swap
            Vector3 tmp;
            tmp = tP.to;
            tP.to = tP.from;
            tP.from = tmp;

            tP.ResetToBeginning();
            tP.PlayForward();
        }
    }

    public void tPFinished() {
        isTweening = false;

        if (isReverse_tp) {
            gameObject.SetActive(false);

            Vector3 tmp;
            tmp = tP.to;
            tP.to = tP.from;
            tP.from = tmp;
        }
        else {
            Debug.Log("TP FInished");
            isReverse_tp = true;

            if (parent.input.text == null) {
                return;
            }

            CommunitySearchAction action = ActionCreator.createAction(ActionTypes.COMMUNITY_SEARCH) as CommunitySearchAction;
            action._type = CommunitySearchAction.searchType.FRIEND;
            action.keyword = parent.input.text;
            gameManager.gameDispatcher.dispatch(action);

            parent.input.text = null;
        }
    }

    //검색결과 목록 생성
    public void makeList() {
        removeList();
        label.SetActive(false);

        if (friendsStore.searchedFriend == null || friendsStore.searchedFriend.Length == 0) {
            label.SetActive(true);
            return;
        }

        SearchedFriend[] list = friendsStore.searchedFriend;
        for(int i=0; i<list.Length; i++) {
            GameObject item = Instantiate(container);
            item.transform.SetParent(content.transform, false);

            item.GetComponent<ButtonIndex>().index = list[i].id;
            item.transform.Find("Name").GetComponent<Text>().text = list[i].nickName;

            Button AddBtn = item.transform.Find("AddButton").GetComponent<Button>();
            AddBtn.onClick.AddListener(() => add(item));

            Image rankImg = item.transform.Find("Rank").GetComponent<Image>();

            int rank = list[i].status.rank;

            int iconRank = (int)Mathf.Ceil((float)rank / 5);

            if (iconRank == 0) {
                rankImg.sprite = mV.ranks[0];
            }
            else {
                rankImg.sprite = mV.ranks[iconRank - 1];
            }
            rankImg.transform.Find("Text").GetComponent<Text>().text = "랭크 " + rank;
        }
    }

    public void removeList() {
        foreach (Transform item in content.transform) {
            Destroy(item.gameObject);
        }
    }

    private void add(GameObject obj) {
        int index = obj.GetComponent<ButtonIndex>().index;

        AddFriendAction addFriendAct = ActionCreator.createAction(ActionTypes.ADD_FRIEND) as AddFriendAction;
        addFriendAct.id = index;
        addFriendAct._type = AddFriendAction.friendType.SEARCH;
        gameManager.gameDispatcher.dispatch(addFriendAct);

        Destroy(obj);
    }
}
