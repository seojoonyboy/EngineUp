using UnityEngine;

public class MainSceneManager : fbl_SceneManager {
    public Result_VC resultViewCtrler;
    public Riding_VC ridingViewCtrler;
    public FriendsViewController friendViewCtrler;
    public GroupViewController groupViewCtrler;
    public StatViewController statViewCtrler;
    public BoxViewController boxViewCtrler;
    public CharacterViewControlller charViewCtrler;
    public BicycleViewController bicycleViewCtrler;
    public HistoryViewController historyViewCtrler;
    public OptionController optionCtrler;

    private GameManager gm;
    public GameObject modal;

    void Start() {
        gm = GameManager.Instance;
        initialize();
    }

    void initialize() {
        resultViewCtrler.ridingStore = gm.ridingStore;
        resultViewCtrler.userStore = gm.userStore;

        ridingViewCtrler.ridingStore = gm.ridingStore;
        ridingViewCtrler.userStore = gm.userStore;
        friendViewCtrler.friendsStore = gm.friendsStore;
        groupViewCtrler.groupStore = gm.groupStore;
        groupViewCtrler.locationStore = gm.locationStore;
        statViewCtrler.userStore = gm.userStore;
        statViewCtrler.locationStore = gm.locationStore;

        boxViewCtrler.userStore = gm.userStore;
        charViewCtrler.userStore = gm.userStore;
        bicycleViewCtrler.userStore = gm.userStore;
        historyViewCtrler.userStore = gm.userStore;
        historyViewCtrler.ridingStore = gm.ridingStore;

        bicycleViewCtrler.bicycleItemStore = gm.bicycleInventStore;
        bicycleViewCtrler.charItemStore = gm.charInvenStore;

        gm.friendsStore.addListener(friendViewCtrler.OnFriendsStoreListener);
        gm.ridingStore.addListener(resultViewCtrler.onRidingListener);
        gm.ridingStore.addListener(ridingViewCtrler.onRidingListener);
        gm.groupStore.addListener(groupViewCtrler.onGroupStoreListener);
        gm.locationStore.addListener(groupViewCtrler.onGroupStoreListener);
        gm.userStore.addListener(statViewCtrler.onUserListener);
        gm.locationStore.addListener(statViewCtrler.onLocationStore);

        gm.boxInvenStore.addListener(boxViewCtrler.onBoxStoreListener);
        gm.userStore.addListener(boxViewCtrler.onUserStoreListener);

        gm.bicycleInventStore.addListener(bicycleViewCtrler.onBicycleItemStoreListener);
        gm.userStore.addListener(bicycleViewCtrler.onUserStoreListener);

        gm.charInvenStore.addListener(charViewCtrler.onCharInvenStore);
        gm.userStore.addListener(charViewCtrler.onUserListener);
        gm.userStore.addListener(bicycleViewCtrler.onUserStoreListener);
        gm.userStore.addListener(resultViewCtrler.onUserListener);

        gm.ridingStore.addListener(historyViewCtrler.ridingStoreListener);

        gm.ridingStore.addListener(optionCtrler.onRidingListener);
    }

    public void offModal() {
        modal.SetActive(false);
    }
}