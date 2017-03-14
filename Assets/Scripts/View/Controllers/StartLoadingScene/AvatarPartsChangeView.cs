#pragma warning disable 0168
#pragma warning disable 0219
#pragma warning disable 0414
using UnityEngine;

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
