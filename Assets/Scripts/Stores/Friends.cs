using Flux;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Friends : AjwStore {
    public Friends(QueueDispatcher<Actions> _dispatcher) : base(_dispatcher) { }
    NetworkManager networkManager = NetworkManager.Instance;

    public Friend[]
        waitingAcceptLists,
        myFriends,
        friendReqLists;

    //검색된 친구
    public SearchedFriend newFriend;

    public Friend addedFriend;
    public string
        msg,
        keyword;

    public ActionTypes eventType;
    public GameObject targetObj;
    public int toUserId;

    public AddFriendPrefab.type addFriendType;

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
                if (searchAct.type == CommunitySearchAction.searchType.FRIEND) {
                    search(searchAct);
                }
                break;

            case ActionTypes.COMMUNITY_DELETE:
                CommunityDeleteAction delAct = action as CommunityDeleteAction;
                if (delAct.type == CommunityDeleteAction.deleteType.FRIEND) {
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
                    .Append("friends?deviceId=")
                    .Append(GameManager.Instance.deviceId);
                Debug.Log(GameManager.Instance.deviceId);
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
                    .Append("friends/requested?deviceId=")
                    .Append(GameManager.Instance.deviceId);
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
        addedFriend = null;
        newFriend = null;

        switch (act.status) {
            case NetworkAction.statusTypes.REQUEST:
                var strBuilder = GameManager.Instance.sb;
                strBuilder.Remove(0, strBuilder.Length);
                strBuilder.Append(networkManager.baseUrl)
                    .Append("users/search?nickName=")
                    .Append(WWW.EscapeURL(act.keyword));
                networkManager.request("GET", strBuilder.ToString(), ncExt.networkCallback(dispatcher, act));
                Debug.Log("Search URL : " + strBuilder.ToString());
                break;
            case NetworkAction.statusTypes.SUCCESS:
                Debug.Log(act.response.data);
                newFriend = SearchedFriend.fromJSON(act.response.data);
                AddFriendAction addFriendAct = ActionCreator.createAction(ActionTypes.ADD_FRIEND) as AddFriendAction;
                addFriendAct.id = newFriend.id;
                addFriendAct.mType = AddFriendAction.type.MYFRIEND;
                dispatcher.dispatch(addFriendAct);
                searchResult = true;
                break;
            case NetworkAction.statusTypes.FAIL:
                msg = "존재하지 않는 아이디입니다.";
                searchResult = false;
                Debug.Log("존재하지 않는 아이디");
                _emitChange();
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
                    .Append("friends?deviceId=")
                    .Append(GameManager.Instance.deviceId);
                WWWForm form = new WWWForm();
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
                addPrefAct.mType = act.mType;
                msg = "친구 신청을 완료하였습니다.";
                dispatcher.dispatch(addPrefAct);
                addResult = true;
                break;
            case NetworkAction.statusTypes.FAIL:
                Debug.Log(act.response.data);
                errorMessage message = errorMessage.fromJSON(act.response.data);
                if(message.non_field_errors != null) {
                    msg = "이미 친구 신청이 완료된 상태입니다.";
                }
                if(message.self_friend_error != null) {
                    msg = "자신에게는 친구 신청을 할 수 없습니다.";
                }
                addResult = false;
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
                    .Append(act.id)
                    .Append("?deviceId=")
                    .Append(GameManager.Instance.deviceId);
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

public class SearchedFriend {
    public int id;
    public string nickName;

    public static SearchedFriend fromJSON(string json) {
        return JsonUtility.FromJson<SearchedFriend>(json);
    }
}

[System.Serializable]
class errorMessage {
    public string[] non_field_errors;
    public string[] self_friend_error;
    public static errorMessage fromJSON(string json) {
        return JsonUtility.FromJson<errorMessage>(json);
    }
}