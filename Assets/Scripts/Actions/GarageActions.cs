using System.Collections;
using UnityEngine;

//자전거 아이템 관련
public class equip_act : NetworkAction {
    public enum type { CHAR, ITEM }
    public type _type;
    public int id;
}
public class getItems_act : equip_act { }
public class unequip_act : equip_act { }
public class garage_lock_act : equip_act {
    public string type;
}
public class garage_sell_act : equip_act { }

//캐릭터 관련
public class getCharacters_act : NetworkAction { }
public class garage_unlock_char : equip_act { }

//박스 관련
public class garage_box_open : NetworkAction {
    public int num = -1;
}