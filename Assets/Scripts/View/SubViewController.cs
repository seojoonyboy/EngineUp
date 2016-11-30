using UnityEngine;
using System.Collections;

public class SubViewController : MonoBehaviour {
    public GameObject
        resultPanel;

    public ResultView resultView;
    private Riding ridingStore;

    void Start() {
        MainSceneManager msm = Camera.main.GetComponent<MainSceneManager>();
        ridingStore = msm.ridingStore;

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
        resultView.setResult(ridingStore.totalDist, ridingStore.totalTime, ridingStore.avgSpeed, ridingStore.maxSpeed, ridingStore.resultData);
    }
}
