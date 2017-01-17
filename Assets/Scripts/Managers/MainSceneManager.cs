public class MainSceneManager : fbl_SceneManager {
    public Result_VC resultViewCtrler;
    public Riding_VC ridingViewCtrler;
    public FriendsViewController friendViewCtrler;
    public GroupViewController groupViewCtrler;

    private GameManager gm;

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

        gm.friendsStore.addListener(friendViewCtrler.OnFriendsStoreListener);
        gm.ridingStore.addListener(resultViewCtrler.onRidingListener);
        gm.ridingStore.addListener(ridingViewCtrler.onRidingListener);
        gm.groupStore.addListener(groupViewCtrler.OnGroupStoreListener);

        CommunityInitAction act = ActionCreator.createAction(ActionTypes.COMMUNITY_INITIALIZE) as CommunityInitAction;
        gm.gameDispatcher.dispatch(act);
    }
}