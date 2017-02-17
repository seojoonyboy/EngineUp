using UnityEngine;
using System.Collections;

public class FriendsProfileViewController : MonoBehaviour {
    public int id = -1;
    public string nickName = "할당 안됨";

    private UILabel nickNameLabel;


    void Awake() {
        nickNameLabel = gameObject.transform.Find("HeaderPanel/NickName").GetComponent<UILabel>();
    }

    public void offPanel() {
        gameObject.SetActive(false);
    }

    public void setInfo(GameObject obj) {
        FriendIndex friendIndex = obj.GetComponent<FriendIndex>();
        id = friendIndex.id;
        nickName = friendIndex.nickName;
        nickNameLabel.text = friendIndex.nickName;
    }
}
