using System.Collections;
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
    private Riding ridingStore;
    private User userStore;

    public HistoryDetailViewController subController;

    void Awake() {
        gm = GameManager.Instance;
        ridingStore = gm.ridingStore;
        userStore = gm.userStore;
    }

    void OnEnable() {
        //makeList();
        GetRidingRecords act = ActionCreator.createAction(ActionTypes.GET_RIDING_RECORDS) as GetRidingRecords;
        act.isFirst = true;
        gm.gameDispatcher.dispatch(act);
    }

    public void ridingStoreListener() {
        ActionTypes storeEventType = ridingStore.eventType;

        if(storeEventType == ActionTypes.GET_RIDING_RECORDS) {
            if(ridingStore.storeStatus == storeStatus.NORMAL) {
                makeList();
            }
        }

        if(storeEventType == ActionTypes.RIDING_DETAILS) {
            if(ridingStore.storeStatus == storeStatus.NORMAL) {
                subController.setInfo(ridingStore.ridingDetails);
                subController.setMap(ridingStore.ridingDetails);
                subController.setDate(ridingStore.ridingDetails.createDate);
                subController.setNickName(userStore.nickName);
            }
        }
    }

    public void makeList() {
        var data = ridingStore.ridingRecords;
        string preDate = null;
        GameObject
            innerItem = null,
            item = null,
            preItem = null;
        for (int i=0; i<data.Length; i++) {
            string[] tmp = data[i].createDate.Split('T');
            string date = tmp[0];
            string time = tmp[1];

            //Debug.Log("Date : " + tmp[0]);
            //Debug.Log("Time : " + tmp[1]);
            
            if (preDate != tmp[0]) {
                //이전과 다른 날짜인 경우
                //새로운 컨테이너를 생성
                item = Instantiate(container);

                item.transform.Find("Header/Label").GetComponent<UILabel>().text = tmp[0];
                GameObject inner = item.transform.Find("Grid/HistoryInnerContainer").gameObject;

                setInfo(inner, data[i].runningTime, gm.userStore.nickName, data[i].get_boxes, data[i].distance.ToString(), data[i].id);
                
                if (preItem != null) {
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
                //이전과 날짜가 같은 경우
                //하위 컨테이너로 붙인다.
                innerItem = Instantiate(innerContainer);
                innerItem.transform.SetParent(item.transform.Find("Grid").transform);
                innerItem.transform.localScale = Vector3.one;
                innerItem.transform.localPosition = Vector3.zero;

                setInfo(innerItem, data[i].runningTime, gm.userStore.nickName, data[i].get_boxes, data[i].distance.ToString(), data[i].id);
            }

            item.transform.SetParent(scrollView.transform);

            item.transform.localScale = Vector3.one;
            item.transform.localPosition = Vector3.zero;

            preDate = tmp[0];
            preItem = item;
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

    private void setInfo(GameObject obj, string runningTime, string nickName, int boxNum, string totalDist, int id) {
        obj.transform.Find("Time").GetComponent<UILabel>().text = runningTime;
        obj.transform.Find("Nickname").GetComponent<UILabel>().text = nickName + "님의 라이딩";
        obj.transform.Find("BoxNum").GetComponent<UILabel>().text = " x" + boxNum;
        obj.transform.Find("TotalDist").GetComponent<UILabel>().text = totalDist + " km";

        obj.AddComponent<ButtonIndex>().index = id;

        GameObject detailButton = obj.transform.Find("DetailButton").gameObject;

        EventDelegate eventBtn = new EventDelegate(this, "onDetail");
        eventBtn.parameters[0] = MakeParameter(obj.gameObject, typeof(GameObject));

        EventDelegate.Add(detailButton.GetComponent<UIButton>().onClick, eventBtn);
    }
}
