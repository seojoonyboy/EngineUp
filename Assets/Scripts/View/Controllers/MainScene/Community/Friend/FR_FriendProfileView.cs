using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FR_FriendProfileView : MonoBehaviour {
    private GameManager gm;
    private Friends friendsStore;
    private Animator animator;
    public SpritesManager sm;
    public MainViewController mV;

    public Image character;
    public GameObject 
        info,
        bicycle,
        notifyModal;
    public Text[]
        profiles, 
        specs,
        records;
    public Image[] profiles_img;

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
            setInfo();
        }

        //slider out
        else if (boolParm == 0) {
            gameObject.SetActive(false);
        }
    }

    //유저 정보 입력
    void setInfo() {
        if(friendsStore.profileReqType == GetFriendInfoAction.type.MYFRIEND) {
            if (friendsStore.selectedFriend == null) {
                string msg = "유저 정보를 불러오는 과정에서 문제가 발생했습니다.";
                onNotifyModal(msg);
                return;
            }
            fr_info_callback friend = friendsStore.selectedFriend;
            status status = friend.toUser.status;

            string nickName = friend.toUser.nickName;
            int rank = status.rank;
            int strength = status.strength;
            int speed = status.speed;
            int endurance = status.endurance;
            int regeneration = status.regeneration;

            record_this_month record = friend.toUser.record_this_month;
            int ridingCount = record.count;
            int distance = record.total_distance;
            string ridingTime = record.riding_time;

            int character_img_id = friend.toUser.represent_character.character_inventory.character;
            int character_lv = friend.toUser.represent_character.character_inventory.lv - 1;

            int fr_img_id = -1;
            int wh_img_id = -1;
            int ds_img_id = -1;

            foreach (RespGetItems item in friend.toUser.equiped_items) {
                if (item.item.parts == "FR") {
                    fr_img_id = item.item.id;
                }
                else if (item.item.parts == "WH") {
                    wh_img_id = item.item.id;
                }
                else if (item.item.parts == "DS") {
                    ds_img_id = item.item.id;
                }
            }

            Image targetImg = bicycle.transform.Find("Frame").GetComponent<Image>();
            targetImg.enabled = true;
            if (fr_img_id == -1 || sm.stage_items[fr_img_id] == null) {
                targetImg.sprite = sm.stage_items[0];
            }
            else {
                targetImg.sprite = sm.stage_items[fr_img_id];
            }

            targetImg = bicycle.transform.Find("Wheel").GetComponent<Image>();
            targetImg.enabled = true;
            if (wh_img_id == -1 || sm.stage_items[wh_img_id] == null) {
                targetImg.sprite = sm.stage_items[53];
            }
            else {
                targetImg.sprite = sm.stage_items[wh_img_id];
            }

            targetImg = bicycle.transform.Find("Engine").GetComponent<Image>();
            targetImg.enabled = true;
            if (ds_img_id == -1 || sm.stage_items[ds_img_id] == null) {
                targetImg.sprite = sm.stage_items[85];
            }
            else {
                targetImg.sprite = sm.stage_items[ds_img_id];
            }

            targetImg = character.GetComponent<Image>();
            targetImg.sprite = sm.Stage_chars[character_img_id].images[character_lv];

            specs[0].text = strength.ToString();
            specs[1].text = endurance.ToString();
            specs[2].text = regeneration.ToString();
            specs[3].text = speed.ToString();

            records[0].text = ridingTime;
            records[1].text = distance.ToString();
            records[2].text = ridingCount.ToString();

            profiles[0].text = nickName;
            profiles[1].text = "랭크 " + rank;

            profiles_img[1].enabled = true;

            int iconRank = (int)Mathf.Ceil((float)rank / 5);

            if (iconRank == 0) {
                profiles_img[1].sprite = mV.ranks[0];
            }
            else {
                profiles_img[1].sprite = mV.ranks[iconRank - 1];
            }
        }

        else if(friendsStore.profileReqType == GetFriendInfoAction.type.WAITINGACCEPT) {
            if (friendsStore.selectedWaitingAccept == null) {
                string msg = "유저 정보를 불러오는 과정에서 문제가 발생했습니다.";
                onNotifyModal(msg);
                return;
            }
            UserData friend = friendsStore.selectedWaitingAccept;
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
            string ridingTime = record.riding_time;

            int character_img_id = friend.represent_character.character_inventory.character;
            int character_lv = friend.represent_character.character_inventory.lv - 1;

            int fr_img_id = -1;
            int wh_img_id = -1;
            int ds_img_id = -1;

            foreach (RespGetItems item in friend.equiped_items) {
                if (item.item.parts == "FR") {
                    fr_img_id = item.item.id;
                }
                else if (item.item.parts == "WH") {
                    wh_img_id = item.item.id;
                }
                else if (item.item.parts == "DS") {
                    ds_img_id = item.item.id;
                }
            }

            Image targetImg = bicycle.transform.Find("Frame").GetComponent<Image>();
            targetImg.enabled = true;
            if (fr_img_id == -1 || sm.stage_items[fr_img_id] == null) {
                targetImg.sprite = sm.stage_items[0];
            }
            else {
                targetImg.sprite = sm.stage_items[fr_img_id];
            }

            targetImg = bicycle.transform.Find("Wheel").GetComponent<Image>();
            targetImg.enabled = true;
            if (wh_img_id == -1 || sm.stage_items[wh_img_id] == null) {
                targetImg.sprite = sm.stage_items[53];
            }
            else {
                targetImg.sprite = sm.stage_items[wh_img_id];
            }

            targetImg = bicycle.transform.Find("Engine").GetComponent<Image>();
            targetImg.enabled = true;
            if (ds_img_id == -1 || sm.stage_items[ds_img_id] == null) {
                targetImg.sprite = sm.stage_items[85];
            }
            else {
                targetImg.sprite = sm.stage_items[ds_img_id];
            }

            targetImg = character.GetComponent<Image>();
            targetImg.sprite = sm.Stage_chars[character_img_id].images[character_lv];

            specs[0].text = strength.ToString();
            specs[1].text = endurance.ToString();
            specs[2].text = regeneration.ToString();
            specs[3].text = speed.ToString();

            records[0].text = ridingTime;
            records[1].text = distance.ToString();
            records[2].text = ridingCount.ToString();

            profiles[0].text = nickName;
            profiles[1].text = "랭크 " + rank;

            profiles_img[1].enabled = true;

            int iconRank = (int)Mathf.Ceil((float)rank / 5);

            if (iconRank == 0) {
                profiles_img[1].sprite = mV.ranks[0];
            }
            else {
                profiles_img[1].sprite = mV.ranks[iconRank - 1];
            }
        }

        
    }

    public void onNotifyModal(string msg) {
        notifyModal.SetActive(true);
        notifyModal.transform.Find("InnerModal/Text").GetComponent<Text>().text = msg;
    }

    void init() {
        foreach(Text text in specs) {
            text.text = null;
        }
        foreach (Text text in records) {
            text.text = null;
        }
        foreach (Text text in profiles) {
            text.text = null;
        }
        Image targetImg = bicycle.transform.Find("Frame").GetComponent<Image>();
        targetImg.enabled = false;

        targetImg = bicycle.transform.Find("Wheel").GetComponent<Image>();
        targetImg.enabled = false;

        targetImg = bicycle.transform.Find("Engine").GetComponent<Image>();
        targetImg.enabled = false;

        profiles_img[1].enabled = false;
    }
}
