using UnityEngine;
using System.Collections;
using System;

public class TopView : MonoBehaviour {
    UILabel nickNameLabel;
    void Start() {
        nickNameLabel = gameObject.transform.Find("InfoPanel/NickName").GetComponent<UILabel>();
        setNickName(GameManager.Instance.userStore.nickName);
    }

    public void setNickName(string nickName) {
        Debug.Log("Change NickName : " + nickName);
        nickNameLabel.text = nickName;
    }
}