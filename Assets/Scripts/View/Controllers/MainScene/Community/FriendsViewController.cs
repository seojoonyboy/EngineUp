using UnityEngine;
using System.Collections;
using System;

public class FriendsViewController : MonoBehaviour {
    public int childNum = 5;
    public GameObject container;

    private GameObject[] itemArr;
    private UIGrid grid;
    private UIInput input;
    private GameManager gameManager;

    public Friends friendsStore;
    public GameObject modal;

    public void OnFriendsStoreListener() {
        if(friendsStore.eventType == ActionTypes.COMMUNITY_INITIALIZE) {
            makeList();
        }
        if(friendsStore.eventType == ActionTypes.COMMUNITY_SEARCH) {
            onSearchFeedbackMsg(friendsStore.msg);
        }
    }

    void Start() {
        input = gameObject.transform.Find("FindFriendPanel/Input").GetComponent<UIInput>();
        input.activeTextColor = Color.black;
        gameManager = GameManager.Instance;
    }

    public void makeList() {
        grid = gameObject.transform.Find("ScrollView/Grid").GetComponent<UIGrid>();
        itemArr = new GameObject[friendsStore.myFriends.Length];
        removeAllList();
        for (int i = 0; i < itemArr.Length; i++) {
            itemArr[i] = Instantiate(container);
            itemArr[i].transform.SetParent(grid.transform);
            itemArr[i].transform.localPosition = Vector3.zero;
            itemArr[i].transform.localScale = Vector3.one;
            itemArr[i].transform.Find("Name").GetComponent<UILabel>().text = friendsStore.myFriends[i].id;
            itemArr[i].transform.Find("Portrait/LevelBg/Label").GetComponent<UILabel>().text = friendsStore.myFriends[i].Level;
            GameObject tmp = itemArr[i].transform.Find("RemoveButton").gameObject;
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

    public void onSearchFeedbackMsg(string msg) {
        modal.SetActive(true);
        modal.transform.Find("ResponseModal/MsgLabel").GetComponent<UILabel>().text = msg;
    }

    //if press search btn and find the user, then exec
    public void addFriend() {
        GameObject item = Instantiate(container);
        GameObject additionalMsg = item.transform.Find("AdditionalMsg").gameObject;
        additionalMsg.SetActive(true);
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
