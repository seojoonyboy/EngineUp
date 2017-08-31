using Flux;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;

public class Friends : AjwStore {
    //store status
    public storeStatus storeStatus = storeStatus.NORMAL;

    public Friends(QueueDispatcher<Actions> _dispatcher) : base(_dispatcher) { }
    NetworkManager networkManager = NetworkManager.Instance;

    public ArrayList
        waitingAcceptLists = new ArrayList(),
        myFriends = new ArrayList(),
        friendReqLists = new ArrayList();

    //검색된 친구
    public SearchedFriend[] searchedFriend;
    public fr_info_callback selectedFriend;

    public Friend addedFriend;
    public string
        msg,
        keyword;

    public ActionTypes eventType;
    //요청 취소인지, 내 친구 삭제인지 구분을 위한 enum
    public CommunityDeleteAction.detailType detailType;
    //요청 대기목록 불러오기인지, 내 친구 목록 불러오기인지 구분을 위한 enum
    public GetMyFriendListAction.type getReqType;
    public GameObject targetObj;
    public int toUserId;
    //요청 식별번호(삭제 시 필요)
    public int queryId;

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
                AddFriendAction addAct = action as AddFriendAction;
                addFriend(addAct);
                break;
            case ActionTypes.SEARCH_RESULT:
                _emitChange();
                break;
            case ActionTypes.GET_FR_INFO:
                GetFriendInfoAction infoAct = action as GetFriendInfoAction;
                getFriendInfo(infoAct);
                break;
        }
        eventType = action.type;
    }

    //친구 요청 목록과 친구 목록을 불러온다.
    private void getMyFriendLists(GetMyFriendListAction payload) {
        switch (payload.status) {
            case NetworkAction.statusTypes.REQUEST:
                storeStatus = storeStatus.WAITING_REQ;
                var strBuilder = GameManager.Instance.sb;
                strBuilder.Remove(0, strBuilder.Length);
                strBuilder.Append(networkManager.baseUrl)
                    .Append("friends");
                networkManager.request("GET", strBuilder.ToString(), ncExt.networkCallback(dispatcher, payload));
                break;
            case NetworkAction.statusTypes.SUCCESS:
                storeStatus = storeStatus.NORMAL;
                Friend[] data = JsonHelper.getJsonArray<Friend>(payload.response.data);
                if (payload._type == GetMyFriendListAction.type.FRIEND) {
                    //myFriends.Clear();
                    myFriends = new ArrayList();
                    foreach (Friend friend in data) {
                        if (friend.friendState == "FRIEND") {
                            myFriends.Add(friend);
                        }
                    }
                }
                else if(payload._type == GetMyFriendListAction.type.WAITING) {
                    //friendReqLists.Clear();
                    friendReqLists = new ArrayList();
                    foreach (Friend friend in data) {
                        if (friend.friendState == "WAITING") {
                            friendReqLists.Add(friend);
                        }
                    }
                }
                getReqType = payload._type;
                break;
            case NetworkAction.statusTypes.FAIL:
                storeStatus = storeStatus.ERROR;
                Debug.Log(payload.response.data);
                break;
        }
        _emitChange();
    }

    //수락 대기 중인 목록을 가져온다.
    private void getWaitingAcceptLists(GetAcceptWaitingListAction payload) {
        switch (payload.status) {
            case NetworkAction.statusTypes.REQUEST:
                storeStatus = storeStatus.WAITING_REQ;
                var strBuilder = GameManager.Instance.sb;
                strBuilder.Remove(0, strBuilder.Length);
                strBuilder.Append(networkManager.baseUrl)
                    .Append("friends/requested?");
                networkManager.request("GET", strBuilder.ToString(), ncExt.networkCallback(dispatcher, payload));
                break;
            case NetworkAction.statusTypes.SUCCESS:
                storeStatus = storeStatus.NORMAL;
                Friend[] data = JsonHelper.getJsonArray<Friend>(payload.response.data);
                waitingAcceptLists = new ArrayList();
                foreach (Friend friend in data) {
                    waitingAcceptLists.Add(friend);
                }
                break;
            case NetworkAction.statusTypes.FAIL:
                storeStatus = storeStatus.ERROR;
                Debug.Log(payload.response.data);
                //_emitChange();
                break;
        }
        _emitChange();
    }

    //친구 검색
    private void search(CommunitySearchAction act) {
        keyword = act.keyword;

        msg = null;
        searchedFriend = null;

        switch (act.status) {
            case NetworkAction.statusTypes.REQUEST:
                storeStatus = storeStatus.WAITING_REQ;
                var strBuilder = GameManager.Instance.sb;
                strBuilder.Remove(0, strBuilder.Length);
                strBuilder.Append(networkManager.baseUrl)
                    .Append("users?nickName=")
                    .Append(WWW.EscapeURL(act.keyword, Encoding.UTF8));
                networkManager.request("GET", strBuilder.ToString(), ncExt.networkCallback(dispatcher, act));
                Debug.Log("Search URL : " + strBuilder.ToString());
                _emitChange();
                break;
            case NetworkAction.statusTypes.SUCCESS:
                storeStatus = storeStatus.NORMAL;
                //Debug.Log(act.response.data);
                searchedFriend = JsonHelper.getJsonArray<SearchedFriend>(act.response.data);
                if(searchedFriend.Length == 0) {
                    msg = "존재하지 않는 아이디입니다.";
                    Debug.Log("존재하지 않는 아이디");
                    storeStatus = storeStatus.ERROR;
                    _emitChange();
                    return;
                }
                //User Store의 id와 대조
                //같으면 오류 메세지
                int myId = GameManager.Instance.userStore.userId;
                Debug.Log(myId);
                if(searchedFriend[0].id == myId) {
                    storeStatus = storeStatus.ERROR;
                    msg = "자기 자신을 검색하였습니다.";
                    _emitChange();
                }
                else {
                    var searchAct = ActionCreator.createAction(ActionTypes.SEARCH_RESULT) as GetSearchListAction;
                    dispatcher.dispatch(searchAct);
                }
                break;
            case NetworkAction.statusTypes.FAIL:
                storeStatus = storeStatus.ERROR;
                msg = "서버통신간 문재가 발생하였습니다.";
                _emitChange();
                break;
        }
    }

    //친구 추가
    private void addFriend(AddFriendAction act) {
        switch (act.status) {
            case NetworkAction.statusTypes.REQUEST:
                storeStatus = storeStatus.WAITING_REQ;
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
                _emitChange();
                break;
            case NetworkAction.statusTypes.SUCCESS:
                storeStatus = storeStatus.NORMAL;

                var _act = ActionCreator.createAction(ActionTypes.GET_MY_FRIEND_LIST) as GetMyFriendListAction;
                if (act._type == AddFriendAction.friendType.ACCEPT) {
                    _act._type = GetMyFriendListAction.type.FRIEND;
                    Friend[] arr = waitingAcceptLists.ToArray(typeof(Friend)) as Friend[];
                    foreach (Friend fr in arr) {
                        if(fr.fromUser.id == act.id) {
                            waitingAcceptLists.Remove(fr);
                        }
                    }
                    var acceptRef = ActionCreator.createAction(ActionTypes.GET_WAITING_FRIEND_ACCEPT_LIST);
                    dispatcher.dispatch(acceptRef);

                    msg = "친구 요청을 수락하였습니다.";
                    _emitChange();
                }
                else if(act._type == AddFriendAction.friendType.SEARCH) {
                    //친구 요청 버튼을 클릭한 경우
                    _act._type = GetMyFriendListAction.type.WAITING;
                    msg = "친구 요청을 하였습니다.";
                    _emitChange();
                }
                dispatcher.dispatch(_act);
                break;
            case NetworkAction.statusTypes.FAIL:
                storeStatus = storeStatus.ERROR;
                Debug.Log(act.response.data);
                if (act.response.data.Contains("non_field_errors")) {
                    msg = "이미 친구 신청이 완료된 상태입니다.";
                }
                if (act.response.data.Contains("self_friend_error")) {
                    msg = "자신에게는 친구 신청을 할 수 없습니다.";
                }
                _emitChange();
                break;
        }
    }

    private void getFriendInfo(GetFriendInfoAction act) {
        switch (act.status) {
            case NetworkAction.statusTypes.REQUEST:
                storeStatus = storeStatus.WAITING_REQ;
                var strBuilder = GameManager.Instance.sb;
                strBuilder.Remove(0, strBuilder.Length);
                strBuilder.Append(networkManager.baseUrl)
                    .Append("friends/")
                    .Append(act.id);
                networkManager.request("GET", strBuilder.ToString(), ncExt.networkCallback(dispatcher, act));
                msg = "친구 정보를 불러오는 중";
                break;
            case NetworkAction.statusTypes.SUCCESS:
                storeStatus = storeStatus.NORMAL;
                selectedFriend = fr_info_callback.fromJSON(act.response.data);
                break;
            case NetworkAction.statusTypes.FAIL:
                storeStatus = storeStatus.ERROR;
                msg = "친구정보를 불러오는 과정에서 문제가 발생하였습니다.";
                Debug.Log(act.response.data);
                break;
        }
        _emitChange();
    }

    private void delete(CommunityDeleteAction act) {
        switch (act.status) {
            case NetworkAction.statusTypes.REQUEST:
                storeStatus = storeStatus.WAITING_REQ;
                var strBuilder = GameManager.Instance.sb;
                strBuilder.Remove(0, strBuilder.Length);
                strBuilder.Append(networkManager.baseUrl)
                    .Append("friends/")
                    .Append(act.id);
                detailType = act._detailType;
                networkManager.request("DELETE", strBuilder.ToString(), ncExt.networkCallback(dispatcher, act));
                _emitChange();
                break;
            case NetworkAction.statusTypes.SUCCESS:
                storeStatus = storeStatus.NORMAL;
                var _act = ActionCreator.createAction(ActionTypes.GET_MY_FRIEND_LIST) as GetMyFriendListAction;
                if(detailType == CommunityDeleteAction.detailType.SENDING) {
                    _act._type = GetMyFriendListAction.type.WAITING;
                    msg = "친구요청을 취소하였습니다.";
                }
                else if(detailType == CommunityDeleteAction.detailType.MYLIST) {
                    _act._type = GetMyFriendListAction.type.FRIEND;
                    msg = "친구를 삭제하였습니다.";
                }
                dispatcher.dispatch(_act);
                _emitChange();
                break;
            case NetworkAction.statusTypes.FAIL:
                storeStatus = storeStatus.ERROR;
                Debug.Log(act.response.data);
                msg = "서버통신간 문제가 발생하였습니다.";
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

[System.Serializable]
public class fr_info_callback {
    public int id;
    public userInfo fromUser;
    public toUser toUser;

    public static fr_info_callback fromJSON(string json) {
        return JsonUtility.FromJson<fr_info_callback>(json);
    }
}

[System.Serializable]
public class toUser : UserData {
    public RespGetItems[] equiped_items;
}