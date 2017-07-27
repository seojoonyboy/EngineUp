using UnityEngine;
using System.Collections;
using Facebook.Unity;
using System.Collections.Generic;

public class FacebookLogin : MonoBehaviour {
    public StartLoadingSceneManager startLoadingSceneManager;

    //void Start() {
    //    if (!FB.IsInitialized) {
    //        // Initialize the Facebook SDK
    //        FB.Init(InitCallback, OnHideUnity);
    //    }
    //    else {
    //        // Already initialized, signal an app activation App Event
    //        FB.ActivateApp();
    //    }
    //}

    private void InitCallback() {
        if (FB.IsLoggedIn) {
            //Debug.Log("Facebook logged in during init");
            var aToken = Facebook.Unity.AccessToken.CurrentAccessToken;
            //로그인 상태인 경우 바로 Login Action 생성
            SigninAction signInAct = ActionCreator.createAction(ActionTypes.SIGNIN) as SigninAction;
            signInAct.login_type = SignupAction.loginType.FB;
            signInAct.token = aToken.TokenString;
            signInAct.isAutoLogin = true;
            GameManager.Instance.gameDispatcher.dispatch(signInAct);
            //startLoadingSceneManager.loadMainScene();
           
            //Debug.Log("User Token : " + aToken.TokenString);
            //Debug.Log("User Id : " + aToken.UserId);
        }
        else {
            //로그인을 한 상태가 아니라면...
            if (FB.IsInitialized) {
                // Signal an app activation App Event
                FB.ActivateApp();
                // Continue with Facebook SDK
                // ...
                Debug.Log("Initialize the Facebook SDK");
                List<string> permissions = new List<string>();
                FB.LogInWithReadPermissions(permissions, AuthCallback);
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
        if (!FB.IsInitialized) {
            // Initialize the Facebook SDK
            FB.Init(InitCallback, OnHideUnity);
        }
        else {
            // Already initialized, signal an app activation App Event
            FB.ActivateApp();
            List<string> permissions = new List<string>();
            FB.LogInWithReadPermissions(permissions, AuthCallback);
        }
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
                //로그인 액션
                SigninAction signInAct = ActionCreator.createAction(ActionTypes.SIGNIN) as SigninAction;
                signInAct.login_type = SignupAction.loginType.FB;
                signInAct.token = aToken.TokenString;
                GameManager.Instance.gameDispatcher.dispatch(signInAct);
            }
            else {
                Debug.Log("User cancelled login");
            }
        }
    }
}
