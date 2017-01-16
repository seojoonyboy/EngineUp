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
    public Friends friendsStore;
    public Groups groupStore;

    void Awake() {
        gameDispatcher = new Dispatcher<Actions>();
        sb = new StringBuilder();
        userStore = new User(gameDispatcher);
        ridingStore = new Riding(gameDispatcher);
        friendsStore = new Friends(gameDispatcher);
        groupStore = new Groups(gameDispatcher);

        deviceId = SystemInfo.deviceUniqueIdentifier;
    }
}
