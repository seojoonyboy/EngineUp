using UnityEngine;
using System.Collections;

public class ResultViewController : MonoBehaviour {
    private Riding ridingStore;

    public UILabel
        totalDist,
        totalTime,
        avgSpeed,
        maxSpeed;

    void Start() {
        MainSceneManager msm = Camera.main.GetComponent<MainSceneManager>();
        ridingStore = msm.ridingStore;
        //setResult();
        //totalDistLabel.text = GameManager.Instance.resultStore.rewardNum.ToString();
    }

    void setResult() {
        totalDist.text = ridingStore.totalDist.ToString() + " KM";

        char delimeter = '.';
        totalTime.text = ridingStore.totalTime.ToString().Split(delimeter)[0];

        avgSpeed.text = ridingStore.avgSpeed.ToString() + " KM/H";
        maxSpeed.text = ridingStore.maxSpeed.ToString() + " KM/H";
    }
}
