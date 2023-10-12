using BonLib.Events;
using UnityEngine;

namespace Core.Runtime.Events.Gameplay
{

    public struct SetPlayableAreaEvent : IEvent
    {
        public bool IsConsumed { get; set; }

        public Vector3 Dimensions;

        public SetPlayableAreaEvent(Vector3 dimensions) : this()
        {
            Dimensions = dimensions;
        }
    }

}