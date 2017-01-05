using Flux;
using System.Text;
using UnityEngine;

public class GameManager : Singleton<GameManager> {
    protected GameManager(){}
    public Dispatcher<Actions> gameDispatcher;
    public StringBuilder sb;
    [System.NonSerialized]
    public string deviceId;
    public User userStore;
    public Riding ridingStore;

    void Awake() {
        gameDispatcher = new Dispatcher<Actions>();
        sb = new StringBuilder();
        userStore = new User(gameDispatcher);
        ridingStore = new Riding(gameDispatcher);

        deviceId = SystemInfo.deviceUniqueIdentifier;
    }
}
