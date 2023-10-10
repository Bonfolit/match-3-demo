using BonLib.Events;
using BonLib.Managers;
using BonLib.Pooling;
using Core.Config;
using Core.Runtime.Events.Gameplay;
using UnityEngine;

namespace Core.Runtime.Managers
{

    public class SlotManager : Manager<SlotManager>,
         IEventHandler<DrawSlotsEvent>
    {
        private SlotConfig m_config;
        public SlotConfig Config => m_config ??= Resources.Load<SlotConfig>("Config/SlotConfig");

        [SerializeField]
        private PoolObject m_slotPoolObject;

        private PoolObject[] m_rentedSlotPoolObjects;

        public override void SubscribeToEvents()
        {
            base.SubscribeToEvents();
            
            EventManager.AddListener<DrawSlotsEvent>(this);
        }

        public void OnEventReceived(ref DrawSlotsEvent evt)
        {
            var drawOffset = new Vector3(((float)(evt.Dimensions.x - 1) / 2f) * Config.Offset.x, ((float)(evt.Dimensions.y - 1) / 2f) * Config.Offset.y, 0f);

            Vector3 pos;

            m_rentedSlotPoolObjects = new PoolObject[evt.Dimensions.x * evt.Dimensions.y];

            for (int i = 0; i < evt.Dimensions.x; i++)
            {
                for (int j = 0; j < evt.Dimensions.y; j++)
                {
                    pos = -drawOffset + new Vector3(i * Config.Offset.x, j * Config.Offset.y, 0f);
                    
                    var rentedSlotPoolObj = PrefabPool.Rent(m_slotPoolObject);
                    m_rentedSlotPoolObjects[j * evt.Dimensions.y + i] = rentedSlotPoolObj;

                    var poolObjTransform = rentedSlotPoolObj.transform;
                    var rend = ((SpriteRenderer)rentedSlotPoolObj.CustomReference);
                    
                    rend.sprite = Config.Sprite;
                    poolObjTransform.position = pos;
                    poolObjTransform.localScale = Config.Scale;
                }
            }
        }
    }

}