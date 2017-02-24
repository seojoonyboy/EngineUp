using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
public class StartMenuController : MonoBehaviour {
    public void onStartLoadingScene() {
        SceneManager.LoadScene("StartLoading");
    }
}
