using UnityEngine;
using System.Collections;

public class FriendsViewController : MonoBehaviour {
    public int childNum = 5;
    public GameObject container;
    UIGrid grid;
    private User userStore;

    void Start() {
        Debug.Log("Start");
        userStore = GameManager.Instance.userStore;
        grid = gameObject.transform.Find("ScrollView/Grid").GetComponent<UIGrid>();
        userStore.addListener(userListener);

        GetCommunityAction act = (GetCommunityAction)ActionCreator.createAction(ActionTypes.GET_COMMUNITY_DATA);
        GameManager.Instance.gameDispatcher.dispatch(act);
    }

    void userListener() {
        Debug.Log("Listen");
    }

    public void makeList() {
        Debug.Log("Make List");
        for (int i = 0; i < childNum; i++) {
            GameObject item = Instantiate(container);
            item.transform.SetParent(grid.transform);
            item.transform.localPosition = Vector3.zero;
            item.transform.localScale = Vector3.one;
        }
        grid.Reposition();
    }

    void removeList() {
        foreach (Transform child in grid.transform) {
            GameObject.Destroy(child.gameObject);
        }
    }
}
