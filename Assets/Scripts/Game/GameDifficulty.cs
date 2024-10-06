using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameDifficulty
{
    const float MIN_MOVE_MULTIPLIER = 0.3f;
    const float TIME_TO_MAX_DIFFICULTY = 70;
    public static float MovementMultiplier
    {
        get
        {
            if (GameManager.Instance == null)
                return 0;
            return Mathf.LerpUnclamped(MIN_MOVE_MULTIPLIER, 1, GameManager.Instance.gameTime *(1/ TIME_TO_MAX_DIFFICULTY));
        }
    }
}
