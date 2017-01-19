﻿using UnityEngine;
using System.Collections;
using System;

public class GroupViewController : MonoBehaviour {
    public GameObject container;

    private GameObject[] itemArr;
    private UIGrid grid;
    private UIInput input;
    private GameManager gameManager;

    public Groups groupStore;
    public GameObject modal;

    public void OnGroupStoreListener() {
        if (groupStore.eventType == ActionTypes.COMMUNITY_INITIALIZE) {
            makeList();
        }
        if (groupStore.eventType == ActionTypes.COMMUNITY_SEARCH) {
            onSearchFeedbackMsg(groupStore.msg);
        }
    }

    void Start() {
        input = gameObject.transform.Find("FindGroupPanel/Input").GetComponent<UIInput>();
        input.activeTextColor = Color.black;
        gameManager = GameManager.Instance;
    }

    public void onSearchFeedbackMsg(string msg) {
        modal.SetActive(true);
        modal.transform.Find("ResponseModal/MsgLabel").GetComponent<UILabel>().text = msg;
    }

    public void makeList() {
        grid = gameObject.transform.Find("ScrollView/Grid").GetComponent<UIGrid>();
        itemArr = new GameObject[groupStore.myGroups.Length];
        removeAllList();
        for (int i = 0; i < itemArr.Length; i++) {
            itemArr[i] = Instantiate(container);
            itemArr[i].transform.SetParent(grid.transform);
            itemArr[i].transform.localPosition = Vector3.zero;
            itemArr[i].transform.localScale = Vector3.one;
            itemArr[i].transform.Find("LocationLabel").GetComponent<UILabel>().text = groupStore.myGroups[i].location;
            itemArr[i].transform.Find("MemberCountLabel").GetComponent<UILabel>().text = groupStore.myGroups[i].memberNum.ToString();
            itemArr[i].transform.Find("GroupNameLabel").GetComponent<UILabel>().text = groupStore.myGroups[i].name;
            //GameObject tmp = itemArr[i].transform.Find("RemoveBtn").gameObject;
            //tmp.GetComponent<ButtonIndex>().index = i;

            //EventDelegate onClick = new EventDelegate(gameObject.GetComponent<FriendsViewController>(), "delete");

            //EventDelegate.Parameter param = new EventDelegate.Parameter();
            //param.obj = tmp;
            //param.field = "index";
            //onClick.parameters[0] = param;

            //EventDelegate.Add(tmp.GetComponent<UIButton>().onClick, onClick);
        }
        grid.Reposition();
    }

    void removeAllList() {
        //NGUI Extension Method
        //Debug.Log("remove");
        Array.Clear(itemArr, 0, itemArr.Length);
        //if (grid.transform.childCount != 0) {
        //    grid.transform.DestroyChildren();
        //}
        foreach (Transform child in grid.transform) {
            if(child.tag == "NoneRemoveList") {
                continue;
            }
            GameObject.Destroy(child.gameObject);
        }
    }

    public void search() {
        string parm = input.value;
        CommunitySearchAction action = ActionCreator.createAction(ActionTypes.COMMUNITY_SEARCH) as CommunitySearchAction;
        action.type = CommunitySearchAction.searchType.FRIEND;
        action.keyword = parm;
        gameManager.gameDispatcher.dispatch(action);
    }

    public void onSerchResult() {
        removeAllList();
        //foreach(Friend friend in userStore.list) {
        //    GameObject item = Instantiate(container);
        //    item.transform.Find("Name").GetComponent<UILabel>().text = friend.id;
        //    item.transform.SetParent(grid.transform);
        //    item.transform.localPosition = Vector3.zero;
        //    item.transform.localScale = Vector3.one;
        //}
        grid.Reposition();
    }

    public void delete(GameObject obj) {
        int index = obj.GetComponent<ButtonIndex>().index;
        Debug.Log("selected index : " + index);
        Destroy(itemArr[index]);
        grid.repositionNow = true;
        grid.Reposition();
        //DeleteCommunityAction action = ActionCreator.createAction(ActionTypes.DELETE_COMMUNITY_DATA) as DeleteCommunityAction;
        //action.key_id = index;
        //gameManager.gameDispatcher.dispatch(action);
    }

    public void OnCloseButton() {

    }

    public void OffCloseButton() {

    }
}
