using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace Veaudry_Integration
{
    public partial class Form1 : Form
    {
        private string UrlEndPoint;
        private string LablePrinter;
        private string ApplicationTitle;
        WaybillObject OrderObject;

        public Form1()
        {
            InitializeComponent();
            try
            {
                UrlEndPoint = ConfigurationManager.AppSettings["URLEndPoint"].ToString().Trim();
                LablePrinter = ConfigurationManager.AppSettings["LocalPrinter"].ToString().Trim();
                ApplicationTitle = ConfigurationManager.AppSettings["ApplicationTitle"].ToString().Trim();
                this.Text = ApplicationTitle;
            }
            catch (ConfigurationException confex)
            {
                MessageBox.Show("Configuration Settings Can Not Be Loaded", "Order Details Collector", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                btnOrderNumber.Enabled = false;
                txtOrderNumber.Enabled = false;
            }
        }
        private string CheckInvalidCharacters(string mv_InputValue)
        {
            Regex r = new Regex("(?:[^a-z0-9 ]|(?<=['\"])s)", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);
            return r.Replace(mv_InputValue, String.Empty);
        }
        private void GetOrderDetails(string mv_OrderNumber)
        {
            try
            {
                if (OrderObject != null)
                {
                    OrderObject = null;
                }
                string check = mv_OrderNumber.Substring(0, 3).ToUpper();
                string mv_OrderNumberVEA = mv_OrderNumber.Remove(0, 3);

                if (check == "VEA")
                {
                    HttpClient clientVEA = new HttpClient();
                    clientVEA.BaseAddress = new Uri(string.Format("https://everythinghair.co.za"));
                    clientVEA.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "71kwwfg1sqpul8gouikh13vv30ttn740");
                    clientVEA.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    HttpRequestMessage requestVEA = new HttpRequestMessage(HttpMethod.Get, string.Format("/rest/V1/orders?searchCriteria[filter_groups][0][filters][0][field]=increment_id&searchCriteria[filter_groups][0][filters][0][value]=" + mv_OrderNumberVEA));
                    HttpResponseMessage responseVEA = clientVEA.SendAsync(requestVEA).Result;
                    if (responseVEA.ReasonPhrase == System.Net.HttpStatusCode.OK.ToString())
                    {
                        string jsonVEA = responseVEA.Content.ReadAsStringAsync().Result;
                        VeaudryWaybillDetails OrderObjectVEA = JsonConvert.DeserializeObject<VeaudryWaybillDetails>(jsonVEA);

                        //WaybillObject OrderObject = new WaybillObject();
                        ConsigneeAdress nca = new ConsigneeAdress();
                        WaybillObject nwo = new WaybillObject();
                        SenderAdress nsa = new SenderAdress();
                        Reference nro = new Reference();
                        Contact nco = new Contact();
                        try
                        {
                            txtConsignee.Text = string.Concat(OrderObjectVEA.items[0].extension_attributes.shipping_assignments[0].shipping.address.firstname).Trim().ToUpper() + " " + string.Concat(OrderObjectVEA.items[0].extension_attributes.shipping_assignments[0].shipping.address.lastname).Trim().ToUpper();

                            try
                            {
                                nca.StreetName = string.Concat(OrderObjectVEA.items[0].extension_attributes.shipping_assignments[0].shipping.address.street[0]).Trim().ToUpper() + " " + string.Concat(OrderObjectVEA.items[0].extension_attributes.shipping_assignments[0].shipping.address.company).Trim().ToUpper();
                                txtAddress.Text = string.Concat(OrderObjectVEA.items[0].extension_attributes.shipping_assignments[0].shipping.address.street[0]).Trim().ToUpper() + " " + string.Concat(OrderObjectVEA.items[0].extension_attributes.shipping_assignments[0].shipping.address.company).Trim().ToUpper();
                            }
                            catch
                            {
                                nca.StreetName = OrderObjectVEA.items[0].extension_attributes.shipping_assignments[0].shipping.address.street[0].Trim().ToUpper();
                                txtAddress.Text = OrderObjectVEA.items[0].extension_attributes.shipping_assignments[0].shipping.address.street[0].Trim().ToUpper();
                            }

                            try
                            {
                                txtSuburb.Text = OrderObjectVEA.items[0].extension_attributes.shipping_assignments[0].shipping.address.street[1].Trim().ToUpper();
                                nca.Suburb = OrderObjectVEA.items[0].extension_attributes.shipping_assignments[0].shipping.address.street[1].Trim().ToUpper();
                            }
                            catch
                            {
                                txtSuburb.Text = OrderObjectVEA.items[0].extension_attributes.shipping_assignments[0].shipping.address.city.Trim().ToUpper(); ;
                                nca.Suburb = OrderObjectVEA.items[0].extension_attributes.shipping_assignments[0].shipping.address.city.Trim().ToUpper(); ;
                            }

                            txtTown.Text = OrderObjectVEA.items[0].extension_attributes.shipping_assignments[0].shipping.address.city.Trim().ToUpper();
                            txtPostCode.Text = OrderObjectVEA.items[0].extension_attributes.shipping_assignments[0].shipping.address.postcode.Trim().ToUpper();

                            comboBox1.Items.Clear();
                            comboBox1.SelectedItem = null;
                            comboBox1.Text = "ECO";
                            comboBox1.Items.Add("ONX");
                            comboBox1.Items.Add("ROEX");

                            cmbxBoxSize.Items.Clear();
                            cmbxBoxSize.SelectedItem = null;
                            cmbxBoxSize.Text = "";
                            cmbxBoxSize.Items.Add("S = 210 x 210 x 70");
                            cmbxBoxSize.Items.Add("M = 290 x 210 x 70");
                            cmbxBoxSize.Items.Add("L = 440 x 150 x 150");

                            nwo.WaybillNo = CheckInvalidCharacters(string.Concat(mv_OrderNumber.ToString().Trim().ToUpper()));
                            nwo.Account = "VEA001";
                            nwo.Service = comboBox1.Text;
                            nwo.WaybillType = 2;
                            nwo.Department = "";
                            nwo.Insurance = 0.0;
                            nwo.Transporter = "FNF";

                            nsa.Consignor = "THE GANTRY";
                            nsa.StreetNo = "";
                            nsa.StreetName = "CNR WITKOPPEN AND THE STRAIGHT AVE";
                            nsa.Complex = "";
                            nsa.UnitNo = "";
                            nsa.Suburb = "FOURWAYS";
                            nsa.Town = "JOHANNESBURG";
                            nsa.PostCode = "2062";
                            nsa.StoreCode = "";
                            nsa.Latitude = "0.000000";
                            nsa.Longitude = "0.000000";
                            nwo.Sender = nsa;

                            nca.Consignee = CheckInvalidCharacters(string.Concat(OrderObjectVEA.items[0].extension_attributes.shipping_assignments[0].shipping.address.firstname).Trim().ToUpper() + " " + string.Concat(OrderObjectVEA.items[0].extension_attributes.shipping_assignments[0].shipping.address.lastname).Trim().ToUpper());
                            nca.StreetNo = "";
                            nca.StreetName = CheckInvalidCharacters(string.Concat(OrderObjectVEA.items[0].extension_attributes.shipping_assignments[0].shipping.address.street[0]).Trim().ToUpper() + " " + string.Concat(OrderObjectVEA.items[0].extension_attributes.shipping_assignments[0].shipping.address.company).Trim().ToUpper());
                            nca.Complex = "";
                            nca.UnitNo = "";
                            nca.Town = CheckInvalidCharacters(OrderObjectVEA.items[0].extension_attributes.shipping_assignments[0].shipping.address.city.Trim().ToUpper());
                            nca.PostCode = CheckInvalidCharacters(OrderObjectVEA.items[0].extension_attributes.shipping_assignments[0].shipping.address.postcode.Trim().ToUpper());
                            nca.StoreCode = "";
                            nca.Latitude = "0.000000";
                            nca.Longitude = "0.000000";
                            nwo.Consignee = nca;

                            List<Reference> nrl = new List<Reference>();
                            nro.ReferenceNo = CheckInvalidCharacters(mv_OrderNumber);
                            nro.ReferenceType = "CUSTOMER WAYBILL NO";
                            nro.Pages = 0;
                            nro.Document = "";
                            nrl.Add(nro);
                            nwo.References = nrl;

                            List<Contact> ncl = new List<Contact>();
                            nco.Firstname = CheckInvalidCharacters(OrderObjectVEA.items[0].extension_attributes.shipping_assignments[0].shipping.address.firstname.Trim().ToUpper());
                            nco.Surname = CheckInvalidCharacters(OrderObjectVEA.items[0].extension_attributes.shipping_assignments[0].shipping.address.lastname.Trim().ToUpper());
                            nco.Telephone = CheckInvalidCharacters(OrderObjectVEA.items[0].extension_attributes.shipping_assignments[0].shipping.address.telephone.Trim().ToUpper());
                            nco.Mobile = CheckInvalidCharacters(OrderObjectVEA.items[0].extension_attributes.shipping_assignments[0].shipping.address.telephone.Trim().ToUpper());
                            nco.Mail = CheckInvalidCharacters(OrderObjectVEA.items[0].extension_attributes.shipping_assignments[0].shipping.address.email.Trim().ToUpper());
                            nco.NationalID = "";
                            ncl.Add(nco);
                            nwo.Contacts = ncl;

                            OrderObject = nwo;

                            string asdf = OrderObject.ToString();

                        }
                        catch (Exception getex)
                        {
                            MessageBox.Show("There was an issue collecting data for your requested Order Number", "Order Details Collector", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            ClearInputData();
                            return;
                        }
                    }
                    else
                    {
                        MessageBox.Show("There was an issue collecting data for your requested Order Number", "Order Details Collector", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        ClearInputData();
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("Incorrect prefix used!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("There was an issue collecting data for your requested Order Number", "Order Details Collector", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                ClearInputData();
                return;
            }
        }

        private void ClearInputData()
        {
            foreach (Control x in this.Controls)
            {
                if (x is TextBox)
                {
                    ((TextBox)x).Text = String.Empty;
                }
            }

            foreach (Control x in groupBox1.Controls)
            {
                if (x is TextBox)
                {
                    ((TextBox)x).Text = String.Empty;
                }
            }
            listView1.Items.Clear();
            TotalParcels = 0;
            txtOrderNumber.Focus();
            OrderObject = null;
        }

        private void btnOrderNumber_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtOrderNumber.Text.Trim().Length != 13)
                {
                    MessageBox.Show("Please enter a valid Order Number!", "Order Details Collector", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    ClearInputData();
                    return;
                }
                if (txtOrderNumber.Text.Trim() != string.Empty)
                {
                    SqlConnection sqlCon = new SqlConnection();
                    sqlCon.ConnectionString =
                    "Data Source=greenmagic_data.fnf.co.za;" +
                    "Initial Catalog=LogisticsSuite;" +
                    "User id=svc_fnfhqsv036;" +
                    "Password=fnf.001$;";
                    sqlCon.Open();
                    string result = string.Empty;
                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = sqlCon;
                        command.CommandType = CommandType.Text;
                        command.CommandText = "select inbound_waybillno from dbo.INBOUND_WAYBILL_DATA where inbound_waybillno = " + "'" + txtOrderNumber.Text.Trim() + "'";

                        SqlDataReader dr = command.ExecuteReader();
                        while (dr.Read())
                        {
                            result = dr["inbound_waybillno"].ToString();


                        }

                    }
                    //if (result != "")
                    //{
                    //    MessageBox.Show("This order has already been processed!", "Order Details Collector", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    //    ClearInputData();
                    //    return;
                    //}
                    //else
                    //{
                        GetOrderDetails(txtOrderNumber.Text.Trim());
                    //}
                }
                else
                {
                    MessageBox.Show("Order Number Must Be Supllied!", "Order Details Collector", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            catch(Exception e2)
            {
                MessageBox.Show(e2.ToString());
            }
        }

        private void txtOrderNumber_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (e.KeyChar == 13)
                {
                    if (txtOrderNumber.Text.Trim() != string.Empty)
                    {
                        if (txtOrderNumber.Text.Trim().Length != 13)
                        {
                            MessageBox.Show("Please enter a valid Order Number!", "Order Details Collector", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            ClearInputData();
                            return;
                        }
                        if (txtOrderNumber.Text.Trim() != string.Empty)
                        {
                            SqlConnection sqlCon = new SqlConnection();
                            sqlCon.ConnectionString =
                            "Data Source=greenmagic_data.fnf.co.za;" +
                            "Initial Catalog=LogisticsSuite;" +
                            "User id=svc_fnfhqsv036;" +
                            "Password=fnf.001$;";
                            sqlCon.Open();
                            string result = string.Empty;
                            using (SqlCommand command = new SqlCommand())
                            {
                                command.Connection = sqlCon;
                                command.CommandType = CommandType.Text;
                                command.CommandText = "select inbound_waybillno from dbo.INBOUND_WAYBILL_DATA where inbound_waybillno = " + "'" + txtOrderNumber.Text.Trim() + "'";

                                SqlDataReader dr = command.ExecuteReader();
                                while (dr.Read())
                                {
                                    result = dr["inbound_waybillno"].ToString();


                                }

                            }
                            if (result != "")
                            {
                                MessageBox.Show("This order has already been processed!", "Order Details Collector", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                ClearInputData();
                                return;
                            }
                            else
                            {
                                GetOrderDetails(txtOrderNumber.Text.Trim());
                            }
                        }
                        else
                        {
                            MessageBox.Show("Correct Order Number Must Be Supllied!", "Order Details Collector", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Order Number Must Be Supllied!", "Order Details Collector", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }
            }
            catch(Exception e1)
            {
                MessageBox.Show(e1.ToString());
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ClearInputData();
        }

        private void txtParcels_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void txtWeight_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar)
            && !char.IsDigit(e.KeyChar)
            && e.KeyChar != '.')
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if (e.KeyChar == '.'
                && (sender as TextBox).Text.IndexOf('.') > -1)
            {
                e.Handled = true;
            }
        }

        private void btnProcessWaybill_Click(object sender, EventArgs e)
        {
            if (!ValidateInputData())
            {
                MessageBox.Show("All Order Details Must Be Supllied!", "Order Details Collector", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            int ParcelCount = Convert.ToInt32(txtParcels.Text);
            double ParcelWeight = Convert.ToDouble(txtWeight.Text, System.Globalization.CultureInfo.InvariantCulture) / ParcelCount;
            List<Parcel> Parcels = new List<Parcel>();
            foreach (ListViewItem item in listView1.Items)
            {
                int BoxSizeParcels = Convert.ToInt32(item.SubItems[0].Text);
                string[] BoxSizes = item.SubItems[1].Text.ToUpper().Split('=');
                string[] BoxDims = BoxSizes[1].Split('X');
                for (int i = 0; i < BoxSizeParcels; i++)
                {
                    Parcels.Add(new Parcel()
                    {
                        Length = Convert.ToDouble(BoxDims[0]) / 10,
                        Width = Convert.ToDouble(BoxDims[1]) / 10,
                        Height = Convert.ToDouble(BoxDims[2]) / 10,
                        Weight = ParcelWeight,
                        Barcode = ""
                    });
                }
            }

            for (int i = 0; i < Parcels.Count; i++)
            {
                Parcels.ElementAt(i).Barcode = string.Concat(OrderObject.WaybillNo, GetParcelNumber(i + 1));
            }

            //for (int i = 0; i < ParcelCount; i++)
            //{
            //    Parcels.Add(new Parcel()
            //    {
            //        Length = 1.0,
            //        Width = 1.0,
            //        Height = 1.0,
            //        Weight = ParcelWeight,
            //        Barcode = string.Concat(OrderObject.WaybillNo, GetParcelNumber(i + 1))
            //    });
            //}
            OrderObject.Parcels = Parcels;
            OrderObject.Packets = Parcels.Count();
            OrderObject.Service = comboBox1.Text.ToString();

            //Sync the Waybill

            if (!string.IsNullOrEmpty(SyncInhouseWaybill(OrderObject.WaybillNo, JsonConvert.SerializeObject(OrderObject))))
            {
                PrintFNFLabel(OrderObject, LablePrinter);
                MessageBox.Show("Order Details Collector", "Waybill Created!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                ClearInputData();
            }
            else
            {
                MessageBox.Show("Order Details Collector", "Failed to Create Waybill", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        public string GetParcelNumber(int mv_ParcelNumberInput)
        {
            string ParcelNumber = string.Empty;
            ParcelNumber = mv_ParcelNumberInput.ToString().PadLeft(4, '0');
            return ParcelNumber;
        }

        private bool ValidateInputData()
        {
            foreach (Control x in this.Controls)
            {
                if (x is TextBox)
                {
                    if (((TextBox)x).Name != "txtPacketAssign")
                    {
                        if (((TextBox)x).Text == String.Empty)
                        {
                            return false;
                        }
                    }
                }
            }

            foreach (Control x in groupBox1.Controls)
            {
                if (x is TextBox)
                {
                    if (((TextBox)x).Name != "txtPacketAssign")
                    {
                        if (((TextBox)x).Text == String.Empty)
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        public void PrintFNFLabel(WaybillObject mv_InboundWaybill, string mv_PrinterName)
        {

            List<string> PrintStrings = new List<string>();

            int TotalParcels = Convert.ToInt32(mv_InboundWaybill.Packets);

            for (int i = 0; i < TotalParcels; i++)
            {
                PrintStrings.Add("CT~~CD,~CC^~CT~");
                PrintStrings.Add("^XA~TA000~JSN^LT0^MNW^MTD^PON^PMN^LH0,0^JMA^PR4,4~SD15^JUS^LRN^CI0^XZ");
                PrintStrings.Add("^XA");
                PrintStrings.Add("^MMT");
                PrintStrings.Add("^PW730");
                PrintStrings.Add("^LL0790");
                PrintStrings.Add("^LS0");

                string VolumiseParcel = "VI";
                PrintStrings.Add(string.Format(@"^LRY
                                                 ^FO88,48^GB49,35,25^FS
                                                 ^FO90,50^CFD,36,18 ^FR^FD{0}^FS", VolumiseParcel));

                string CompanyName = "FAST + FURIOUS";
                PrintStrings.Add(string.Format("^FO225,50 ^CFD,36,18 ^FR^FD{0}^FS", CompanyName));

                string DateScanned = string.Concat("DATE PRINTED : ", DateTime.Now.ToString("yyy-MM-dd"));
                PrintStrings.Add(string.Format("^FO225,100 ^CFD,22,12 ^FD{0}^FS", DateScanned));

                string DeliverTo = "DELIVER TO : ";
                PrintStrings.Add(string.Format("^FO80,140 ^CFD,30,12 ^FD{0}^FS", DeliverTo));

                string Consignee = mv_InboundWaybill.Consignee.Consignee.Trim();
                PrintStrings.Add(string.Format("^FO240,140 ^CFD,30,12 ^FD{0}^FS", Consignee));

                string Address = string.Concat(mv_InboundWaybill.Consignee.Complex.Trim(), ' ', mv_InboundWaybill.Consignee.UnitNo.Trim(), ' ', mv_InboundWaybill.Consignee.StreetNo.Trim(), ' ', mv_InboundWaybill.Consignee.StreetName.Trim());
                PrintStrings.Add(string.Format("^FO80,180 ^CFD,30,12 ^FD{0}^FS", Address.Trim()));

                string Suburb = mv_InboundWaybill.Consignee.Suburb.Trim();
                PrintStrings.Add(string.Format("^FO80,220 ^CFD,30,12 ^FD{0}^FS", Suburb));

                string Town = mv_InboundWaybill.Consignee.Town.Trim();
                PrintStrings.Add(string.Format("^FO80,260 ^CFD,30,12 ^FD{0}^FS", Town));

                string PostCode = mv_InboundWaybill.Consignee.PostCode.Trim();
                PrintStrings.Add(string.Format("^FO80,300 ^CFD,30,12 ^FD{0}^FS", PostCode));

                string Reference;
                try
                {
                    Reference = string.Concat("REF : ", mv_InboundWaybill.References.ElementAt(0).ReferenceNo.ToString().Trim());
                }
                catch (Exception)
                {
                    Reference = string.Concat("REF : ", "");
                }
                PrintStrings.Add(string.Format("^FO400,260 ^CFD,30,12 ^FD{0}^FS", Reference));

                string Service = string.Concat("SRV TYPE : ", mv_InboundWaybill.Service);
                PrintStrings.Add(string.Format(@"^LRY
                                             ^FO78,338^GB180,35,25^FS
                                             ^FO80,340^FR^CFD,30,12^FD{0}^FS", Service));

                string Route = string.Concat("ROUTE : ", "UNDEFINED");
                PrintStrings.Add(string.Format(@"^LRY
                                             ^FO398,298 ^GB225,35,25^FS
                                             ^FO400,300 ^CFD,30,12 ^FD{0}^FS", Route));

                string Hub = string.Empty;
                if (string.IsNullOrEmpty(""))
                {
                    Hub = string.Concat("DEST HUB : ", "JHB");
                }
                else
                {
                    Hub = string.Concat("DEST HUB : ", "");
                }

                PrintStrings.Add(string.Format(@"^LRY
                                             ^FO398,338 ^GB190,35,25^FS
                                             ^FO400,340 ^CFD,30,12 ^FD{0}^FS", Hub));

                string WaybillNo = mv_InboundWaybill.WaybillNo.Trim();
                PrintStrings.Add(string.Format("^FO80,380 ^CFD,30,12 ^FD{0}^FS", WaybillNo));

                string Mobile;
                try
                {
                    Mobile = string.Concat("PHONE : ", mv_InboundWaybill.Contacts.ElementAt(0).Mobile.Trim());
                }
                catch (Exception)
                {
                    Mobile = string.Concat("PHONE : ", "");
                }
                PrintStrings.Add(string.Format("^FO400,380 ^CFD,30,12 ^FD{0}^FS", Mobile));

                string Pieces = string.Format("PIECES {0} OF {1}", i + 1, TotalParcels);
                PrintStrings.Add(string.Format("^FO80,420 ^CFD,30,12 ^FD{0}^FS", Pieces));

                string Weight = string.Concat("PARCEL WEIGHT : ", Convert.ToDouble(mv_InboundWaybill.Parcels.ElementAt(i).Weight));
                PrintStrings.Add(string.Format("^FO400,420 ^CFD,30,12 ^FD{0}^FS", Weight));

                string Barcode = mv_InboundWaybill.Parcels.ElementAt(i).Barcode.Trim();
                PrintStrings.Add(string.Format("^FO120,460 ^BY2^BCN,100,Y,N,N^FD{0}^FS", Barcode));
                //^ FO100,100 ^BY3^BCN,100,Y,N,N^FD{0}^FS
                PrintStrings.Add("^XZ");

                int mv_PrintByteLength = 0;
                IntPtr PrintBytes = ConvertPrintStringstoByetArra(PrintStrings, ref mv_PrintByteLength);
                RawPrinterHelper.RawPrinterHelper.SendBytesToPrinter(mv_PrinterName, PrintBytes, mv_PrintByteLength);
                PrintStrings.Clear();
            }
        }
        private IntPtr ConvertPrintStringstoByetArra(List<string> mv_PrintStrings, ref int mv_PrintByteLength)
        {
            StringBuilder sb_PrintStrings = new StringBuilder();
            foreach (string item in mv_PrintStrings)
            {
                sb_PrintStrings.AppendLine(item);
            }
            string sbtest = sb_PrintStrings.ToString();

            byte[] PrintStringBytes = Encoding.UTF8.GetBytes(sbtest);
            IntPtr UnmanagePrintBytes = new IntPtr(0);
            mv_PrintByteLength = PrintStringBytes.Length;
            UnmanagePrintBytes = Marshal.AllocCoTaskMem(mv_PrintByteLength);
            Marshal.Copy(PrintStringBytes, 0, UnmanagePrintBytes, mv_PrintByteLength);
            return UnmanagePrintBytes;
        }

        public string SyncInhouseWaybill(string mv_WaybillNo, string mv_WaybillJsonString)
        {
            string ReturnResult = string.Empty;
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(string.Format("{0}", UrlEndPoint));
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, string.Format("/api/InhouseUpload/ImportWaybill/{0}", mv_WaybillNo));
                request.Content = new StringContent(mv_WaybillJsonString, System.Text.Encoding.UTF8, "application/json");
                HttpResponseMessage response = client.SendAsync(request).Result;
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    using (HttpContent content = response.Content)
                    {
                        ReturnResult = content.ReadAsStringAsync().Result;
                    }
                }
            }
            catch (Exception ex)
            {
                //Do Nothing;
            }
            return ReturnResult;
        }

        private int TotalParcels = 0;

        private void txtPacketAssign_KeyPress(object sender, KeyPressEventArgs e)

        {
            if (e.KeyChar == 13)
            {
                if (!string.IsNullOrEmpty(txtPacketAssign.Text))
                {
                    try
                    {
                        string PacketSize = cmbxBoxSize.SelectedItem.ToString();
                        ListViewItem li = new ListViewItem(txtPacketAssign.Text.Trim());
                        li.SubItems.Add(PacketSize);
                        listView1.Items.Add(li);
                        TotalParcels = TotalParcels + Convert.ToInt32(txtPacketAssign.Text);
                        cmbxBoxSize.SelectedItem = null;
                        cmbxBoxSize.Text = "";
                        txtPacketAssign.Clear();
                        txtParcels.Text = TotalParcels.ToString();
                        cmbxBoxSize.Focus();
                    }
                    catch (Exception)
                    {
                        //Do Nothing
                    }
                }
            }
        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            listView1.Items.RemoveAt(listView1.SelectedIndices[0]);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtOrderNumber.Text.Trim().Length != 13)
                {
                    MessageBox.Show("Please enter a valid Order Number!", "Order Details Collector", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    ClearInputData();
                    return;
                }
                if (txtOrderNumber.Text.Trim() != string.Empty)
                {
                    SqlConnection sqlCon = new SqlConnection();
                    sqlCon.ConnectionString =
                    "Data Source=greenmagic_data.fnf.co.za;" +
                    "Initial Catalog=LogisticsSuite;" +
                    "User id=svc_fnfhqsv036;" +
                    "Password=fnf.001$;";
                    sqlCon.Open();
                    string result = string.Empty;
                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = sqlCon;
                        command.CommandType = CommandType.Text;
                        command.CommandText = "select inbound_json from dbo.INBOUND_WAYBILL_DATA where inbound_waybillno = " + "'" + txtOrderNumber.Text.Trim() + "'";

                        SqlDataReader dr = command.ExecuteReader();
                        while (dr.Read())
                        {
                            result = dr["inbound_json"].ToString();
                        }
                    }
                    if (result == "")
                    {
                        MessageBox.Show("This order has not been processed yet!", "Order Details Collector", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        ClearInputData();
                        return;
                    }
                    else
                    {
                        DialogResult r1 = MessageBox.Show("Are you sure you would like reprint this waybill?", "Order Details Collector", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                        if (r1 == DialogResult.Yes)
                        {
                            WaybillObject reprint = JsonConvert.DeserializeObject<WaybillObject>(result);
                            PrintFNFLabel(reprint, LablePrinter);
                            ClearInputData();
                        }
                        else
                        {
                            MessageBox.Show("Waybill will not be printed", "Order Details Collector", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            ClearInputData();
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Order Number Must Be Supllied!", "Order Details Collector", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            catch (Exception e2)
            {
                MessageBox.Show(e2.ToString());
            }
        }
    }
}

