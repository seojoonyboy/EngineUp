public class MainSceneManager : SceneManager {
    public Riding ridingStore;
    void Start(){
        ridingStore = new Riding(GameManager.Instance.gameDispatcher);
    }
}
