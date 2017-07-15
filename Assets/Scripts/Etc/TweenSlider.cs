using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("NGUI/Tween/Slider")]
public class TweenSlider : UITweener {
    public float from = 0.0f;
    public float to = 0.0f;

    protected override void OnUpdate(float factor, bool isFinished) {
        GetComponent<Slider>().value = from * (1f - factor) + to * factor;
    }

    static public TweenSlider Begin(GameObject go, float duration, float sliderVal) {
        TweenSlider comp = UITweener.Begin<TweenSlider>(go, duration);

        if(duration <= 0f) {
            comp.Sample(1f, true);
            comp.enabled = false;
        }
        return comp;
    }
}
