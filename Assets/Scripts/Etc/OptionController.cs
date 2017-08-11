using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class OptionController : MonoBehaviour {
    GameManager gm;
    SoundManager sm;

    Riding ridingStore;
    public GameObject[] modals;

    private Animator animator;

    private TweenPosition tP;
    private bool 
        isReverse_tp,
        isTweening = false;

    void Awake() {
        gm = GameManager.Instance;
        sm = SoundManager.Instance;
        ridingStore = gm.ridingStore;

        animator = GetComponent<Animator>();
    }

    void OnEnable() {
        animator.Play("SlideIn");
    }

    void playSlideIn() {
        animator.Play("SlideIn");
    }

    public void onBackButton() {
        animator.Play("SlideOut");
    }

    public void slideFinished(AnimationEvent animationEvent) {
        int boolParm = animationEvent.intParameter;

        //slider in
        if (boolParm == 1) {
            
        }

        //slider out
        else if (boolParm == 0) {
            gameObject.SetActive(false);
        }
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
