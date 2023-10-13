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
        private ItemManager m_itemManager;
        private SlotManager m_slotManager;

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
            m_slotManager.InitializeSlots(in evt.Dimensions);
            m_itemManager.InitializeItems();

            var totalArea = new Vector3(
                (evt.Dimensions.x - 1) * evt.UnitOffset.x + m_slotManager.Config.Scale.x, 
                (evt.Dimensions.y - 1) * evt.UnitOffset.y + m_slotManager.Config.Scale.y, 
                1f);
            
            var setPlayableAreaEvt = new SetPlayableAreaEvent(totalArea);
            EventManager.SendEvent(ref setPlayableAreaEvt);
        }
    }

}