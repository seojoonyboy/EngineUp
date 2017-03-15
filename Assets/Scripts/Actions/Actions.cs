using UnityEngine;
using System;
using System.Text;

public enum ActionTypes {
    SIGNUP, SIGNIN, GAME_START, GAME_END,
    GET_GPS_DATA, RIDING_START, RIDING_END,
    COMMUNITY_SEARCH, COMMUNITY_DELETE, ADD_FRIEND, GET_MY_FRIEND_LIST, GET_WAITING_FRIEND_ACCEPT_LIST, ADD_COMMUNITY_FRIEND_PREFAB, DELETE_COMMUNITY_FRIEND_PREFAB,
    GROUP_GET_MEMBERS, GROUP_SEARCH, GROUP_ADD, GROUP_DETAIL, GROUP_CHECK_MY_STATUS, GROUP_JOIN, GROUP_EDIT,
    GET_DISTRICT_DATA, GET_CITY_DATA, GROUP_MEMBER_ACCEPT, GROUP_BAN, GROUP_DESTROY, MY_GROUP_PANEL
}

public class Actions{
    public ActionTypes type;
}

public static class ActionCreator{
    public static Actions createAction(ActionTypes _type){
        Actions _return = null;
        switch(_type){
        case ActionTypes.SIGNUP:
            _return = new SignupAction();
            break;
        case ActionTypes.SIGNIN:
            _return = new SigninAction();
            break;
        case ActionTypes.GAME_START:
            _return = new GameStartAction();
            break;
        case ActionTypes.GET_GPS_DATA:
            _return = new GetGPSDataAction();
            break;
        case ActionTypes.RIDING_START:
            _return = new RidingStartAction();
            break;
        case ActionTypes.RIDING_END:
            _return = new RidingEndAction();
            break;
        case ActionTypes.COMMUNITY_SEARCH:
            _return = new CommunitySearchAction();
            break;
        case ActionTypes.COMMUNITY_DELETE:
            _return = new CommunityDeleteAction();
            break;
        case ActionTypes.ADD_FRIEND:
            _return = new AddFriendAction();
            break;
        case ActionTypes.GET_MY_FRIEND_LIST:
            _return = new GetMyFriendListAction();
            break;
        case ActionTypes.GET_WAITING_FRIEND_ACCEPT_LIST:
            _return = new GetAcceptWaitingListAction();
            break;
        case ActionTypes.ADD_COMMUNITY_FRIEND_PREFAB:
            _return = new AddFriendPrefab();
            break;
        case ActionTypes.DELETE_COMMUNITY_FRIEND_PREFAB:
            _return = new DelFriendPrefab();
            break;
        case ActionTypes.GROUP_GET_MEMBERS:
            _return = new Group_getMemberAction();
            break;
        case ActionTypes.GROUP_DETAIL:
            _return = new Group_detail();
            break;
        case ActionTypes.GROUP_SEARCH:
            _return = new Group_search();
            break;
        case ActionTypes.GET_DISTRICT_DATA:
            _return = new GetDistrictsData();
            break;
        case ActionTypes.GET_CITY_DATA:
            _return = new GetCityData();
            break;
        case ActionTypes.GROUP_ADD:
            _return = new Group_AddAction();
            break;
        case ActionTypes.GROUP_CHECK_MY_STATUS:
            _return = new Group_checkMyStatus();
            break;
        case ActionTypes.GROUP_JOIN:
            _return = new Group_join();
            break;
        case ActionTypes.GROUP_MEMBER_ACCEPT:
            _return = new Group_accept();
            break;
        case ActionTypes.GROUP_BAN:
            _return = new Group_ban();
            break;
        case ActionTypes.GROUP_EDIT:
            _return = new Group_AddAction();
            break;
        case ActionTypes.GROUP_DESTROY:
            _return = new Group_del();
            break;
        case ActionTypes.MY_GROUP_PANEL:
            _return = new Group_myGroups();
            break;
        }
         _return.type = _type;
        return _return;
    }
}

public class SignupAction : NetworkAction {
    //facebook, normal
    public enum loginType { FB, NO }
    public loginType login_type;
    public string token = null;
    public string nickName;
    public int charIndex;
}

public class SigninAction : SignupAction { }

public class NetworkAction : Actions {
    public enum statusTypes {REQUEST, SUCCESS, FAIL};
    public statusTypes status = statusTypes.REQUEST;
    public HttpResponse response;
}
public class GameStartAction : NetworkAction{
    public string message;
}

public class GetGPSDataAction : Actions {
    public LocationInfo GPSInfo;
    public string timeText;
}

public class RidingStartAction : NetworkAction {}
public class RidingEndAction : Actions {}

public class RidingResultAction : Actions {
    public string nickname;
    public StringBuilder data = new StringBuilder();
}

public class CommunityInitAction : NetworkAction {
}

public class CommunitySearchAction : NetworkAction {
    public enum searchType { GROUP, FRIEND };
    public searchType _type;
    public string keyword;
}

public class CommunityDeleteAction : NetworkAction {
    public enum deleteType { GROUP, FRIEND };
    public deleteType _type;
    public GameObject targetGameObj;
    public int id;
}

public class AddFriendAction : NetworkAction {
    public int id;
    public enum friendType { MYFRIEND, WAITING, REQUEST };
    public friendType mType;
}

public class AddFriendPrefab : AddFriendAction { }

//수락 대기 목록 불러오는 액션
public class GetAcceptWaitingListAction : NetworkAction { }
//내 친구 목록 불러오는 액션
public class GetMyFriendListAction : NetworkAction { }

public class DelFriendPrefab : Actions {
    public GameObject targetObj;
}

public class GetDistrictsData : NetworkAction {
    public int id;
}
public class GetCityData : NetworkAction {
    public int id;
}