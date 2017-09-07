﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UIWidgets;

public class FR_ReceivesView : MonoBehaviour {
    public GameObject container;
    public FriendsViewController parent;

    public GameManager gameManager;
    public Friends friendsStore;
    public MainViewController mV;

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

            GameObject innerContainer = item.transform.Find("InnerContainer").gameObject;

            innerContainer.transform.Find("Name").GetComponent<Text>().text = lists[i].fromUser.nickName;
            //containerInit(item, myFriendGrid);
            Button delBtn = item.transform.Find("DenyButton").GetComponent<Button>();
            delBtn.onClick.AddListener(() => reject(item));

            Button acceptBtn = item.transform.Find("AcceptButton").GetComponent<Button>();
            acceptBtn.onClick.AddListener(() => accept(item));

            item.GetComponent<Button>().onClick.AddListener(() => showProfile(item));

            item.GetComponent<FriendIndex>().nickName = lists[i].fromUser.nickName;

            Image rankImg = innerContainer.transform.Find("Rank").GetComponent<Image>();

            int rank = lists[i].fromUser.rank;

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
        action._detailType = CommunityDeleteAction.detailType.RECEIVE;
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
