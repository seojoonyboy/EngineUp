using Flux;
using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Collections;

public class RidingResult : Store<Actions> {
    public int rewardNum;
    public string url = "http://175.158.15.120:9000/ridings/";
    public bool isSendSucced = false;

    public RidingResult(Dispatcher<Actions> _dispatcher):base(_dispatcher){ }
    
    protected override void _onDispatch(Actions action) {
        switch(action.type) {
            case ActionTypes.RIDING_END:
            break;

            case ActionTypes.RIDING_RESULT:
            RidingResultAction act = action as RidingResultAction;
            //NetworkManager network = NetworkManager.Instance;
            NetworkManager.Instance._send(url,act.nickname,act.data.ToString());
            break;

            case ActionTypes.POST_FAIL:
            isSendSucced = false;
            break;

            case ActionTypes.POST_SUCCESS:
            isSendSucced = true;
            break;            
        }
        _emmetChange();
    }
}