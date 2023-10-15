using System.Threading.Tasks;
using BonLib.DependencyInjection;
using BonLib.Events;
using BonLib.Managers;
using BonLib.Pooling;
using Codice.Client.BaseCommands;
using Core.Config;
using Core.Runtime.Events.Gameplay;
using Core.Runtime.Slots;
using Core.Solver;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

namespace Core.Runtime.Managers
{

    public class CascadeManager : Manager<CascadeManager>,
        IEventHandler<TriggerCascadeEvent>,
        IEventHandler<SwapSlotsEvent>
    {
        private CascadeConfig m_config;
        public CascadeConfig Config =>
            m_config ??= Resources.Load<CascadeConfig>("Config/CascadeConfig");
        
        private GravityManager m_gravityManager;
        private BoardManager m_boardManager;
        private ItemManager m_itemManager;
        private GraphicManager m_graphicManager;

        private Task m_cascadeTask;

        public override void ResolveDependencies()
        {
            base.ResolveDependencies();

            m_gravityManager = DI.Resolve<GravityManager>();
            m_boardManager = DI.Resolve<BoardManager>();
            m_itemManager = DI.Resolve<ItemManager>();
            m_graphicManager = DI.Resolve<GraphicManager>();
        }

        public override void SubscribeToEvents()
        {
            base.SubscribeToEvents();
            
            EventManager.AddListener<TriggerCascadeEvent>(this);
            EventManager.AddListener<SwapSlotsEvent>(this, Priority.Critical);
        }

        public void OnEventReceived(ref TriggerCascadeEvent evt)
        {
            if (m_cascadeTask != null && !m_cascadeTask.IsCompleted)
            {
                return;
            }
            
            m_cascadeTask = TriggerCascade();
        }

        public void OnEventReceived(ref SwapSlotsEvent evt)
        {
            if (m_cascadeTask != null && !m_cascadeTask.IsCompleted)
            {
                evt.IsConsumed = true;
            }
        }

        [Button]
        public async Task TriggerCascade()
        {
            const int CASCADE_LIMIT = 1000;
            var cascadeCount = 0;
            var continueCascade = true;

            while (continueCascade && cascadeCount < CASCADE_LIMIT)
            {
                cascadeCount++;
                
                var appliedGravity = await m_gravityManager.TryApplyGravity();

                var matchState = m_boardManager.GetCurrentMatchState();
                var hasMatches = BoardSolver.CheckMatches(in matchState, out var matches);

                if (hasMatches)
                {
                    foreach (var matchData in matches)
                    {
                        foreach (var matchDataIndex in matchData.Indices)
                        {
                            var item = m_boardManager.GetItemAtSlot(matchDataIndex);
                            if (!item.IsValid)
                                continue;

                            var graphicTransform = ((PoolObject)m_graphicManager.GetItemGraphic(in item).Target).transform;
                            graphicTransform.DOScale(Vector3.zero, Config.DestroyDuration)
                                .SetEase(Config.DestroyEase)
                                .OnComplete(() =>
                                {
                                    var evt = new DestroyItemEvent(item, new Slot(matchDataIndex));
                                    EventManager.SendEvent(ref evt);
                                });
                        }
                    }

                    await Task.Delay((int)(Config.DestroyDuration * 1000f));
                }

                continueCascade = appliedGravity || hasMatches;
            }
        }
    }

}