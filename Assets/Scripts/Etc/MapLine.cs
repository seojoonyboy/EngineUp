using UnityEngine;
using System.Collections.Generic;

public class MapLine : MonoBehaviour {
    public void drawLine(List<Vector2> list) {
        OnlineMaps.instance.AddDrawingElement(new OnlineMapsDrawingLine(list, Color.red, 3f));
    }

    public void drawMark(Vector2 pos) {
        gameObject.GetComponent<OnlineMaps>().AddMarker(pos);
    }
}