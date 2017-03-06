using Flux;

public abstract class AjwStore : Store<Actions> {
    public AjwStore(QueueDispatcher<Actions> _dispatcher) : base(_dispatcher) { }
    public new QueueDispatcher<Actions> dispatcher {
        get { return (QueueDispatcher<Actions>) base.dispatcher; }
    }
}

public enum storeStatus { ERROR, NORMAL, WAITING_REQ };