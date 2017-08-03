using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sorting : MonoBehaviour {
    public static void itemSort(ArrayList list, int type) {
        int index = 0;
        switch (type) {
            //자전거 아이템
            case 0:
                index = PlayerPrefs.GetInt("Filter_BICYCLE");
                if (index == 1) {
                    list.Sort(new Bicycle_SortByName());
                }
                else if (index == 2) {
                    list.Sort(new Bicycle_SortByGrade());
                }
                break;
            //캐릭터 아이템
            case 1:
                index = PlayerPrefs.GetInt("Filter_CHAR");
                break;
        }
    }
}

public class Bicycle_SortByGrade : IComparer, IComparer<RespGetItems> {
    public int Compare(RespGetItems x, RespGetItems y) {
        //throw new NotImplementedException();
        int xGrade = x.item.grade;
        int yGrade = y.item.grade;

        if (xGrade == yGrade) {
            return x.id.CompareTo(y.id);
        }
        else {
            return xGrade.CompareTo(yGrade);
        }
    }

    public int Compare(object x, object y) {
        //throw new NotImplementedException();
        RespGetItems _x = x as RespGetItems;
        RespGetItems _y = y as RespGetItems;

        if (_x.id == _y.id) {
            return _x.id.CompareTo(_y.id);
        }
        return Compare(_x, _y);
    }
}

public class Bicycle_SortByName : IComparer, IComparer<RespGetItems> {
    public int Compare(RespGetItems x, RespGetItems y) {
        string xName = x.item.name;
        string yName = y.item.name;

        if (xName == yName) {
            return x.id.CompareTo(y.id);
        }
        else {
            return x.item.name.CompareTo(y.item.name);
        }
    }

    public int Compare(object x, object y) {
        RespGetItems _x = x as RespGetItems;
        RespGetItems _y = y as RespGetItems;

        if (_x.id == _y.id) {
            return _x.id.CompareTo(_y.id);
        }
        return Compare(_x, _y);
    }
}


//public class Character_SortByGrade : IComparer, IComparer<Character_inventory> {
//    public int Compare(Character_inventory x, Character_inventory y) {
//        //throw new NotImplementedException();
//        int xGrade = x.item.grade;
//        int yGrade = y.item.grade;

//        if (xGrade == yGrade) {
//            return x.id.CompareTo(y.id);
//        }
//        else {
//            return xGrade.CompareTo(yGrade);
//        }
//    }

//    public int Compare(object x, object y) {
//        //throw new NotImplementedException();
//        Character_inventory _x = x as Character_inventory;
//        Character_inventory _y = y as Character_inventory;

//        if (_x.id == _y.id) {
//            return _x.id.CompareTo(_y.id);
//        }
//        return Compare(_x, _y);
//    }
//}