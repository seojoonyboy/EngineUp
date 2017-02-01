using UnityEngine;

public class StatViewController : MonoBehaviour {
    public UILabel nickNameLabel;
    public Riding ridingStore;
    public User userStore;

    void Start() {
        nickNameLabel.text = userStore.nickName;
    }

    public void onUserListener() {
        nickNameLabel.text = userStore.nickName;
    }
}