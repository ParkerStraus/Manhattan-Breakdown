using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;


public interface IGameHandler
{


    bool CanthePlayersMove()
    {
        return true;
    }
    void OnKill(int whoDied)
    {
    }

    void UpdateMainUI(int UIOverride, string[] Text)
    {

    }

    bool IsPaused()
    {
        return false;
    }

    bool IsMine()
    {
        return true;
    }

    int[] GetScore()
    {
        return new int[] { 0, 0, 0, 0 };
    }

    int GetPlayerAmt() { return 0; }
}
