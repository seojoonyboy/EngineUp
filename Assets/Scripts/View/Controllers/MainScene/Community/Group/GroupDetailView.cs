using UnityEngine;
using System.Collections;

public class GroupDetailView : MonoBehaviour {
    public GroupViewController controller;
    public UILabel 
        groupName, 
        groupLocation,
        groupDesc,
        memberCount;
    public int id;

    public GameObject
        signupButton,
        showMemberButton,
        quitMemberButton,
        settingButton;

    private GameManager gm;
    // 이벤트 parameter를 생성하여 리턴.

    void OnEnable() {
        gm = GameManager.Instance;
        Group group = controller.groupStore.clickedGroup;
        groupName.text = group.name;
        groupLocation.text = group.locationDistrict + " " + group.locationCity;
        memberCount.text = group.membersCount + " 명";
        groupDesc.text = group.groupIntro;
        id = group.id;

        //그룹원 혹은 그룹장인지 확인
        //그룹원 보기 버튼, 그룹 가입 버튼, 그룹 설정 버튼 활성화 여부를 위함
        Group_checkMyStatus checkMyStatAct = ActionCreator.createAction(ActionTypes.GROUP_CHECK_MY_STATUS) as Group_checkMyStatus;
        checkMyStatAct.id = id;
        checkMyStatAct.userId = GameManager.Instance.userStore.userId;
        gm.gameDispatcher.dispatch(checkMyStatAct);
    }

    public void offPanel() {
        gameObject.SetActive(false);
        signupButton.SetActive(false);
        showMemberButton.SetActive(false);
        quitMemberButton.SetActive(false);
        settingButton.SetActive(false);
    }

    public void onShowMemberButton(GameObject obj) {
        controller.onPanel(obj);
    }

    public void onJoinButton() {
        Group_join groupJoinAct = ActionCreator.createAction(ActionTypes.GROUP_JOIN) as Group_join;
        groupJoinAct.id = id;
        gm.gameDispatcher.dispatch(groupJoinAct);
    }

    public void setViewMode(string type) {
        switch (type) {
            case "OWNER":
                showMemberButton.SetActive(true);
                quitMemberButton.SetActive(true);
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
}
