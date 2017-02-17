using UnityEngine;
using System.Collections;

public class Group_getMemberAction : NetworkAction {
    public int id;
}

public class Group_search : NetworkAction {
    public string keyword;
}

public class Group_OnPanel : Actions {
    public int index;
}