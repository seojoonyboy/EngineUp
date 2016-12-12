using UnityEngine;
using System.Collections;
using System.Text;
using System;

public class ResultView : MonoBehaviour {

    public UILabel
        totalDist,
        totalTime,
        avgSpeed,
        maxSpeed,
        reultTestLabel;

    public float
        centerLog,
        centerLat,
        zoomLV,
        mWidth,
        mHeight;

    public GameObject map;

    public void setResult(float mDist, TimeSpan mTime, float mAvgSpeed, float mMaxSpeed) {
        totalDist.text = mDist.ToString() + " KM";

        char delimeter = '.';
        totalTime.text = mTime.ToString().Split(delimeter)[0];

        avgSpeed.text = mAvgSpeed.ToString() + " KM/H";
        maxSpeed.text = mMaxSpeed.ToString() + " KM/H";
    }

    public void setMapLine(StringBuilder sb) {
        string[] arr = sb.ToString().Split('\n');

        string[] lat = new string[arr.Length];
        string[] lon = new string[arr.Length];

        for(int i=0; i<arr.Length-1;i++) {
            string[] tmp = arr[i].Split('|');
            lat[i] = tmp[1];
            lon[i] = tmp[2];
        }
        map.GetComponent<MapLine>().drawLine(lat,lon);
    }
}
