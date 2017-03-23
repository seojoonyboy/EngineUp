using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridAutoSize : MonoBehaviour {
    private UIGrid grid;
    public UIGrid[] childGrids;
    
    public UISprite target;

    public float 
        standardGridWidth,
        standardWidth,
        standardChildGridWidth;

    void Start() {
        grid = gameObject.GetComponent<UIGrid>();
        standardChildGridWidth = childGrids[0].cellWidth;
        float offsetX = target.width * standardChildGridWidth / standardWidth;
        for(int i=0; i<childGrids.Length; i++) {
            childGrids[i].cellWidth = offsetX;
        }
        grid.cellWidth = standardGridWidth * target.width / standardWidth;
        grid.gameObject.GetComponent<UICenterOnChild>().Recenter();
    }
}
