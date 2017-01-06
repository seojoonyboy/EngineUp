public class MainSceneManager : fbl_SceneManager {
    public Result_VC resultViewCtrler;
    public Riding_VC ridingViewCtrler;
    public Community_VC communityViewCtrler;

    private GameManager gm;

    void Start() {
        gm = GameManager.Instance;
        initialize();

        GetCommunityAction act = ActionCreator.createAction(ActionTypes.GET_COMMUNITY_DATA) as GetCommunityAction;
        act.type = GetCommunityAction.requestType.ALL;
        GameManager.Instance.gameDispatcher.dispatch(act);
    }

    void initialize() {
        resultViewCtrler.ridingStore = gm.ridingStore;
        ridingViewCtrler.ridingStore = gm.ridingStore;
        ridingViewCtrler.userStore = gm.userStore;
        communityViewCtrler.userStore = gm.userStore;

        gm.userStore.addListener(communityViewCtrler.onUserListener);
        gm.ridingStore.addListener(resultViewCtrler.onRidingListener);
        gm.ridingStore.addListener(ridingViewCtrler.onRidingListener);
    }
}