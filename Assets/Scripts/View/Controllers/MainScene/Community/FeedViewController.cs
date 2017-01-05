using UnityEngine;
using System.Collections;

public class FeedViewController : MonoBehaviour {
    public int childNum = 5;
    public GameObject container;
    UIGrid grid;
    private User userStore;

    public void makeList() {
        Debug.Log("Make List");
        userStore = GameManager.Instance.userStore;
        grid = gameObject.transform.Find("ScrollView/Grid").GetComponent<UIGrid>();
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
