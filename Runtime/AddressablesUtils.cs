using System;

using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;

namespace SevenGame.Utility {

    public static class AddressablesUtils {
        public static T LoadAsset<T>( string path ) where T : Object {
            AsyncOperationHandle<T> opHandle = Addressables.LoadAssetAsync<T>(path);
            return opHandle.WaitForCompletion();
        }
        
        public static void LoadAssetAsync<T>( string path, Action<T> succeedCallback, Action<string> failCallback ) where T : Object {
            Addressables.LoadAssetAsync<T>(path).Completed += op => {
                if (op.Status == AsyncOperationStatus.Succeeded) {
                    succeedCallback(op.Result);
                } else {
                    failCallback(op.OperationException.Message);
                }
            };
        }
    }
}