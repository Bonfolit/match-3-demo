using BonLib.DependencyInjection;
using BonLib.Managers;
using BonLib.Pooling;
using Core.Config;
using Core.Runtime.Graphics;
using Core.Runtime.Slots;
using NaughtyAttributes;
using UnityEngine;

namespace Core.Runtime.Managers
{

    public class SlotManager : Manager<SlotManager>
    {
        private static int MAIN_TEX_ID = Shader.PropertyToID("_MainTex");
        private static int STENCIL_REF_ID = Shader.PropertyToID("_StencilRef");
        
        private SlotConfig m_config;
        public SlotConfig Config => m_config ??= Resources.Load<SlotConfig>("Config/SlotConfig");
        
        [SerializeField]
        private PoolObject m_slotPoolObject;
        
        private Slot[] m_slots;
        
        private GraphicManager m_graphicManager;
        private BoardManager m_boardManager;

        public override void ResolveDependencies()
        {
            base.ResolveDependencies();

            m_graphicManager = DI.Resolve<GraphicManager>();
            m_boardManager = DI.Resolve<BoardManager>();
        }

        public void InitializeSlots(in Vector2Int dimensions)
        {
            var count = dimensions.x * dimensions.y;
            m_slots = new Slot[count];

            for (int i = 0; i < dimensions.x; i++)
            {
                for (int j = 0; j < dimensions.y; j++)
                {
                    var index = j * dimensions.x + i;
                    
                    var pos = m_boardManager.GetWorldPosition(index);
                    
                    var rentedSlotPoolObj = PrefabPool.Rent(m_slotPoolObject);
                    var handle = m_graphicManager.CreateHandle(rentedSlotPoolObj);

                    m_slots[index] = new Slot(index, in handle);
                    
                    var poolObjTransform = rentedSlotPoolObj.transform;
                    
                    poolObjTransform.position = pos;
                    poolObjTransform.localScale = Config.Scale;
                }
            }
        }

        public Slot GetSlot(int index)
        {
            return m_slots[index];
        }

        [Button]
        public void ClearSlots()
        {
            for (var i = 0; i < m_slots.Length; i++)
            {
                var graphic = m_graphicManager.GetGraphic(m_slots[i].GraphicHandle);

                m_graphicManager.DisposeHandle(in m_slots[i].GraphicHandle);
                
                PrefabPool.Return(((PoolObject)graphic.Target));
            }
        }
    }

}