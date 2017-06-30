using UnityEngine;
using System.Collections;
using Facebook.Unity;

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

    public UISprite panel;
    public float color;

    void Awake() {
        gm = GameManager.Instance;
        sm = SoundManager.Instance;
        ridingStore = gm.ridingStore;

        tP = gameObject.transform.Find("Background").GetComponent<TweenPosition>();

        panel = gameObject.transform.Find("Background").GetComponent<UISprite>();
        color = panel.alpha;

        panel.alpha = 0;
    }

    public void onPanel() {
        panel.alpha = color;
        tweenPos();

        blockingCollPanel.SetActive(true);
        isReverse_tp = false;
    }

    void offPanel() {
        panel.alpha = 0f;
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
            offPanel();
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
