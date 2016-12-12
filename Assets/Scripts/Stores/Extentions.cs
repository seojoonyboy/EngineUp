using UnityEngine;
using Flux;

public class NetworkCallbackExtention{
    public NetworkManager.Callback networkCallback(Dispatcher<Actions> dispatcher, NetworkAction act){
        return (HttpResponse response) => {
            if(response.isError){
                Debug.Log(response.errorMessage);
            } else {
                if(response.responseCode>=200 && response.responseCode < 300) {  // request success
                    act.status = NetworkAction.statusTypes.SUCCESS;
                } else {
                    act.status = NetworkAction.statusTypes.FAIL;
                }
                act.response = response;
                dispatcher.dispatch(act);
            }
        };
    }
}