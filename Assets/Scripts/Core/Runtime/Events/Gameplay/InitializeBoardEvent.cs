using BonLib.Events;
using UnityEngine;

namespace Core.Runtime.Events.Gameplay
{

    public struct InitializeBoardEvent : IEvent
    {
        public bool IsConsumed { get; set; }
        
        public Vector2Int Dimensions;

        public InitializeBoardEvent(Vector2Int dimensions) : this()
        {
            Dimensions = dimensions;
        }
    }

}