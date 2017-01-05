public class MainSceneManager : fbl_SceneManager {
    public Result_VC resultViewCtrler;
    public Riding_VC ridingViewCtrler;
    private GameManager gm;

    void Awake(){        
        GetCommunityAction act = ActionCreator.createAction(ActionTypes.GET_COMMUNITY_DATA) as GetCommunityAction;
        GameManager.Instance.gameDispatcher.dispatch(act);
    }

    void Start() {
        gm = GameManager.Instance;
        initialize();
    }

    void initialize() {
        resultViewCtrler.ridingStore = gm.ridingStore;
        gm.ridingStore.addListener(resultViewCtrler.onRidingListener);

        ridingViewCtrler.ridingStore = gm.ridingStore;
        ridingViewCtrler.userStore = gm.userStore;
        gm.ridingStore.addListener(ridingViewCtrler.onRidingListener);
    }
}