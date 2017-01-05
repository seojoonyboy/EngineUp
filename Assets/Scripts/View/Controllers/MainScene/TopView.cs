using UnityEngine;

public class TopView : MonoBehaviour {
    UILabel nickNameLabel;
    void Start() {
        nickNameLabel = gameObject.transform.Find("InfoPanel/NickName").GetComponent<UILabel>();
        setNickName(GameManager.Instance.userStore.nickName);
    }

    public void setNickName(string nickName) {
        //Log("Change NickName : " + nickName);
        nickNameLabel.text = nickName;
    }
}