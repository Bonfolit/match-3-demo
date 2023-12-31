﻿using System;
using System.Threading.Tasks;
using BonLib.DependencyInjection;
using BonLib.Events;
using BonLib.Managers;
using BonLib.Pooling;
using Core.Runtime.Events.Gameplay;
using Core.Runtime.Slots;
using Core.Solver;
using DG.Tweening;
using UnityEngine;

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

            var fromRenderer = ((SpriteRenderer)((PoolObject)m_graphicManager.GetGraphic(in fromItem.GraphicHandle).Target).CustomReference);
            var toRenderer = ((SpriteRenderer)((PoolObject)m_graphicManager.GetGraphic(in toItem.GraphicHandle).Target).CustomReference);
            var fromTransform = fromRenderer.transform;
            var toTransform = toRenderer.transform;

            fromRenderer.sortingOrder += 1;

            if (!hasMatches)
            {
                fromTransform.DOMove(toPos, duration)
                    .OnComplete(() =>
                    {
                        fromTransform.DOMove(fromPos, duration)
                            .OnComplete(() =>
                            {
                                fromRenderer.sortingOrder -= 1;
                            });
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
            
            fromRenderer.sortingOrder -= 1;
            
            m_boardManager.SwapSlots(in fromSlot, in toSlot);

            var cascadeEvt = new TriggerCascadeEvent();
            EventManager.SendEvent(ref cascadeEvt);
        }
    }

}