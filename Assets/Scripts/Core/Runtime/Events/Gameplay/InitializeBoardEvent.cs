using BonLib.Events;
using UnityEngine;

namespace Core.Runtime.Events.Gameplay
{

    public struct InitializeBoardEvent : IEvent
    {
        public bool IsConsumed { get; set; }
        
        public Vector2Int Dimensions;
        public Vector2 UnitOffset;


        public InitializeBoardEvent(Vector2Int dimensions, Vector2 unitOffset) : this()
        {
            Dimensions = dimensions;
            UnitOffset = unitOffset;
        }
    }

}