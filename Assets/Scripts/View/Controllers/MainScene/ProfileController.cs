using UnityEngine;
using System.Collections;

public class ProfileController : MonoBehaviour {
    public UILabel nickNameLabel;
    private Riding ridingStore;
    private User userStore;
    Animation anim;
    public GameObject[] arrows;

    public bool
        isOver,
        isDown;
    void Start() {
        ridingStore = GameManager.Instance.ridingStore;
        userStore = GameManager.Instance.userStore;

        userStore.addListener(onUserListener);

        anim = GetComponent<Animation>();
        isOver = true;
        isDown = false;
    }

    void onUserListener() {
        nickNameLabel.text = userStore.nickName;
    }

    public void setNickName(string nickName) {
        nickNameLabel.text = nickName;
    }
}
