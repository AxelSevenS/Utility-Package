using System.Text;
using System.Text.RegularExpressions;

namespace SevenGame.Utility {
    public static class StringUtility {

        public static StringBuilder Nicify(this string t){

            StringBuilder result = new("", t.Length);
            const char spaceChar = ' ';
        
            for(int i = 0; i < t.Length; i++){
                if(char.IsUpper(t[i]) == true && i != 0){
                    result.Append(spaceChar);
                }
                result.Append(t[i]);
            }
            return result;
        }

        public static string[] UppercaseSplit(this string t){
            return Regex.Split(t, @"(?<!^)(?=[A-Z])");
        }
    }
}
