using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FR_FriendProfileView : MonoBehaviour {
    private GameManager gm;
    private Friends friendsStore;
    private Animator animator;

    public Image character;
    public GameObject 
        info,
        bicycle,
        notifyModal;

    private void Awake() {
        gm = GameManager.Instance;
        friendsStore = gm.friendsStore;

        animator = GetComponent<Animator>();
    }

    void OnDisable() {
        //모든 정보 지우기
        init();
    }

    void OnEnable() {
        Invoke("playSlideIn", 0.2f);
    }

    void init() {
        
    }

    void playSlideIn() {
        animator.Play("SlideIn");
    }

    public void slideFinished(AnimationEvent animationEvent) {
        int boolParm = animationEvent.intParameter;

        //slider in
        if (boolParm == 1) {
            setInfo();
        }

        //slider out
        else if (boolParm == 0) {
            gameObject.SetActive(false);
        }
    }

    //유저 정보 입력
    void setInfo() {
        if(friendsStore.selectedFriend == null) {
            string msg = "유저 정보를 불러오는 과정에서 문제가 발생했습니다.";
            onNotifyModal(msg);
            return;
        }

        UserData friend = friendsStore.selectedFriend[0];
        status status = friend.status;

        string nickName = friend.nickName;
        int rank = status.rank;
        int strength = status.strength;
        int speed = status.speed;
        int endurance = status.endurance;
        int regeneration = status.regeneration;

        record_this_month record = friend.record_this_month;
        int ridingCount = record.count;
        int distance = record.total_distance;
        string ridingTime = record.riding_time.Split('.')[0];
    }

    public void onNotifyModal(string msg) {
        notifyModal.SetActive(true);
        notifyModal.transform.Find("InnerModal/Text").GetComponent<Text>().text = msg;
    }
}
