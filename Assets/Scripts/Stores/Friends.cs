using Flux;
using UnityEngine;
using System.Collections;
using System;

public class Friends : Store<Actions> {
    public Friends(Dispatcher<Actions> _dispatcher) : base(_dispatcher) { }
    NetworkManager networkManager = NetworkManager.Instance;

    public ArrayList friendReqLists = new ArrayList();
    public ArrayList myFriends = new ArrayList();
    public ArrayList waitingAcceptLists = new ArrayList();

    public string
        msg,
        keyword;

    public bool searchResult = false;
    public ActionTypes eventType;
    public GameObject targetItem;
    public int toUserId;

    NetworkCallbackExtention ncExt = new NetworkCallbackExtention();

    protected override void _onDispatch(Actions action) {
        switch (action.type) {
            case ActionTypes.COMMUNITY_INITIALIZE:
                getMyFriendLists(action as CommunityInitAction);
                //getWaitingAcceptLists(action as CommunityInitAction);
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
                AddFriendAction addAct = action as AddFriendAction;
                addFriend(addAct);
                break;
        }
        eventType = action.type;
    }

    //친구 요청 목록과 친구 목록을 불러온다.
    private void getMyFriendLists(CommunityInitAction payload) {
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
                friendReqLists.Clear();
                Friend[] data = JsonHelper.getJsonArray<Friend>(payload.response.data);
                foreach(Friend friend in data) {
                    if(friend.friendState == "WAITING") {
                        friendReqLists.Add(friend);
                    }
                    else if(friend.friendState == "FRIEND") {
                        myFriends.Add(friend);
                    }
                }
                _emitChange();
                break;
            case NetworkAction.statusTypes.FAIL:
                Debug.Log(payload.response.data);
                _emitChange();
                break;
        }
    }

    //수락 대기 중인 목록을 가져온다.
    private void getWaitingAcceptLists(CommunityInitAction payload) {
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
                waitingAcceptLists.Clear();
                Friend[] data = JsonHelper.getJsonArray<Friend>(payload.response.data);
                foreach(Friend friend in data) {
                    waitingAcceptLists.Add(friend);
                }
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
                UserData user = UserData.fromJSON(act.response.data);
                msg = act.keyword + "님을 추가하시겠습니까?";
                searchResult = true;
                toUserId = user.id;
                _emitChange();
                break;
            case NetworkAction.statusTypes.FAIL:
                msg = "일치하는 사용자가 없어 친구추가에 실패했습니다.";
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
                form.AddField("toUser", toUserId);
                networkManager.request("POST", strBuilder.ToString(), form, ncExt.networkCallback(dispatcher, act));
                break;
            case NetworkAction.statusTypes.SUCCESS:
                Debug.Log("친구 추가 완료");
                _emitChange();
                break;
            case NetworkAction.statusTypes.FAIL:
                Debug.Log(act.response.data);
                _emitChange();
                break;
        }
    }

    private void delete(CommunityDeleteAction act) {
        switch (act.status) {
            case NetworkAction.statusTypes.REQUEST:
                var strBuilder = GameManager.Instance.sb;
                strBuilder.Remove(0, strBuilder.Length);
                strBuilder.Append(networkManager.baseUrl)
                    .Append("users/")
                    .Append(GameManager.Instance.deviceId);
                targetItem = act.targetGameObj;
                //networkManager.request("GET", strBuilder.ToString(), ncExt.networkCallback(dispatcher, payload));
                break;
            case NetworkAction.statusTypes.SUCCESS:
                //msg = keyword + " 로 검색 결과";
                break;
            case NetworkAction.statusTypes.FAIL:
                break;
        }
    }

    public bool isNullOrEmpty<T>(T[] array) {
        return array == null || array.Length == 0;
    }
}

[System.Serializable]
public class Friend {
    public string id;
    public int toUser_id;
    public int fromUser_id;
    public string friendState;

    public static Friend fromJSON(string json) {
        return JsonUtility.FromJson<Friend>(json);
    }
}