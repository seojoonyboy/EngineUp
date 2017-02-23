﻿using Flux;
using UnityEngine;
using System.Collections;

public class Groups : AjwStore {
    public Groups(QueueDispatcher<Actions> _dispatcher) : base(_dispatcher) { }

    NetworkManager networkManager = NetworkManager.Instance;
    NetworkCallbackExtention ncExt = new NetworkCallbackExtention();

    public Group[] 
        myGroups,
        searchedGroups;

    public Group clickedGroup;
    public ActionTypes eventType;
    public int sceneIndex = -1;
    public bool 
        addResult = false,
        isGroupMember = false;

    public Member[] 
        myInfoInGroup,
        groupMembers;

    protected override void _onDispatch(Actions action) {
        switch (action.type) {
            case ActionTypes.GROUP_CHECK_MY_STATUS:
                Group_checkMyStatus getMyStatAct = action as Group_checkMyStatus;
                checkMyStat(getMyStatAct);
                break;
            case ActionTypes.GROUP_GET_MEMBERS:
                Group_getMemberAction getMembersAct = action as Group_getMemberAction;
                getMembers(getMembersAct);
                break;
            case ActionTypes.GROUP_MY_GROUPS:
                Group_myGroups getMyGroupAct = action as Group_myGroups;
                getMyGroups(getMyGroupAct);
                break;
            case ActionTypes.GROUP_DETAIL:
                Group_detail getMemberAct = action as Group_detail;
                getGroupDetail(getMemberAct);
                break;
            case ActionTypes.GROUP_SEARCH:
                Group_search searchAct = action as Group_search;
                searchGroups(searchAct);
                break;
            case ActionTypes.GROUP_ADD:
                Group_AddAction addAct = action as Group_AddAction;
                addGroup(addAct);
                break;
            case ActionTypes.GROUP_ON_PANEL:
                Debug.Log("그룹 하위패널 활성화 액션 발생");
                Group_OnPanel onGroupPanelAct = action as Group_OnPanel;
                int index = onGroupPanelAct.index;
                onPanel(index);
                break;
            case ActionTypes.GROUP_JOIN:
                Group_join groupJoinAct = action as Group_join;
                joinGroup(groupJoinAct);
                break;
            case ActionTypes.GROUP_MEMBER_ACCEPT:
                Group_accept acceptMemberAct = action as Group_accept;
                acceptMember(acceptMemberAct);
                break;
        }
        eventType = action.type;
    }

    //해당 그룹 멤버 목록 가져오기
    private void getMembers(Group_getMemberAction payload) {
        switch (payload.status) {
            case NetworkAction.statusTypes.REQUEST:
                var strBuilder = GameManager.Instance.sb;
                strBuilder.Remove(0, strBuilder.Length);
                strBuilder.Append(networkManager.baseUrl)
                    .Append("groups/")
                    .Append(payload.id)
                    .Append("/members");
                networkManager.request("GET", strBuilder.ToString(), ncExt.networkCallback(dispatcher, payload));
                break;
            case NetworkAction.statusTypes.SUCCESS:
                Debug.Log(payload.response.data);
                groupMembers = JsonHelper.getJsonArray<Member>(payload.response.data);
                Group_OnPanel onGroupPanel = ActionCreator.createAction(ActionTypes.GROUP_ON_PANEL) as Group_OnPanel;
                //그룹원 보기 패널 활성화
                if (payload.forMemberManage) {
                    Debug.Log("그룹원 관리를 위한 그룹원 목록 가져오기");
                    onGroupPanel.index = 5;
                }
                else if (!payload.forMemberManage) {
                    onGroupPanel.index = 0;
                }
                dispatcher.dispatch(onGroupPanel);
                break;
            case NetworkAction.statusTypes.FAIL:
                Debug.Log(payload.response.data);
                break;
        }
    }

    //내 그룹 목록 가져오기
    private void getMyGroups(Group_myGroups payload) {
        switch (payload.status) {
            case NetworkAction.statusTypes.REQUEST:
                var strBuilder = GameManager.Instance.sb;
                strBuilder.Remove(0, strBuilder.Length);
                strBuilder.Append(networkManager.baseUrl)
                    .Append("/groups");
                networkManager.request("GET", strBuilder.ToString(), ncExt.networkCallback(dispatcher, payload));
                break;
            case NetworkAction.statusTypes.SUCCESS:
                Debug.Log(payload.response.data);
                myGroups = JsonHelper.getJsonArray<Group>(payload.response.data);
                _emitChange();
                break;
            case NetworkAction.statusTypes.FAIL:
                Debug.Log(payload.response.data);
                break;
        }
    }

    //그룹 검색 결과 목록 가져오기
    private void searchGroups(Group_search payload) {
        switch (payload.status) {
            case NetworkAction.statusTypes.REQUEST:
                var strBuilder = GameManager.Instance.sb;
                strBuilder.Remove(0, strBuilder.Length);
                strBuilder.Append(networkManager.baseUrl)
                    .Append("groups?name=")
                    .Append(payload.keyword);
                networkManager.request("GET", strBuilder.ToString(), ncExt.networkCallback(dispatcher, payload));
                break;
            case NetworkAction.statusTypes.SUCCESS:
                searchedGroups = JsonHelper.getJsonArray<Group>(payload.response.data);
                Group_OnPanel onGroupPanel = ActionCreator.createAction(ActionTypes.GROUP_ON_PANEL) as Group_OnPanel;
                onGroupPanel.index = 1;
                dispatcher.dispatch(onGroupPanel);
                break;
            case NetworkAction.statusTypes.FAIL:
                Debug.Log(payload.response.data);
                break;
        }
    }

    //그룹 상세보기 정보 목록 가져오기
    private void getGroupDetail(Group_detail payload) {
        switch (payload.status) {
            case NetworkAction.statusTypes.REQUEST:
                var strBuilder = GameManager.Instance.sb;
                strBuilder.Remove(0, strBuilder.Length);
                strBuilder.Append(networkManager.baseUrl)
                    .Append("groups/")
                    .Append(payload.id);
                networkManager.request("GET", strBuilder.ToString(), ncExt.networkCallback(dispatcher, payload));
                break;
            case NetworkAction.statusTypes.SUCCESS:
                Debug.Log(payload.response.data);
                clickedGroup = Group.fromJSON(payload.response.data);
                Group_OnPanel onGroupPanel = ActionCreator.createAction(ActionTypes.GROUP_ON_PANEL) as Group_OnPanel;
                onGroupPanel.index = 7;
                dispatcher.dispatch(onGroupPanel);
                break;
            case NetworkAction.statusTypes.FAIL:
                Debug.Log(payload.response.data);
                break;
        }
    }

    //그룹 내 나의 상태 확인하기
    private void checkMyStat(Group_checkMyStatus payload) {
        switch (payload.status) {
            case NetworkAction.statusTypes.REQUEST:
                var strBuilder = GameManager.Instance.sb;
                strBuilder.Remove(0, strBuilder.Length);
                strBuilder.Append(networkManager.baseUrl)
                    .Append("groups/")
                    .Append(payload.id)
                    .Append("/members?userId=")
                    .Append(payload.userId);
                networkManager.request("GET", strBuilder.ToString(), ncExt.networkCallback(dispatcher, payload));
                break;
            case NetworkAction.statusTypes.SUCCESS:
                Debug.Log("그룹 멤버 확인에 대한 callback : " + payload.response.data);
                myInfoInGroup = JsonHelper.getJsonArray<Member>(payload.response.data);
                if (myInfoInGroup.Length == 0) {
                    isGroupMember = false;
                }
                else {
                    isGroupMember = true;
                }
                _emitChange();
                break;
            case NetworkAction.statusTypes.FAIL:
                Debug.Log(payload.response.data);
                break;
        }
    }

    //그룹 추가하기
    private void addGroup(Group_AddAction payload) {
        switch (payload.status) {
            case NetworkAction.statusTypes.REQUEST:
                var strBuilder = GameManager.Instance.sb;
                strBuilder.Remove(0, strBuilder.Length);
                strBuilder.Append(networkManager.baseUrl)
                    .Append("groups");
                //허용 글자 수 초과시
                if (payload.name.Length >= 25) {
                    payload.status = NetworkAction.statusTypes.FAIL;
                    return;
                }
                WWWForm form = new WWWForm();

                form.AddField("name", payload.name);
                form.AddField("locationDistrict", payload.district);
                form.AddField("locationCity", payload.city);

                networkManager.request("POST", strBuilder.ToString(), form, ncExt.networkCallback(dispatcher, payload));
                break;
            case NetworkAction.statusTypes.SUCCESS:
                Debug.Log(payload.response.data);
                addResult = true;
                _emitChange();
                break;
            case NetworkAction.statusTypes.FAIL:
                Debug.Log(payload.response.data);
                addResult = false;
                _emitChange();
                break;
        }
    }

    //그룹 가입하기
    private void joinGroup(Group_join payload) {
        switch (payload.status) {
            case NetworkAction.statusTypes.REQUEST:
                var strBuilder = GameManager.Instance.sb;
                strBuilder.Remove(0, strBuilder.Length);
                strBuilder.Append(networkManager.baseUrl)
                    .Append("groups/")
                    .Append(payload.id)
                    .Append("/join");
                WWWForm form = new WWWForm();
                form.AddField("tmp", 0);
                Debug.Log("join group url : " + strBuilder.ToString());
                networkManager.request("POST", strBuilder.ToString(), form, ncExt.networkCallback(dispatcher, payload));
                break;
            case NetworkAction.statusTypes.SUCCESS:
                Debug.Log(payload.response.data);
                _emitChange();
                break;
            case NetworkAction.statusTypes.FAIL:
                Debug.Log(payload.response.data);
                break;
        }
    }

    //그룹 가입 승인
    private void acceptMember(Group_accept payload) {
        switch (payload.status) {
            case NetworkAction.statusTypes.REQUEST:
                var strBuilder = GameManager.Instance.sb;
                strBuilder.Remove(0, strBuilder.Length);
                strBuilder.Append(networkManager.baseUrl)
                    .Append("groups/")
                    .Append(payload.id)
                    .Append("/members/")
                    .Append(payload.memberId)
                    .Append("/admission");
                Debug.Log("accept group member url : " + strBuilder.ToString());
                networkManager.request("PUT", strBuilder.ToString(), ncExt.networkCallback(dispatcher, payload));
                break;
            case NetworkAction.statusTypes.SUCCESS:
                Debug.Log(payload.response.data);
                Group_detail getMemberAct = ActionCreator.createAction(ActionTypes.GROUP_GET_MEMBERS) as Group_detail;
                getMemberAct.id = payload.id;
                dispatcher.dispatch(getMemberAct);
                break;
            case NetworkAction.statusTypes.FAIL:
                Debug.Log(payload.response.data);
                break;
        }
    }

    //그룹 강퇴, 그룹 탈퇴
    private void delMember() {

    }

    private void onPanel(int index) {
        sceneIndex = index;
        _emitChange();
    }
}

[System.Serializable]
public class Group {
    public int id;
    public string name;
    public string groupIntro;
    public string locationDistrict;
    public string locationCity;
    public int membersCount;

    public static Group fromJSON(string json) {
        return JsonUtility.FromJson<Group>(json);
    }
}

[System.Serializable]
public class Member {
    public int id;
    public string joinDate;
    public string memberState;
    public string memberGrade;
    public CallbackUser user;

    public static Member fromJSON(string json) {
        return JsonUtility.FromJson<Member>(json);
    }
}

[System.Serializable]
public class CallbackUser {
    public int id;
    public string nickName;
    public int representative;
    public string createDate;
}