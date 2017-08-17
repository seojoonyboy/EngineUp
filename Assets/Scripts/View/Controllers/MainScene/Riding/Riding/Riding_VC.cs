#pragma warning disable 0168
#pragma warning disable 0219
#pragma warning disable 0414
using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;

public class Riding_VC : MonoBehaviour {
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
    private SoundManager sm;

    public Text
        avgSpeedLabel,
        distLabel,
        maxLabel,
        timeLabel,
        uphillDistanceLabel,
        boxLabel,
        ridingStart_questionLabel;

    public Riding ridingStore;
    public User userStore;

    public Slider slider;

    private int boxNum = 0;

    private TweenPosition tP;
    private bool
        isReverse_tp = false,
        isTweening = false;
    void Start() {
        gameManager = GameManager.Instance;
        sm = SoundManager.Instance;
    }

    void Awake() {
        tP = StartPanel.GetComponent<TweenPosition>();
    }

    void OnEnable() {
        tweenPos();
    }

    void OnDisable() {
        reset();
    }

    private void reset() {
        StartPanel.SetActive(true);
        ridingPanel.SetActive(false);

        isPausePressed = false;
        pauseModal.SetActive(isPausePressed);

        beforeStartModal_ButtonContainer.SetActive(true);
        beforeStartModal_AnimContainer.SetActive(false);

        tP.from = new Vector3(0, -1920, 0);
        tP.to = Vector3.zero;
        
        avgSpeedLabel.text = "0.0";
        distLabel.text = "0.0";
        maxLabel.text = "0.0";
        uphillDistanceLabel.text = "00";
        timeLabel.text = "00:00:00";
        slider.value = 0;
        slider.transform.Find("Num").GetComponent<Text>().text = 0 + "/" + slider.maxValue;

        ridingStart_questionLabel.text = "라이딩을 시작할까요?";

        isReverse_tp = false;

        resetToggle();
    }

    public void tweenPos() {
        if (isTweening) {
            return;
        }
        isTweening = true;
        if (!isReverse_tp) {
            tP.ResetToBeginning();
            tP.PlayForward();
        }
        else {
            //swap
            Vector3 tmp;
            tmp = tP.to;
            tP.to = tP.from;
            tP.from = tmp;

            tP.ResetToBeginning();
            tP.PlayForward();
        }
    }

    public void tPFinished() {
        isTweening = false;

        if (isReverse_tp) {
            gameObject.SetActive(false);
            isReverse_tp = false;
        }
        else {
            isReverse_tp = true;
        }
    }

    //라이딩 시작 버튼 클릭시
    public void onRidingStartButton() {
        sm.playEffectSound(0);

        ridingStart_questionLabel.text = "라이딩을 시작합니다.";
        beforeStartModal_ButtonContainer.SetActive(false);
        beforeStartModal_AnimContainer.SetActive(true);
        beforeStartModal_AnimContainer.GetComponent<RidingStartAnimController>().startAnim();

        //초기 위칫값을 지정한다. (받은 값들의 평균)
        gpsReceiver = Instantiate(gpsPref);
        MyInfo act = ActionCreator.createAction(ActionTypes.MYINFO) as MyInfo;
        act._type = MyInfo.type.RIDING_START;
        gameManager.gameDispatcher.dispatch(act);
    }

    //라이딩 종료 버튼 눌렀을 때
    //모달 활성화
    public void onRidingEndButton() {
        sm.playEffectSound(1);

        exitModal.SetActive(true);
        exitModal.transform.Find("InnerModal/Description").GetComponent<Text>().text = "지금 종료하시면 총 " + boxNum + "개의 상자를 얻을 수 있습니다.";
        //라이딩 일시정지
        Time.timeScale = 0;
    }

    public void refreshTxt(float currSpeed, float avgSpeed,double dist, string time, float maxSpeed, float uphillDist, int boxNum) {
        //Debug.Log("RIDING LISTENER");
        //currSpeedLabel.text = (Math.Round(currSpeed, 2, MidpointRounding.AwayFromZero)).ToString() + " KM/H";
        if(avgSpeed == 0) {
            avgSpeedLabel.text = "0.0";
        }
        else {
            avgSpeedLabel.text = (Math.Round(avgSpeed, 2, MidpointRounding.AwayFromZero)).ToString();
        }
        
        if(dist == 0) {
            distLabel.text = "0.0";
        }
        else {
            distLabel.text = (Math.Round(dist, 2, MidpointRounding.AwayFromZero)).ToString();
        }
        
        if(String.IsNullOrEmpty(time)) {
            timeLabel.text = "00:00:00";
        }
        else {
            timeLabel.text = time;
        }

        if(maxSpeed == 0) {
            maxLabel.text = "0.0";
        }
        else {
            maxLabel.text = (Math.Round(maxSpeed, 2, MidpointRounding.AwayFromZero)).ToString();
        }
        
        if(uphillDist == 0) {
            uphillDistanceLabel.text = "00";
        }
        else {
            uphillDistanceLabel.text = (Math.Round(uphillDist, 2, MidpointRounding.AwayFromZero)).ToString();
        }
        boxLabel.text = boxNum.ToString();
        this.boxNum = boxNum;
    }

    private void sliderRefresh(double dist) {
        float _dist = (float)dist;
        if(_dist < slider.maxValue) {
            slider.value = _dist;
        }
        else {
            float remainder = _dist % slider.maxValue;
            slider.value = remainder;
        }

        int val = (int)slider.value;
        slider.transform.Find("Num").GetComponent<Text>().text = val + "/" + slider.maxValue;
    }

    //최종적으로 종료 모달에서 종료 버튼을 눌렀을 때
    public void ridingEnd() {
        gameObject.SetActive(false);

        exitModal.SetActive(false);

        RidingEndAction action = (RidingEndAction)ActionCreator.createAction(ActionTypes.RIDING_END);
        gameManager.gameDispatcher.dispatch(action);

        stopGPSReceive();

        Time.timeScale = 1;
    }

    //종료 모달에서 취소 버튼을 눌렀을 때
    public void onCancelExitButton() {
        Time.timeScale = 1;
        exitModal.SetActive(false);

        var toggle = exitBtn.GetComponent<Toggle>();
        toggle.isOn = false;
        OnToggle(toggle);
    }

    public void stopGPSReceive() {
        Destroy(gpsReceiver);
    }

    public IEnumerator ridingStart() {
        //라이딩 화면 활성화
        ridingPanel.SetActive(true);
        yield return new WaitForSeconds(1);
        StartPanel.SetActive(false);

        Actions act = ActionCreator.createAction(ActionTypes.RIDING_START);
        GameManager.Instance.gameDispatcher.dispatch(act);

        gpsReceiver.GetComponent<GPSReceiver>().isFirstLoc = false;
        //gpsReceiver = Instantiate(gpsPref);
    }

    public void pauseButtonPressed(bool isTutorial = false) {
        sm.playEffectSound(0);
        pauseModal.SetActive(true);
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

        string time = ridingStore.preTime;
        if (String.IsNullOrEmpty(ridingStore.preTime)) {
            time = "00:00:00";
        }
        
        refreshTxt(currSpeed, avgSpeed, dist, time, maxSpeed, uphillDistance, boxNum);
        sliderRefresh(dist);
    }

    public void OnToggle(Toggle toggle) {
        int index = toggle.gameObject.GetComponent<ButtonIndex>().index;
        GameObject obj = toggle.gameObject;

        obj.transform.Find("OffImg").gameObject.SetActive(!toggle.isOn);
        obj.transform.Find("OffLabel").gameObject.SetActive(!toggle.isOn);
        obj.transform.Find("OnImg").gameObject.SetActive(toggle.isOn);
        obj.transform.Find("OnLabel").gameObject.SetActive(toggle.isOn);

        switch (index) {
            case 0:
                onRidingEndButton();
                exitModal.SetActive(toggle.isOn);
                break;
            case 1:
                pauseButtonPressed();
                pauseModal.SetActive(toggle.isOn);
                break;
        }
    }

    private void resetToggle() {
        pauseBtn.GetComponent<Toggle>().isOn = false;
        pauseBtn.transform.Find("OffImg").gameObject.SetActive(true);
        pauseBtn.transform.Find("OffLabel").gameObject.SetActive(true);
        pauseBtn.transform.Find("OnImg").gameObject.SetActive(false);
        pauseBtn.transform.Find("OnLabel").gameObject.SetActive(false);

        exitBtn.GetComponent<Toggle>().isOn = false;
        exitBtn.transform.Find("OffImg").gameObject.SetActive(true);
        exitBtn.transform.Find("OffLabel").gameObject.SetActive(true);
        exitBtn.transform.Find("OnImg").gameObject.SetActive(false);
        exitBtn.transform.Find("OnLabel").gameObject.SetActive(false);
    }
}