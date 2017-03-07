using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GroupMemberManageView : MonoBehaviour {
    public GameObject 
        container,
        memberHeader,
        waitingHeader,
        groupModal;

    public GroupViewController controller;
    public UIGrid grid;

    private GameManager gm;

    public Member[] allMembers;

    List<Member> members = new List<Member>();
    List<Member> waitingMembers = new List<Member>();

    public int groupId;

    void Start() {
        gm = GameManager.Instance;
    }

    void OnEnable() {
        gm = GameManager.Instance;
        Group_getMemberAction act = ActionCreator.createAction(ActionTypes.GROUP_GET_MEMBERS) as Group_getMemberAction;
        act.id = controller.detailView.id;
        gm.gameDispatcher.dispatch(act);
    }

    //탈퇴 버튼, 거부 버튼, 강퇴 버튼
    void onQuitGroup(GameObject obj) {
        int index = obj.transform.parent.parent.GetComponent<GroupIndex>().id;
        Group_ban banAct = ActionCreator.createAction(ActionTypes.GROUP_BAN) as Group_ban;
        banAct.id = groupId;
        banAct.memberId = index;
        gm.gameDispatcher.dispatch(banAct);
    }

    //승인 버튼
    void onAccept(GameObject obj) {
        int index = obj.transform.parent.parent.GetComponent<GroupIndex>().id;
        Debug.Log("승인버튼 클릭, index : " + index);
        Group_accept acceptAct = ActionCreator.createAction(ActionTypes.GROUP_MEMBER_ACCEPT) as Group_accept;
        acceptAct.id = groupId;
        acceptAct.memberId = index;
        gm.gameDispatcher.dispatch(acceptAct);
    }

    public void makeList() {
        members = new List<Member>();
        removeAllList();
        allMembers = controller.groupStore.groupMembers;
        groupId = controller.detailView.id;

        for(int i=0; i<allMembers.Length; i++) {
            if (allMembers[i].memberState == "MB") {
                members.Add(allMembers[i]);
            }
            else if(allMembers[i].memberState == "WT") {
                waitingMembers.Add(allMembers[i]);
            }
        }

        //멤버 헤더 prefab 생성
        GameObject header = Instantiate(memberHeader);
        header.transform.SetParent(grid.transform);
        header.transform.localPosition = Vector3.zero;
        header.transform.localScale = Vector3.one;

        //멤버 prefab 생성
        foreach (Member member in members) {
            GameObject item = Instantiate(container);
            item.transform.SetParent(grid.transform);
            //그룹장은 멤버 리스트에 포함시키지 않는다.
            if(member.memberGrade == "GO") {
                continue;
            }
            item.transform.Find("MemberManageType").gameObject.SetActive(true);
            item.transform.Find("Name_normal_type").GetComponent<UILabel>().text = member.user.nickName;
            item.GetComponent<GroupIndex>().id = member.id;

            item.transform.localPosition = Vector3.zero;
            item.transform.localScale = Vector3.one;

            EventDelegate delEvent = new EventDelegate(this, "onQuitGroup");
            GameObject onQuitBtn = item.transform.Find("MemberManageType/BanButton").gameObject;
            delEvent.parameters[0] = MakeParameter(onQuitBtn, typeof(GameObject));
            EventDelegate.Add(onQuitBtn.GetComponent<UIButton>().onClick, delEvent);
        }

        //가입 신청자 헤더 prefab 생성
        GameObject header2 = Instantiate(waitingHeader);
        header2.transform.SetParent(grid.transform);
        header2.transform.localPosition = Vector3.zero;
        header2.transform.localScale = Vector3.one;

        //가입 신청자 prefab 생성
        foreach (Member member in waitingMembers) {
            GameObject item = Instantiate(container);
            item.transform.SetParent(grid.transform);
            item.transform.Find("MemberAdmissionType").gameObject.SetActive(true);

            item.transform.Find("Name_normal_type").GetComponent<UILabel>().text = member.user.nickName;
            item.GetComponent<GroupIndex>().id = member.id;

            EventDelegate acceptEvent = new EventDelegate(this, "onAccept");
            GameObject acceptBtn = item.transform.Find("MemberAdmissionType/AcceptButton").gameObject;
            acceptEvent.parameters[0] = MakeParameter(acceptBtn, typeof(GameObject));
            EventDelegate.Add(acceptBtn.GetComponent<UIButton>().onClick, acceptEvent);

            EventDelegate rejectEvent = new EventDelegate(this, "onQuitGroup");
            GameObject rejectBtn = item.transform.Find("MemberAdmissionType/RejectButton").gameObject;
            rejectEvent.parameters[0] = MakeParameter(rejectBtn, typeof(GameObject));
            EventDelegate.Add(rejectBtn.GetComponent<UIButton>().onClick, rejectEvent);

            item.transform.localPosition = Vector3.zero;
            item.transform.localScale = Vector3.one;
        }
        containerInit();
    }

    private void removeAllList() {
        grid.transform.DestroyChildren();
        waitingMembers.Clear();
        members.Clear();
    }

    private void containerInit() {
        grid.repositionNow = true;
        grid.Reposition();
    }

    public void offPanel() {
        gameObject.SetActive(false);
    }

    private EventDelegate.Parameter MakeParameter(Object _value, System.Type _type) {
        EventDelegate.Parameter param = new EventDelegate.Parameter();  // 이벤트 parameter 생성.
        param.obj = _value;   // 이벤트 함수에 전달하고 싶은 값.
        param.expectedType = _type;    // 값의 타입.

        return param;
    }

    //Group Detail View에게서 리스너 할당 받음.
    public void onGroupStoreListener() {
        Groups groupStore = controller.groupStore;
        ActionTypes groupStoreEventType = groupStore.eventType;

        if(groupStoreEventType == ActionTypes.GROUP_GET_MEMBERS) {
            if(groupStore.storeStatus == storeStatus.NORMAL) {
                makeList();
            }
        }

        if(groupStoreEventType == ActionTypes.GROUP_MEMBER_ACCEPT || groupStoreEventType == ActionTypes.GROUP_BAN) {
            groupModal.SetActive(true);
            groupModal.transform.Find("Modal/Label").GetComponent<UILabel>().text = groupStore.message;
        }
    }
}
