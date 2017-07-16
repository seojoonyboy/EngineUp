using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class OptionController : MonoBehaviour {
    GameManager gm;
    SoundManager sm;

    Riding ridingStore;
    public GameObject[] modals;

    private TweenPosition tP;
    public GameObject blockingCollPanel;
    private bool 
        isReverse_tp,
        isTweening = false;

    void Awake() {
        gm = GameManager.Instance;
        sm = SoundManager.Instance;
        ridingStore = gm.ridingStore;

        tP = GetComponent<TweenPosition>();
    }

    void OnEnable() {
        tweenPos();

        blockingCollPanel.SetActive(true);
        isReverse_tp = false;
    }

    public void tweenPos() {
        sm.playEffectSound(0);
        if(isTweening) {
            return;
        }

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
        isTweening = true;
    }

    public void tPFinished() {
        blockingCollPanel.SetActive(false);
        isTweening = false;

        if (isReverse_tp) {
            gameObject.SetActive(false);
            gameObject.transform.Find("TopPanel").gameObject.SetActive(false);
        }
        else {
            gameObject.transform.Find("TopPanel").gameObject.SetActive(true);
        }

        isReverse_tp = true;
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
