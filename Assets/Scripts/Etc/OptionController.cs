using UnityEngine;
using System.Collections;
using Facebook.Unity;

public class OptionController : MonoBehaviour {
    GameManager gm;
    Riding ridingStore;
    public GameObject[] modals;

    private TweenPosition tP;
    public GameObject blockingCollPanel;
    private bool isReverse_tp;
    void Awake() {
        gm = GameManager.Instance;
        ridingStore = gm.ridingStore;

        tP = gameObject.transform.Find("Background").GetComponent<TweenPosition>();
    }

    void OnEnable() {
        tweenPos();

        blockingCollPanel.SetActive(true);
        isReverse_tp = false;
    }

    void OnDisable() {
        tP.ResetToBeginning();
    }

    public void tweenPos() {
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

    public void tPFinished() {
        blockingCollPanel.SetActive(false);

        if (isReverse_tp) {
            gameObject.SetActive(false);
            gameObject.transform.Find("TopPanel").gameObject.SetActive(false);
        }
        else {
            gameObject.transform.Find("TopPanel").gameObject.SetActive(true);
        }

        isReverse_tp = true;
    }

    public void offPanel() {
        gameObject.SetActive(false);
    }

    public void onModal(GameObject obj) {
        int index = obj.GetComponent<ButtonIndex>().index;
        modals[index].SetActive(true);
    }

    public void offModal(GameObject obj) {
        int index = obj.GetComponent<ButtonIndex>().index;
        modals[index].SetActive(false);
    }

    //라이딩 기록 제거
    public void rmvRidingRecords() {
        RidingRecordsRmv act = ActionCreator.createAction(ActionTypes.RIDING_RECORDS_REMOVE) as RidingRecordsRmv;
        gm.gameDispatcher.dispatch(act);
    }

    public void onRidingListener() {
        
    }
}
