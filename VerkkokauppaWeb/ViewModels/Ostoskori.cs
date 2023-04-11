using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VerkkokauppaWeb.ViewModels
{
    public class Ostoskori
    {
        public string TuoteID { get; set; }
        public string Nimi { get; set; }
        public decimal Määrä { get; set; }
        public decimal Kappalehinta { get; set; }
        public decimal Summa { get; set; }
         public string KuvaPolku { get; set; }
        public byte[] Kuva { get; set; }
    }
}