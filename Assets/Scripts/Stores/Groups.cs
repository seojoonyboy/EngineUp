using Flux;
using UnityEngine;
using System;
using System.Collections;
using System.Text;

public class Groups : AjwStore {
    //store status
    public storeStatus storeStatus = storeStatus.NORMAL;
    //store message
    public string message;

    public Groups(QueueDispatcher<Actions> _dispatcher) : base(_dispatcher) { }

    NetworkManager networkManager = NetworkManager.Instance;
    NetworkCallbackExtention ncExt = new NetworkCallbackExtention();

    public Group[] 
        myGroups,
        searchedGroups;

    public Posts[] posts;
    public Posts callbackPost;

    public Group clickedGroup;
    public ActionTypes eventType;
    public int sceneIndex = -1;
    public bool isGroupMember = false;

    public Member[] 
        myInfoInGroup,
        groupMembers;

    private int tmp_groupIndex;
    public string groupAddCallbackMsg;
    public string groupEditCallbackMsg;

    //UTF8Encoding utf8 = new UTF8Encoding();
    public string postsCallbackHeader;
    public GameObject target;
    protected override void _onDispatch(Actions action) {
        switch (action.type) {
            //내 그룹 패널 활성화 액션
            case ActionTypes.MY_GROUP_PANEL:
                Group_myGroups getMyGroupAct = action as Group_myGroups;
                getMyGroups(getMyGroupAct);
                break;
            case ActionTypes.GROUP_ADD:
                Group_AddAction addAct = action as Group_AddAction;
                addGroup(addAct);
                break;
            case ActionTypes.GROUP_DETAIL:
                Group_detail getMemberAct = action as Group_detail;
                getGroupDetail(getMemberAct);
                break;

            case ActionTypes.GROUP_CHECK_MY_STATUS:
                Group_checkMyStatus getMyStatAct = action as Group_checkMyStatus;
                checkMyStat(getMyStatAct);
                break;
            case ActionTypes.GROUP_GET_MEMBERS:
                Group_getMemberAction getMembersAct = action as Group_getMemberAction;
                getMembers(getMembersAct);
                break;
            case ActionTypes.GROUP_SEARCH:
                Group_search searchAct = action as Group_search;
                searchGroups(searchAct);
                break;
            case ActionTypes.GROUP_JOIN:
                Group_join groupJoinAct = action as Group_join;
                joinGroup(groupJoinAct);
                break;
            case ActionTypes.GROUP_MEMBER_ACCEPT:
                Group_accept acceptMemberAct = action as Group_accept;
                acceptMember(acceptMemberAct);
                break;
            case ActionTypes.GROUP_BAN:
                Group_ban banAct = action as Group_ban;
                delMember(banAct);
                break;
            case ActionTypes.GROUP_EDIT:
                Group_AddAction editAct = action as Group_AddAction;
                modifyGroupInfo(editAct);
                break;
            case ActionTypes.GROUP_DESTROY:
                Group_del groupDelAct = action as Group_del;
                delGroup(groupDelAct);
                break;
            case ActionTypes.GROUP_POSTS:
                Group_posts getPostsAct = action as Group_posts;
                getGroupPosts(getPostsAct);
                break;
            case ActionTypes.GROUP_ADD_POST:
                Group_addPosts addPostsAct = action as Group_addPosts;
                addPosts(addPostsAct);
                break;
            case ActionTypes.GROUP_DEL_POST:
                Group_delPost delPostAct = action as Group_delPost;
                delPost(delPostAct);
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
                storeStatus = storeStatus.NORMAL;

                Debug.Log(payload.response.data);
                groupMembers = JsonHelper.getJsonArray<Member>(payload.response.data);
                _emitChange();
                break;
            case NetworkAction.statusTypes.FAIL:
                storeStatus = storeStatus.ERROR;

                Debug.Log(payload.response.data);
                _emitChange();
                break;
        }
    }

    //내 그룹 목록 가져오기
    private void getMyGroups(Group_myGroups payload) {
        switch (payload.status) {
            case NetworkAction.statusTypes.REQUEST:
                storeStatus = storeStatus.WAITING_REQ;
                setMessage(1);

                var strBuilder = GameManager.Instance.sb;
                strBuilder.Remove(0, strBuilder.Length);
                strBuilder.Append(networkManager.baseUrl)
                    .Append("groups");
                networkManager.request("GET", strBuilder.ToString(), ncExt.networkCallback(dispatcher, payload));
                break;
            case NetworkAction.statusTypes.SUCCESS:
                storeStatus = storeStatus.NORMAL;
                Debug.Log(payload.response.data);
                myGroups = JsonHelper.getJsonArray<Group>(payload.response.data);
                
                break;
            case NetworkAction.statusTypes.FAIL:
                storeStatus = storeStatus.ERROR;
                setMessage(3);

                Debug.Log(payload.response.data);
                break;
        }
        _emitChange();
    }

    //그룹 검색 결과 목록 가져오기
    private void searchGroups(Group_search payload) {
        switch (payload.status) {
            case NetworkAction.statusTypes.REQUEST:
                storeStatus = storeStatus.WAITING_REQ;

                //byte[] contents = utf8.GetBytes(payload.keyword);
                var strBuilder = GameManager.Instance.sb;
                strBuilder.Remove(0, strBuilder.Length);
                strBuilder.Append(networkManager.baseUrl)
                    .Append("groups?name=")
                    .Append(WWW.EscapeURL(payload.keyword, Encoding.UTF8));
                networkManager.request("GET", strBuilder.ToString(), ncExt.networkCallback(dispatcher, payload));
                Debug.Log("그룹 검색 url : " + strBuilder.ToString());
                break;
            case NetworkAction.statusTypes.SUCCESS:
                storeStatus = storeStatus.NORMAL;

                searchedGroups = JsonHelper.getJsonArray<Group>(payload.response.data);
                _emitChange();
                break;
            case NetworkAction.statusTypes.FAIL:
                storeStatus = storeStatus.ERROR;

                Debug.Log(payload.response.data);
                _emitChange();
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
                storeStatus = storeStatus.NORMAL;

                Debug.Log(payload.response.data);
                clickedGroup = Group.fromJSON(payload.response.data);
                _emitChange();
                break;
            case NetworkAction.statusTypes.FAIL:
                storeStatus = storeStatus.ERROR;

                Debug.Log(payload.response.data);
                _emitChange();
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
                Debug.Log("그룹 내 나의상태 확인 url : " + strBuilder);
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
                storeStatus = storeStatus.WAITING_REQ;
                setMessage(1);

                var strBuilder = GameManager.Instance.sb;
                strBuilder.Remove(0, strBuilder.Length);
                strBuilder.Append(networkManager.baseUrl)
                    .Append("groups");
                WWWForm form = new WWWForm();
                Debug.Log("name : " + payload.name);
                Debug.Log("locationDistrict : " + payload.district);
                Debug.Log("locationCity : " + payload.city);

                form.AddField("name", payload.name);
                form.AddField("locationDistrict", payload.district);
                form.AddField("locationCity", payload.city);

                if(payload.desc != null) {
                    form.AddField("groupIntro", payload.desc);
                }

                networkManager.request("POST", strBuilder.ToString(), form, ncExt.networkCallback(dispatcher, payload));
                break;
            case NetworkAction.statusTypes.SUCCESS:
                storeStatus = storeStatus.NORMAL;
                message = "그룹 추가에 성공하였습니다.";
                //Debug.Log(payload.response.data);
                //_emitChange();

                Group_myGroups myGroupAct = ActionCreator.createAction(ActionTypes.MY_GROUP_PANEL) as Group_myGroups;
                dispatcher.dispatch(myGroupAct);

                break;
            case NetworkAction.statusTypes.FAIL:
                storeStatus = storeStatus.ERROR;
                Debug.Log(payload.response.data);
                GroupAddError addErrorCallback = GroupAddError.fromJSON(payload.response.data);
                //그룹명 입력 오류
                if (addErrorCallback.name != null) {
                    if(addErrorCallback.locationDistrict != null) {
                        message = "그룹명과 지역명을 입력해주세요.";
                    }
                    else {
                        if (addErrorCallback.name[0].Contains("25")) {
                            message = "그룹명은 최대 25까지 허용됩니다.";
                        }
                        else {
                            message = "그룹명을 입력해주세요.";
                        }
                    }
                }

                else {
                    if (addErrorCallback.locationDistrict != null) {
                        message = "지역명을 입력해주세요.";
                    }
                }
                _emitChange();
                break;
        }
    }

    //그룹 정보 수정
    private void modifyGroupInfo(Group_AddAction payload) {
        switch (payload.status) {
            case NetworkAction.statusTypes.REQUEST:
                storeStatus = storeStatus.WAITING_REQ;

                var strBuilder = GameManager.Instance.sb;
                strBuilder.Remove(0, strBuilder.Length);
                strBuilder.Append(networkManager.baseUrl)
                    .Append("groups/")
                    .Append(payload.id);

                WWWForm form = new WWWForm();

                form.AddField("groupIntro", payload.desc);
                form.AddField("locationDistrict", payload.district);
                form.AddField("locationCity", payload.city);
                form.AddField("name", payload.name);

                networkManager.request("PUT", strBuilder.ToString(), form, ncExt.networkCallback(dispatcher, payload));
                tmp_groupIndex = payload.id;
                break;
            case NetworkAction.statusTypes.SUCCESS:
                storeStatus = storeStatus.NORMAL;
                message = "그룹 정보 수정에 성공하였습니다.";

                Debug.Log(payload.response.data);
                _emitChange();
                break;
            case NetworkAction.statusTypes.FAIL:
                storeStatus = storeStatus.ERROR;
                GroupAddError addErrorCallback = GroupAddError.fromJSON(payload.response.data);
                if(addErrorCallback.groupIntro != null) {
                    if (addErrorCallback.groupIntro[0].Contains("200")) {
                        Debug.Log("!!");
                        message = "그룹 소개글은 최대 200자까지 지원됩니다.";
                        _emitChange();
                    }
                }
                else {
                    message = "알 수 없는 오류입니다.";
                }
                Debug.Log(payload.response.data);
                break;
        }
    }

    //그룹 가입하기
    private void joinGroup(Group_join payload) {
        switch (payload.status) {
            case NetworkAction.statusTypes.REQUEST:
                storeStatus = storeStatus.WAITING_REQ;

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
                storeStatus = storeStatus.NORMAL;
                message = "그룹 가입 신청을 완료하였습니다.";
                Debug.Log(payload.response.data);
                _emitChange();
                break;
            case NetworkAction.statusTypes.FAIL:
                storeStatus = storeStatus.ERROR;
                message = "그룹 신청간에 문제가 발생하였습니다.";
                Debug.Log(payload.response.data);
                _emitChange();
                break;
        }
    }

    //그룹 가입 승인
    private void acceptMember(Group_accept payload) {
        switch (payload.status) {
            case NetworkAction.statusTypes.REQUEST:
                storeStatus = storeStatus.WAITING_REQ;

                var strBuilder = GameManager.Instance.sb;
                strBuilder.Remove(0, strBuilder.Length);
                strBuilder.Append(networkManager.baseUrl)
                    .Append("groups/")
                    .Append(payload.id)
                    .Append("/members/")
                    .Append(payload.memberId)
                    .Append("/admission");
                tmp_groupIndex = payload.id;
                Debug.Log("accept group member url : " + strBuilder.ToString());
                WWWForm form = new WWWForm();
                form.AddField("tmp", 0);
                networkManager.request("PUT", strBuilder.ToString(), form, ncExt.networkCallback(dispatcher, payload));
                break;
            case NetworkAction.statusTypes.SUCCESS:
                storeStatus = storeStatus.NORMAL;
                message = "그룹 요청을 수락하였습니다.";
                Group_getMemberAction act = ActionCreator.createAction(ActionTypes.GROUP_GET_MEMBERS) as Group_getMemberAction;
                act.id = tmp_groupIndex;
                getMembers(act);
                _emitChange();
                break;
            case NetworkAction.statusTypes.FAIL:
                storeStatus = storeStatus.ERROR;
                message = "그룹 요청 수락에 실패하였습니다.";
                Debug.Log(payload.response.data);
                _emitChange();
                break;
        }
    }

    //그룹 강퇴, 그룹 탈퇴, 요청 거부
    private void delMember(Group_ban payload) {
        switch (payload.status) {
            case NetworkAction.statusTypes.REQUEST:
                var strBuilder = GameManager.Instance.sb;
                strBuilder.Remove(0, strBuilder.Length);
                strBuilder.Append(networkManager.baseUrl)
                    .Append("groups/")
                    .Append(payload.id)
                    .Append("/members/")
                    .Append(payload.memberId);
                Debug.Log("delete group member url : " + strBuilder.ToString());
                tmp_groupIndex = payload.id;
                networkManager.request("DELETE", strBuilder.ToString(), ncExt.networkCallback(dispatcher, payload));
                break;
            case NetworkAction.statusTypes.SUCCESS:
                storeStatus = storeStatus.NORMAL;
                message = "그룹원을 제거(거부)하였습니다.";
                Group_getMemberAction act = ActionCreator.createAction(ActionTypes.GROUP_GET_MEMBERS) as Group_getMemberAction;
                act.id = tmp_groupIndex;
                getMembers(act);
                _emitChange();
                break;
            case NetworkAction.statusTypes.FAIL:
                storeStatus = storeStatus.ERROR;
                message = "제거(거부)간에 문제가 발생하였습니다.";
                Debug.Log(payload.response.data);
                _emitChange();
                break;
        }
    }

    //그룹 삭제
    private void delGroup(Group_del payload) {
        switch (payload.status) {
            case NetworkAction.statusTypes.REQUEST:
                storeStatus = storeStatus.WAITING_REQ;

                var strBuilder = GameManager.Instance.sb;
                strBuilder.Remove(0, strBuilder.Length);
                strBuilder.Append(networkManager.baseUrl)
                    .Append("groups/")
                    .Append(payload.id);
                networkManager.request("DELETE", strBuilder.ToString(), ncExt.networkCallback(dispatcher, payload));
                break;
            case NetworkAction.statusTypes.SUCCESS:
                storeStatus = storeStatus.NORMAL;
                message = "그룹이 해체되었습니다.";

                Group_myGroups myGroupAct = ActionCreator.createAction(ActionTypes.MY_GROUP_PANEL) as Group_myGroups;
                dispatcher.dispatch(myGroupAct);

                _emitChange();
                break;
            case NetworkAction.statusTypes.FAIL:
                storeStatus = storeStatus.ERROR;
                setMessage(3);

                _emitChange();
                Debug.Log(payload.response.data);
                break;
        }
    }

    //그룹 이야기 가져오기
    private void getGroupPosts(Group_posts payload) {
        switch (payload.status)
        {
            case NetworkAction.statusTypes.REQUEST:
                storeStatus = storeStatus.WAITING_REQ;
                var strBuilder = GameManager.Instance.sb;
                strBuilder.Remove(0, strBuilder.Length);

                strBuilder.Append(networkManager.baseUrl)
                    .Append("groups/")
                    .Append(payload.id)
                    .Append("/posts");

                if (!payload.isFirst) {
                    //다음 페이지 로드
                    if (postsCallbackHeader.Contains("next"))
                    {
                        int startIndex = postsCallbackHeader.IndexOf('?');
                        int endIndex = postsCallbackHeader.IndexOf('>');
                        string str = postsCallbackHeader.Substring(startIndex, endIndex - startIndex);
                        strBuilder.Append(str);
                    }
                    //다음 페이지가 더이상 없는 경우
                    else {
                        Debug.Log("다음 글 없음");
                        return;
                    }
                }
                networkManager.request("GET", strBuilder.ToString(), ncExt.networkCallback(dispatcher, payload));
                break;
            case NetworkAction.statusTypes.SUCCESS:
                storeStatus = storeStatus.NORMAL;
                Debug.Log(payload.response.data);
                postsCallbackHeader = payload.response.header;
                posts = JsonHelper.getJsonArray<Posts>(payload.response.data);
                _emitChange();
                break;
            case NetworkAction.statusTypes.FAIL:
                storeStatus = storeStatus.ERROR;
                GetPostsError error = GetPostsError.fromJSON(payload.response.data);
                string detail = error.detail;
                bool tmp = detail.Contains("permission");
                if (tmp) {
                    message = "접근 권한이 없습니다.";
                }
                else {
                    setMessage(3);
                }
                //그룹원이 아닌경우 ERROR callback
                //Debug.Log(payload.response.data);
                setMessage(3);

                _emitChange();
                break;
        }
    }

    //그룹 이야기 생성
    private void addPosts(Group_addPosts payload) {
        switch (payload.status)
        {
            case NetworkAction.statusTypes.REQUEST:
                storeStatus = storeStatus.WAITING_REQ;

                var strBuilder = GameManager.Instance.sb;
                strBuilder.Remove(0, strBuilder.Length);
                strBuilder.Append(networkManager.baseUrl)
                    .Append("groups/")
                    .Append(payload.id)
                    .Append("/posts");

                WWWForm form = new WWWForm();
                form.AddField("text", payload.context);

                networkManager.request("POST", strBuilder.ToString(), form, ncExt.networkCallback(dispatcher, payload));
                break;
            case NetworkAction.statusTypes.SUCCESS:
                storeStatus = storeStatus.NORMAL;
                message = "그룹 이야기가 추가되었습니다.";
                callbackPost = Posts.fromJSON(payload.response.data);
                Debug.Log(payload.response.data);
                _emitChange();
                break;
            case NetworkAction.statusTypes.FAIL:
                storeStatus = storeStatus.ERROR;
                setMessage(3);

                _emitChange();
                Debug.Log(payload.response.data);
                break;
        }
    }

    //그룹 이야기 삭제
    //자신이 쓴 글이거나 그룹장인 경우 삭제가 가능
    private void delPost(Group_delPost payload) {
        switch (payload.status)
        {
            case NetworkAction.statusTypes.REQUEST:
                storeStatus = storeStatus.WAITING_REQ;
                target = payload.target;

                var strBuilder = GameManager.Instance.sb;
                strBuilder.Remove(0, strBuilder.Length);
                strBuilder.Append(networkManager.baseUrl)
                    .Append("groups/")
                    .Append(payload.id)
                    .Append("/posts/")
                    .Append(payload.postId);
                
                networkManager.request("DELETE", strBuilder.ToString(), ncExt.networkCallback(dispatcher, payload));
                break;
            case NetworkAction.statusTypes.SUCCESS:
                storeStatus = storeStatus.NORMAL;
                message = "그룹 이야기가 삭제되었습니다.";
                callbackPost = Posts.fromJSON(payload.response.data);
                Debug.Log(payload.response.data);
                _emitChange();
                break;
            case NetworkAction.statusTypes.FAIL:
                storeStatus = storeStatus.ERROR;

                GetPostsError error = GetPostsError.fromJSON(payload.response.data);
                string detail = error.detail;

                bool tmp = detail.Contains("permission");
                if (tmp) {
                    message = "삭제 권한이 없습니다.";
                }
                else {
                    setMessage(3);
                }

                _emitChange();
                Debug.Log(payload.response.data);
                break;
        }
    }

    private void onPanel(int index) {
        sceneIndex = index;
        _emitChange();
    }

    private void setMessage(int type) {
        switch (type) {
            //서버 요청중
            case 1:
                message = "서버 요청중입니다. 잠시만 기다려 주세요.";
                break;
            case 2:
                message = "서버 요청에 성공하였습니다.";
                break;
            case 3:
                message = "서버 요청간에 문제가 발생하였습니다.";
                break;
        }
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

[System.Serializable]
public class GroupAddError {
    public string[] name;
    public string[] locationDistrict;
    public string[] locationCity;
    public string[] groupIntro;

    public static GroupAddError fromJSON(string json) {
        return JsonUtility.FromJson<GroupAddError>(json);
    }
}

[System.Serializable]
public class GetPostsError {
    public string detail;

    public static GetPostsError fromJSON(string json) {
        return JsonUtility.FromJson<GetPostsError>(json);
    }
}

[System.Serializable]
public class Posts {
    public int id;
    public string text;
    public CallbackUser writer;
    public string createDate;

    public static Posts fromJSON(string json)
    {
        return JsonUtility.FromJson<Posts>(json);
    }
}
