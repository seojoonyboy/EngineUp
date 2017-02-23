using UnityEngine;
using System.Collections;

public class GroupMemberManageView : MonoBehaviour {
    public GameObject container;
    public GroupViewController controller;
    public UIGrid
        top_grid,
        member_grid,
        req_grid;

    private GameManager gm;

    public Member[] members;
    public int groupId;

    void Start() {
        gm = GameManager.Instance;

    }

    void OnEnable() {
        makeList();
    }

    //탈퇴 버튼, 거부 버튼, 강퇴 버튼
    void onQuitGroup() {

    }

    //승인 버튼
    void onAccept(GameObject obj) {
        int index = obj.transform.parent.GetComponent<GroupIndex>().id;
        Group_accept acceptAct = ActionCreator.createAction(ActionTypes.GROUP_MEMBER_ACCEPT) as Group_accept;
        acceptAct.id = groupId;
        acceptAct.memberId = index;
        gm.gameDispatcher.dispatch(acceptAct);
    }

    public void makeList() {
        removeAllList();
        members = controller.groupStore.groupMembers;
        groupId = controller.detailView.id;
        for (int i = 0; i < members.Length; i++) {
            GameObject item = Instantiate(container);
            if (members[i].memberState == "MB") {
                item.transform.SetParent(member_grid.transform);
                item.transform.Find("MemberManageType").gameObject.SetActive(true);
            }
            else if (members[i].memberState == "WT") {
                item.transform.SetParent(req_grid.transform);
                item.transform.Find("MemberAdmissionType").gameObject.SetActive(true);
            }
            item.transform.localPosition = Vector3.zero;
            item.transform.localScale = Vector3.one;

            item.transform.Find("Name_normal_type").GetComponent<UILabel>().text = members[i].user.nickName;
            item.GetComponent<GroupIndex>().id = members[i].id;
        }
        containerInit();
    }

    private void removeAllList() {
        req_grid.transform.DestroyChildren();
        member_grid.transform.DestroyChildren();
    }

    private void containerInit() {
        req_grid.repositionNow = true;
        member_grid.repositionNow = true;

        req_grid.Reposition();
        member_grid.Reposition();

        top_grid.repositionNow = true;
        top_grid.Reposition();
    }

    public void offPanel() {
        gameObject.SetActive(false);
    }
}
