using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veaudry_Integration
{

    public class VeaudryWaybillDetails
    {
        public Item[] items { get; set; }
    }
    public class Item
    {
        public Extension_Attributes extension_attributes { get; set; }
    }
    public class Extension_Attributes
    {
        public Shipping_Assignments[] shipping_assignments { get; set; }
    }

    public class Shipping_Assignments
    {
        public Shipping shipping { get; set; }
    }

    public class Shipping
    {
        public Address address { get; set; }
    }

    public class Address
    {
        public string address_type { get; set; }
        public string city { get; set; }
        public string company { get; set; }
        public string country_id { get; set; }
        public int customer_address_id { get; set; }
        public string email { get; set; }
        public int entity_id { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public int parent_id { get; set; }
        public string postcode { get; set; }
        public string region { get; set; }
        public string region_code { get; set; }
        public string[] street { get; set; }
        public string telephone { get; set; }
    }


}
