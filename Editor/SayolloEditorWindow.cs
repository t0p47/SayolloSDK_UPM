using UnityEditor;
using UnityEngine;

namespace sayollo.SDK {
    public class SayolloEditorWindow: EditorWindow
    {
        [MenuItem("Sayollo/Editor")]
        private static void ShowWindow() {
            GetWindow<SayolloEditorWindow>("Sayollo");
        }

        private void OnGUI()
        {
            GUILayout.Label("Color the selected objects!", EditorStyles.boldLabel);

            if (GUILayout.Button("Create video ad")) {
                SayolloMenuItems.LoadPrefab("Prefabs/VideoAd");
            }

            if (GUILayout.Button("Create purchase view")) {
                SayolloMenuItems.LoadPrefab("Prefabs/PurchaseView");
            }
        }
    }

    
}

