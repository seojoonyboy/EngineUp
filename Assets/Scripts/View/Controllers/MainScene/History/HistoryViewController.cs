using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HistoryViewController : MonoBehaviour {
    public GameObject 
        container,
        innerContainer;

    public GameObject[] items;
    public UIScrollView scrollView;

    public int containerHeight = 400;

    void OnEnable() {
        makeList();
    }

    public void makeList() {
        items = new GameObject[5];
        for(int i = 0; i < 5; i++) {
            items[i] = Instantiate(container);

            items[i].transform.SetParent(scrollView.transform);
            items[i].transform.localPosition = Vector3.zero;
            items[i].transform.localScale = Vector3.one;

            int childCnt = 0;
            for(int j = 0; j < i; j++) {
                GameObject item = Instantiate(innerContainer);

                UIGrid grid = items[i].transform.Find("Grid").GetComponent<UIGrid>();
                item.transform.SetParent(grid.transform);

                item.transform.localPosition = Vector3.zero;
                item.transform.localScale = Vector3.one;

                childCnt++;
            }

            items[i].GetComponent<UIWidget>().ResetAnchors();

            if (i != 0) {
                UIAnchor anchor = items[i].AddComponent<UIAnchor>();
                anchor.runOnlyOnce = false;
                anchor.container = items[i - 1].transform.Find("Grid").gameObject;
                anchor.side = UIAnchor.Side.Bottom;
                anchor.pixelOffset = new Vector2(0, -400.0f);
            }
        }
    }

    public void offPanel() {
        gameObject.SetActive(false);
    }
}
