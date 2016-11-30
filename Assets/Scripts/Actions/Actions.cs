using UnityEngine;
using System;

public enum ActionTypes {
    GAME_START,
    GAME_END,
    EDIT_NICKNAME,
    GET_GPS_DATA,
    RIDING_START,
    RIDING_END
}
public class Actions{
    public ActionTypes type;
}

public static class ActionCreator{
    public static Actions createAction(ActionTypes _type){
        Actions _return = null;
        switch(_type){
        case ActionTypes.EDIT_NICKNAME:
            _return = new EditNickNameAction();
            break;
        case ActionTypes.GET_GPS_DATA:
            _return = new GetGPSDataAction();
            break;
        case ActionTypes.RIDING_START:
        case ActionTypes.RIDING_END:
            _return =  new RidingEndAction();
            break;
        }
         _return.type = _type;
        return _return;
    }
}

public class StartAction : Actions{
    public string message;
}

public class EditNickNameAction : Actions{
    public string nickname;
}

public class GetGPSDataAction : Actions {
    public LocationInfo GPSInfo;
}

// public class RidingStartAction : Actions {}
public class RidingEndAction : Actions {

}