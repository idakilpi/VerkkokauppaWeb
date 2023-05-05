using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;

namespace VerkkokauppaWeb.Validation
{
    public class PasswordValidation : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value == null)
            {
                return true;
            }

            var password = value.ToString();

            if (password.Length < 6)
            {
                return false;
            }

            if (!password.Any(char.IsUpper))
            {
                return false;
            }

            if (!password.Any(char.IsDigit))
            {
                return false;
            }

            return true;
        }

        public override string FormatErrorMessage(string name)
        {
            return $"{name}n tulee olla vähintään 6 merkkiä pitkä. Sen täytyy sisältää vähintään yksi iso kirjain ja yksi numero.";
        }
    }
}