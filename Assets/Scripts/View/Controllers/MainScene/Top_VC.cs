using UnityEngine;

public class Top_VC : MonoBehaviour {
    UILabel nickNameLabel;
    private Riding ridingStore;
    private User userStore;

    void Start() {
        ridingStore = GameManager.Instance.ridingStore;
        userStore = GameManager.Instance.userStore;

        nickNameLabel = gameObject.transform.Find("InfoPanel/NickName").GetComponent<UILabel>();
        userStore.addListener(onUserListener);
    }

    void onUserListener() {
        if(userStore.eventType == ActionTypes.USER_CREATE) {
            setNickName(userStore.nickName);
        }
    }

    public void setNickName(string nickName) {
        nickNameLabel.text = nickName;
    }
}