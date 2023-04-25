using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace VerkkokauppaWeb.ViewModels
{
    public class TilausrivitModel
    {
        public int TilausriviID { get; set; }
        public int TilausID { get; set; }
        public int TuoteID { get; set; }

        public decimal KappaleHinta { get; set; }
        public int Maara { get; set; }

        public string TuoteNimi { get; set; }
    }
}