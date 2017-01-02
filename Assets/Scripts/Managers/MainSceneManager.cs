public class MainSceneManager : fbl_SceneManager {
    public Riding ridingStore;
    void Start(){
        ridingStore = new Riding(GameManager.Instance.gameDispatcher);
        GetCommunityAction act = ActionCreator.createAction(ActionTypes.GET_COMMUNITY_DATA) as GetCommunityAction;
        GameManager.Instance.gameDispatcher.dispatch(act);
        // resultStore = new RidingResult(GameManager.Instance.gameDispatcher);
    }
}
