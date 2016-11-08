using UnityEngine;
using System;

public enum ActionTypes {
    GAME_START,
    GAME_END,
    EDIT_NICKNAME,
    GET_GPS_DATA
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
            _return.type = _type;
            break;
        }
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
    public LocationInfo info;
}