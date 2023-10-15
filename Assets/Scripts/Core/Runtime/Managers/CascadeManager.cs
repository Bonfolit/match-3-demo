using System.Threading.Tasks;
using BonLib.DependencyInjection;
using BonLib.Events;
using BonLib.Managers;
using Codice.Client.BaseCommands;
using Core.Runtime.Events.Gameplay;
using Core.Runtime.Slots;
using Core.Solver;
using NaughtyAttributes;

namespace Core.Runtime.Managers
{

    public class CascadeManager : Manager<CascadeManager>,
        IEventHandler<TriggerCascadeEvent>
    {
        private GravityManager m_gravityManager;
        private BoardManager m_boardManager;
        private ItemManager m_itemManager;

        private Task m_cascadeTask;

        public override void ResolveDependencies()
        {
            base.ResolveDependencies();

            m_gravityManager = DI.Resolve<GravityManager>();
            m_boardManager = DI.Resolve<BoardManager>();
            m_itemManager = DI.Resolve<ItemManager>();
        }

        public override void SubscribeToEvents()
        {
            base.SubscribeToEvents();
            
            EventManager.AddListener<TriggerCascadeEvent>(this);
        }

        public void OnEventReceived(ref TriggerCascadeEvent evt)
        {
            if (m_cascadeTask != null && !m_cascadeTask.IsCompleted)
            {
                return;
            }
            
            m_cascadeTask = TriggerCascade();
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

                            var evt = new DestroyItemEvent(item, new Slot(matchDataIndex));
                            EventManager.SendEvent(ref evt);
                        }
                    }
                }

                continueCascade = appliedGravity || hasMatches;
            }
        }
    }

}