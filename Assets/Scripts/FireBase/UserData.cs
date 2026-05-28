using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq.Expressions;

[System.Serializable]
public class UserData
{
    public string NickName;
    public int Coin;
    public int Score;

    public string UnitList;
    public string Inventory;

    public UserData()
    {
        
    }

    public UserData(string nickName)
    {
        NickName = nickName;
        Coin = 500;
        Score = 0;

        Dictionary<string, bool> unitList = new Dictionary<string, bool>();
        unitList["Unit1"] = true;

        for (int i = 2; i < 6; i++)
        {
            unitList["Unit" + i] = false;
        }

        Dictionary<string, int> inventory = new Dictionary<string, int>();
        inventory["Potion"] = 0;
        inventory["Bomb"] = 0;
        inventory["Ticket"] = 0;

        UnitList = JsonConvert.SerializeObject(unitList);
        Inventory = JsonConvert.SerializeObject(inventory);
    }
}
