using Flux;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Garages : AjwStore {
    public Garages(QueueDispatcher<Actions> _dispatcher) : base(_dispatcher) { }
    public ActionTypes eventType;

    protected override void _onDispatch(Actions action) {
        switch (action.type) {

        }
        eventType = action.type;
    }
}
