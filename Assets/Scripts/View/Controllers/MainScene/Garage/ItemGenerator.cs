using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGenerator : MonoBehaviour {
    public string[] spriteNameArr;
    public CharacterViewControlller controller;
    public GameObject slot;
    public UIGrid grid;
    public UIAtlas atlas;

    void Start() {
        makeLists();
    }

    public void makeLists() {
        for(int i=0; i<spriteNameArr.Length; i++) {
            GameObject item = Instantiate(slot);
            UISprite sprite = item.transform.Find("Sprite").GetComponent<UISprite>();
            sprite.atlas = atlas;
            sprite.spriteName = spriteNameArr[i];

            item.transform.SetParent(grid.transform);

            item.transform.localScale = Vector3.one;
            item.transform.localPosition = Vector3.zero;

            EventDelegate.Parameter param = new EventDelegate.Parameter();
            param.obj = item;
            EventDelegate clickEvent = new EventDelegate(controller, "charSelected");
            clickEvent.parameters[0] = param;
            EventDelegate.Add(item.GetComponent<UIButton>().onClick, clickEvent);

            initGrid();
        }
    }

    public void initGrid() {
        grid.repositionNow = true;
        grid.Reposition();
    }
}
