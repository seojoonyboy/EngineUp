using UnityEngine;
using System.Collections;

public class NormalLogin : MonoBehaviour {
    void Start() {
        //sendSignInAction();
    }

    public void onLoginButton() {
        sendSignInAction();
    }

    private void sendSignInAction() {
        SigninAction signInAct = ActionCreator.createAction(ActionTypes.SIGNIN) as SigninAction;
        signInAct.login_type = SignupAction.loginType.NO;
        GameManager.Instance.gameDispatcher.dispatch(signInAct);
    }
}
