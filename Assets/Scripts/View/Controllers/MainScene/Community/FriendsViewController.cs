using UnityEngine;
using System.Collections;

public class FriendsViewController : MonoBehaviour {
    public int childNum = 5;
    public GameObject container;
    UIGrid grid;
    UIInput input;

    private User userStore;

    void Start() {
        input = gameObject.transform.Find("InputBackground/Input").GetComponent<UIInput>();
    }

    public void makeList() {
        userStore = GameManager.Instance.userStore;
        grid = gameObject.transform.Find("ScrollView/Grid").GetComponent<UIGrid>();
        removeList();
        for (int i = 0; i < childNum; i++) {
            GameObject item = Instantiate(container);
            item.transform.SetParent(grid.transform);
            item.transform.localPosition = Vector3.zero;
            item.transform.localScale = Vector3.one;
        }
        grid.Reposition();
    }

    void removeList() {
        //NGUI Extension Method
        if(grid.transform.childCount != 0) {
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
        GameManager.Instance.gameDispatcher.dispatch(action);
    }
}
