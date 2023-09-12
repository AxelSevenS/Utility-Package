#if USE_LOCALIZATION

using System;
using System.Reflection;

using UnityEngine;

using UnityEngine.Localization;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace SevenGame.Utility {

    /// <summary>
    /// A wrapper for LocalizedString that allows for compatibility with non-localized strings.
    /// </summary>
    [Serializable]
    public struct LocalizedStringCompat {

        [SerializeField] private bool _useLocalization;
        [SerializeField] private string _value;
        [SerializeField] private LocalizedString _localizedValue;

        public LocalizedStringCompat(string value) {
            _value = value;
            _localizedValue = null;
            _useLocalization = false;
        }

        public LocalizedStringCompat(LocalizedString value) {
            _value = null;
            _localizedValue = value;
            _useLocalization = true;
        }

        public readonly void GetValue(Action<string> callback) {
            if (_useLocalization) {
                AsyncOperationHandle<string> handle = _localizedValue.GetLocalizedStringAsync();
                handle.Completed += (AsyncOperationHandle<string> result) => {
                    if (result.Status == AsyncOperationStatus.Succeeded) {
                        callback?.Invoke(result.Result);
                    } else {
                        callback?.Invoke("Error");
                    }
                };
            } else {
                callback?.Invoke(_value);
            }
        }
    }

}

#endif