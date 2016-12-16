public class MainSceneManager : fbl_SceneManager {
    public Riding ridingStore;
    void Start(){
        ridingStore = new Riding(GameManager.Instance.gameDispatcher);
        // resultStore = new RidingResult(GameManager.Instance.gameDispatcher);
    }
}
