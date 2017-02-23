﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GroupMemberManageView : MonoBehaviour {
    public GameObject 
        container,
        memberHeader,
        waitingHeader;

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
        members = new List<Member>();
        makeList();
    }

    //탈퇴 버튼, 거부 버튼, 강퇴 버튼
    void onQuitGroup() {

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
        removeAllList();
        allMembers = controller.groupStore.groupMembers;
        groupId = controller.detailView.id;

        for(int i=0; i<allMembers.Length; i++) {
            if (allMembers[i].memberState == "MB") {
                members.Add(allMembers[i]);
            }
            else if(allMembers[i].memberState == "WT") {
                waitingMembers.Add(allMembers[i]);
                Debug.Log("!!");
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
            item.transform.Find("MemberManageType").gameObject.SetActive(true);
            item.transform.Find("Name_normal_type").GetComponent<UILabel>().text = member.user.nickName;
            item.GetComponent<GroupIndex>().id = member.id;

            item.transform.localPosition = Vector3.zero;
            item.transform.localScale = Vector3.one;
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
}
