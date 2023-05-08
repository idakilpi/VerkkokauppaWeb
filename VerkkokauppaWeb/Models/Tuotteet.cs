//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace VerkkokauppaWeb.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Tuotteet
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Tuotteet()
        {
            this.Tilausrivit = new HashSet<Tilausrivit>();
        }

        public int TuoteID { get; set; }
        public int KategoriaID { get; set; }
        public string TuoteNimi { get; set; }
        public decimal Hinta { get; set; }
        public int Varastomaara { get; set; }
        public string Kuvaus { get; set; }
        public string KuvaPolku { get; set; }
        public byte[] Kuva { get; set; }
    
        public virtual Kategoriat Kategoriat { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Tilausrivit> Tilausrivit { get; set; }
    }
}
