using UnityEngine;
using UnityEditor;

namespace BoidSim {
    [CustomEditor(typeof(BoidSettings))]
    [CanEditMultipleObjects]
    public class BoidSettingsEditor : Editor 
    {
        public override void OnInspectorGUI()
        {
            var script = (BoidSettings) target;
            base.OnInspectorGUI ();
            
            if (GUILayout.Button ("Make active")) {
                script.Activate ();
            }
        }
    }
}
