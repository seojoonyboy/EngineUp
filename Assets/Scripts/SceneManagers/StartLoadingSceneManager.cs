﻿using UnityEngine;

public class StartLoadingSceneManager : SceneManager {
    public GameObject gameManager;

    void Awake() {
        if(GameManager.Instance == null){
            Instantiate(gameManager);
        }
        var _gameManager = GameManager.Instance;
        _gameManager.userStore = new User(_gameManager.gameDispatcher);
    }

    void Start() {

    }

    public void loadMainScene() {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Main");
    }
}
