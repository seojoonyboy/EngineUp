#pragma warning disable 0168
#pragma warning disable 0219
#pragma warning disable 0414
using UnityEngine;
using System;
using System.Collections;

public class Riding_VC : MonoBehaviour {
    public MainBtnController mainBtnCtrler;
    public TutotrialManager tutManager;
    public GameObject 
        gpsPref,
        pauseModal,
        StartPanel,
        exitModal,
        beforeStartModal_ButtonContainer,
        beforeStartModal_AnimContainer,
        ridingPanel,
        pauseBtn,
        exitBtn;

    private GameObject gpsReceiver;

    private bool isPausePressed = false;

    //private GameObject gpsManager;
    private GameManager gameManager;

    public UILabel
        currSpeedLabel,
        avgSpeedLabel,
        distLabel,
        maxLabel,
        timeLabel,
        uphillDistanceLabel,
        boxLabel;

    public Riding ridingStore;
    public User userStore;

    public UISlider slider;
    public int sliderMaxValue;

    private int boxNum = 0;
    void Start() {
        gameManager = GameManager.Instance;
        //gosReceiver = Instantiate(gpsPref);
    }

    void OnEnable() {
        avgSpeedLabel.text = "0";
        distLabel.text = "0";
        maxLabel.text = "0";
        uphillDistanceLabel.text = "0";
    }

    void OnDisable() {
        isPausePressed = false;
        pauseModal.SetActive(isPausePressed);
        beforeStartModal_AnimContainer.SetActive(false);
        StartPanel.SetActive(true);

        //mainBtnCtrler.offToggleGroup();
    }

    //라이딩 시작 취소 버튼 클릭시
    public void onRidingCancelButton() {
        gameObject.SetActive(false);
    }

    //라이딩 시작 버튼 클릭시
    public void onRidingStartButton(bool isTutorial = false) {
        offBeforeStartModal();
        beforeStartModal_AnimContainer.SetActive(true);
        //초기 위칫값을 지정한다. (받은 값들의 평균)
        gpsReceiver = Instantiate(gpsPref);
        if(isTutorial) {
            StartCoroutine(freeze(5.0f));
        }
        MyInfo act = ActionCreator.createAction(ActionTypes.MYINFO) as MyInfo;
        act._type = MyInfo.type.RIDING_START;
        gameManager.gameDispatcher.dispatch(act);
    }

    IEnumerator freeze(float interval) {
        float val = interval;
        while(val >= 0.0f) {
            val -= 1.0f;
            yield return new WaitForSeconds(1.0f);
        }
        pauseButtonPressed();
        tutManager.ridingPaused();
        Debug.Log("Freezing");
    }

    //라이딩 종료 버튼 눌렀을 때
    //모달 활성화
    public void onRidingEndButton() {
        exitModal.SetActive(true);
        exitModal.transform.Find("Modal/Description").GetComponent<UILabel>().text = "지금 종료하시면 \n총 " + boxNum + "개의 상자를 얻을 수 있습니다.";
        //라이딩 일시정지
        Time.timeScale = 0;
    }

    public void offBeforeStartModal() {
        beforeStartModal_ButtonContainer.SetActive(false);
    }

    public void refreshTxt(float currSpeed, float avgSpeed,double dist, string time, float maxSpeed, float uphillDist, int boxNum) {
        //Debug.Log("RIDING LISTENER");
        //currSpeedLabel.text = (Math.Round(currSpeed, 2, MidpointRounding.AwayFromZero)).ToString() + " KM/H";
        avgSpeedLabel.text = (Math.Round(avgSpeed,2,MidpointRounding.AwayFromZero)).ToString();
        distLabel.text = (Math.Round(dist,2,MidpointRounding.AwayFromZero)).ToString();
        timeLabel.text = time;
        maxLabel.text = (Math.Round(maxSpeed, 2, MidpointRounding.AwayFromZero)).ToString();
        uphillDistanceLabel.text = (Math.Round(uphillDist, 2, MidpointRounding.AwayFromZero)).ToString();
        boxLabel.text = "X " + boxNum;
        this.boxNum = boxNum;
    }

    private void sliderRefresh(double dist) {
        float _dist = (float)dist;
        if(_dist < sliderMaxValue) {
            slider.value = _dist / sliderMaxValue;
        }
        else {
            float remainder = (_dist % sliderMaxValue) / sliderMaxValue;
            slider.value = remainder;
        }
    }

    //최종적으로 종료 모달에서 종료 버튼을 눌렀을 때
    public void ridingEnd() {
        //offToggleGroup();
        gameObject.SetActive(false);
        exitModal.SetActive(false);

        RidingEndAction action = (RidingEndAction)ActionCreator.createAction(ActionTypes.RIDING_END);
        gameManager.gameDispatcher.dispatch(action);

        stopGPSReceive();

        Time.timeScale = 1;
    }

    //종료 모달에서 취소 버튼을 눌렀을 때
    public void onCancelExitButton() {
        //offToggleGroup();
        Time.timeScale = 1;
        exitModal.SetActive(false);
    }

    public void stopGPSReceive() {
        Destroy(gpsReceiver);
    }

    public IEnumerator ridingStart() {
        ridingPanel.SetActive(true);
        yield return new WaitForSeconds(1);
        StartPanel.SetActive(false);
        beforeStartModal_ButtonContainer.SetActive(true);

        Actions act = ActionCreator.createAction(ActionTypes.RIDING_START);
        GameManager.Instance.gameDispatcher.dispatch(act);

        gpsReceiver.GetComponent<GPSReceiver>().isFirstLoc = false;
        //gpsReceiver = Instantiate(gpsPref);
    }

    public void pauseButtonPressed(bool isTutorial = false) {
        //이미 일시정지 버튼을 누른 상태인 경우
        if (isPausePressed) {
            isPausePressed = false;
            Time.timeScale = 1;
            Debug.Log("RESUME");
        }
        else {
            GetGPSDataAction stopAct = (GetGPSDataAction)ActionCreator.createAction(ActionTypes.GET_GPS_DATA);
            LocationInfo info = Input.location.lastData;
            coordData data = new coordData(info.longitude, info.latitude, info.altitude, info.timestamp, info.horizontalAccuracy, info.verticalAccuracy);
            stopAct.GPSInfo = data;
            stopAct.isStop = true;
            gameManager.gameDispatcher.dispatch(stopAct);
            isPausePressed = true;
            Time.timeScale = 0;
            Debug.Log("PAUSED");
        }
        if(!isTutorial) {
            pauseModal.SetActive(isPausePressed);
        }
    }

    public void onRidingListener() {
        float currSpeed = ridingStore.curSpeed;
        float avgSpeed = ridingStore.avgSpeed;
        float maxSpeed = ridingStore.maxSpeed;
        float uphillDistance = ridingStore.uphillDistance;
        double dist = Math.Round(ridingStore.totalDist, 2);
        int boxNum = ridingStore.boxes;
        char delimeter = '.';
        //string time = ridingStore.totalTime.ToString().Split(delimeter)[0];
        string time = ridingStore.totalTime;
        refreshTxt(currSpeed, avgSpeed, dist, time, maxSpeed, uphillDistance, boxNum);
        sliderRefresh(dist);
    }

    void offToggleGroup() {
        UIToggle toggle = UIToggle.GetActiveToggle(5);
        toggle.value = false;
    }
}