using System.Collections;
using UnityEngine;

//자전거 아이템 관련
public class getItems_act : NetworkAction { }
public class equip_act : NetworkAction {
    public int id;
}
public class unequip_act : equip_act { }
public class garage_lock_act : equip_act {
    public string type;
}

//캐릭터 관련
public class getCharacters_act : NetworkAction { }