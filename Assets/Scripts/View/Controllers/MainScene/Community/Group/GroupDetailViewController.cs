using UnityEngine;
using System.Collections;

public class GroupDetailViewController : MonoBehaviour {
    public GroupViewController controller;

    public GroupMemberManageView memberManage;
    public GroupSettingChangeView settingChange;
    public GroupMemberView memberView;
    public GroupDelView groupDelView;
    public GroupManageMainView manageMainView;

    public UILabel
        groupName,
        groupLocation,
        groupDesc,
        memberCount;
    public int id;

    public GameObject
        signupButton,
        showMemberButton,
        showMemberOwnerButton,
        quitMemberButton,
        settingButton,
        modal,
        groupModal,
        storyPref;

    private GameManager gm;
    private GameObject storyGrid;
    private Groups groupStore;
    // 이벤트 parameter를 생성하여 리턴.
    private bool isFirstGetPosts = true;

    void Start() {
        storyGrid = gameObject.transform.Find("ScrollView/StoryGrid").gameObject;
    }

    public void OnEnable() {
        gm = GameManager.Instance;
    }

    void OnDisable() {
        gameObject.transform.Find("ScrollView/StoryGrid").transform.DestroyChildren();
        isFirstGetPosts = true;
        containerInit();
        gameObject.transform.Find("ScrollView").gameObject.AddComponent<SpringPanel>().target = new Vector3(-700.0f, -2.0f, 0.0f);
    }

    public void refreshTxt() {
        Group group = controller.groupStore.clickedGroup;
        groupName.text = group.name;
        groupLocation.text = group.locationDistrict + " " + group.locationCity;
        memberCount.text = group.membersCount + " 명";
        groupDesc.text = group.groupIntro;
        id = group.id;

        //그룹원 보기 버튼, 그룹 가입 버튼, 그룹 설정 버튼 활성화 여부를 위함
        Group_checkMyStatus checkMyStatAct = ActionCreator.createAction(ActionTypes.GROUP_CHECK_MY_STATUS) as Group_checkMyStatus;
        checkMyStatAct.id = id;
        checkMyStatAct.userId = GameManager.Instance.userStore.userId;
        gm.gameDispatcher.dispatch(checkMyStatAct);
    }

    public void getPosts() {
        //그룹 포스트 불러오기
        Group_posts getPosts = ActionCreator.createAction(ActionTypes.GROUP_POSTS) as Group_posts;
        getPosts.id = id;
        if (isFirstGetPosts) {
            getPosts.isFirst = true;
        }
        gm.gameDispatcher.dispatch(getPosts);
        isFirstGetPosts = false;
    }

    public void offPanel() {
        gameObject.SetActive(false);
        signupButton.SetActive(false);
        showMemberButton.SetActive(false);
        quitMemberButton.SetActive(false);
        settingButton.SetActive(false);
        showMemberOwnerButton.SetActive(false);

        Group_myGroups getMyGroupAct = ActionCreator.createAction(ActionTypes.MY_GROUP_PANEL) as Group_myGroups;
        gm.gameDispatcher.dispatch(getMyGroupAct);
    }

    public void onShowMemberButton(GameObject obj) {
        controller.subPanels[0].SetActive(true);
    }

    //탈퇴버튼 클릭
    public void onModal() {
        modal.SetActive(true);
    }

    //최종적으로 탈퇴 버튼 클릭
    public void onQuitGroupButton() {
        Debug.Log("그룹 탈퇴 버튼 클릭");
        Group_ban groupBanAct = ActionCreator.createAction(ActionTypes.GROUP_BAN) as Group_ban;
        groupBanAct.id = id;
        groupBanAct.memberId = controller.groupStore.myInfoInGroup[0].id;
        gm.gameDispatcher.dispatch(groupBanAct);

        modal.SetActive(false);
        gameObject.SetActive(false);

        showMemberButton.SetActive(false);
        quitMemberButton.SetActive(false);
    }

    //탈퇴 취소
    public void cancelQuitGroup() {
        modal.SetActive(false);
    }

    public void onJoinButton() {
        Group_join groupJoinAct = ActionCreator.createAction(ActionTypes.GROUP_JOIN) as Group_join;
        groupJoinAct.id = id;
        gm.gameDispatcher.dispatch(groupJoinAct);
    }

    public void setViewMode(string type) {
        switch (type) {
            case "OWNER":
                //showMemberButton.SetActive(true);
                showMemberOwnerButton.SetActive(true);
                //quitMemberButton.SetActive(true);
                settingButton.SetActive(true);
                break;
            case "MEMBER":
                showMemberButton.SetActive(true);
                quitMemberButton.SetActive(true);
                signupButton.SetActive(false);
                break;
            case "VISITOR":
                signupButton.SetActive(true);
                break;
        }
    }

    //Group View Controller에게서 리스너 할당 받음.
    public void onGroupStoreListener() {
        //하위 Controller에게 리스너 재할당
        memberManage.onGroupStoreListener();
        settingChange.onGroupStoreListener();
        groupDelView.onGroupStoreListener();
        manageMainView.onGroupStoreListener();

        groupStore = controller.groupStore;
        ActionTypes groupStoreEventType = groupStore.eventType;

        if (groupStoreEventType == ActionTypes.GROUP_DETAIL) {
            if (groupStore.storeStatus == storeStatus.NORMAL) {
                refreshTxt();
                getPosts();
            }
        }

        if (groupStoreEventType == ActionTypes.GROUP_JOIN) {
            if (groupStore.storeStatus == storeStatus.NORMAL) {
                setViewMode("MEMBER");
            }
            else if (groupStore.storeStatus == storeStatus.ERROR) {
                setViewMode("VISITOR");
            }
            groupModal.SetActive(true);
            groupModal.transform.Find("Modal/Label").GetComponent<UILabel>().text = groupStore.message;
        }

        if (groupStoreEventType == ActionTypes.GROUP_GET_MEMBERS) {
            memberView.makeList();
        }

        if (groupStoreEventType == ActionTypes.GROUP_MEMBER_ACCEPT || groupStoreEventType == ActionTypes.GROUP_BAN) {
            //memberCount.text = groupStore.groupMembers.Length + " 명";
            Group_detail refreshAct = ActionCreator.createAction(ActionTypes.GROUP_DETAIL) as Group_detail;
            refreshAct.id = id;
            gm.gameDispatcher.dispatch(refreshAct);
        }

        if (groupStoreEventType == ActionTypes.GROUP_CHECK_MY_STATUS) {
            if (groupStore.isGroupMember) {
                Debug.Log("그룹 멤버임");
                if (groupStore.myInfoInGroup[0].memberGrade == "GO") {
                    Debug.Log("그룹장임");
                    setViewMode("OWNER");
                }
                else if (groupStore.myInfoInGroup[0].memberGrade == "GM") {
                    setViewMode("MEMBER");
                }
            }
            else {
                setViewMode("VISITOR");
                Debug.Log("그룹 멤버가 아님");
            }
        }

        if (groupStoreEventType == ActionTypes.GROUP_POSTS) {
            if(groupStore.storeStatus == storeStatus.NORMAL) {
                makePostLists();
                gameObject.transform.Find("ScrollView").GetComponent<refresh>().flag = true;
            }
            //grid 갱신
            containerInit();
        }
    }

    public void makePostLists() {
        for (int i = 0; i < groupStore.posts.Length; i++) {
            GameObject item = Instantiate(storyPref);
            item.transform.SetParent(storyGrid.transform);

            item.transform.localPosition = Vector3.zero;
            item.transform.localScale = Vector3.one;

            UILabel memberLabel = item.transform.Find("Header/Writer").GetComponent<UILabel>();
            memberLabel.text = groupStore.posts[i].writer.nickName;

            UILabel dateLabel = item.transform.Find("Header/Date").GetComponent<UILabel>();
            dateLabel.text = groupStore.posts[i].writer.createDate;

            UILabel contextLabel = item.transform.Find("Body/Label").GetComponent<UILabel>();
            contextLabel.text = groupStore.posts[i].text;
        }
    }

    private void containerInit() {
        storyGrid.GetComponent<UIGrid>().repositionNow = true;
        storyGrid.GetComponent<UIGrid>().Reposition();
    }
}