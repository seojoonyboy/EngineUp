using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BicycleListViewController : MonoBehaviour {
    private GameManager gm;
    public BicycleItem_Inventory bicycleItemStore;
    public BicycleViewController parent;
    public SpritesManager spriteManager;
    public BicycleDetailViewController details;

    private Animator animator;
    public GameObject 
        content,
        rowContainer;           //스크롤 영역 한줄 Container

    public GameObject selectedItem;
    void Awake() {
        gm = GameManager.Instance;

        animator = GetComponent<Animator>();
        bicycleItemStore = parent.bicycleItemStore;
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
            makeList();
        }

        //slider out
        else if (boolParm == 0) {
            gameObject.SetActive(false);
        }
    }

    public void makeList() {
        removeList();

        //아이템 갯수를 통해 몇줄 컨테이너 필요한지 계산
        var type = parent.selectedType;
        ArrayList items = null;
        switch(type) {
            case SelectedType.EG:
                items = bicycleItemStore.engineItems;
                break;
            case SelectedType.FR:
                items = bicycleItemStore.frameItems;
                break;
            case SelectedType.WH:
                items = bicycleItemStore.wheelItems;
                break;
        }

        int itemNum = items.Count;
        int containerNum = (int)System.Math.Ceiling(itemNum / 4.0f);
        //Debug.Log(itemNum);
        //Debug.Log(containerNum);
        int cnt = 0;
        for(int i=0; i<containerNum; i++) {
            GameObject rC = Instantiate(rowContainer);
            rC.transform.SetParent(content.transform, false);

            for(int j=0; j<rC.transform.childCount; j++) {
                if (cnt < itemNum) {
                    GameObject item = rC.transform.GetChild(j).gameObject;

                    Info info = item.AddComponent<Info>();
                    RespGetItems data = (RespGetItems)items[cnt];
                    RespItem _item = data.item;
                    info.id = data.id;
                    info.desc = _item.desc;
                    info.name = _item.name;
                    info.grade = _item.grade;
                    info.limit_rank = _item.limit_rank;
                    info.parts = _item.parts;
                    info.gear = _item.gear;
                    info.imageId = _item.id;
                    info.speed = _item.speed;
                    info.endurance = _item.endurance;
                    info.strength = _item.strength;
                    info.recovery = _item.regeneration;

                    if (data.is_equiped == "true") {
                        info.is_equiped = true;
                        item.transform.Find("Equip").GetComponent<Image>().enabled = true;
                    }

                    Text name = item.transform.Find("Name").GetComponent<Text>();
                    name.text = info.name;
                    name.enabled = true;

                    Image image = image = item.transform.Find("Image_EG").GetComponent<Image>();

                    if(type == SelectedType.FR) {
                        image = item.transform.Find("Image_FR").GetComponent<Image>();
                    }
                    else if(type == SelectedType.WH) {
                        image = item.transform.Find("Image_WH_mask/Image_WH").GetComponent<Image>();
                    }

                    image.enabled = true;

                    var tmp = spriteManager.stage_items[info.imageId - 1];
                    if (tmp != null) {
                        image.sprite = spriteManager.stage_items[info.imageId - 1];
                    }
                    else {
                        //image.sprite = spriteManager.default_slots[info.grade - 1];
                        image.enabled = false;
                    }

                    image = item.transform.Find("Grade").GetComponent<Image>();
                    image.enabled = true;
                    image.sprite = spriteManager.grade_items[info.grade - 1];
                    if(item.GetComponent<Button>() == null) {
                        item.AddComponent<Button>();
                    }
                    item.GetComponent<Button>().onClick.AddListener(() => onDetail(item));
                    cnt++;
                }
            }
        }
    }

    private void removeList() {
        foreach(Transform child in content.transform) {
            Destroy(child.gameObject);
        }
    }

    private void onDetail(GameObject obj) {
        details.gameObject.SetActive(true);
        selectedItem = obj;
    }

    public void onFilterButton(GameObject obj) {
        obj.SetActive(!obj.activeSelf);
    }

    public void filterSelected(int index) {
        PlayerPrefs.SetInt("Filter_BICYCLE", index);
        itemSort act = ActionCreator.createAction(ActionTypes.GARAGE_ITEM_SORT) as itemSort;
        switch (index) {
            case 1:
                Debug.Log("이름순 정렬");
                act._type = itemSort.type.NAME;
                break;
            case 2:
                Debug.Log("등급순 정렬");
                act._type = itemSort.type.GRADE;
                break;
        }
        gm.gameDispatcher.dispatch(act);
    }
}

public class Info : MonoBehaviour {
    public int id;
    public bool is_equiped;
    public bool is_locked;
    public int imageId;

    public string name;
    public string desc;
    public int grade;
    public int gear;
    public string parts;
    public int limit_rank;

    public int speed;
    public int recovery;
    public int strength;
    public int endurance;
}