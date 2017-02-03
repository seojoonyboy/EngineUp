﻿using UnityEngine;
using System;
using System.Text;

public enum ActionTypes {
    GAME_START,
    GAME_END,
    EDIT_NICKNAME,
    GET_GPS_DATA,
    RIDING_START,
    RIDING_END,
    RIDING_RESULT,
    POST_FAIL,
    POST_SUCCESS,
    USER_CREATE,
    //COMMUNITY_INITIALIZE,
    COMMUNITY_SEARCH,
    COMMUNITY_DELETE,
    ADD_FRIEND,
    GET_MY_FRIEND_LIST,
    GET_WAITING_FRIEND_ACCEPT_LIST,
    ADD_COMMUNITY_FRIEND_PREFAB
}

public class Actions{
    public ActionTypes type;
}

public static class ActionCreator{
    public static Actions createAction(ActionTypes _type){
        Actions _return = null;
        switch(_type){
        case ActionTypes.GAME_START:
            _return = new GameStartAction();
            break;
        case ActionTypes.EDIT_NICKNAME:
            _return = new EditNickNameAction();
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
        case ActionTypes.RIDING_RESULT:
            _return = new RidingResultAction();
            break;
        case ActionTypes.POST_FAIL:
            _return = new Actions();
            break;
        case ActionTypes.POST_SUCCESS:
            _return = new Actions();
            break;
        case ActionTypes.USER_CREATE:
            _return = new UserCreateAction();
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
        }
         _return.type = _type;
        return _return;
    }
}


public class NetworkAction : Actions {
    public enum statusTypes {REQUEST, SUCCESS, FAIL};
    public statusTypes status = statusTypes.REQUEST;
    public HttpResponse response;
}
public class GameStartAction : NetworkAction{
    public string message;
}

public class EditNickNameAction : Actions{
    public string nickname;
}

public class GetGPSDataAction : Actions {
    public LocationInfo GPSInfo;
}

public class RidingStartAction : NetworkAction {}
public class RidingEndAction : Actions {}

public class RidingResultAction : EditNickNameAction {
    public StringBuilder data = new StringBuilder();
}

public class UserCreateAction : NetworkAction {
    public string deviceId;
    public string nickName;
}

public class CommunityInitAction : NetworkAction {
}

public class CommunitySearchAction : NetworkAction {
    public enum searchType { GROUP, FRIEND };
    public searchType type;
    public string keyword;
}

public class CommunityDeleteAction : NetworkAction {
    public enum deleteType { GROUP, FRIEND };
    public deleteType type;
    public GameObject targetGameObj;
    public int id;
}

public class AddFriendAction : NetworkAction {
    public int id;
    public bool needPref = false;
}

//수락 대기 목록 불러오는 액션
public class GetAcceptWaitingListAction : NetworkAction { }
//내 친구 목록 불러오는 액션
public class GetMyFriendListAction : NetworkAction { }

public class AddFriendPrefab : Actions {
    //내 친구, 수락대기, 요청상태 순
    public enum type { MYFRIEND, WAITING, REQUEST};
    public type mType;
}