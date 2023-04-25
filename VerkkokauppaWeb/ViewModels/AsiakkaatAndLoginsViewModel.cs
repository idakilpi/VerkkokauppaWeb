using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using VerkkokauppaWeb.Models;

namespace VerkkokauppaWeb.ViewModels
{
    public class AsiakkaatAndLoginsViewModel
    {
        [Key]
        public int AsiakasID { get; set; }
        public string Etunimi { get; set; }
        public string Sukunimi { get; set; }
        public string Email { get; set; }
        public string Kayttajatunnus { get; set; }
        public string Salasana { get; set; }


    }
}