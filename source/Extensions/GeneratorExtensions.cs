//	* ********************************************************************
//	*  © 2020 RazorSoft Media, DBA                                       *
//	*         Lone Star Logistics & Transport, LLC. All Rights Reserved  *
//	*         David Boarman                                              *
//	* ********************************************************************


using System;


namespace RazorSoft.Core.Extensions {

    public enum LetterCase {
        Upper,
        Lower,
        Mixed
    }


    public static class GeneratorExtensions {
        /// <summary>
        /// Generates a random string of letters.
        /// </summary>
        /// <param name="self">The random number generator being used to generate the string.</param>
        /// <param name="length">The max length of the string.</param>
        /// <param name="cas">The alphabetical case of the letters being generated.</param>
        public static string GenerateLetterString(this Random self, int length, LetterCase cas = LetterCase.Mixed) {
            var Letter = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var value = string.Empty;

            for (int i = 0; i < length; ++i) {
                var c = 'a';
                switch (cas) {
                    case LetterCase.Lower: c = Letter[self.Next(0, 25)]; break;
                    case LetterCase.Upper: c = Letter[self.Next(26, 51)]; break;
                    default: c = Letter[self.Next(0, 51)]; break;
                }

                value += c;
            }

            return value;
        }
        /// <summary>
        /// Generates a random string of numbers.
        /// </summary>
        /// <param name="self">The random number generator being used to generate the string.</param>
        /// <param name="length">The max length of the string.</param>
        /// <example></example>
        /// <returns></returns>
        public static string GenerateNumberString(this Random self, int length) {
            var Number = "0123456789";
            var value = string.Empty;

            for (int i = 0; i < length; ++i) {
                value += Number[self.Next(0, 9)];
            }

            return value;
        }
        /// <summary>
        /// Generates a random string of letters and numbers.
        /// </summary>
        /// <param name="self">The random number generator being used to generate the string.</param>
        /// <param name="length">The max length of the string.</param>
        /// <param name="cas">The alphabetical case of the letters being generated.</param>
        public static string GenerateAlphaNumericString(this Random self, int length, LetterCase cas = LetterCase.Mixed) {
            var Letter = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var Number = "0123456789";
            var numlet = false;
            var value = string.Empty;

            for (int i = 0; i < length; ++i) {
                numlet = self.Next(0, 10) > 5 ? true : false;
                var c = 'a';

                if (numlet) {
                    switch (cas) {
                        case LetterCase.Lower: c = Letter[self.Next(0, 25)]; break;
                        case LetterCase.Upper: c = Letter[self.Next(26, 51)]; break;
                        default: c = Letter[self.Next(0, 51)]; break;
                    }

                }
                else {
                    c = Number[self.Next(0, 9)];
                }

                value += c;
            }

            return value;
        }
    }
}
