using System.Collections.Generic;
using System.Linq;
using BonLib.DependencyInjection;
using BonLib.Managers;
using Core.Runtime.Helpers;
using Core.Solver;
using NaughtyAttributes;

namespace Core.Runtime.Managers
{

    public class GravityManager : Manager<GravityManager>
    {
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
        public bool TryApplyGravity()
        {
            var moveCommands = GenerateMoveCommands();

            for (int i = 0; i < moveCommands.Count; i++)
            {
                var command = moveCommands[i];
                var originPos = m_boardManager.GetWorldPosition(command.ToIndex);
                var targetPos = m_boardManager.GetWorldPosition(command.ToIndex);
                var targetSlot = m_slotManager.GetSlot(command.ToIndex);
                
                m_graphicManager.MoveItem(in command.Item, targetPos);
                m_boardManager.SetAddress(ref command.Item, in targetSlot);
            }

            return true;
        }

        private List<MoveCommand> GenerateMoveCommands()
        {
            var boardState = m_boardManager.GetCurrentBoardState();

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