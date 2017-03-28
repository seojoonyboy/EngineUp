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
        storyAddButton,
        quitMemberButton,
        settingButton,
        modal,
        communityModal,
        groupModal,
        storyPref,
        storyModal,
        storyDelModal;

    private GameManager gm;

    private UIGrid 
        storyGrid,
        storyAddGrid;

    private UIInput addStoryInput;
    private Groups groupStore;
    // 이벤트 parameter를 생성하여 리턴.
    private bool isFirstGetPosts = true;

    void Start() {
        GameObject tmp = gameObject.transform.Find("ScrollView").gameObject;
        storyGrid = tmp.transform.Find("StoryGrid").gameObject.GetComponent<UIGrid>();
        tmp = tmp.transform.Find("StoryAddGrid").gameObject;
        storyAddGrid = tmp.GetComponent<UIGrid>();
        tmp = gameObject.transform.Find("AddStoryModal").gameObject;
        addStoryInput = tmp.transform.Find("Modal/InputField").GetComponent<UIInput>();
    }

    public void OnEnable() {
        gm = GameManager.Instance;
    }

    void OnDisable() {
        storyGrid.transform.DestroyChildren();
        foreach(Transform obj in storyAddGrid.transform) {
            if(obj.tag == "AddBtn") {
                continue;
            }
            else {
                Destroy(obj.gameObject);
            }
        }
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
        Debug.Log("포스트 요청");
    }

    public void offPanel() {
        gameObject.SetActive(false);
        signupButton.SetActive(false);
        showMemberButton.SetActive(false);
        quitMemberButton.SetActive(false);
        settingButton.SetActive(false);
        showMemberOwnerButton.SetActive(false);
        storyAddButton.SetActive(false);

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

    //그룹 가입하기 버튼 클릭
    public void onJoinButton() {
        Group_join groupJoinAct = ActionCreator.createAction(ActionTypes.GROUP_JOIN) as Group_join;
        groupJoinAct.id = id;
        gm.gameDispatcher.dispatch(groupJoinAct);
    }

    //그룹이야기 추가하기 버튼 클릭
    //그룹원인 경우(그룹장 포함)만 추가 가능
    public void onStoryAddModal() {
        storyModal.SetActive(true);
    }

    //그룹 이야기 삭제 버튼 클릭
    public void onStoryDelModal() {
        storyDelModal.SetActive(true);
    }

    public void offStoryDelModal() {
        storyDelModal.SetActive(false);
    }

    //최종적으로 스토리 추가시
    public void onAddStoryButton() {
        string value = addStoryInput.value;
        int index = id;

        Group_addPosts addAct = ActionCreator.createAction(ActionTypes.GROUP_ADD_POST) as Group_addPosts;
        addAct.id = index;
        addAct.context = value;
        gm.gameDispatcher.dispatch(addAct);
    }

    //Server 요청 성공시 storyAddGrid에 추가
    //grid 재정렬
    public void addStoryCallback() {
        GameObject newItem = Instantiate(storyPref);
        newItem.transform.SetParent(storyAddGrid.transform);

        newItem.transform.localPosition = Vector3.zero;
        newItem.transform.localScale = Vector3.one;

        Posts post = groupStore.callbackPost;
        newItem.transform.Find("Header/Writer").GetComponent<UILabel>().text = post.writer.nickName;
        //Date 형식 변환 필요
        newItem.transform.Find("Header/Date").GetComponent<UILabel>().text = post.createDate;
        newItem.transform.Find("Body/Label").GetComponent<UILabel>().text = post.text;
        newItem.GetComponent<GroupIndex>().id = post.id;

        containerInit();
    }

    public void offStoryModal() {
        storyModal.SetActive(false);
    }

    public void setViewMode(string type) {
        switch (type) {
            case "OWNER":
                //showMemberButton.SetActive(true);
                showMemberOwnerButton.SetActive(true);
                //quitMemberButton.SetActive(true);
                settingButton.SetActive(true);
                storyAddButton.SetActive(true);
                break;
            case "MEMBER":
                showMemberButton.SetActive(true);
                quitMemberButton.SetActive(true);
                signupButton.SetActive(false);
                storyAddButton.SetActive(true);
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
                //gameObject.transform.Find("ScrollView").GetComponent<refresh>().flag = true;
            }
            //grid 갱신
            containerInit();
        }

        if(groupStoreEventType == ActionTypes.GROUP_ADD_POST) {
            if(groupStore.storeStatus == storeStatus.NORMAL) {
                addStoryCallback();
            }
            offStoryModal();
            communityModal.SetActive(true);
            communityModal.transform.Find("Modal/Label").GetComponent<UILabel>().text = groupStore.message;
        }

        if(groupStoreEventType == ActionTypes.GROUP_DEL_POST) {
            if(groupStore.storeStatus == storeStatus.NORMAL) {
                Destroy(groupStore.target);
                containerInit();
            }
            communityModal.SetActive(true);
            communityModal.transform.Find("Modal/Label").GetComponent<UILabel>().text = groupStore.message;
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

            item.GetComponent<GroupIndex>().id = groupStore.posts[i].id;

            GameObject tmp = item.transform.Find("Header/ShowButton").gameObject;

            EventDelegate.Parameter ContainerParam = new EventDelegate.Parameter();

            ContainerParam.obj = tmp;
            EventDelegate buttonContainerEvent = new EventDelegate(this, "postButtonContainer");
            buttonContainerEvent.parameters[0] = ContainerParam;
            EventDelegate.Add(tmp.GetComponent<UIButton>().onClick, buttonContainerEvent);

            EventDelegate.Parameter param = new EventDelegate.Parameter();
            EventDelegate delEvent = new EventDelegate(this, "delPost");
            param.obj = item;
            delEvent.parameters[0] = param;
            tmp = item.transform.Find("ButtonContainer/DelButton").gameObject;
            EventDelegate.Add(tmp.GetComponent<UIButton>().onClick, delEvent);

            EventDelegate modifyEvent = new EventDelegate(this, "modifyPost");
            modifyEvent.parameters[0] = param;
            tmp = item.transform.Find("ButtonContainer/ModifyButton").gameObject;
            EventDelegate.Add(tmp.GetComponent<UIButton>().onClick, modifyEvent);
        }
    }

    private void containerInit() {
        storyAddGrid.repositionNow = true;
        storyAddGrid.Reposition();

        storyGrid.repositionNow = true;
        storyGrid.Reposition();
    }

    private void postButtonContainer(GameObject obj) {
        Debug.Log(obj.name);
        bool isOn = obj.GetComponent<boolIndex>().isOn;

        obj.transform.parent.parent.Find("ButtonContainer").gameObject.SetActive(!isOn);
        obj.GetComponent<boolIndex>().isOn = !isOn;
    }

    private void delPost(GameObject obj) {
        //Destroy(obj);
        //containerInit();

        int index = obj.GetComponent<GroupIndex>().id;
        Group_delPost delAct = ActionCreator.createAction(ActionTypes.GROUP_DEL_POST) as Group_delPost;
        delAct.id = id;
        delAct.postId = index;
        delAct.target = obj;

        gm.gameDispatcher.dispatch(delAct);
    }

    private void modifyPost(GameObject obj) {
        storyModal.SetActive(true);
        int index = obj.GetComponent<GroupIndex>().id;
        addStoryInput.value = groupStore.posts[index].text;
    }
}