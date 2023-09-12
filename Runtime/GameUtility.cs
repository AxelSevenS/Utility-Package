using System.Reflection;

using UnityEngine;

namespace SevenGame.Utility {

    public static class GameUtility {

        public static T SafeDestroy<T>(this T obj) where T : Object {
            #if UNITY_EDITOR
                if (!Application.isPlaying)
                    Object.DestroyImmediate(obj);
                else
            #endif
                Object.Destroy(obj);
            
            return null;
        }

        public static void SafeDestroy<T>(ref T obj) where T : Object {
            #if UNITY_EDITOR
                if (!Application.isPlaying)
                    Object.DestroyImmediate(obj);
                else
            #endif
                Object.Destroy(obj);
            
            obj = null;
        }
        
        public static void SetLayerRecursively(this GameObject gameObject, int newLayer) {
            gameObject.layer = newLayer;
        
            foreach ( Transform child in gameObject.transform ) {
                child.gameObject.SetLayerRecursively( newLayer );
            }
        }

    }
}
