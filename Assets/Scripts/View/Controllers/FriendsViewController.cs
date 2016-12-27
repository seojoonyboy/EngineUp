using UnityEngine;
using System.Collections;

public class FriendsViewController : MonoBehaviour {
    public int childNum = 5;
    public GameObject container;
    UIGrid grid;

    void Start() {
        grid = gameObject.transform.Find("ScrollView/Grid").GetComponent<UIGrid>();
        //Debug.Log("Awake");
        for (int i = 0; i < childNum; i++) {
            GameObject item = Instantiate(container);
            item.transform.SetParent(grid.transform);
            item.transform.localPosition = Vector3.zero;
            item.transform.localScale = Vector3.one;
        }
        grid.Reposition();
    }
}
