#pragma warning disable 0168
#pragma warning disable 0219
#pragma warning disable 0414
using UnityEngine;
using System;
using System.Collections;

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
        exitBtn,
        blockingCollPanel;

    private GameObject gpsReceiver;

    private bool isPausePressed = false;

    //private GameObject gpsManager;
    private GameManager gameManager;
    private SoundManager sm;

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

    private TweenPosition tP;
    private bool
        isReverse_tp,
        isTweening = false;

    public UISprite 
        startPanelSprite,
        ridingPanelSprite,
        start_buttonCon_sprite,
        start_animCon_sprite;

    private float startPanelColor, ridingPanelColor;

    void Start() {
        gameManager = GameManager.Instance;
        sm = SoundManager.Instance;
    }

    void Awake() {
        tP = StartPanel.transform.Find("Background").GetComponent<TweenPosition>();
        
        startPanelColor = startPanelSprite.alpha;
        ridingPanelColor = ridingPanelSprite.alpha;
        //라이딩 시작화면, 라이딩 화면 비활성화
        startPanelSprite.alpha = 0;
        ridingPanelSprite.alpha = 0;
        start_animCon_sprite.alpha = 0;
    }

    //메인화면에서 라이딩 버튼 클릭시
    public void onPanel() {
        //시작 화면 활성화
        startPanelSprite.alpha = startPanelColor;
        start_buttonCon_sprite.alpha = startPanelColor;
        tweenPos();

        avgSpeedLabel.text = "0";
        distLabel.text = "0";
        maxLabel.text = "0";
        uphillDistanceLabel.text = "0";
        
        blockingCollPanel.SetActive(true);
        isReverse_tp = false;
    }

    private void offPanel() {
        startPanelSprite.alpha = 0;

        isPausePressed = false;
        pauseModal.SetActive(isPausePressed);
        blockingCollPanel.SetActive(false);
        isReverse_tp = false;
    }

    public void tweenPos() {
        if (isTweening) {
            return;
        }
        isTweening = true;
        blockingCollPanel.SetActive(true);
        if (!isReverse_tp) {
            tP.ResetToBeginning();
            tP.PlayForward();
        }
        else {
            sm.playEffectSound(0);

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
        blockingCollPanel.SetActive(false);

        if (isReverse_tp) {
            offPanel();
        }
        isReverse_tp = true;
    }

    //라이딩 시작 버튼 클릭시
    public void onRidingStartButton() {
        sm.playEffectSound(0);

        start_buttonCon_sprite.alpha = 0;
        start_animCon_sprite.alpha = startPanelColor;
        start_animCon_sprite.GetComponent<RidingStartAnimController>().startAnim();

        //초기 위칫값을 지정한다. (받은 값들의 평균)
        gpsReceiver = Instantiate(gpsPref);
        MyInfo act = ActionCreator.createAction(ActionTypes.MYINFO) as MyInfo;
        act._type = MyInfo.type.RIDING_START;
        gameManager.gameDispatcher.dispatch(act);
    }

    //라이딩 종료 버튼 눌렀을 때
    //모달 활성화
    public void onRidingEndButton() {
        sm.playEffectSound(0);

        exitModal.SetActive(true);
        exitModal.transform.Find("Modal/Description").GetComponent<UILabel>().text = "지금 종료하시면 \n총 " + boxNum + "개의 상자를 얻을 수 있습니다.";
        //라이딩 일시정지
        Time.timeScale = 0;
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
        offToggleGroup();
        offPanel();

        ridingPanelSprite.alpha = 0;
        startPanelSprite.alpha = 0;

        exitModal.SetActive(false);

        RidingEndAction action = (RidingEndAction)ActionCreator.createAction(ActionTypes.RIDING_END);
        gameManager.gameDispatcher.dispatch(action);

        stopGPSReceive();

        Time.timeScale = 1;
    }

    //종료 모달에서 취소 버튼을 눌렀을 때
    public void onCancelExitButton() {
        offToggleGroup();
        Time.timeScale = 1;
        exitModal.SetActive(false);
    }

    public void stopGPSReceive() {
        Destroy(gpsReceiver);
    }

    public IEnumerator ridingStart() {
        //라이딩 화면 활성화
        ridingPanelSprite.alpha = ridingPanelColor;
        yield return new WaitForSeconds(1);
        startPanelSprite.alpha = 0;
        start_animCon_sprite.alpha = 0;

        Actions act = ActionCreator.createAction(ActionTypes.RIDING_START);
        GameManager.Instance.gameDispatcher.dispatch(act);

        gpsReceiver.GetComponent<GPSReceiver>().isFirstLoc = false;
        //gpsReceiver = Instantiate(gpsPref);
    }

    public void pauseButtonPressed(bool isTutorial = false) {
        sm.playEffectSound(0);
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
        string time = ridingStore.preTime;
        refreshTxt(currSpeed, avgSpeed, dist, time, maxSpeed, uphillDistance, boxNum);
        sliderRefresh(dist);
    }

    void offToggleGroup() {
        UIToggle toggle = UIToggle.GetActiveToggle(5);
        toggle.value = false;
    }
}