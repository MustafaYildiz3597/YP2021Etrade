//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Nero2021.Data
{
    using System;
    using System.Collections.Generic;
    
    public partial class MUSTERILER
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public MUSTERILER()
        {
            this.ADRESLER = new HashSet<ADRESLER>();
            this.MPRODUCTS = new HashSet<MPRODUCTS>();
            this.Orders = new HashSet<Orders>();
            this.TEKLIFLER = new HashSet<TEKLIFLER>();
            this.TPRODUCTS = new HashSet<TPRODUCTS>();
            this.YETKILI_KISILER = new HashSet<YETKILI_KISILER>();
            this.TOPLANTILAR = new HashSet<TOPLANTILAR>();
            this.Ticket = new HashSet<Ticket>();
            this.MusteriDocument = new HashSet<MusteriDocument>();
        }
    
        public int ID { get; set; }
        public Nullable<int> FIRMA_TIPI { get; set; }
        public string FIRMA_ADI { get; set; }
        public Nullable<int> FATURA_ADRESI { get; set; }
        public Nullable<int> TESLIMAT_ADRESI { get; set; }
        public string ADRES { get; set; }
        public string POSTA_KODU { get; set; }
        public string VERGI_DAIRESI { get; set; }
        public string VERGI_NUMARASI { get; set; }
        public string MARKA { get; set; }
        public string MAIL_ADRESI { get; set; }
        public string WEB_STESI { get; set; }
        public string GSM { get; set; }
        public string TEL_1 { get; set; }
        public string TEL_2 { get; set; }
        public string FAX { get; set; }
        public string IMAGE { get; set; }
        public string NOT { get; set; }
        public System.DateTime TARIH { get; set; }
        public Nullable<int> ULKE { get; set; }
        public Nullable<int> SEHIR { get; set; }
        public string VADE_ALT_RAKAM { get; set; }
        public Nullable<int> CariVadeAltRakamID { get; set; }
        public string VADE { get; set; }
        public Nullable<int> CariVadeID { get; set; }
        public string ODEME_SEKLI { get; set; }
        public Nullable<int> CariOdemeSekliID { get; set; }
        public string TESLIMAT_SEKLI { get; set; }
        public Nullable<int> CariTeslimatSekliID { get; set; }
        public string NAKLIYE_ODEMESI { get; set; }
        public Nullable<int> CariNakliyeOdemesiID { get; set; }
        public string FIRMNICK { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<System.DateTime> DeletedOn { get; set; }
        public string DeletedBy { get; set; }
        public Nullable<bool> IsB2BDealer { get; set; }
        public Nullable<int> B2BDealerTypeID { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ADRESLER> ADRESLER { get; set; }
        public virtual CariNakliyeOdemesi CariNakliyeOdemesi { get; set; }
        public virtual CariOdemeSekli CariOdemeSekli { get; set; }
        public virtual CariTeslimatSekli CariTeslimatSekli { get; set; }
        public virtual CariVade CariVade { get; set; }
        public virtual CariVadeAltRakam CariVadeAltRakam { get; set; }
        public virtual FIRMA_TIPLERI FIRMA_TIPLERI { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MPRODUCTS> MPRODUCTS { get; set; }
        public virtual ULKELER ULKELER { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Orders> Orders { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TEKLIFLER> TEKLIFLER { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TPRODUCTS> TPRODUCTS { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<YETKILI_KISILER> YETKILI_KISILER { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TOPLANTILAR> TOPLANTILAR { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Ticket> Ticket { get; set; }
        public virtual B2BDealerType B2BDealerType { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MusteriDocument> MusteriDocument { get; set; }
        public virtual SEHIRLER SEHIRLER { get; set; }
    }
}
