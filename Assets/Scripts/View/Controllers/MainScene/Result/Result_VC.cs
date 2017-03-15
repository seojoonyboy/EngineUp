using UnityEngine;
using System.Collections;
using System;

public class Result_VC : MonoBehaviour {

    public UILabel
        totalDist,
        totalTime,
        avgSpeed,
        maxSpeed;

    public GameObject map;
    public Riding ridingStore;

    void OnDisable() {
        map.GetComponent<OnlineMaps>().RemoveAllDrawingElements();
        gameObject.SetActive(false);
    }

    public void onRidingListener() {
        //Debug.Log("On Riding Listener in Result VC");
        if (ridingStore.eventType == ActionTypes.RIDING_END) {
            onResultPanel();
            setResult(ridingStore.totalDist, ridingStore.totalTime, ridingStore.avgSpeed, ridingStore.maxSpeed);
            setMapLine(ref ridingStore.coordList);
        }
    }

    public void setResult(float mDist, string mTime, float mAvgSpeed, float mMaxSpeed) {        
        totalDist.text = (Math.Round(mDist, 2, MidpointRounding.AwayFromZero)).ToString() + " KM";

        char delimeter = '.';
        totalTime.text = mTime.ToString().Split(delimeter)[0];

        avgSpeed.text = (Math.Round(mAvgSpeed, 2, MidpointRounding.AwayFromZero)).ToString() + " KM/H";
        maxSpeed.text = (Math.Round(mMaxSpeed, 2, MidpointRounding.AwayFromZero)).ToString() + " KM/H";
    }

    public void setMapLine(ref ArrayList coordList) {
        float[] lat = new float[coordList.Count];
        float[] lon = new float[coordList.Count];

        for(int i=0; i<coordList.Count;i++) {
            lat[i] = ((coordData)coordList[i]).latitude;
            lon[i] = ((coordData)coordList[i]).longitude;
            Debug.Log("Lat : " + lat[i] + ", Lon" + lon[i]);
        }
        map.GetComponent<MapLine>().drawLine(lat,lon);
    }

    public void onResultPanel() {
        gameObject.SetActive(true);
    }

    public void offResultPanel() {
        OnDisable();
    }
}
