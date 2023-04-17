using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VerkkokauppaWeb.Models;

namespace VerkkokauppaWeb.ViewModels
{
    public class AsiakkaatAndLoginsViewModel
    {
        public int AsiakasID { get; set; }
        public string Nimi { get; set; }
        public string Katuosoite { get; set; }
        public string Postinumero { get; set; }
        public string Postitoimipaikka { get; set; }
        public string Email { get; set; }
        public string Puhelinnumero { get; set; }
        public string Salasana { get; set; }
        public string Tunnus { get; set; }

    }
}