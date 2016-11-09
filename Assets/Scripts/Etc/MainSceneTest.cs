using UnityEngine;

public class MainSceneTest : MonoBehaviour {
    public GameObject gameManager;
    void Awake() {
        if(GameManager.Instance == null){
            Instantiate(gameManager);
        }
        var _gameManager = GameManager.Instance;
        _gameManager.userStore = new User(_gameManager.gameDispatcher);

        EditNickNameAction act = (EditNickNameAction)ActionCreator.createAction(ActionTypes.EDIT_NICKNAME);

        act.nickname = "테스트닉네임";
        GameManager.Instance.gameDispatcher.dispatch(act);
    }
}
