using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class NetworkManager : Singleton<NetworkManager> {
    protected NetworkManager() { }

    public delegate void Callback(HttpResponse response);
    public string baseUrl = "http://52.78.149.126:8000/";

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
                www = UnityWebRequest.Post(url, data);
                break;
            case "PUT":
                www = UnityWebRequest.Put(url,data.data);
                www.SetRequestHeader("Content-Type","application/x-www-form-urlencoded");
                break;
            case "DELETE":
                www = UnityWebRequest.Delete(url);
                www.downloadHandler = new DownloadHandlerBuffer();
                break;
            case "GET":
            default:
                www = UnityWebRequest.Get(url);
                break;
        }
        yield return www.Send();
        callback(new HttpResponse(www));
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
    }
}