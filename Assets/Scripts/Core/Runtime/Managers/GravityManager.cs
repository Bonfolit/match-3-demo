using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BonLib.DependencyInjection;
using BonLib.Managers;
using BonLib.Pooling;
using Core.Config;
using Core.Runtime.Helpers;
using Core.Runtime.Items;
using Core.Runtime.Slots;
using Core.Solver;
using DG.Tweening;
using DG.Tweening.Core;
using NaughtyAttributes;
using UnityEngine;

namespace Core.Runtime.Managers
{

    public class GravityManager : Manager<GravityManager>
    {
        private GravityConfig m_config;
        public GravityConfig Config => m_config ??= Resources.Load<GravityConfig>("Config/GravityConfig");
        
        private BoardManager m_boardManager;
        private SlotManager m_slotManager;
        private ItemManager m_itemManager;
        private GraphicManager m_graphicManager;

        public override void ResolveDependencies()
        {
            base.ResolveDependencies();

            m_boardManager = DI.Resolve<BoardManager>();
            m_slotManager = DI.Resolve<SlotManager>();
            m_itemManager = DI.Resolve<ItemManager>();
            m_graphicManager = DI.Resolve<GraphicManager>();
        }

        [Button]
        public async Task<bool> TryApplyGravity()
        {
            var boardState = m_boardManager.GetCurrentBoardState();

            var moveCommands = GenerateMoveCommands(in boardState);

            var commandCount = moveCommands.Count;

            if (commandCount == 0)
            {
                return false;
            }

            var tasks = new Task[commandCount];

            for (int i = 0; i < moveCommands.Count; i++)
            {
                var command = moveCommands[i];
                var targetSlot = m_slotManager.GetSlot(command.ToIndex);
                var coords = command.ToIndex.GetCoordinates(boardState.Width);
                var delay = coords.y * Config.DelayPerRow;

                var task = ApplyGravity(command.Item, targetSlot, delay);
                tasks[i] = task;
            }

            await Task.WhenAll(tasks);

            return true;
        }
        
        public async Task ApplyGravity(Item item, Slot slot, float delay)
        {
            var origin = m_boardManager.GetWorldPosition(item.Address.Slot.Id);
            var destination = m_boardManager.GetWorldPosition(slot.Id);

            var dist = Vector3.Distance(origin, destination);

            var duration = Mathf.Sqrt(2 * (dist / Config.Acceleration));

            var poolTransform = ((PoolObject)m_graphicManager.GetItemGraphic(in item).Target).transform;

            poolTransform.DOJump(destination, 0, 1, duration)
                .SetDelay(delay);
            
            m_boardManager.SetAddress(ref item, in slot);

            await Task.Delay((int)((duration + delay) * 1000f));
        }

        private List<MoveCommand> GenerateMoveCommands(in BoardState boardState)
        {
            var gaps = new bool[boardState.Height];

            var moveCommands = new List<MoveCommand>();

            for (int i = 0; i < boardState.Width; i++)
            {
                gaps.Populate(false);

                for (int j = 0; j < boardState.Height; j++)
                {
                    var index = i + boardState.Width * j;
                    if (!boardState.Items[index].IsValid)
                    {
                        gaps[j] = true;
                    }
                }

                for (int j = boardState.Height - 1; j >= 0; j--)
                {
                    if (gaps[j])
                        continue;

                    var gapCount = 0;

                    for (int k = j; k >= 0; k--)
                    {
                        if (gaps[k])
                        {
                            gapCount++;
                        }
                    }

                    if (gapCount == 0)
                    {
                        break;
                    }

                    var index = i + boardState.Width * j;

                    var moveCommand = new MoveCommand(index, index - boardState.Width * gapCount,
                        boardState.Items[index]);

                    moveCommands.Add(moveCommand);
                }
            }

            moveCommands = moveCommands.OrderBy(x => x.ToIndex).ToList();
            
            return moveCommands;
        }
    }

}