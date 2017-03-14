using Flux;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;

public class Friends : AjwStore {
    public Friends(QueueDispatcher<Actions> _dispatcher) : base(_dispatcher) { }
    NetworkManager networkManager = NetworkManager.Instance;

    public Friend[]
        waitingAcceptLists,
        myFriends,
        friendReqLists;

    //검색된 친구
    public SearchedFriend[] searchedFriend;
    public Friend addedFriend;
    public string
        msg,
        keyword;

    public ActionTypes eventType;
    public GameObject targetObj;
    public int toUserId;
    //요청 식별번호(삭제 시 필요)
    public int queryId;

    public AddFriendPrefab.friendType addFriendType;

    public bool
        addResult = false,
        searchResult = false;

    NetworkCallbackExtention ncExt = new NetworkCallbackExtention();

    protected override void _onDispatch(Actions action) {
        switch (action.type) {
            case ActionTypes.GET_MY_FRIEND_LIST:
                getMyFriendLists(action as GetMyFriendListAction);
                break;

            case ActionTypes.GET_WAITING_FRIEND_ACCEPT_LIST:
                getWaitingAcceptLists(action as GetAcceptWaitingListAction);
                break;

            case ActionTypes.COMMUNITY_SEARCH:
                CommunitySearchAction searchAct = action as CommunitySearchAction;
                if (searchAct._type == CommunitySearchAction.searchType.FRIEND) {
                    search(searchAct);
                }
                break;

            case ActionTypes.COMMUNITY_DELETE:
                CommunityDeleteAction delAct = action as CommunityDeleteAction;
                if (delAct._type == CommunityDeleteAction.deleteType.FRIEND) {
                    delete(delAct);
                }
                break;
            case ActionTypes.ADD_FRIEND:
                Debug.Log("Add Friend 액션");
                AddFriendAction addAct = action as AddFriendAction;
                addFriend(addAct);
                break;

            case ActionTypes.ADD_COMMUNITY_FRIEND_PREFAB:
                AddFriendPrefab addPrefabAct = action as AddFriendPrefab;
                addFriendPrefab(addPrefabAct);
                break;

            case ActionTypes.DELETE_COMMUNITY_FRIEND_PREFAB:
                DelFriendPrefab delPrefabAct = action as DelFriendPrefab;
                delFriendPrefab(delPrefabAct);
                break;
        }
        eventType = action.type;
    }

    //친구 요청 목록과 친구 목록을 불러온다.
    private void getMyFriendLists(GetMyFriendListAction payload) {
        switch (payload.status) {
            case NetworkAction.statusTypes.REQUEST:
                var strBuilder = GameManager.Instance.sb;
                strBuilder.Remove(0, strBuilder.Length);
                strBuilder.Append(networkManager.baseUrl)
                    .Append("friends");
                networkManager.request("GET", strBuilder.ToString(), ncExt.networkCallback(dispatcher, payload));
                break;
            case NetworkAction.statusTypes.SUCCESS:
                Friend[] data = JsonHelper.getJsonArray<Friend>(payload.response.data);

                ArrayList tmpListWaiting = new ArrayList();
                ArrayList tmpListFriend = new ArrayList();
                Debug.Log(payload.response.data);
                foreach(Friend friend in data) {
                    if (friend.friendState == "WAITING") {
                        tmpListWaiting.Add(friend);
                        Debug.Log("수락대기중");
                    }
                    else if (friend.friendState == "FRIEND") {
                        tmpListFriend.Add(friend);
                    }
                }
                friendReqLists = (Friend[])tmpListWaiting.ToArray(typeof(Friend));
                myFriends = (Friend[])tmpListFriend.ToArray(typeof(Friend));
                GetAcceptWaitingListAction getWaitingListAction = ActionCreator.createAction(ActionTypes.GET_WAITING_FRIEND_ACCEPT_LIST) as GetAcceptWaitingListAction;
                dispatcher.dispatch(getWaitingListAction);
                //_emitChange();
                break;
            case NetworkAction.statusTypes.FAIL:
                Debug.Log(payload.response.data);
                //_emitChange();
                break;
        }
    }

    //수락 대기 중인 목록을 가져온다.
    private void getWaitingAcceptLists(GetAcceptWaitingListAction payload) {
        switch (payload.status) {
            case NetworkAction.statusTypes.REQUEST:
                var strBuilder = GameManager.Instance.sb;
                strBuilder.Remove(0, strBuilder.Length);
                strBuilder.Append(networkManager.baseUrl)
                    .Append("friends/requested?");
                networkManager.request("GET", strBuilder.ToString(), ncExt.networkCallback(dispatcher, payload));
                break;
            case NetworkAction.statusTypes.SUCCESS:
                ArrayList list = new ArrayList();
                Friend[] data = JsonHelper.getJsonArray<Friend>(payload.response.data);
                Debug.Log(payload.response.data);
                foreach(Friend friend in data) {
                    list.Add(friend);
                }
                waitingAcceptLists = (Friend[])list.ToArray(typeof(Friend));
                _emitChange();
                break;
            case NetworkAction.statusTypes.FAIL:
                Debug.Log(payload.response.data);
                //_emitChange();
                break;
        }
    }

    //친구 검색
    private void search(CommunitySearchAction act) {
        keyword = act.keyword;

        msg = null;
        searchResult = false;
        addResult = false;
        searchedFriend = null;

        switch (act.status) {
            case NetworkAction.statusTypes.REQUEST:
                var strBuilder = GameManager.Instance.sb;
                strBuilder.Remove(0, strBuilder.Length);
                strBuilder.Append(networkManager.baseUrl)
                    .Append("users?nickName=")
                    .Append(WWW.EscapeURL(act.keyword, Encoding.UTF8));
                networkManager.request("GET", strBuilder.ToString(), ncExt.networkCallback(dispatcher, act));
                Debug.Log("Search URL : " + strBuilder.ToString());
                break;
            case NetworkAction.statusTypes.SUCCESS:
                Debug.Log(act.response.data);
                searchedFriend = JsonHelper.getJsonArray<SearchedFriend>(act.response.data);
                if(searchedFriend.Length == 0) {
                    msg = "존재하지 않는 아이디입니다.";
                    searchResult = false;
                    Debug.Log("존재하지 않는 아이디");
                    _emitChange();
                    return;
                }
                AddFriendAction addFriendAct = ActionCreator.createAction(ActionTypes.ADD_FRIEND) as AddFriendAction;
                addFriendAct.id = searchedFriend[0].id;
                addFriendAct.mType = AddFriendAction.friendType.REQUEST;
                dispatcher.dispatch(addFriendAct);
                searchResult = true;
                break;
            case NetworkAction.statusTypes.FAIL:
                Debug.Log(act.response.data);
                break;
        }
    }

    //친구 추가
    private void addFriend(AddFriendAction act) {
        switch (act.status) {
            case NetworkAction.statusTypes.REQUEST:
                var strBuilder = GameManager.Instance.sb;
                strBuilder.Remove(0, strBuilder.Length);
                strBuilder.Append(networkManager.baseUrl)
                    .Append("friends?");
                WWWForm form = new WWWForm();
                //form.headers.Add("Authorization", "Token " + GameManager.Instance.userStore.userTokenId);
                form.AddField("toUser", act.id);
                Debug.Log("친구 요청 URL : " + strBuilder);
                Debug.Log("친구 요청 ID : " + act.id);
                networkManager.request("POST", strBuilder.ToString(), form, ncExt.networkCallback(dispatcher, act));
                break;
            case NetworkAction.statusTypes.SUCCESS:
                //친구 프리팹 생성 액션
                Debug.Log("친구 추가에 대한 response data : " + act.response.data);
                addedFriend = Friend.fromJSON(act.response.data);
                AddFriendPrefab addPrefAct = ActionCreator.createAction(ActionTypes.ADD_COMMUNITY_FRIEND_PREFAB) as AddFriendPrefab;
                queryId = addedFriend.id;
                addPrefAct.mType = act.mType;

                msg = "친구 신청을 완료하였습니다.";
                dispatcher.dispatch(addPrefAct);
                addResult = true;
                break;
            case NetworkAction.statusTypes.FAIL:
                Debug.Log(act.response.data);
                if (act.response.data.Contains("non_field_errors")) {
                    msg = "이미 친구 신청이 완료된 상태입니다.";
                }
                if (act.response.data.Contains("self_friend_error")) {
                    msg = "자신에게는 친구 신청을 할 수 없습니다.";
                }
                //errorMessage message = errorMessage.fromJSON(act.response.data);
                //if(message.non_field_errors != null) {
                //    msg = "이미 친구 신청이 완료된 상태입니다.";
                //}
                //if(message.self_friend_error != null) {
                //    msg = "자신에게는 친구 신청을 할 수 없습니다.";
                //}
                addResult = false;
                addedFriend = null;
                _emitChange();
                break;
        }
    }

    private void addFriendPrefab(AddFriendPrefab act) {
        addFriendType = act.mType;
        _emitChange();
        //초기화
        //msg = null;
        //searchResult = false;
        //addResult = false;
    }

    private void delFriendPrefab(DelFriendPrefab act) {
        _emitChange();
    }

    private void delete(CommunityDeleteAction act) {
        //Debug.Log(act.response.data);
        switch (act.status) {
            case NetworkAction.statusTypes.REQUEST:
                var strBuilder = GameManager.Instance.sb;
                strBuilder.Remove(0, strBuilder.Length);
                strBuilder.Append(networkManager.baseUrl)
                    .Append("friends/")
                    .Append(act.id);
                networkManager.request("DELETE", strBuilder.ToString(), ncExt.networkCallback(dispatcher, act));
                Debug.Log("DELETE REQUEST URL : " + strBuilder.ToString());
                break;
            case NetworkAction.statusTypes.SUCCESS:
                Debug.Log("delete success");
                targetObj = act.targetGameObj;
                DelFriendPrefab delPrefabAct = ActionCreator.createAction(ActionTypes.DELETE_COMMUNITY_FRIEND_PREFAB) as DelFriendPrefab;
                dispatcher.dispatch(delPrefabAct);
                //msg = keyword + " 로 검색 결과";
                break;
            case NetworkAction.statusTypes.FAIL:
                _emitChange();
                break;
        }
    }

    public bool isNullOrEmpty<T>(T[] array) {
        return array == null || array.Length == 0;
    }
}

[System.Serializable]
public class Friend {
    public int id;
    public userInfo toUser;
    public userInfo fromUser;
    public string friendState;

    public static Friend fromJSON(string json) {
        return JsonUtility.FromJson<Friend>(json);
    }
}

[System.Serializable]
public class userInfo {
    public int id;
    public string nickName;
}

[System.Serializable]
public class SearchedFriend {
    public int id;
    public string nickName;

    public static SearchedFriend fromJSON(string json) {
        return JsonUtility.FromJson<SearchedFriend>(json);
    }
}

[System.Serializable]
class errorMessage {
    public string[] non_field_errors = null;
    public string[] self_friend_error = null;
    public static errorMessage fromJSON(string json) {
        return JsonUtility.FromJson<errorMessage>(json);
    }
}