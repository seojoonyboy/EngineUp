using UnityEngine;

public class MainSceneTest : MonoBehaviour {
    public GameObject 
        gameManager,
        networkManager;

    void Awake() {
        if(GameManager.Instance == null){
            Instantiate(gameManager);
        }
        var _gameManager = GameManager.Instance;
        //_gameManager.userStore = new User(_gameManager.gameDispatcher);

        //EditNickNameAction act = (EditNickNameAction)ActionCreator.createAction(ActionTypes.EDIT_NICKNAME);

        //act.nickname = "테스트닉네임";
        //GameManager.Instance.gameDispatcher.dispatch(act);

        if(NetworkManager.Instance == null) {
            Instantiate(networkManager);
        }
    }
}
