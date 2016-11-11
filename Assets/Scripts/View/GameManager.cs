using Flux;
using System.Text;
public class GameManager : Singleton<GameManager> {
    protected GameManager(){}
    public Dispatcher<Actions> gameDispatcher = new Dispatcher<Actions>();
    public StringBuilder sb = new StringBuilder();

    public User userStore;

}
