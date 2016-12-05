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