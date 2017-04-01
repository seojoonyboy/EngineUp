using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterViewControlller : MonoBehaviour {
    GameManager gm;
    Char_Inventory charInvenStore;
    //character_inventory[] characters;

    public GameObject
        mainStageChar,
        lv1Slot,
        lv10Slot,
        lv20Slot,
        itemPref,
        selectedChar;

    public GameObject
        disabledUnlockButton,
        unlockButton;

    public GameObject
        descModal;

    public UIAtlas[] altlasArr;
    public RuntimeAnimatorController[] animatorArr;
    
    public UIGrid 
        itemGrid,
        sideBarGrid;

    public UILabel lvLabel;

    void Awake() {
        gm = GameManager.Instance;
        charInvenStore = gm.charInvenStore;
    }

    void OnEnable() {
        //characters = charInvenStore.myCharacters.character_inventory;
        //Debug.Log(characters[0].cost);
        //setMainChar(0);
    }

    //내 캐릭터중 하나 선택시
    public void charSelected(GameObject obj) {
        selectedChar = obj;
        int index = obj.GetComponent<ButtonIndex>().index;
        setMainChar(index);
        setSideBar(index);
        setSlot();
        setButton();
        //Debug.Log(index);
    }

    private void setMainChar(int index) {
        mainStageChar.GetComponent<Animator>().runtimeAnimatorController = animatorArr[0];
    }

    private void setSideBar(int index) {
        
    }

    private void setSlot() {
        //for(int i=0; i<characters.Length; i++) {
        //    GameObject item = Instantiate(itemPref);
        //    item.transform.SetParent(itemGrid.transform);
        //}
        setLocked();
    }

    //해제되지 않은 캐릭터 처리
    private void setLocked() {
        
    }

    //해금 가능 조건인지 판단
    private void setButton() {
        //해금이 가능한 경우
        //해금이 불가능한 경우
        
    }

    //캐릭터 해금하기
    public void unlockChar() {
        
    }

    //캐릭터 스토리 보기
    public void showCharDesc() {
        descModal.SetActive(true);
    }

    public void offCharDesc() {
        descModal.SetActive(false);
    }
}
