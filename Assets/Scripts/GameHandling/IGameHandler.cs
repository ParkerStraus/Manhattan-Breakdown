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
}
