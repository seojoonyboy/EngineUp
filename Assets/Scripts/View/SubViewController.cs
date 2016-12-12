using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class SubViewController : MonoBehaviour {
    public GameObject
        resultPanel;

    private ResultView resultView;
    private Riding ridingStore;

    void Start() {
        MainSceneManager msm = Camera.main.GetComponent<MainSceneManager>();
        ridingStore = msm.ridingStore;

        addListener();
    }

    public void onResultPanel() {
        resultPanel.SetActive(true);
    }

    public void offResultPanel() {
        resultPanel.SetActive(false);
    }

    void addListener() {
        resultView = resultPanel.GetComponent<ResultView>();
        //store에 리스너를 등록
        //ridingResultStore.addListener(resultListener);
    }

    void resultListener() {
        Debug.Log("Result Listener");
        resultView.setResult(ridingStore.totalDist,ridingStore.totalTime,ridingStore.avgSpeed,ridingStore.maxSpeed);
        resultView.setMapLine(ridingStore.resultData);
    }
}
