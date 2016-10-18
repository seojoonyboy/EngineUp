public enum ActionTypes {
    GAME_START,
    GAME_END
}
public class Actions{
    public ActionTypes type;
}

public class StartAction : Actions{
    public string message;
    public StartAction(string _message){
        type = ActionTypes.GAME_START;
        message = _message;
    }
}