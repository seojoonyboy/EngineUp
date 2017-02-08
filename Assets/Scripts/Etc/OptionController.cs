using UnityEngine;
using System.Collections;
using Facebook.Unity;

public class OptionController : MonoBehaviour {

    public void close() {
        gameObject.SetActive(false);
    }

    public void logOut() {
        FB.LogOut(); 
        Debug.Log("로그아웃 버튼 클릭");
        
    }
}
