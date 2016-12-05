using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class NetworkManager : Singleton<NetworkManager> {
    protected NetworkManager() { }

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

        if(www.isError) {
            Debug.Log(www.error);
        }

        else {
            Debug.Log("FORM UPLOAD COMPLETE");
        }
    }
}