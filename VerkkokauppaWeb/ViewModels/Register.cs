using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace VerkkokauppaWeb.ViewModels
{
    public class Register
    {
        [Key]
        public int AsiakasID { get; set; }

        [Required(ErrorMessage = "Kirjoita etunimi.")]
        public string Etunimi { get; set; }

        [Required(ErrorMessage = "Kirjoita sukunimi.")]
        public string Sukunimi { get; set; }

        [Required(ErrorMessage = "Kirjoita sähköpostiosoite.")]
        [RegularExpression("^[\\w-\\.]+@([\\w-]+\\.)+[\\w-]{2,4}$", ErrorMessage = "Tarkista sähköpostiosoite")]

        public string Email { get; set; }

        [Required(ErrorMessage = "Kirjoita salasana.")]
        [DataType(DataType.Password)]
        public string Salasana { get; set; }

        //[Compare("Password", ErrorMessage = "Vahvista salasana")]
        //[DataType(DataType.Password)]
        //public string VahvistaSalasana { get; set; }

        public string Tunnus { get; set; }
    }
}