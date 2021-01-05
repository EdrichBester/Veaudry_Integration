using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veaudry_Integration
{
    public class WaybillObject
    {
        public string WaybillNo { get; set; }
        public string Account { get; set; }
        public string Service { get; set; }
        public int WaybillType { get; set; }
        public int Packets { get; set; }
        public string Department { get; set; }
        public double Insurance { get; set; }
        public string Transporter { get; set; }
        public SenderAdress Sender { get; set; }
        public ConsigneeAdress Consignee { get; set; }
        public List<Parcel> Parcels { get; set; }
        public List<Reference> References { get; set; }
        public List<Note> Notes { get; set; }
        public List<SpecialInstruction> SpecialInstructions { get; set; }
        public List<Contact> Contacts { get; set; }
        private string pvt_UniqueReference = "";
        public string UniqueReference { get { return pvt_UniqueReference; } set { pvt_UniqueReference = (value == null) ? value : ""; } }
    }
    public class SenderAdress
    {
        public string Consignor { get; set; }
        public string StreetNo { get; set; }
        public string StreetName { get; set; }
        public string Complex { get; set; }
        public string UnitNo { get; set; }
        public string Suburb { get; set; }
        public string Town { get; set; }
        public string PostCode { get; set; }
        public string StoreCode { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
    }
    public class ConsigneeAdress
    {
        public string Consignee { get; set; }
        public string StreetNo { get; set; }
        public string StreetName { get; set; }
        public string Complex { get; set; }
        public string UnitNo { get; set; }
        public string Suburb { get; set; }
        public string Town { get; set; }
        public string PostCode { get; set; }
        public string StoreCode { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
    }
    public class Parcel
    {
        public string Barcode { get; set; }

        private double pvt_Length;
        public double Length { get { return pvt_Length; } set { if (value <= 0) { pvt_Length = 1.00; } else { pvt_Length = value; } } }

        private double pvt_Width;
        public double Width { get { return pvt_Width; } set { if (value <= 0) { pvt_Width = 1.00; } else { pvt_Width = value; } } }

        private double pvt_Height;
        public double Height { get { return pvt_Height; } set { if (value <= 0) { pvt_Height = 1.00; } else { pvt_Height = value; } } }
        private double pvt_Weight;
        public double Weight { get { return pvt_Weight; } set { if (value <= 0) { pvt_Weight = 1.00; } else { pvt_Weight = value; } } }
    }
    public class Reference
    {
        private string pvt_ReferenceNo = "";
        public string ReferenceNo { get { return pvt_ReferenceNo; } set { pvt_ReferenceNo = value; } }
        private string pvt_ReferenceType = "";
        public string ReferenceType { get { return pvt_ReferenceType; } set { pvt_ReferenceType = value; } }
        private int pvt_Pages = 0;
        public int Pages { get { return pvt_Pages; } set { pvt_Pages = value; } }
        private string pvt_Document = "";
        public string Document { get { return pvt_Document; } set { pvt_Document = value; } }
    }
    public class Note
    {
        public string Message { get; set; }
    }
    public class SpecialInstruction
    {
        public string Instruction { get; set; }
        public string Code { get; set; }
    }
    public class Contact
    {
        public string Firstname { get; set; }
        public string Surname { get; set; }
        public string Telephone { get; set; }
        public string Mobile { get; set; }
        public string Mail { get; set; }
        public string NationalID { get; set; }
    }
}
