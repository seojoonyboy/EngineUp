using UnityEngine;
using System.Collections;
using System;

public class TopViewController : MonoBehaviour {
    void Start() {
        UILabel nickNameLabel = gameObject.transform.Find("InfoPanel/NickName").GetComponent<UILabel>();
        Debug.Log(GameManager.Instance.userStore.nickName);
        GameManager gameManager = GameManager.Instance;
        string nickName = gameManager.userStore.nickName;
        if(String.IsNullOrEmpty(nickName)) {
            return;
        }
        else {
            nickNameLabel.text = GameManager.Instance.userStore.nickName;
        }        
    }


}
