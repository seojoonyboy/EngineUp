using UnityEngine;

public class Top_VC : MonoBehaviour {
    UILabel nickNameLabel;
    private Riding ridingStore;
    private User userStore;

    void Start() {
        ridingStore = GameManager.Instance.ridingStore;
        userStore = GameManager.Instance.userStore;
    }

    void onUserListener() {

    }
}