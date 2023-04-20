using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using VerkkokauppaWeb.Models;

namespace VerkkokauppaWeb.ViewModels
{
    public class TuotteetAndLoginsViewModel
    {
        public IEnumerable<Tuotteet> Tuotteet { get; set; }
        public Logins Logins { get; set; }

        public int LoginID { get; set; }
        public int AsiakasID { get; set; }
        public string Kayttajatunnus { get; set; }
        public string Salasana { get; set; }

        public string LoginErrorMessage { get; internal set; }

        public virtual Asiakkaat Asiakkaat { get; set; }

        public int TuoteID { get; set; }
        public string TuoteNimi { get; set; }
        public decimal Hinta { get; set; }
        public string Kuvaus { get; set; }
    }
}