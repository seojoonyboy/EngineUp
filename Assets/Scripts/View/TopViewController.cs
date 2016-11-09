﻿using UnityEngine;
using System.Collections;

public class TopViewController : MonoBehaviour {
    void Start() {
        UILabel nickNameLabel = gameObject.transform.Find("InfoPanel/Label").GetComponent<UILabel>();
        //Debug.Log(GameManager.Instance.userStore.nickName);
        nickNameLabel.text = GameManager.Instance.userStore.nickName;
    }
}
