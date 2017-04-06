using UnityEngine;

public class MainSceneManager : fbl_SceneManager {
    public Result_VC resultViewCtrler;
    public Riding_VC ridingViewCtrler;
    public FriendsViewController friendViewCtrler;
    public GroupViewController groupViewCtrler;
    public StatViewController statViewCtrler;
    public GarageViewController garageViewCtrler;
    public BoxViewController boxViewCtrler;

    private GameManager gm;
    public GameObject modal;

    void Start() {
        gm = GameManager.Instance;
        initialize();
    }

    void initialize() {
        resultViewCtrler.ridingStore = gm.ridingStore;
        ridingViewCtrler.ridingStore = gm.ridingStore;
        ridingViewCtrler.userStore = gm.userStore;
        friendViewCtrler.friendsStore = gm.friendsStore;
        groupViewCtrler.groupStore = gm.groupStore;
        groupViewCtrler.locationStore = gm.locationStore;
        statViewCtrler.userStore = gm.userStore;

        gm.friendsStore.addListener(friendViewCtrler.OnFriendsStoreListener);
        gm.ridingStore.addListener(resultViewCtrler.onRidingListener);
        gm.ridingStore.addListener(ridingViewCtrler.onRidingListener);
        gm.groupStore.addListener(groupViewCtrler.onGroupStoreListener);
        gm.locationStore.addListener(groupViewCtrler.onGroupStoreListener);
        gm.userStore.addListener(statViewCtrler.onUserListener);

        gm.bicycleInventStore.addListener(garageViewCtrler.onBicycleStoreListener);
        gm.charInvenStore.addListener(garageViewCtrler.onCharStoreListener);
        gm.userStore.addListener(garageViewCtrler.onUserStoreListener);

        gm.boxInvenStore.addListener(boxViewCtrler.onStoreListener);
    }

    public void offModal() {
        modal.SetActive(false);
    }
}