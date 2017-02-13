using UnityEngine;

public class StatViewController : MonoBehaviour {
    public UILabel nickNameLabel;
    public Riding ridingStore;
    public User userStore;

    void Start() {
        nickNameLabel.text = userStore.nickName;
        GameManager.Instance.userStore.addListener(onUserListener);
    }

    public void onUserListener() {
        nickNameLabel.text = userStore.nickName;
    }
}