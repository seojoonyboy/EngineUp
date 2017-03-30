using Flux;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Char_Inventory : AjwStore {
    public Char_Inventory(QueueDispatcher<Actions> _dispatcher) : base(_dispatcher) { }

    protected override void _onDispatch(Actions action) {
        switch (action.type) {
            case ActionTypes.SIGNUP:
                
                break;
        }
    }
}