using UnityEngine;
using Flux;

public class NetworkCallbackExtention{
    public NetworkManager.Callback networkCallback(QueueDispatcher<Actions> dispatcher, NetworkAction act){
        return (HttpResponse response) => {
            // if(response.isError){
            //     Debug.Log(response.errorMessage);
            //     Debug.Log(response.request.url);
            //     Debug.Log(response.request.responseCode);
            // } else {
                if(response.responseCode>=200 && response.responseCode < 300) {  // request success
                    act.status = NetworkAction.statusTypes.SUCCESS;
                } else {
                    act.status = NetworkAction.statusTypes.FAIL;
                }
                act.response = response;
                dispatcher.dispatch(act);
            // }
        };
    }
}