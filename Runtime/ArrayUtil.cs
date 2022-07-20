using System.Collections;
using System.Collections.Generic;

namespace SevenGame.Utility {

    public static class ArrayUtil {
        
        public static Array<T> Add(this Array<T> array, T value) {
            Array<T> newArray = new Array<T>(array.Length + 1);
            for (int i = 0; i < array.Length; i++) {
                newArray[i] = array[i];
            }
            newArray[array.Length] = value;
            return newArray;
        }

        public static Array<T> Remove(this Array<T> array, T value) {
            Array<T> newArray = new Array<T>(array.Length - 1);
            for (int i = 0; i < array.Length; i++) {
                if (array[i] != value) {
                    newArray[i] = array[i];
                }
            }
            return newArray;
        }

        public static Array<T> RemoveAtIndex(this Array<T> array, int index) {
            Array<T> newArray = new Array<T>(array.Length - 1);
            for (int i = 0; i < array.Length; i++) {
                if (i == index) continue;
                newArray[i] = array[i];
            }
            return newArray;
        }

        public static Array<T> RemoveRange(this Array<T> array, int startIndex, int count) {
            Array<T> newArray = new Array<T>(array.Length - count);
            for (int i = 0; i < array.Length; i++) {
                if (i < startIndex) {
                    newArray[i] = array[i];
                } else if (i >= startIndex + count) {
                    newArray[i] = array[i];
                }
            }
            return newArray;
        }
    }
}