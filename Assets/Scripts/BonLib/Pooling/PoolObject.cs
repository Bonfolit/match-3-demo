using UnityEngine;
using Object = UnityEngine.Object;

namespace BonLib.Pooling
{
    public class PoolObject : MonoBehaviour
    {
        [SerializeField]
        private PoolObject m_template;
        public PoolObject Template
        {
            get => m_template;
            set
            {
                m_template = value;
            }
        }
        
        public Object CustomReference;
    }

}