using UnityEngine;

public class StatViewController : MonoBehaviour {
    public UILabel nickNameLabel;
    public Riding ridingStore;
    public User userStore;

    public void onUserListener() {
        nickNameLabel.text = userStore.nickName;
    }

    void Start() {
        nickNameLabel.text = GameManager.Instance.userStore.nickName;
    }
}