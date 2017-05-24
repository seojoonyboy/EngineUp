using UnityEngine;
using System.Collections;
using Facebook.Unity;

public class OptionController : MonoBehaviour {
    GameManager gm;
    Riding ridingStore;
    public GameObject[] modals;

    void Start() {
        gm = GameManager.Instance;
        ridingStore = gm.ridingStore;
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
