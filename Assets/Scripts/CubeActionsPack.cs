using System;
using System.Collections.Generic;
using Deblockle.Gameplay;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Deblockle.Managers
{
    [CreateAssetMenu(fileName = "New Cube Actions Pack", menuName = "Deblockle/ActionsPack", order = 0)]
    public class CubeActionsPack : SerializedScriptableObject
    {
        [OdinSerialize] private Dictionary<int, Tuple<ActionType, Sprite>> cubeActions;

        public  Sprite GetIcon(int id)
        {
            return cubeActions[id].Item2;
        }

        public ActionType GetAction(int id)
        {
            return cubeActions[id].Item1;
        }
    }
}