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

        private GraphicManager m_graphicManager;
        private ItemManager m_itemManager;
        private BoardManager m_boardManager;

        [SerializeField]
        private PoolObject m_slotPoolObject;

        private GraphicHandle[] m_slotGraphicHandles;

        private Vector2Int m_boardDimensions;

        public override void ResolveDependencies()
        {
            base.ResolveDependencies();
            
            m_graphicManager = DI.Resolve<GraphicManager>();
            m_itemManager = DI.Resolve<ItemManager>();
            m_boardManager = DI.Resolve<BoardManager>();
        }

        public override void SubscribeToEvents()
        {
            base.SubscribeToEvents();
            
            EventManager.AddListener<InitializeBoardEvent>(this);
        }

        public void OnEventReceived(ref InitializeBoardEvent evt)
        {
            m_boardDimensions = evt.Dimensions;
            
            DrawSlots(in m_boardDimensions);
            DrawItems();
        }

        private void DrawSlots(in Vector2Int dimensions)
        {
            var drawOffset = new Vector3(((float)(dimensions.x - 1) / 2f) * Config.Offset.x, ((float)(dimensions.y - 1) / 2f) * Config.Offset.y, 0f);

            Vector3 pos;
            
            m_slotGraphicHandles = new GraphicHandle[dimensions.x * dimensions.y];

            for (int i = 0; i < dimensions.x; i++)
            {
                for (int j = 0; j < dimensions.y; j++)
                {
                    var index = j * dimensions.y + i;
                    
                    pos = -drawOffset + new Vector3(i * Config.Offset.x, j * Config.Offset.y, 0f);
                    
                    var rentedSlotPoolObj = PrefabPool.Rent(m_slotPoolObject);
                    m_slotGraphicHandles[index] = m_graphicManager.CreateHandle(rentedSlotPoolObj);

                    var poolObjTransform = rentedSlotPoolObj.transform;
                    var rend = ((SpriteRenderer)rentedSlotPoolObj.CustomReference);
                    
                    rend.sprite = Config.Sprite;
                    rend.sortingOrder = -1;
                    
                    poolObjTransform.position = pos;
                    poolObjTransform.localScale = Config.Scale;
                }
            }
        }

        private void DrawItems()
        {
            var templateIds = m_itemManager.GetAllTemplateIDs();

            var boardState = m_boardManager.GenerateNewBoardState(in templateIds);
            
            var drawOffset = new Vector3(
                ((float)(boardState.Width - 1) / 2f) * Config.Offset.x, 
                ((float)(boardState.Height - 1) / 2f) * Config.Offset.y, 
                0f);
            
            Vector3 pos;

            for (int i = 0; i < boardState.Width; i++)
            {
                for (int j = 0; j < boardState.Height; j++)
                {
                    var index = j * boardState.Width + i;
                    
                    pos = -drawOffset + new Vector3(i * Config.Offset.x, j * Config.Offset.y, 0f);

                    var item = m_itemManager.CreateItem(boardState.Ids[index]);

                    var graphic = m_graphicManager.GetItemGraphic(in item);
                    var poolObj = ((PoolObject)graphic.Target);
                    var rend = ((SpriteRenderer)poolObj.CustomReference);
                    rend.transform.position = pos;
                }
            }
        }

        [Button]
        public void ClearSlotGraphics()
        {
            for (var i = 0; i < m_slotGraphicHandles.Length; i++)
            {
                var graphic = m_graphicManager.GetGraphic(m_slotGraphicHandles[i]);
                
                m_graphicManager.DisposeHandle(m_slotGraphicHandles[i]);
                
                PrefabPool.Return(((PoolObject)graphic.Target));
            }
        }
    }

}