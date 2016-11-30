using Flux;
using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Collections;

public class RidingResult : Store<Actions> {
    public int rewardNum;
    public string url = "http://175.158.15.120:9000/ridings/";

    public RidingResult(Dispatcher<Actions> _dispatcher):base(_dispatcher){ }
    
    protected override void _onDispatch(Actions action) {
        switch(action.type) {
            case ActionTypes.RIDING_END:
            break;
        }
    }
}