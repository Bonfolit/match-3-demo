using BonLib.Events;
using UnityEngine;

namespace Core.Runtime.Events.Gameplay
{

    public struct DrawSlotsEvent : IEvent
    {
        public bool IsConsumed { get; set; }
        
        public Vector2Int Dimensions;

        public DrawSlotsEvent(Vector2Int dimensions) : this()
        {
            Dimensions = dimensions;
        }
    }

}