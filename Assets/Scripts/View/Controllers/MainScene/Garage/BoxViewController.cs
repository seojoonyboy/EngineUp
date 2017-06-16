using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxViewController : MonoBehaviour {
    public UILabel 
        numOfBoxLabel,
        singleModalBoxNum,
        multiModalBoxNum;

    private GameManager gm;
    private Box_Inventory boxStore;
    private TweenPosition tP;

    public User userStore;

    public GameObject 
        loadingModal,
        notifyModal,
        boxOpenModal,
        singleOpenButton,
        multiOpenButton;

    public GameObject 
        _openEffect,
        multiOpenTable,
        blockingCollPanel;

    public UIAtlas 
        bicycleAtlas,
        charAtlas,
        uiAtlas;
    private bool isReverse_tp;

    void Awake() {
        gm = GameManager.Instance;
        boxStore = gm.boxInvenStore;

        tP = gameObject.transform.Find("Background").GetComponent<TweenPosition>();
    }

    void OnEnable() {
        MyInfo act = ActionCreator.createAction(ActionTypes.MYINFO) as MyInfo;
        gm.gameDispatcher.dispatch(act);

        tweenPos();

        blockingCollPanel.SetActive(true);
        isReverse_tp = false;
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

    public void onUserStoreListener() {
        if(userStore.eventType == ActionTypes.MYINFO) {
            if(userStore.storeStatus == storeStatus.NORMAL) {
                string boxNum = userStore.myData.boxes.ToString();
                numOfBoxLabel.text = boxNum;
                singleModalBoxNum.text = "x " + boxNum;
                multiModalBoxNum.text = "x " + boxNum;
            }
        }
    }

    void OnDisable() {
        tP.ResetToBeginning();
    }

    public void onBoxStoreListener() {
        if (boxStore.eventType == ActionTypes.BOX_OPEN) {
            if (boxStore.storeStatus == storeStatus.WAITING_REQ) {
                loadingModal.SetActive(true);
            }

            else if (boxStore.storeStatus == storeStatus.NORMAL) {
                //박스 열기 정상 동작시
                loadingModal.SetActive(false);
                boxOpenModal.SetActive(true);

                var items = boxStore.openedItem;
                int itemCount = items.Length;

                if(itemCount == 1) {
                    GameObject modal = boxOpenModal.transform.Find("SingleModal").gameObject;
                    modal.SetActive(true);
                    UILabel name = modal.transform.Find("Name").GetComponent<UILabel>();
                    UISprite sprite = modal.transform.Find("Icon").GetComponent<UISprite>();

                    var openedItem = boxStore.openedItem;
                    string type = openedItem[0].type;
                    if (type == "item") {
                        string spriteName = openedItem[0].item.id + "-1";
                        sprite.atlas = bicycleAtlas;
                        sprite.spriteName = spriteName;
                        name.text = openedItem[0].item.name;
                    }
                    else if (type == "character") {
                        string spriteName = openedItem[0].character.id.ToString();
                        sprite.atlas = charAtlas;
                        sprite.spriteName = spriteName;
                        name.text = openedItem[0].character.name;
                    }
                }
                
                else {
                    GameObject modal = boxOpenModal.transform.Find("MultipleModal").gameObject;
                    modal.SetActive(true);
                    UITable table = modal.transform.Find("Table").GetComponent<UITable>();

                    table.repositionNow = true;
                    table.Reposition();

                    List<Transform> list = table.GetChildList();

                    foreach(Transform item in list) {
                        item.Find("Name").GetComponent<UILabel>().text = "";
                        UISprite img = item.Find("Image").GetComponent<UISprite>();
                        img.atlas = uiAtlas;
                        img.spriteName = "boxOpen_slot_multi";
                    }

                    StartCoroutine(openEffect(list, itemCount, items, modal));
                }
            }
        }
    }

    //공구함 열기 버튼 클릭시
    //박스 열기 Action 이전 Animation 발생시킴
    //Animation 마지막에 open 함수 실행
    public void startAnim(GameObject obj) {
        offBoxOpenModal();
        int index = obj.GetComponent<ButtonIndex>().index;
        open(index);
    }

    //박스 열기 Action 전송
    public void open(int index) {
        //button index 판별
        int boxNum = userStore.myData.boxes;
        int openNum = 0;
        garage_box_open act = ActionCreator.createAction(ActionTypes.BOX_OPEN) as garage_box_open;
        switch (index) {
            case 0:
                openNum = 1;
                break;
            case 1:
                openNum = 10;
                break;
        }
        //더 이상 열 수 있는 박스가 없는 경우 Modal 활성화
        if(boxNum >= openNum) {
            act.num = openNum;
            gm.gameDispatcher.dispatch(act);
            singleOpenButton.GetComponent<UIPlaySound>().Play();
        }
        else {
            onNotifyModal();
            notifyModal.GetComponent<UIPlaySound>().Play();
        }
    }

    //박스가 없는 경우
    public void onNotifyModal() {
        notifyModal.SetActive(true);
    }

    public void offNotifyModal() {
        notifyModal.SetActive(false);
    }

    public void offBoxOpenModal() {
        boxOpenModal.transform.Find("MultipleModal").gameObject.SetActive(false);
        boxOpenModal.transform.Find("SingleModal").gameObject.SetActive(false);

        boxOpenModal.SetActive(false);
    }

    IEnumerator openEffect(List<Transform> list, int itemCount, Box_Inventory.boxOpenCallback[] items, GameObject modal) {
        int cnt = 0;
        modal.transform.Find("Buttons/ConfirmButton").gameObject.GetComponent<UIButton>().enabled = false;
        modal.transform.Find("Buttons/CancelButton").gameObject.GetComponent<UIButton>().enabled = false;
        foreach (Transform item in list) {
            if (cnt < itemCount) {
                //effect
                GameObject effect = Instantiate(_openEffect);
                effect.transform.SetParent(item.transform);
                effect.transform.localPosition = new Vector3(0f, 55f, 0f);
                effect.transform.localScale = Vector3.one;

                //setUI
                string type = items[cnt].type;
                UILabel label = item.Find("Name").GetComponent<UILabel>();
                UISprite sprite = item.Find("Image").GetComponent<UISprite>();
                
                if (type == "item") {
                    sprite.atlas = bicycleAtlas;
                    string spriteName = items[cnt].item.id + "-1";
                    sprite.spriteName = spriteName;

                    label.text = items[cnt].item.name;
                }
                else if (type == "character") {
                    sprite.atlas = charAtlas;
                    string spriteName = items[cnt].character.id.ToString();
                    sprite.spriteName = spriteName;
                    label.text = items[cnt].character.name;
                }
                cnt++;
            }

            yield return new WaitForSeconds(1.0f);
        }
        modal.transform.Find("Buttons/ConfirmButton").gameObject.GetComponent<UIButton>().enabled = true;
        modal.transform.Find("Buttons/CancelButton").gameObject.GetComponent<UIButton>().enabled = true;
    }

    public void offPanel() {
        gameObject.SetActive(false);
    }
}
