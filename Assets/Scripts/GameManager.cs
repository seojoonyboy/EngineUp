using Flux;
public class GameManager : Singleton<GameManager> {
    protected GameManager(){}
    public Dispatcher<Actions> gameDispatcher = new Dispatcher<Actions>();

}
