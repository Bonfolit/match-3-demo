#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace BonLib.Pooling
{

    [CustomEditor(typeof(PrefabPool))]
    public class PrefabPoolEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            PrefabPool pool = (PrefabPool)target;

            if (GUILayout.Button("Populate"))
            {
                pool.Populate();
            }
        }
    }

}
#endif