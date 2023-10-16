using System;
using System.Threading.Tasks;
using BonLib.DependencyInjection;
using BonLib.Events;
using BonLib.Managers;
using BonLib.Pooling;
using Core.Runtime.Events.Gameplay;
using Core.Runtime.Slots;
using Core.Solver;
using DG.Tweening;

namespace Core.Runtime.Managers
{

    public class SwapManager : Manager<SwapManager>,
        IEventHandler<SwapSlotsEvent>
    {
        private BoardManager m_boardManager;
        private GraphicManager m_graphicManager;

        private Task m_swapTask;

        public override void ResolveDependencies()
        {
            base.ResolveDependencies();

            m_boardManager = DI.Resolve<BoardManager>();
            m_graphicManager = DI.Resolve<GraphicManager>();
        }

        public override void SubscribeToEvents()
        {
            base.SubscribeToEvents();
            
            EventManager.AddListener<SwapSlotsEvent>(this);
        }

        public void OnEventReceived(ref SwapSlotsEvent evt)
        {
            if (m_swapTask != null && !m_swapTask.IsCompleted)
                return;

            m_swapTask = SwapSlots(evt.FromSlot, evt.ToSlot, evt.Duration);
        }
        
        private async Task SwapSlots(Slot fromSlot, Slot toSlot, float duration)
        {
            var matchState = m_boardManager.GetCurrentMatchState();
            
            var fromItem = m_boardManager.GetItemAtSlot(in fromSlot);
            var toItem = m_boardManager.GetItemAtSlot(in toSlot);

            if (!fromItem.IsValid || !toItem.IsValid)
                return;

            matchState.TemplateIds[fromSlot.Id] = toItem.TemplateId;
            matchState.TemplateIds[toSlot.Id] = fromItem.TemplateId;

            var hasMatches = BoardSolver.CheckMatches(in matchState, out _);

            var fromPos = m_boardManager.GetWorldPosition(fromSlot.Id);
            var toPos = m_boardManager.GetWorldPosition(toSlot.Id);

            var fromTransform = ((PoolObject)m_graphicManager.GetGraphic(in fromItem.GraphicHandle).Target).transform;
            var toTransform = ((PoolObject)m_graphicManager.GetGraphic(in toItem.GraphicHandle).Target).transform;

            var temp = fromPos;
            temp.z += .1f;
            fromTransform.position = temp;

            if (!hasMatches)
            {
                fromTransform.DOMove(toPos, duration)
                    .OnComplete(() =>
                    {
                        fromTransform.DOMove(fromPos, duration);
                    });
                toTransform.DOMove(fromPos, duration).OnComplete(() =>
                {
                    toTransform.DOMove(toPos, duration);
                });
                
                await Task.Delay((int)(2f * duration * 1000f));
                return;
            }
            
            fromTransform.DOMove(toPos, duration);
            toTransform.DOMove(fromPos, duration);
            
            await Task.Delay((int)(duration * 1000f));
            
            m_boardManager.ClearAddress(in fromItem);
            m_boardManager.ClearSlot(in fromSlot);
            m_boardManager.ClearAddress(in toItem);
            m_boardManager.ClearSlot(in toSlot);
            
            m_boardManager.SetAddress(ref fromItem, in toSlot);
            m_boardManager.SetAddress(ref toItem, in fromSlot);

            var cascadeEvt = new TriggerCascadeEvent();
            EventManager.SendEvent(ref cascadeEvt);
        }
    }

}