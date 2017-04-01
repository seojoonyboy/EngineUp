using Flux;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BicycleItem_Inventory : AjwStore {
    public BicycleItem_Inventory(QueueDispatcher<Actions> _dispatcher) : base(_dispatcher) { }
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
