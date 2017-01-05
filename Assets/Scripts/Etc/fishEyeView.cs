using UnityEngine;
using System.Collections;

public class fishEyeView : MonoBehaviour {
    Transform mTrans;
    UIPanel mPanel;
    UIWidget mWidget;
    float 
        pos,
        dist,
        cellWidth,
        downScale = 0.35f;

    void Start() {
        mTrans = transform;
        //Scroll View's Panel
        mPanel = mTrans.parent.parent.GetComponent<UIPanel>();
        mWidget = GetComponent<UIWidget>();
        cellWidth = mWidget.width;
    }

    void Update() {
        pos = mTrans.localPosition.x - mPanel.clipOffset.x;
        dist = Mathf.Clamp(Mathf.Abs(pos), 0f, cellWidth);
        //Debug.Log("Dist : " + dist);
        mWidget.width = System.Convert.ToInt32(((cellWidth - dist * downScale) / cellWidth) * cellWidth);
    }
}
