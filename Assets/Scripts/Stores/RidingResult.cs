using Flux;
using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Collections;
using UnityEngine.Networking;

public class RidingResult : Store<Actions> {
    public int rewardNum;
    //아이템 획득 결과
    //최고속도, 평균속도, 시간, 거리는 Riging Store의 Data 활용
    public RidingResult(Dispatcher<Actions> _dispatcher):base(_dispatcher){ }
    
    protected override void _onDispatch(Actions action) {
        switch(action.type) {
            case ActionTypes.RIDING_END:
            getServerData();
            break;
        }

    }

    private void getServerData() {
        //서버에 접근하여 data GET  
        string url = "";
        UnityWebRequest www = UnityWebRequest.Get(url);

        if(www.isError) {
            Debug.Log("Error while request");
        }
        else {
            Debug.Log(www.downloadHandler.text);
            byte[] result = www.downloadHandler.data;
        }
    }
}
