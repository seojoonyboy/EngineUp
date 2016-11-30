public class MainSceneManager : SceneManager {
    public Riding ridingStore;
    public RidingResult resultStore;
    void Start(){
        ridingStore = new Riding(GameManager.Instance.gameDispatcher);
        resultStore = new RidingResult(GameManager.Instance.gameDispatcher);
    }
}
