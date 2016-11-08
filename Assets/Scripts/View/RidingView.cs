using UnityEngine;
using System.Collections;
using System;

public enum LocationState {
    Disabled,
    TimedOut,
    Failed,
    Enabled
}

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

    IEnumerator Start() {
        //GPS 허용이 켜져있지 않으면 종료한다.
        state = LocationState.Disabled;
        if (!Input.location.isEnabledByUser) {
            yield break;
        }

        //locationService를 시작한다.
        Input.location.Start(100,1);

        int maxWait = 20;
        distance = 0;
        time = 0;
        //locationService가 켜지고 있는 상태이거나 최대 대기시간이 아직 되지 않은 경우 대기한다.
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0) {
            yield return new WaitForSeconds(1);
            maxWait--;
        }
        //최대 대기 시간을 초과한 경우 종료한다.
        if(maxWait < 1) {
            state = LocationState.TimedOut;
            Debug.Log("Timed Out");
            yield break;
        }

        //locationService 연결이 실패한 경우 종료한다.
        if(Input.location.status == LocationServiceStatus.Failed) {
            state = LocationState.Failed;
            yield break;
        }

        //location 접근
        else {
            state = LocationState.Enabled;
            StartCoroutine("DistBetweenLoc");
        }
    }

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