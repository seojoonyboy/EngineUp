using UnityEngine;
using System.Collections;

public class NormalLogin : MonoBehaviour {
    void Start() {
        //sendSignInAction();
    }

    public void onLoginButton(bool isAutoLogin = false) {
        sendSignInAction(isAutoLogin);
    }

    private void sendSignInAction(bool isAutoLogin) {
        SigninAction signInAct = ActionCreator.createAction(ActionTypes.SIGNIN) as SigninAction;
        signInAct.login_type = SignupAction.loginType.NO;
        signInAct.isAutoLogin = isAutoLogin;
        GameManager.Instance.gameDispatcher.dispatch(signInAct);
    }
}
