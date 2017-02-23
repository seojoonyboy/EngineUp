using UnityEngine;
using System.Collections;

public class GroupMemberView : MonoBehaviour {
    //View Controller로부터 그룹의 id를 할당받는다.
    public int index;
    public GroupViewController controller;

    public UIGrid 
        top_grid,
        owner_grid,
        member_grid;

    public Member[] members;

    public GameObject container;

    void OnEnable() {
        removeAllList();
        members = controller.groupStore.groupMembers;
        makeList();
    }

    public void makeList() {
        for(int i=0; i<members.Length; i++) {
            if(members[i].memberState != "MB") {
                continue;
            }
            else {
                GameObject item = Instantiate(container);
                if (members[i].memberGrade == "GO") {
                    item.transform.SetParent(owner_grid.transform);
                }
                else if(members[i].memberGrade == "GM") {
                    item.transform.SetParent(member_grid.transform);
                }
                item.transform.localPosition = Vector3.zero;
                item.transform.localScale = Vector3.one;

                item.transform.Find("Name_normal_type").GetComponent<UILabel>().text = members[i].user.nickName;
                item.transform.Find("NormalType").gameObject.SetActive(true);
            }
        }
        containerInit();
    }

    public void offPanel() {
        gameObject.SetActive(false);
    }

    private void removeAllList() {
        owner_grid.transform.DestroyChildren();
        member_grid.transform.DestroyChildren();
    }

    private void containerInit() {
        owner_grid.repositionNow = true;
        member_grid.repositionNow = true;

        owner_grid.Reposition();
        member_grid.Reposition();

        top_grid.repositionNow = true;
        top_grid.Reposition();
    }
}
