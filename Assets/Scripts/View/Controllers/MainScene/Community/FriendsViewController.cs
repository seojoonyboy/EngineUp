﻿using UnityEngine;
using System.Collections;
using System;

public class FriendsViewController : MonoBehaviour {
    public int childNum = 5;
    public GameObject container;

    private GameObject[] itemArr;
    private UIGrid grid;
    private UIInput input;
    private GameManager gameManager;

    private User userStore;

    void Start() {
        input = gameObject.transform.Find("InputBackground/Input").GetComponent<UIInput>();
        gameManager = GameManager.Instance;
    }

    public void makeList() {
        userStore = GameManager.Instance.userStore;
        grid = gameObject.transform.Find("ScrollView/Grid").GetComponent<UIGrid>();
        itemArr = new GameObject[userStore.myFriends.Length];
        removeAllList();
        for (int i = 0; i < itemArr.Length; i++) {
            itemArr[i] = Instantiate(container);
            itemArr[i].transform.SetParent(grid.transform);
            itemArr[i].transform.localPosition = Vector3.zero;
            itemArr[i].transform.localScale = Vector3.one;
            itemArr[i].transform.Find("Name").GetComponent<UILabel>().text = userStore.myFriends[i].id;
            itemArr[i].transform.Find("Portrait/LevelBg/Label").GetComponent<UILabel>().text = userStore.myFriends[i].Level;
            GameObject tmp = itemArr[i].transform.Find("RemoveBtn").gameObject;
            tmp.GetComponent<ButtonIndex>().index = i;

            EventDelegate onClick = new EventDelegate(gameObject.GetComponent<FriendsViewController>(), "delete");

            EventDelegate.Parameter param = new EventDelegate.Parameter();
            param.obj = tmp;
            param.field = "index";
            onClick.parameters[0] = param;

            EventDelegate.Add(tmp.GetComponent<UIButton>().onClick, onClick);
        }
        grid.Reposition();
    }

    void removeAllList() {
        //NGUI Extension Method
        Debug.Log("remove");
        Array.Clear(itemArr, 0, itemArr.Length);
        if (grid.transform.childCount != 0) {
            grid.transform.DestroyChildren();
        }
        //foreach (Transform child in grid.transform) {
        //    GameObject.Destroy(child.gameObject);
        //}
    }

    public void search() {
        string parm = input.value;
        GetCommunityAction action = ActionCreator.createAction(ActionTypes.GET_COMMUNITY_DATA) as GetCommunityAction;
        action.type = GetCommunityAction.requestType.FRIENDS;
        action.keyword = parm;
        gameManager.gameDispatcher.dispatch(action);
    }

    public void onSerchResult() {
        removeAllList();
        foreach(Friend friend in userStore.list) {
            GameObject item = Instantiate(container);
            item.transform.Find("Name").GetComponent<UILabel>().text = friend.id;
            item.transform.SetParent(grid.transform);
            item.transform.localPosition = Vector3.zero;
            item.transform.localScale = Vector3.one;
        }
        grid.Reposition();
    }
    
    public void delete(GameObject obj) {
        int index = obj.GetComponent<ButtonIndex>().index;
        Debug.Log("selected index : " + index);
        Destroy(itemArr[index]);
        grid.repositionNow = true;
        grid.Reposition();
        DeleteCommunityAction action = ActionCreator.createAction(ActionTypes.DELETE_COMMUNITY_DATA) as DeleteCommunityAction;
        action.key_id = index;
        gameManager.gameDispatcher.dispatch(action);
    }

    public void OnCloseButton() {

    }

    public void OffCloseButton() {

    }
}
