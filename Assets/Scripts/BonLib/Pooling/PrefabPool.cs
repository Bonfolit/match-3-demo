using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BonLib.Pooling
{
    public class PrefabPool : MonoBehaviour
    {
        private static Dictionary<int, PrefabPool> s_map = new Dictionary<int, PrefabPool>();

        public PoolObject Template;
        
        [SerializeField]
        private List<PoolObject> m_objects = new List<PoolObject>();

        [SerializeField]
        private int m_capacity;

        private void Awake()
        {
            RegisterPool(Template, this);

            for (var i = 0; i < m_objects.Count; i++)
            {
                var poolObject = m_objects[i];
                poolObject.Template = Template;
            }
        }

        public static void RegisterPool(PoolObject template, PrefabPool pool)
        {
            int instanceId = template.GetInstanceID();
            if (!s_map.ContainsKey(instanceId))
            {
                s_map.Add(instanceId, pool);
            }
        }
        
        public static void UnregisterPool(PoolObject template)
        {
            int instanceId = template.GetInstanceID();
            if (s_map.ContainsKey(instanceId))
            {
                s_map.Remove(instanceId);
            }
        }

#if UNITY_EDITOR
        public void Populate()
        {
            foreach (Transform child in transform)
            {
                if (child == transform)
                    continue;
                
                DestroyImmediate(child.gameObject);
            }
            
            m_objects = new List<PoolObject>();

            for (int i = 0; i < m_capacity; i++)
            {
                AddNew();
            }
            
            EditorUtility.SetDirty(this);
        }
#endif
       
        public void AddNew()
        {
            PoolObject instance;
#if UNITY_EDITOR
            if (!Application.isPlaying && UnityEditor.PrefabUtility.IsPartOfPrefabAsset(Template))
            {
                instance = (PoolObject)UnityEditor.PrefabUtility.InstantiatePrefab(Template, transform);
                instance.gameObject.name = $"Instance_{Template.gameObject.name}_{m_objects.Count:000}";
                
                instance.gameObject.SetActive(false);
            }
            else
#endif
            {
                instance = Instantiate(Template, Vector3.zero, Quaternion.identity, transform);
            }
            
            m_objects.Add(instance);
        }
        
        public static PoolObject Rent(PoolObject template)
        {
            int instanceId = template.GetInstanceID();
            if (s_map.ContainsKey(instanceId))
            {
                PrefabPool pool = s_map[instanceId];
                if (pool.m_objects.Count == 0)
                {
                    pool.AddNew();
                }

                var count = pool.m_objects.Count;
                PoolObject instance = pool.m_objects[count - 1];
                pool.m_objects.RemoveAt(count - 1);
                instance.gameObject.SetActive(true);
                return instance;
            }

            return null;
        }
        
        public static void Return(PoolObject instance)
        {
            int instanceId = instance.Template.GetInstanceID();
            if (s_map.ContainsKey(instanceId))
            {
                PrefabPool pool = s_map[instanceId];
                instance.gameObject.SetActive(false);
                pool.m_objects.Add(instance);
            }
        }

        private void OnDestroy()
        {
            UnregisterPool(Template);
        }
    }

}