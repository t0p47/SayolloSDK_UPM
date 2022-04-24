using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Sayollo {
    static class SayolloMenuItems
    {
        const int menuPriority = 1;

        [MenuItem("GameObject/Sayollo/VideoAd")]
        static void CreateVideoAd() {
            LoadPrefab("Prefabs/VideoAd");
        }

        [MenuItem("GameObject/Sayollo/PurchaseView")]
        static void CreatePurchaseView() {
            LoadPrefab("Prefabs/PurchaseView");
        }

        public static void LoadPrefab(string prefabPath) {
            GameObject prefab = Resources.Load(prefabPath) as GameObject;
            
            GameObject instantiated = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            Undo.RegisterCreatedObjectUndo(instantiated, $"Create {instantiated.name}");
            //Selection.activeObject = instantiated;
        }
    }
}

