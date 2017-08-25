using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterDetailViewController : MonoBehaviour {
    private Animator animator;
    private GameManager gm;
    private User userStore;

    public GameObject 
        stage,
        notifyModal;

    public CharacterListViewController parent;
    public SpritesManager spriteManager;

    public GameObject[] specs;

    private Color32 increaseColor = new Color32(31, 196, 97, 255);
    private Color32 decreaseColor = new Color32(201, 48, 48, 255);
    private Color32 defaultColor = new Color32(255, 255, 255, 255);
    void Awake() {
        gm = GameManager.Instance;
        userStore = gm.userStore;
        animator = GetComponent<Animator>();
    }

    void OnEnable() {
        Invoke("playSlideIn", 0.2f);
    }

    void OnDisable() {
        init();
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
            //makeList();
            setInfo();
        }

        //slider out
        else if (boolParm == 0) {
            gameObject.SetActive(false);
        }
    }

    private void init() {
        string[] str = new string[] { "Tier1", "Tier2", "Tier3" };
        GameObject tier;
        Image img;

        for (int i = 0; i < 3; i++) {
            tier = stage.transform.Find(str[i]).gameObject;
            img = tier.transform.Find("Image/Character").GetComponent<Image>();
            img.enabled = false;
            tier.transform.Find("Image/Shadow").GetComponent<Image>().enabled = false;
            Text name = tier.transform.Find("Name").GetComponent<Text>();
            name.enabled = false;
            name.text = "";
            tier.transform.Find("Tier").GetComponent<Text>().enabled = false;

            img.color = Color.white;
            img.transform.parent.Find("Unlocked").gameObject.SetActive(false);
        }
    }

    private void setInfo() {
        CharInfo info = parent.selectedItem.GetComponent<CharInfo>();
        int imageId = info.imageId - 1;
        int grade = 0;
        if(info.has_character) {
            grade = info.lv - 1;
        }
        else {
            grade = 0;
        }
        string[] str = new string[] { "Tier1", "Tier2", "Tier3" };
        GameObject tier;
        Image img;

        for (int i=0; i<3; i++) {
            tier = stage.transform.Find(str[i]).gameObject;

            img = tier.transform.Find("Image/Character").GetComponent<Image>();
            img.enabled = true;

            if(grade < i) {
                img.color = Color.black;
                img.transform.parent.Find("Unlocked").gameObject.SetActive(true);
            }

            img.sprite = spriteManager.Stage_chars[imageId].images[i];
            tier.transform.Find("Image/Shadow").GetComponent<Image>().enabled = true;

            Text name = tier.transform.Find("Name").GetComponent<Text>();
            name.enabled = true;
            name.text = info.name;

            tier.transform.Find("Tier").GetComponent<Text>().enabled = true;
        }

        var itemSpects = userStore.itemSpects;

        int pre_endurance = itemSpects.Char_endurance + itemSpects.Item_endurance;
        int pre_speed = itemSpects.Char_speed + itemSpects.Item_speed;
        int pre_str = itemSpects.Char_strength + itemSpects.Item_strength;
        int pre_recovery = itemSpects.Char_regeneration + itemSpects.Item_regeneration;

        int new_endurance = info.endurance + itemSpects.Item_endurance;
        int new_str = info.strength + itemSpects.Item_strength;
        int new_recovery = info.regeneration + itemSpects.Item_regeneration;
        int new_speed = info.speed + itemSpects.Item_speed;

        int diffStr = new_str - pre_str;
        int diffRecovery = new_recovery - pre_recovery;
        int diffSpeed = new_speed - pre_speed;
        int diffEndurance = new_endurance - pre_endurance;

        Text pre_str_text = specs[0].transform.Find("PreVal").GetComponent<Text>();
        Text pre_end_text = specs[1].transform.Find("PreVal").GetComponent<Text>();
        Text pre_rec_text = specs[2].transform.Find("PreVal").GetComponent<Text>();
        Text pre_speed_text = specs[3].transform.Find("PreVal").GetComponent<Text>();

        pre_str_text.text = pre_str.ToString();
        pre_end_text.text = pre_endurance.ToString();
        pre_rec_text.text = pre_recovery.ToString();
        pre_speed_text.text = pre_speed.ToString();

        Text new_str_text = specs[0].transform.Find("ChangeVal").GetComponent<Text>();
        Text new_end_text = specs[1].transform.Find("ChangeVal").GetComponent<Text>();
        Text new_rec_text = specs[2].transform.Find("ChangeVal").GetComponent<Text>();
        Text new_speed_text = specs[3].transform.Find("ChangeVal").GetComponent<Text>();

        new_str_text.text = new_str.ToString();
        new_end_text.text = new_endurance.ToString();
        new_rec_text.text = new_recovery.ToString();
        new_speed_text.text = new_speed.ToString();

        //이전보다 근력 증가
        if (diffStr > 0) {
            new_str_text.color = increaseColor;
        }
        else if (diffStr == 0) {
            new_str_text.color = defaultColor;
        }
        else if (diffStr < 0) {
            new_str_text.color = decreaseColor;
        }

        if (diffSpeed > 0) {
            new_speed_text.color = increaseColor;
        }
        else if (diffSpeed == 0) {
            new_speed_text.color = defaultColor;
        }
        else if (diffSpeed < 0) {
            new_speed_text.color = decreaseColor;
        }

        if (diffEndurance > 0) {
            new_end_text.color = increaseColor;
        }
        else if (diffEndurance == 0) {
            new_end_text.color = defaultColor;
        }
        else if (diffEndurance < 0) {
            new_end_text.color = decreaseColor;
        }

        if (diffRecovery > 0) {
            new_rec_text.color = increaseColor;
        }
        else if (diffRecovery == 0) {
            new_rec_text.color = defaultColor;
        }
        else if (diffRecovery < 0) {
            new_rec_text.color = decreaseColor;
        }
    }

    private void setStat() {
        
    }

    private spec diffSpec(CharInfo info, int pre_endurance, int pre_speed, int pre_str, int pre_recovery) {
        spec specs = new spec();
        var itemSpects = userStore.itemSpects;

        int selected_str = info.strength;
        int selected_speed = info.speed;
        int selected_endurance = info.endurance;
        int selected_recovery = info.regeneration;

        specs.diff_endurance = selected_endurance - pre_endurance;
        specs.diff_strength = selected_str - pre_str;
        specs.diff_recovery = selected_recovery - pre_recovery;
        specs.diff_speed = selected_speed - pre_speed;

        return specs;
    }

    private class spec {
        public int diff_strength = 0;
        public int diff_speed = 0;
        public int diff_endurance = 0;
        public int diff_recovery = 0;

        public int pre_str = 0;
        public int pre_end = 0;
        public int pre_speed = 0;
        public int pre_rec = 0;
    }

    //캐릭터 장착하기
    public void equipCharButton() {
        CharInfo info = parent.selectedItem.GetComponent<CharInfo>();

        if(info.has_character) {
            if(info.paid >= info.cost) {
                equip_act act = ActionCreator.createAction(ActionTypes.GARAGE_ITEM_EQUIP) as equip_act;
                act._type = equip_act.type.CHAR;
                act.id = info.id;
                gm.gameDispatcher.dispatch(act);

                onNotifyModal("파트너를 교체하였습니다.");
            }
            else {
                onNotifyModal("보유하고 있지 않은 파트너입니다.");
            }
        }
        else {
            onNotifyModal("보유하고 있지 않은 파트너입니다.");
        }
    }

    public void close() {
        onBackButton();
        parent.onBackButton();
    }

    public void offNotifyModal() {
        notifyModal.SetActive(false);
        close();
    }

    public void onNotifyModal(string msg) {
        notifyModal.SetActive(true);
        notifyModal.transform.Find("InnerModal/Text").GetComponent<Text>().text = msg;
    }
}
