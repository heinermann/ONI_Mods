using System.Collections.Generic;

namespace Heinermann.CritterRename
{
  static class Util
  {
    public static bool IsNullOrWhitespace(string value)
    {
      return string.IsNullOrEmpty(value) || value.Trim().Length == 0;
    }

    // Source: https://www.rosettacode.org/wiki/Roman_numerals/Encode#C.23
    static uint[] nums = { 1000, 900, 500, 400, 100, 90, 50, 40, 10, 9, 5, 4, 1 };
    static string[] rum = { "M", "CM", "D", "CD", "C", "XC", "L", "XL", "X", "IX", "V", "IV", "I" };

    public static string ToRoman(uint number)
    {
      string value = "";
      for (int i = 0; i < nums.Length && number != 0; i++)
      {
        while (number >= nums[i])
        {
          number -= nums[i];
          value += rum[i];
        }
      }
      return value;
    }



    // Source: https://www.rosettacode.org/wiki/Roman_numerals/Decode#C.23
    private static readonly Dictionary<char, int> RomanDictionary = new Dictionary<char, int>
                                                                            {
                                                                                {'I', 1},
                                                                                {'V', 5},
                                                                                {'X', 10},
                                                                                {'L', 50},
                                                                                {'C', 100},
                                                                                {'D', 500},
                                                                                {'M', 1000}
                                                                            };

    public static int FromRoman(string roman)
    {
      /* Make the input string upper-case,
       * because the dictionary doesn't support lower-case characters. */
      roman = roman.ToUpper();

      /* total = the current total value that will be returned.
       * minus = value to subtract from next numeral. */
      int total = 0, minus = 0;

      for (int i = 0; i < roman.Length; i++) // Iterate through characters.
      {
        // Get the value for the current numeral. Takes subtraction into account.
        int thisNumeral = RomanDictionary[roman[i]] - minus;

        /* Checks if this is the last character in the string, or if the current numeral
         * is greater than or equal to the next numeral. If so, we will reset our minus
         * variable and add the current numeral to the total value. Otherwise, we will
         * subtract the current numeral from the next numeral, and continue. */
        if (i >= roman.Length - 1 ||
            thisNumeral + minus >= RomanDictionary[roman[i + 1]])
        {
          total += thisNumeral;
          minus = 0;
        }
        else
        {
          minus = thisNumeral;
        }
      }

      return total; // Return the total.
    }
  }
}
