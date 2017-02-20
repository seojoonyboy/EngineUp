using UnityEngine;
using System.Collections;

public class GroupSearchView : MonoBehaviour {
    public GroupViewController controller;

    public GameObject 
        container,
        emptyMessage;

    public UIGrid grid;

    void OnEnable() {
        removeAllList();
        Group[] searchedGroups = controller.groupStore.searchedGroups;
        int length = searchedGroups.Length;
        if(length == 0) {
            emptyMessage.SetActive(true);
            return;
        }

        for(int i=0; i<length; i++) {
            GameObject item = Instantiate(container);

            item.transform.SetParent(grid.transform);
            item.transform.localPosition = Vector3.zero;
            item.transform.localScale = Vector3.one;

            item.GetComponent<GroupIndex>().id = searchedGroups[i].id;
            item.transform.Find("LocationLabel").GetComponent<UILabel>().text = searchedGroups[i].locationDistrict + " " + searchedGroups[i].locationCity;
            item.transform.Find("GroupNameLabel").GetComponent<UILabel>().text = searchedGroups[i].name;
            item.transform.Find("MemberCountLabel").GetComponent<UILabel>().text = "멤버 " + searchedGroups[i].membersCount + "명";
        }
        containerInit();
    }

    void OnDisable() {
        emptyMessage.SetActive(false);
    }

    private void containerInit() {
        grid.repositionNow = true;
        grid.Reposition();
    }

    private void removeAllList() {
        grid.transform.DestroyChildren();
    }

    public void OffPanel() {
        gameObject.SetActive(false);
    }
}