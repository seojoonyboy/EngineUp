using Flux;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Char_Inventory : AjwStore {
    public Char_Inventory(QueueDispatcher<Actions> _dispatcher) : base(_dispatcher) { }
    public represent_character myCharacters;

    private GameManager gm = GameManager.Instance;
    private User userStore = GameManager.Instance.userStore;

    protected override void _onDispatch(Actions action) {
        switch (action.type) {
            case ActionTypes.SIGNIN:
                string[] ids = new string[1];
                ids[0] = userStore.dispatchToken;
                gm.gameDispatcher.waitFor(ids);
                myCharacters = userStore.myCharacters;
                break;
        }
    }
}

[System.Serializable]
public class character_inventory {
    public int id;
    public int paid;
    public int lv;
    public int exp;
    public int user;
    public int character;
}