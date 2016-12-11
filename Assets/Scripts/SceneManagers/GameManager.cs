using Flux;
using System.Text;
public class GameManager : Singleton<GameManager> {
    protected GameManager(){}
    public Dispatcher<Actions> gameDispatcher = new Dispatcher<Actions>();
    public StringBuilder sb = new StringBuilder();
    [System.NonSerialized]
    public string deviceId;
    public User userStore;
}
