using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelsListSO", menuName = "Scriptable Objects/LevelsListSO")]
public class LevelsListSO : ScriptableObject
{
    [SerializeField] private List<GameLevel> gameLevelsList;

    public List<GameLevel> GameLevelsList => gameLevelsList;
}
