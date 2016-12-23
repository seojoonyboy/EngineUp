using UnityEngine;
using System.Collections;

public class AvatarPartsChangeView : MonoBehaviour {
    GameManager gameManager;
    User userStore;

    void Start() {
        gameManager = GameManager.Instance;
        userStore = gameManager.userStore;
    }

    void OnEnable() {
        SetItemList();
    }

    void SetItemList() {

    }
}
