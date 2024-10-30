using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DataManager : SingletonManager<DataManager>
{
    public Text Gold; //��� �г�

    public int totalGold = 0;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Update()
    {
        Gold.text = $"{totalGold}";
    }
}
