using UnityEngine;
using System.Collections;
using Facebook.Unity;
using System.Collections.Generic;

public class FacebookLogin : MonoBehaviour {
    public GameObject profileModal;
    public GameObject label;
    public GameObject image;
    public StartLoadingSceneManager startLoadingSceneManager;

    void Awake() {
        if (!FB.IsInitialized) {
            // Initialize the Facebook SDK
            FB.Init(InitCallback, OnHideUnity);
        }
        else {
            // Already initialized, signal an app activation App Event
            FB.ActivateApp();
        }
    }

    private void InitCallback() {
        if (FB.IsLoggedIn) {
            Debug.Log("Facebook logged in during init");
            startLoadingSceneManager.loadMainScene();
            var aToken = Facebook.Unity.AccessToken.CurrentAccessToken;
            Debug.Log("User Token : " + aToken.TokenString);
            Debug.Log("User Id : " + aToken.UserId);
        }
        else {
            //로그인을 한 상태가 아니라면...
            if (FB.IsInitialized) {
                // Signal an app activation App Event
                FB.ActivateApp();
                // Continue with Facebook SDK
                // ...
                Debug.Log("Initialize the Facebook SDK");
            }
            else {
                Debug.Log("Failed to Initialize the Facebook SDK");
            }
        }
    }

    private void OnHideUnity(bool isGameShown) {
        if (!isGameShown) {
            // Pause the game - we will need to hide
            Time.timeScale = 0;
        }
        else {
            // Resume the game - we're getting focus again
            Time.timeScale = 1;
        }
    }

    //if Facebook login button clicked
    public void FBlogin() {
        List<string> permissions = new List<string>();
        FB.LogInWithReadPermissions(permissions, AuthCallback);
    }

    private void AuthCallback(IResult result) {
        if (result.Error != null) {
            Debug.Log(result.Error);
        }
        else {
            if (FB.IsLoggedIn) {
                //sign in url
                // AccessToken class will have session details
                var aToken = Facebook.Unity.AccessToken.CurrentAccessToken;
                // Print current access token's User ID
                Debug.Log("FB is logged in");
                Debug.Log(aToken.UserId);
                Debug.Log(aToken.TokenString);
                // Print current access token's granted permissions
                foreach (string perm in aToken.Permissions) {
                    Debug.Log(perm);
                }
                startLoadingSceneManager.loadMainScene();
                //profileModal.SetActive(true);
                //gameObject.transform.parent.gameObject.SetActive(false);

                FB.API("/me?fields=first_name", HttpMethod.GET, displayUserName);
                FB.API("/me/picture?type=square&height=128&width=128", HttpMethod.GET, displayProfileImg);
            }
            else {
                //user 토큰이 존재하지 않는 경우
                //sign up url
                Debug.Log("User cancelled login");
            }
        }
    }

    private void displayUserName(IResult result) {
        if (result.Error == null) {
            label.GetComponent<UILabel>().text = "Hi, " + result.ResultDictionary["first_name"];
        }
        else {
            Debug.Log(result.Error);
        }
    }

    private void displayProfileImg(IGraphResult result) {
        if (result.Texture != null) {
            image.GetComponent<UITexture>().mainTexture = result.Texture;
        }
    }
}
