using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class NetworkManager : Singleton<NetworkManager> {
    protected NetworkManager() { }

    public delegate void Callback(HttpResponse response);
    public string baseUrl = "http://175.158.15.120:9000/";

    public void request(string method, string url, WWWForm data, Callback callback){
        StartCoroutine(_request(method, url, data, callback));
    }
    public void request(string method, string url, Callback callback){
        request(method, url, null, callback);
    }

    IEnumerator _request(string method, string url, WWWForm data, Callback callback){
        UnityWebRequest www;
        switch(method){
            case "POST":
                www = UnityWebRequest.Post(url, data.ToString());
                break;
            case "PUT":
                www = UnityWebRequest.Put(url, data.ToString());
                break;
            case "DELETE":
                www = UnityWebRequest.Delete(url);
                break;
            case "GET":
            default:
                www = UnityWebRequest.Get(url);
                break;
        }
        yield return www.Send();
        callback(new HttpResponse(www));
    }

    public void _send(string url,string nickname,string data) {
        StartCoroutine(_sendToServer(url,nickname,data));
    }

    IEnumerator _sendToServer(string url,string nickname,string data) {
        WWWForm form = new WWWForm();
        //Debug.Log(ridingStore.resultData.ToString());
        form.AddField("name",nickname);
        form.AddField("location_name","ChunCheon");
        form.AddField("location_desc","Romantic");
        form.AddField("raw_data",data);

        UnityWebRequest www = UnityWebRequest.Post(url,form);
        yield return www.Send();

        ActionTypes type = ActionTypes.POST_FAIL;
        if(www.isError) {
            type = ActionTypes.POST_FAIL;
            Debug.Log(www.error);
        }

        else {
            type = ActionTypes.POST_SUCCESS;
            Debug.Log("FORM UPLOAD COMPLETE");
        }

        Actions action = ActionCreator.createAction(type);
        GameManager.Instance.gameDispatcher.dispatch(action);
    }
}

public class HttpResponse {
    public bool isError;
    public string errorMessage;
    public string data;
    public long responseCode;
    public UnityWebRequest request;

    public HttpResponse(UnityWebRequest _request){
        request = _request;
        responseCode = _request.responseCode;
        isError = _request.isError;
        errorMessage = _request.error;
        data = _request.downloadHandler.text;
        if (responseCode < 200 || responseCode >= 300) {
            isError = true;
            errorMessage = data;
        }
    }
}