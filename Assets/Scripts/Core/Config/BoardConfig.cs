﻿using BonLib.DependencyInjection;
using UnityEngine;

namespace Core.Config
{
    [CreateAssetMenu(fileName = "BoardConfig", menuName = "Config/Board Config", order = 0)]
    public class BoardConfig : ScriptableObject
    {
        public Vector2Int Dimensions;
        public Vector2 Offset;
        public bool[] SpawnColumns;
    }

}