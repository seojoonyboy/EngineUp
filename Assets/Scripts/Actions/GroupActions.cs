using UnityEngine;
using System.Collections;

public class Group_search : NetworkAction {
    public string keyword;
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
    public bool forDestroyManage = false;
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
public class Group_del : Group_myGroups { }
public class Group_posts : Group_myGroups {
    public bool isFirst = false;
}
public class Group_addPosts : Group_myGroups {
    public string context;
}
public class Group_delPost : Group_myGroups {
    public GameObject target;
    public int postId;
}
public class Group_modifyPost : Group_myGroups { }