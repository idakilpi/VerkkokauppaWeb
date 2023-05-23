using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using VerkkokauppaWeb.Validation;
using VerkkokauppaWeb.ViewModels;
using VerkkokauppaWeb.Models;

namespace VerkkokauppaWeb.ViewModels
{

    public class Register
    {
        [Key]
        public int AsiakasID { get; set; }

        [Required(ErrorMessage = "Kirjoita etunimi")]
        public string Etunimi { get; set; }

        [Required(ErrorMessage = "Kirjoita sukunimi")]
        public string Sukunimi { get; set; }

        [Required(ErrorMessage = "Kirjoita sähköpostiosoite")]
        [RegularExpression("^[\\w-\\.]+@([\\w-]+\\.)+[\\w-]{2,4}$", ErrorMessage = "Tarkista sähköpostiosoite!")]
        public string Email { get; set; }

        [PasswordValidation]
        [Required(ErrorMessage = "Kirjoita salasana")]
        [DataType(DataType.Password)]
        public string Salasana { get; set; }

        [Display(Name = "Vahvista salasana")]
        [Required(ErrorMessage = "Kirjoita salasana")]
        [DataType(DataType.Password)]
        [System.ComponentModel.DataAnnotations.Compare("Salasana", ErrorMessage = "Salasana ei täsmää!")]
        public string VahvistaSalasana { get; set; }

        public virtual AsiakkaatAndLoginsViewModel Logs { get; set; }

    }
}