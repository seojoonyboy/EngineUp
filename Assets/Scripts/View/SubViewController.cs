using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class SubViewController : MonoBehaviour {
    public GameObject
        resultPanel;

    public ResultView resultView;
    private Riding ridingStore;
    private RidingResult ridingResultStore;

    void Start() {
        MainSceneManager msm = Camera.main.GetComponent<MainSceneManager>();
        ridingStore = msm.ridingStore;
        ridingResultStore = msm.resultStore;

        addListener();
    }

    public void onResultPanel() {
        resultPanel.SetActive(true);
    }

    void addListener() {
        resultView = resultPanel.GetComponent<ResultView>();
        //store에 리스너를 등록
        ridingStore.addListener(resultListener);        
    }

    void resultListener() {
        Debug.Log("RESULT VIEW RIDING LISTENER");
        resultView.setResult(ridingStore.totalDist, ridingStore.totalTime, ridingStore.avgSpeed, ridingStore.maxSpeed);
        //StartCoroutine("_sendToServer");
    }

    IEnumerator _sendToServer() {
        WWWForm form = new WWWForm();
        Debug.Log(ridingStore.resultData.ToString());
        form.AddField("name","seojoonwon");
        form.AddField("location_name","ChunCheon");
        form.AddField("location_desc","Romantic");
        form.AddField("raw_data",ridingStore.resultData.ToString());

        UnityWebRequest www = UnityWebRequest.Post(ridingResultStore.url,form);
        yield return www.Send();

        if(www.isError) {
            Debug.Log(www.error);
        }

        else {
            Debug.Log("FORM UPLOAD COMPLETE");
        }
    }
}
