using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item {
    public int id;
    public string name;
    public string desc;
}

public class BicycleItem : Item {
    public int grade;
    public int gear;
    public string parts;
    public int limit_rank;

    public int strength;
    public int speed;
    public int endurance;
    public int regeneration;
}

public class CharacterItem : Item {
    
}
