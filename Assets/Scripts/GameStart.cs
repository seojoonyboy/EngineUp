using UnityEngine;
public class GameStart : MonoBehaviour {
    public GameObject gameManager;

    void Awake() {
        if(GameManager.Instance == null){
            Instantiate(gameManager);
        }
    }

    void Start() {

    }
}
