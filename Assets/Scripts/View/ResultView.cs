using UnityEngine;
using System.Collections;
using System.Text;
using System;
using System.Collections;

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

    public void setMapLine(ref ArrayList coordList) {
        string[] lat = new string[coordList.Count];
        string[] lon = new string[coordList.Count];

        for(int i=0; i<coordList.Count;i++) {
            lat[i] = ((coordData)coordList[i]).latitude.ToString();
            lon[i] = ((coordData)coordList[i]).longitude.ToString();
            Debug.Log("Lat : " + lat[i] + ", Lon" + lon[i]);
        }        
        map.GetComponent<MapLine>().drawLine(lat,lon);
    }
}
