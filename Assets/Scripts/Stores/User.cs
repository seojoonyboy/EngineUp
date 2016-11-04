using Flux;

public class User : Store<Actions> {
    // prop
    public string nickName;

    // end of prop
    public User(Dispatcher<Actions> _dispatcher) : base(_dispatcher){}
    protected override void _onDispatch(Actions action){
        switch(action.type){
        case ActionTypes.EDIT_NICKNAME:
            nickName = (action as EditNickNameAction).nickname;
            break;
        }
    }
}