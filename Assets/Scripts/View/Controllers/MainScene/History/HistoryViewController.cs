﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HistoryViewController : MonoBehaviour {
    public GameObject 
        container,
        innerContainer;

    public GameObject[] items;
    public UIScrollView scrollView;

    public int containerHeight = 400;
    private GameManager gm;
    public Riding ridingStore;
    public User userStore;

    public HistoryDetailViewController subController;
    public TweenManager tM;

    public GameObject containerFirstTarget;
    private bool isFirstGetRidingData = true;

    private GameObject
            innerItem = null,
            item = null,
            preItem = null;
    private string preDate = null;

    private TweenPosition tP;
    private bool isReverse_tp;

    public GameObject blockingCollPanel;
    void Awake() {
        gm = GameManager.Instance;
        tP = gameObject.transform.Find("Background").GetComponent<TweenPosition>();
    }

    public void onPanel() {
        tweenPos();

        blockingCollPanel.SetActive(true);
        isReverse_tp = false;
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
        blockingCollPanel.SetActive(true);
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
        blockingCollPanel.SetActive(false);
        if (isReverse_tp) {
            gameObject.SetActive(false);
            gameObject.transform.Find("TopPanel").gameObject.SetActive(false);
        }
        else {
            blockingCollPanel.SetActive(false);
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
                    subController.mapHeader.SetActive(true);
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

            //Debug.Log("Date : " + tmp[0]);
            //Debug.Log("Time : " + tmp[1]);
            
            if (preDate != tmp[0]) {
                //Debug.Log("날짜가 다름");
                //이전과 다른 날짜인 경우
                //새로운 컨테이너를 생성
                item = Instantiate(container);
                item.transform.Find("Header/Label").GetComponent<UILabel>().text = date[0] + " . " + date[1] + " . " + date[2];
                GameObject inner = item.transform.Find("Grid/HistoryInnerContainer").gameObject;

                setInfo(inner, data[i].runningTime, gm.userStore.nickName, data[i].get_boxes, data[i].distance, data[i].id);
                
                UIDragScrollView dS = inner.AddComponent<UIDragScrollView>();
                dS.scrollView = scrollView;

                if (preItem == null) {
                    UIAnchor anchor = item.AddComponent<UIAnchor>();
                    anchor.container = containerFirstTarget;
                    anchor.runOnlyOnce = true;
                    anchor.side = UIAnchor.Side.Bottom;
                    anchor.pixelOffset = new Vector2(0, -245.0f);
                }
                
                else {
                    UIGrid grid = preItem.transform.Find("Grid").GetComponent<UIGrid>();
                    grid.repositionNow = true;
                    grid.Reposition();
                    UIAnchor anchor = item.AddComponent<UIAnchor>();
                    anchor.runOnlyOnce = false;
                    anchor.container = preItem.transform.Find("Grid").gameObject;
                    anchor.side = UIAnchor.Side.Bottom;
                    anchor.pixelOffset = new Vector2(0, -400.0f);
                }
            }

            else {
                //Debug.Log("날짜가 같음");
                //이전과 날짜가 같은 경우
                //하위 컨테이너로 붙인다.
                innerItem = Instantiate(innerContainer);

                innerItem.transform.SetParent(item.transform.Find("Grid").transform);
                innerItem.transform.localScale = Vector3.one;
                innerItem.transform.localPosition = Vector3.zero;

                setInfo(innerItem, data[i].runningTime, gm.userStore.nickName, data[i].get_boxes, data[i].distance, data[i].id);

                UIDragScrollView dS = innerItem.AddComponent<UIDragScrollView>();
                dS.scrollView = scrollView;
            }

            item.transform.SetParent(scrollView.transform);

            item.transform.localScale = Vector3.one;
            item.transform.localPosition = Vector3.zero;

            preDate = tmp[0];
            preItem = item;
        }

        if(item != null) {
            UIGrid _grid = item.transform.Find("Grid").GetComponent<UIGrid>();
            _grid.repositionNow = true;
            _grid.Reposition();
        }
    }

    public void offPanel() {
        gameObject.SetActive(false);
    }

    void onDetail(GameObject obj) {
        //Debug.Log("On detail");
        int index = obj.GetComponent<ButtonIndex>().index;
        subController.id = index;

        subController.onPanel();
    }

    private EventDelegate.Parameter MakeParameter(UnityEngine.Object _value, System.Type _type) {
        EventDelegate.Parameter param = new EventDelegate.Parameter();  // 이벤트 parameter 생성.
        param.obj = _value;   // 이벤트 함수에 전달하고 싶은 값.
        param.expectedType = _type;    // 값의 타입.

        return param;
    }

    private void setInfo(GameObject obj, string runningTime, string nickName, int boxNum, float totalDist, int id) {
        string[] splited = runningTime.Split('.');
        obj.transform.Find("Time").GetComponent<UILabel>().text = splited[0];
        obj.transform.Find("Nickname").GetComponent<UILabel>().text = nickName + "님의 라이딩";
        obj.transform.Find("BoxNum").GetComponent<UILabel>().text = " x" + boxNum;
        obj.transform.Find("TotalDist").GetComponent<UILabel>().text = (Math.Round(totalDist, 2, MidpointRounding.AwayFromZero)) + " km";

        obj.AddComponent<ButtonIndex>().index = id;

        EventDelegate eventBtn = new EventDelegate(this, "onDetail");
        eventBtn.parameters[0] = MakeParameter(obj.gameObject, typeof(GameObject));

        EventDelegate.Add(obj.GetComponent<UIButton>().onClick, eventBtn);
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
