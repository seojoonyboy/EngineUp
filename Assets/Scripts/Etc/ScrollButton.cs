using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollButton : MonoBehaviour {
    public float springStrength = 8.0f;

    public UIScrollView scrollView;
    public UIGrid grid;

    private int elementsPerPage = 3;
    private int currentScrolledElements;
    private Vector3 startingScrollPosition;

    // Use this for initialization
    void Start() {
        //elementsPerPage = (int)(scrollView.panel.clipRange.z / grid.cellWidth);
        //Debug.Log(elementsPerPage);
        currentScrolledElements = 0;
        startingScrollPosition = scrollView.panel.cachedTransform.localPosition;
    }

    // Update is called once per frame
    void Update() {

    }


    /// <summary>
    /// Scrolls until target position matches target panelAnchorPosition (may be the center of the panel, one of its sides, etc)
    /// </summary> 
    void MoveBy(Vector3 target) {
        if (scrollView != null && scrollView.panel != null) {
            // Spring the panel to this calculated position
            SpringPanel.Begin(scrollView.panel.cachedGameObject, startingScrollPosition - target, springStrength);
        }
    }


    public void NextPage() {
        if (scrollView != null && scrollView.panel != null) {
            currentScrolledElements += elementsPerPage;
            if (currentScrolledElements > (grid.transform.childCount - elementsPerPage)) {
                currentScrolledElements = (grid.transform.childCount - elementsPerPage);
            }
            float nextScroll = grid.cellWidth * currentScrolledElements;
            Vector3 target = new Vector3(nextScroll, 0.0f, 0.0f);
            MoveBy(target);
        }
    }

    public void PreviousPage() {
        if (scrollView != null && scrollView.panel != null) {
            currentScrolledElements -= elementsPerPage;
            if (currentScrolledElements < 0) {
                currentScrolledElements = 0;
            }
            float nextScroll = grid.cellWidth * currentScrolledElements;
            Vector3 target = new Vector3(nextScroll, 0.0f, 0.0f);
            MoveBy(target);
        }
    }
}
