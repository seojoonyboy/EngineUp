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
        if (userStore.eventType == ActionTypes.USER_CREATE) {
            setNickName(userStore.nickName);
        }
    }

    public void setNickName(string nickName) {
        nickNameLabel.text = nickName;
    }

    public void Scroll() {
        if (isOver) {
            isOver = false;
            if (isDown) {
                anim.Play("Up");
            }
            else {
                anim.Play("Down");
            }
        }
    }

    public void scrollDownDone() {
        isOver = true;
        isDown = true;
        arrows[0].GetComponent<UISprite>().flip = UIBasicSprite.Flip.Vertically;
        arrows[1].GetComponent<UISprite>().flip = UIBasicSprite.Flip.Vertically;
    }

    public void scrollUpDone() {
        isOver = true;
        isDown = false;
        arrows[0].GetComponent<UISprite>().flip = UIBasicSprite.Flip.Nothing;
        arrows[1].GetComponent<UISprite>().flip = UIBasicSprite.Flip.Nothing;
    }
}
