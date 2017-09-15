using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GroupMemberManageView : MonoBehaviour {
    public GroupViewController controller;
    public GroupDetailView detailView;

    private Animator animator;
    private GameManager gm;

    public GameObject 
        waiting_container,
        member_container,
        header_container,
        content,
        message;

    public int groupId;

    void Awake() {
        animator = GetComponent<Animator>();
        gm = GameManager.Instance;
    }

    void OnEnable() {
        Invoke("playSlideIn", 0.2f);
    }

    void playSlideIn() {
        animator.Play("SlideIn");
    }

    public void onBackButton() {
        animator.Play("SlideOut");
    }

    public void slideFinished(AnimationEvent animationEvent) {
        int boolParm = animationEvent.intParameter;

        //slider in
        if (boolParm == 1) {
            Group_getMemberAction act = ActionCreator.createAction(ActionTypes.GROUP_GET_MEMBERS) as Group_getMemberAction;
            act.id = controller.detailView.id;
            gm.gameDispatcher.dispatch(act);

            groupId = detailView.id;
        }

        //slider out
        else if (boolParm == 0) {
            message.SetActive(false);
        }
    }

    //탈퇴 버튼, 거부 버튼, 강퇴 버튼
    void onQuitGroup(GameObject obj) {
        int index = obj.GetComponent<GroupIndex>().id;
        Group_ban banAct = ActionCreator.createAction(ActionTypes.GROUP_BAN) as Group_ban;
        banAct.id = groupId;
        banAct.memberId = index;
        gm.gameDispatcher.dispatch(banAct);
    }

    //승인 버튼
    void onAccept(GameObject obj) {
        int index = obj.GetComponent<GroupIndex>().id;
        Group_accept acceptAct = ActionCreator.createAction(ActionTypes.GROUP_MEMBER_ACCEPT) as Group_accept;
        acceptAct.id = groupId;
        acceptAct.memberId = index;
        gm.gameDispatcher.dispatch(acceptAct);
    }

    public void makeList() {
        if (!gameObject.activeSelf) {
            return;
        }

        Member[] members = controller.groupStore.groupMembers;
        removeAllList();

        int meberNum = 0;
        int waitNum = 0;

        for(int i=0; i<members.Length; i++) {
            if(members[i].memberState == "GM") {
                GameObject item = Instantiate(member_container);
                item.transform.SetParent(content.transform, false);

                item.GetComponent<GroupIndex>().id = members[i].id;
                item.transform.Find("InnerContainer/Nickname").GetComponent<Text>().text = members[i].user.nickName;
                item.transform.Find("InnerContainer/Rank/Text").GetComponent<Text>().text = "랭크 : " + members[i].memberGrade;

                Button banBtn = item.transform.Find("InnerContainer/BanButton").GetComponent<Button>();
                banBtn.onClick.AddListener(() => onQuitGroup(item));

                meberNum++;
            }

            else if(members[i].memberState == "WT") {
                GameObject item = Instantiate(waiting_container);
                item.transform.SetParent(content.transform, false);

                item.GetComponent<GroupIndex>().id = members[i].id;
                item.transform.Find("InnerContainer/Nickname").GetComponent<Text>().text = members[i].user.nickName;
                item.transform.Find("InnerContainer/Rank/Text").GetComponent<Text>().text = "랭크 : " + members[i].memberGrade;

                Button acceptBtn = item.transform.Find("AcceptButton").GetComponent<Button>();
                acceptBtn.onClick.AddListener(() => onAccept(item));

                Button banBtn = item.transform.Find("InnerContainer/BanButton").GetComponent<Button>();
                banBtn.onClick.AddListener(() => onQuitGroup(item));

                waitNum++;
            }
        }

        if(waitNum != 0 && meberNum != 0) {
            //내 그룹 멤버 header
            GameObject header = Instantiate(header_container);
            header.transform.SetParent(content.transform, false);
            header.transform.SetAsFirstSibling();

            //수락 대기 header
            header = Instantiate(header_container);
            header.transform.SetParent(content.transform, false);
            header.transform.SetSiblingIndex(meberNum);
        }
        else {
            message.SetActive(true);
        }
    }

    private void removeAllList() {
        foreach(Transform child in content.transform) {
            Destroy(child.gameObject);
        }
    }
}
