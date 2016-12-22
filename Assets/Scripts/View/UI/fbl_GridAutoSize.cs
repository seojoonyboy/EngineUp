using UnityEngine;
using System.Collections;

public class fbl_GridAutoSize : MonoBehaviour {
    UIGrid mGrid;
    UIPanel parentPanel;
    Component[] uiSprites;

    void OnEnable() {
        mGrid = GetComponent<UIGrid>();
        parentPanel = transform.parent.GetComponent<UIPanel>();
        uiSprites = GetComponentsInChildren(typeof(UISprite));
        foreach (UISprite sprite in uiSprites) {
            sprite.SetDimensions((int)parentPanel.height, (int)parentPanel.height);
        }
        mGrid.cellWidth = GetComponentInChildren<UISprite>().width * 1.5f;
    }
}