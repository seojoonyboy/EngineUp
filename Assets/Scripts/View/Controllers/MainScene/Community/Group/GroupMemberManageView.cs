using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GroupMemberManageView : MonoBehaviour {
    public GroupViewController controller;
    public GroupDetailView detailView;
    public MainViewController mV;

    private Animator animator;
    private GameManager gm;

    public GameObject 
        waiting_container,
        member_container,
        header_container,
        content,
        message,
        content_members,
        content_receivings,
        content_members_nullMessage,
        content_receivings_nullMessage;

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
            gameObject.SetActive(false);
        }
    }

    //탈퇴 버튼, 거부 버튼, 강퇴 버튼
    void onQuitGroup(GameObject obj) {
        int index = obj.GetComponent<FriendIndex>().id;
        Group_ban banAct = ActionCreator.createAction(ActionTypes.GROUP_BAN) as Group_ban;
        banAct.id = groupId;
        banAct.memberId = index;
        gm.gameDispatcher.dispatch(banAct);

        Debug.Log("On Quit");
    }

    //승인 버튼
    void onAccept(GameObject obj) {
        int index = obj.GetComponent<FriendIndex>().id;
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

        int memberNum = 0;
        int waitingNum = 0;

        for(int i=0; i<members.Length; i++) {
            if(members[i].memberState == "MB" && members[i].memberGrade == "GM") {
                GameObject item = Instantiate(member_container);
                item.transform.SetParent(content_members.transform, false);

                item.GetComponent<FriendIndex>().id = members[i].id;
                item.transform.Find("InnerContainer/Nickname").GetComponent<Text>().text = members[i].user.nickName;

                int rank = members[i].user.status.rank;
                int iconRank = (int)Mathf.Ceil((float)rank / 5);

                if (iconRank == 0) {
                    item.transform.Find("Rank/Image").GetComponent<Image>().sprite = mV.ranks[0];
                } else {
                    item.transform.Find("Rank/Image").GetComponent<Image>().sprite = mV.ranks[iconRank - 1];
                }

                item.transform.Find("Rank/Text").GetComponent<Text>().text = "랭크 " + rank;

                Button banBtn = item.transform.Find("InnerContainer/BanButton").GetComponent<Button>();
                banBtn.onClick.AddListener(() => onQuitGroup(item));

                memberNum++;
            }

            else if(members[i].memberState == "WT") {
                GameObject item = Instantiate(waiting_container);
                item.transform.SetParent(content_receivings.transform, false);

                item.GetComponent<FriendIndex>().id = members[i].id;
                item.transform.Find("InnerContainer/Name").GetComponent<Text>().text = members[i].user.nickName;
                int rank = members[i].user.status.rank;
                item.transform.Find("InnerContainer/Image/Text").GetComponent<Text>().text = "랭크 " + rank;
                int iconRank = (int)Mathf.Ceil((float)rank / 5);

                if(iconRank == 0) {
                    item.transform.Find("InnerContainer/Image/Rank").GetComponent<Image>().sprite = mV.ranks[0];
                }
                else {
                    item.transform.Find("InnerContainer/Image/Rank").GetComponent<Image>().sprite = mV.ranks[iconRank - 1];
                }

                Button acceptBtn = item.transform.Find("AcceptButton").GetComponent<Button>();
                acceptBtn.onClick.AddListener(() => onAccept(item));

                Button banBtn = item.transform.Find("DenyButton").GetComponent<Button>();
                banBtn.onClick.AddListener(() => onQuitGroup(item));

                waitingNum++;
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(content.GetComponent<RectTransform>());
        }

        if(waitingNum == 0) {
            content_receivings_nullMessage.SetActive(true);
        } else {
            content_receivings_nullMessage.SetActive(false);
        }

        if(memberNum == 0) {
            content_members_nullMessage.SetActive(true);
        } else {
            content_members_nullMessage.SetActive(false);
        }
    }

    private void removeAllList() {
        foreach(Transform child in content_receivings.transform) {
            Destroy(child.gameObject);
        }
        foreach (Transform child in content_members.transform) {
            Destroy(child.gameObject);
        }
    }
}
