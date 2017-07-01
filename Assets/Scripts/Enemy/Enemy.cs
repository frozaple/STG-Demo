using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : BattleObject
{
    public int hp = 0;

    void Awake()
    {
        useScript = true;
    }
}
