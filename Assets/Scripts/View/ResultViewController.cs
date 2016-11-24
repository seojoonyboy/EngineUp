using UnityEngine;
using System.Collections;

public class ResultViewController : MonoBehaviour {
    private Riding ridingStore;
    void Start() {
        UILabel totalDistLabel = gameObject.transform.Find("").GetComponent<UILabel>();

        MainSceneManager msm = Camera.main.GetComponent<MainSceneManager>();
        ridingStore = msm.ridingStore;
        
        //totalDistLabel.text = GameManager.Instance.resultStore.rewardNum.ToString();
    }
}
