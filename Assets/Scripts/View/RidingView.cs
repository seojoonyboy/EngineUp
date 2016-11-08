using UnityEngine;
using System.Collections;
using System;

public class RidingView : MonoBehaviour {
    public UILabel 
        latitudeLabel,
        longitudeLabel,
        accuracyLabel,
        distLabel,
        timeLabel;
    private float
        latitude,
        longitude,
        distance,
        time;
    private const float EARTH_RADIUS = 6371;
    private LocationState state;
    LocationInfo currentGPSPosition;

    IEnumerator DistBetweenLoc() {
        while(state == LocationState.Enabled) {
            currentGPSPosition = Input.location.lastData;
            latitude = currentGPSPosition.latitude;
            longitude = currentGPSPosition.longitude;
            //3초 간격으로 위치 갱신, 거리 계산
            yield return new WaitForSeconds(3.0f);            

            float deltaDist = calcDist(ref latitude, ref longitude) * 1000f;

            if (deltaDist > 0) {
                distance += deltaDist;
            }

            distLabel.text = distance.ToString() + "m";
            latitudeLabel.text = latitude.ToString();
            longitudeLabel.text = longitude.ToString();
            accuracyLabel.text = currentGPSPosition.horizontalAccuracy.ToString();
        }
    }

    float calcDist(ref float lastLatitude, ref float lastLongitude) {
        currentGPSPosition = Input.location.lastData;

        float newLatitude = currentGPSPosition.latitude;
        float newLongitude = currentGPSPosition.longitude;

        float deltaLatitude = (newLatitude - lastLatitude) * Mathf.Deg2Rad;
        float deltaLongitude = (newLongitude - lastLongitude) * Mathf.Deg2Rad;

        float a = Mathf.Pow(Mathf.Sin(deltaLongitude / 2), 2) 
            + Mathf.Cos(lastLatitude * Mathf.Deg2Rad) * Mathf.Cos(newLatitude * Mathf.Deg2Rad)
            * Mathf.Pow(Mathf.Sin(deltaLongitude / 2), 2);

        lastLatitude = newLatitude;
        lastLongitude = newLongitude;

        float c = 2 * Mathf.Atan2(Mathf.Sqrt(a), Mathf.Sqrt(1 - a));

        return EARTH_RADIUS * c;
    }
}