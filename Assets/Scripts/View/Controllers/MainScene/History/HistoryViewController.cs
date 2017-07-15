using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class HistoryViewController : MonoBehaviour {
    public GameObject 
        container,
        innerContainer;

    public GameObject[] items;
    public GameObject scrollView;

    private GameManager gm;
    public Riding ridingStore;
    public User userStore;

    public HistoryDetailViewController subController;
    public TweenManager tM;

    private bool isFirstGetRidingData = true;

    private GameObject
            innerItem = null,
            item = null,
            preItem = null;
    private string preDate = null;

    private TweenPosition tP;
    private bool isReverse_tp;

    void Awake() {
        gm = GameManager.Instance;
        tP = GetComponent<TweenPosition>();
    }

    void OnEnable() {
        tweenPos();

        isReverse_tp = false;
    }

    void OnDisable() {

    }

    public void onBackButton() {
        foreach (Transform obj in scrollView.transform) {
            Destroy(obj.gameObject);
        }
        innerItem = null;
        item = null;
        preItem = null;
        preDate = null;
        isFirstGetRidingData = true;
    }

    public void tweenPos() {
        bool isTweening = tM.isTweening;
        if (isTweening) {
            return;
        }
        tM.isTweening = true;

        if (!isReverse_tp) {
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

    public void tpFinished() {
        tM.isTweening = false;

        if (isReverse_tp) {
            gameObject.SetActive(false);
            gameObject.transform.Find("TopPanel").gameObject.SetActive(false);
        }
        else {
            gameObject.transform.Find("TopPanel").gameObject.SetActive(true);
            getRidingDataSets();
        }

        isReverse_tp = true;
    }

    public void ridingStoreListener() {
        if (ridingStore.eventType == ActionTypes.GET_RIDING_RECORDS) {
            if (ridingStore.storeStatus == storeStatus.NORMAL) {
                makeList();
            }
        }

        if (ridingStore.eventType == ActionTypes.RIDING_DETAILS) {
            if (ridingStore.storeStatus == storeStatus.NORMAL) {
                if(ridingStore.callRecType == GetRidingRecords.callType.HISTORY) {
                    subController.setInfo(ridingStore.ridingDetails);
                    subController.setMap(ridingStore.ridingDetails);
                    subController.setDate(ridingStore.ridingDetails.createDate);
                    subController.setNickName(userStore.nickName);
                }
            }
        }
    }

    public void makeList() {
        var data = ridingStore.ridingRecords;
        for (int i=0; i<data.Length; i++) {
            string[] tmp = data[i].createDate.Split('T');
            string[] date = tmp[0].Split('-');
            string time = tmp[1];

            if (preDate != tmp[0]) {
                //Debug.Log("날짜가 다름");
                //이전과 다른 날짜인 경우
                //새로운 컨테이너를 생성

                item = Instantiate(container);
                item.transform.Find("Header/Label").GetComponent<Text>().text = date[0] + " . " + date[1] + " . " + date[2];
                GameObject inner = item.transform.Find("HistoryInnerContainer/Container").gameObject;

                item.transform.SetParent(scrollView.transform);

                item.transform.localScale = Vector3.one;
                item.transform.localPosition = Vector3.zero;

                setInfo(inner, data[i].runningTime, gm.userStore.nickName, data[i].get_boxes, data[i].distance, data[i].id);
            }

            else {
                //Debug.Log("날짜가 같음");
                //이전과 날짜가 같은 경우
                //하위 컨테이너로 붙인다.
                innerItem = Instantiate(innerContainer);

                innerItem.transform.SetParent(item.transform);
                innerItem.transform.localScale = Vector3.one;
                innerItem.transform.localPosition = Vector3.zero;

                GameObject obj = innerItem.transform.Find("Container").gameObject;
                obj.transform.localScale = Vector3.one;
                obj.transform.localPosition = Vector3.zero;

                setInfo(obj, data[i].runningTime, gm.userStore.nickName, data[i].get_boxes, data[i].distance, data[i].id);
            }
            preDate = tmp[0];
            preItem = item;
        }
    }

    void onDetail(GameObject obj) {
        //Debug.Log("On detail");
        int index = obj.GetComponent<ButtonIndex>().index;
        subController.id = index;

        subController.gameObject.SetActive(true);
    }

    private EventDelegate.Parameter MakeParameter(UnityEngine.Object _value, System.Type _type) {
        EventDelegate.Parameter param = new EventDelegate.Parameter();  // 이벤트 parameter 생성.
        param.obj = _value;   // 이벤트 함수에 전달하고 싶은 값.
        param.expectedType = _type;    // 값의 타입.

        return param;
    }

    private void setInfo(GameObject obj, string runningTime, string nickName, int boxNum, float totalDist, int id) {
        string[] splited = runningTime.Split('.');
        obj.transform.Find("Time").GetComponent<Text>().text = splited[0];
        obj.transform.Find("Nickname").GetComponent<Text>().text = nickName + "님의 라이딩";
        obj.transform.Find("BoxNum").GetComponent<Text>().text = " x" + boxNum;
        obj.transform.Find("TotalDist").GetComponent<Text>().text = (Math.Round(totalDist, 2, MidpointRounding.AwayFromZero)) + " km";

        obj.AddComponent<ButtonIndex>().index = id;

        obj.GetComponent<Button>().onClick.AddListener(() => onDetail(obj));
    }

    //이전 라이딩 기록 추가로 불러옴
    public void getRidingDataSets() {
        GetRidingRecords act = ActionCreator.createAction(ActionTypes.GET_RIDING_RECORDS) as GetRidingRecords;
        if(isFirstGetRidingData) {
            act.isFirst = true;
        }
        gm.gameDispatcher.dispatch(act);
        isFirstGetRidingData = false;
    }
}
