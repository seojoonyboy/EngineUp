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

    public SearchedFriend newFriend;

    public string
        msg,
        keyword;

    public bool 
        searchResult = false,
        deleteResult = false,
        addResult = false,
        needNewPref;
    public ActionTypes eventType;
    public GameObject targetObj;
    public int toUserId;

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
                _emitChange();
                break;
            case NetworkAction.statusTypes.FAIL:
                Debug.Log(payload.response.data);
                _emitChange();
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
                _emitChange();
                break;
        }
    }

    //친구 검색
    private void search(CommunitySearchAction act) {
        keyword = act.keyword;
        switch (act.status) {
            case NetworkAction.statusTypes.REQUEST:
                var strBuilder = GameManager.Instance.sb;
                strBuilder.Remove(0, strBuilder.Length);
                strBuilder.Append(networkManager.baseUrl)
                    .Append("users/search?nickName=")
                    .Append(act.keyword);
                networkManager.request("GET", strBuilder.ToString(), ncExt.networkCallback(dispatcher, act));
                break;
            case NetworkAction.statusTypes.SUCCESS:
                Debug.Log(act.response.data);
                newFriend = SearchedFriend.fromJSON(act.response.data);
                msg = "친구추가 신청이 발송되었습니다.";
                searchResult = true;
                //toUserId = newFriend.id;
                _emitChange();
                break;
            case NetworkAction.statusTypes.FAIL:
                msg = "일치하는 사용자가 없어 친구추가에 실패했습니다.";
                searchResult = false;
                _emitChange();
                break;
        }
    }

    //친구 추가
    private void addFriend(AddFriendAction act) {
        switch (act.status) {
            case NetworkAction.statusTypes.REQUEST:
                needNewPref = act.needPref;
                var strBuilder = GameManager.Instance.sb;
                strBuilder.Remove(0, strBuilder.Length);
                strBuilder.Append(networkManager.baseUrl)
                    .Append("friends?deviceId=")
                    .Append(GameManager.Instance.deviceId);
                WWWForm form = new WWWForm();
                Debug.Log("ID : " + act.id);
                form.AddField("toUser", act.id);
                Debug.Log(strBuilder);
                networkManager.request("POST", strBuilder.ToString(), form, ncExt.networkCallback(dispatcher, act));
                break;
            case NetworkAction.statusTypes.SUCCESS:
                addResult = true;
                _emitChange();
                break;
            case NetworkAction.statusTypes.FAIL:
                Debug.Log(act.response.data);
                addResult = false;
                _emitChange();
                break;
        }
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
                deleteResult = true;
                targetObj = act.targetGameObj;
                _emitChange();
                //msg = keyword + " 로 검색 결과";
                break;
            case NetworkAction.statusTypes.FAIL:
                deleteResult = false;
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