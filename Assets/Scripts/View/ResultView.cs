using UnityEngine;
using System.Collections;
using System.Text;

public class ResultView : MonoBehaviour {
    private Riding ridingStore;

    public UILabel
        totalDist,
        totalTime,
        avgSpeed,
        maxSpeed;

    public UITexture mapTexture;
    private StringBuilder sb;

    private string
        url = "https://maps.googleapis.com/maps/api/staticmap?center=37.881561,127.730199&zoom=14&size=640x400&path=weight:3%7Ccolor:blue%7Cenc:{coaHnetiVjM??_SkM??~R&key=",
        api_key = "AIzaSyBtDjeVHb2nspGojQpo-n-n1mf5_l_o6tk";

    void Start() {
        MainSceneManager msm = Camera.main.GetComponent<MainSceneManager>();
        ridingStore = msm.ridingStore;
        StartCoroutine("setMap");
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

    IEnumerator setMap() {
        makeURL();
        WWW www = new WWW(sb.ToString());
        yield return www;

        mapTexture.material.mainTexture = www.texture;
        mapTexture.transform.localScale = Vector3.one;

        mapTexture.MakePixelPerfect();
    }

    private void makeURL() {
        sb = new StringBuilder();
        sb.Append(url).Append(api_key);
    }
}
