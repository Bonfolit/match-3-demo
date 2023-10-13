using BonLib.DependencyInjection;
using BonLib.Events;
using BonLib.Managers;
using BonLib.Pooling;
using Core.Config;
using Core.Runtime.Events.Gameplay;
using Core.Runtime.Graphics;
using NaughtyAttributes;
using UnityEngine;

namespace Core.Runtime.Managers
{

    public class BoardInitializer : Manager<BoardInitializer>,
        IEventHandler<InitializeBoardEvent>
    {
        private SlotConfig m_config;
        public SlotConfig Config => m_config ??= Resources.Load<SlotConfig>("Config/SlotConfig").Bind();

        private ItemManager m_itemManager;
        private SlotManager m_slotManager;

        private GraphicHandle[] m_slotGraphicHandles;

        private Vector2Int m_boardDimensions;

        public override void ResolveDependencies()
        {
            base.ResolveDependencies();
            
            m_itemManager = DI.Resolve<ItemManager>();
            m_slotManager = DI.Resolve<SlotManager>();
        }

        public override void SubscribeToEvents()
        {
            base.SubscribeToEvents();
            
            EventManager.AddListener<InitializeBoardEvent>(this);
        }

        public void OnEventReceived(ref InitializeBoardEvent evt)
        {
            m_boardDimensions = evt.Dimensions;

            m_slotManager.InitializeSlots(in m_boardDimensions);
            m_itemManager.InitializeItems();

            var totalArea = new Vector3(
                (m_boardDimensions.x - 1) * Config.Offset.x + Config.Scale.x, 
                (m_boardDimensions.y - 1) * Config.Offset.y + Config.Scale.y, 
                1f);
            
            var setPlayableAreaEvt = new SetPlayableAreaEvent(totalArea);
            EventManager.SendEvent(ref setPlayableAreaEvt);
        }
    }

}