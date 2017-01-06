using UnityEngine;
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
    GET_COMMUNITY_DATA
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
        case ActionTypes.GET_COMMUNITY_DATA:
            _return = new GetCommunityAction();
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

public class GetCommunityAction : NetworkAction {
    public enum requestType { ALL, FEEDS, FRIENDS, GROUP };
    //type default : ALL
    public requestType type = requestType.ALL;
    public string keyword;
}

public class EditNickNameAction : Actions{
    public string nickname;
}

public class GetGPSDataAction : Actions {
    public LocationInfo GPSInfo;
}

public class RidingStartAction : NetworkAction {

}

public class RidingEndAction : Actions {

}

public class RidingResultAction : EditNickNameAction {
    public StringBuilder data = new StringBuilder();
}

public class UserCreateAction : NetworkAction {
    public string deviceId;
    public string nickName;
}