using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterViewControlller : MonoBehaviour {
    public GameObject
        mainChar,
        lv1Slot,
        lv10Slot,
        lv20Slot;

    public UIAtlas[] altlasArr;
    public Animator[] animatorArr;

    public void charSelected(GameObject obj) {
        int index = obj.GetComponent<ButtonIndex>().index;
        setMainChar(index);
        setSideBar(index);
        Debug.Log(index);
    }

    private void setMainChar(int index) {
        
    }

    private void setSideBar(int index) {

    }
}
