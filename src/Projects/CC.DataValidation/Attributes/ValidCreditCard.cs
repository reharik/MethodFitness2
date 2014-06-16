using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CC.DataValidation.Attributes
{
    public class ValidCreditCardAttribute:ValidationAttribute
    {
        private IEnumerable<string> exceptions = new[]{"4111111111111111"};

        public override bool IsValid(object fieldValue)
        {
            if (fieldValue == null)
                return true;
            string str1 = fieldValue.ToString();
            string cardNumber = string.Empty;
            foreach (char c in str1)
            {
                if (char.IsNumber(c))
                    cardNumber = cardNumber + c.ToString();
                else if (c != 32 && c != 45)
                    return false;
            }
            if (exceptions.Any(str2 => cardNumber == str2))
            {
                return true;
            }
            return IsValidCardType(cardNumber) && this.IsLuhnValid(cardNumber);
        }

        private bool IsLuhnValid(string cardNumber)
        {
            int length = cardNumber.Length;
            int num1 = 0;
            int num2 = length % 2;
            byte[] bytes = new ASCIIEncoding().GetBytes(cardNumber);
            for (int index = 0; index < length; ++index)
            {
                bytes[index] -= (byte)48;
                if ((index + num2) % 2 == 0)
                    bytes[index] *= (byte)2;
                num1 += (int)bytes[index] > 9 ? (int)bytes[index] - 9 : (int)bytes[index];
            }
            return num1 % 10 == 0;
        }

        private bool IsValidCardType(string cardNumber)
        {
            if (Regex.IsMatch(cardNumber, "^(51|52|53|54|55)"))
                return cardNumber.Length == 16;
            if (Regex.IsMatch(cardNumber, "^(4)"))
                return cardNumber.Length == 16;
            if (Regex.IsMatch(cardNumber, "^(34|37)"))
                return cardNumber.Length == 15;
            return false;
        }

        /// <summary>
        /// Define the known card types
        /// 
        /// </summary>
        [Flags]
        [Serializable]
        public enum CardType
        {
            MasterCard = 1,
            VISA = 2,
            Amex = 4,
            All = Amex | VISA | MasterCard,
        }
    }
}