using UnityEngine;
using System.Collections;
using System;

public enum LocationState {
    Disabled,
    TimedOut,
    Failed,
    Enabled
}

public class GPSManager : MonoBehaviour {
    private LocationState state;
    LocationInfo currentGPSPosition;

    private float
        latitude,
        longitude,
        distance,
        time;

    void OnEnable() {
        StartCoroutine("checkGPSConfig");
    }

    void OnDisable() {
        StopCoroutine("checkGPSConfig");
    }

    IEnumerator checkGPSConfig() {
        //GPS 허용이 켜져있지 않으면 종료한다.
        state = LocationState.Disabled;
        if (!Input.location.isEnabledByUser) {
            yield break;
        }

        //locationService를 시작한다.
        Input.location.Start(100, 1);

        int maxWait = 20;
        //locationService가 켜지고 있는 상태이거나 최대 대기시간이 아직 되지 않은 경우 대기한다.
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0) {
            yield return new WaitForSeconds(1);
            maxWait--;
        }
        //최대 대기 시간을 초과한 경우 종료한다.
        if (maxWait < 1) {
            state = LocationState.TimedOut;
            Debug.Log("Timed Out");
            yield break;
        }

        //locationService 연결이 실패한 경우 종료한다.
        if (Input.location.status == LocationServiceStatus.Failed) {
            state = LocationState.Failed;
            yield break;
        }

        //location 접근
        else {
            state = LocationState.Enabled;
            getData();
        }
    }

    private void getData() {
        currentGPSPosition = Input.location.lastData;
        latitude = currentGPSPosition.latitude;
        longitude = currentGPSPosition.longitude;

        //Action 생성
        GetGPSDataAction action = (GetGPSDataAction)ActionCreator.createAction(ActionTypes.GET_GPS_DATA);
        action.info = currentGPSPosition;
        GameManager.Instance.gameDispatcher.dispatch(action);
    }
}
