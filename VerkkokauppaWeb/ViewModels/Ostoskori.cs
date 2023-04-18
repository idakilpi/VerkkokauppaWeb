using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VerkkokauppaWeb.Models;

namespace VerkkokauppaWeb.ViewModels
{
    public class Ostoskori
    {
        public int TuoteID { get; set; }
        public string TuoteNimi { get; set; }
        public Nullable<int> Määrä { get; set; }
        public decimal Kappalehinta { get; set; }
        public decimal Summa { get; set; }
        public string KuvaPolku { get; set; }
        public byte[] Kuva { get; set; }

        //public Asiakkaat Asiakas { get; set; }
        public int AsiakasID { get; set; }
        public string Etunimi { get; set; }
        public string Sukunimi { get; set; }
        public string Email { get; set; }
        public string ToimitusOsoite { get; set; }
        public string ToimitusPostinumero { get; set; }
    }
}