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
        ridingResultStore.addListener(resultListener);
    }

    void resultListener() {
        Debug.Log("Result Listener");
        resultView.setResult(ridingStore.totalDist,ridingStore.totalTime,ridingStore.avgSpeed,ridingStore.maxSpeed, ridingResultStore.isSendSucced);
        resultView.setMapLine(ridingStore.resultData);
    }
}
