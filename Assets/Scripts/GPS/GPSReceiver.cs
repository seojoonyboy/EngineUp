#pragma warning disable 0168
#pragma warning disable 0219
#pragma warning disable 0414
using UnityEngine;
using System.Collections;
using System;

public enum LocationState {
    Disabled,
    TimedOut,
    Failed,
    Enabled
}

public class GPSReceiver : MonoBehaviour {
    float time = 0;
    private const float gpsInterval = 1f;
    private LocationState state;
    LocationInfo currGPSInfo;

    void Update() {
        time += Time.deltaTime;
        //Debug.Log(time);
    }

    IEnumerator Start() {
        //GPS 허용이 켜져있지 않으면 종료한다.
        state = LocationState.Disabled;
        //StartCoroutine("getData");
        if (!Input.location.isEnabledByUser) {
            yield break;
        }

        //locationService를 시작한다.
        Input.location.Start(1, 1);

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
            StartCoroutine("getData");
        }
    }

    IEnumerator getData() {
        //Action 생성
        GetGPSDataAction action = (GetGPSDataAction)ActionCreator.createAction(ActionTypes.GET_GPS_DATA);
        GameManager gameManager = GameManager.Instance;
        
        while (true) {
            //Debug.Log("GET_GPS_DATA Action 발생시킴");
            
            TimeSpan timeSpane = TimeSpan.FromSeconds(time);
            string timeText = string.Format("{0:D2}:{1:D2}:{2:D2}", timeSpane.Hours, timeSpane.Minutes, timeSpane.Seconds);
            Debug.Log(timeText);

            currGPSInfo = Input.location.lastData;
            action.GPSInfo = currGPSInfo;
            action.timeText = timeText;
            gameManager.gameDispatcher.dispatch(action);
            yield return new WaitForSeconds(gpsInterval);
        }
    }
}
