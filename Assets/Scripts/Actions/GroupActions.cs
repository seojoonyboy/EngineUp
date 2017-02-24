using UnityEngine;
using System.Collections;

public class Group_search : NetworkAction {
    public string keyword;
}

public class Group_OnPanel : Actions {
    public int index;
}

public class Group_AddAction : NetworkAction {
    public string name;
    public string district;
    public string city;
    public string desc = null;
    public int id;
}

public class Group_myGroups : NetworkAction {
    public int id;
}

public class Group_getMemberAction : Group_myGroups {
    public bool forMemberManage = false;
}
public class Group_detail : Group_myGroups { }
public class Group_checkMyStatus : Group_myGroups {
    public int userId;
}
public class Group_join : Group_myGroups { }
public class Group_accept : Group_myGroups {
    public int memberId;
}
public class Group_ban : Group_accept { }
public class Group_detail_refresh : Group_myGroups { }