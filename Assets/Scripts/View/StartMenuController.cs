using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
public class StartMenuController : MonoBehaviour {
    SoundManager sm;

    void Awake() {
        sm = SoundManager.Instance;
    }

    public void onStartLoadingScene() {
        SceneManager.LoadScene("StartLoading");
    }

    public void onStartButton() {
        SoundManager.Instance.playEffectSound(0);
        Invoke("onStartLoadingScene", 1.0f);
    }
}
