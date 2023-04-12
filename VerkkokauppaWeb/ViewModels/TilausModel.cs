using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VerkkokauppaWeb.ViewModels
{
    public class TilausModel
    {
        public int TilausID { get; set; }
        public int AsiakasID { get; set; }
        public System.DateTime TilausPvm { get; set; }
        public System.DateTime LahetysPvm { get; set; }
    }
}