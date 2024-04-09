using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
//using System.Data.OracleClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using DAL;
using Oracle.DataAccess.Client;

namespace Nesher.UpdateXml
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

        }

        private OracleConnection conn;
        private List<XmlStorage> allXmls;
        private string lid;
        private string cid;

        private void button2_Click(object sender, EventArgs e)
        {

            try
            {
                var xmlDal = new MockDataLayer();
                xmlDal.Connect();

                allXmls = xmlDal.GetAllXmlStorage();
                
                {
                    string sql =
                        "select * from xml_storage where TABLE_NAME='CLIENT_SAMPLE_DETAILS' and Length (xml_data) > 0";// " and lab_id=1 and entity_id =3884";
                    // "SELECT * FROM TableWithLobs";
                 //   string strconnONE1 = "DATA SOURCE=tst1;PASSWORD=lims;USER ID=LIMS";
                    string strconn = "DATA SOURCE=TEST;PASSWORD=lims;USER ID=LIMS";
                    using (conn = new OracleConnection(strconn))
                    {
                        conn.Open();
                        using (OracleCommand cmd = new OracleCommand(sql, conn))
                        {
                            using (IDataReader dataReader = cmd.ExecuteReader())
                            {
                                while (dataReader.Read())
                                {
                                    lid = dataReader["LAB_ID"].ToString();
                                    cid = dataReader["Entity_ID"].ToString();
                                    byte[] byteArray = (Byte[])dataReader["xml_data"];

                                    String pathONE1 = @"C:\Xml_documents\workstation_322\" + cid + "_" + lid + ".xml";
                                    String path = @"C:\Xml_documents\NESHER_BACKUP\" + cid + "_" + lid + ".xml";
                                    if (File.Exists(path))
                                    {
                                        continue;
                                    }

                                    using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write))
                                    {
                                        fs.Write(byteArray, 0, byteArray.Length);
                                    }
                                    XDocument xDoc = XDocument.Load(path);

                                    for (int i = 6; i < 11; i++)
                                    {

                                        //לבדוק אם קיים כבר האלמנט הזה

                                        var colTag = xDoc.Descendants().Where(x => x.Name == "Columns").FirstOrDefault();
                                        colTag.Add(new XElement("Telerik.WinControls.UI.GridViewTextBoxColumn",
                                                                new XAttribute("Width", "105"),
                                                                new XAttribute("MinWidth", "105"),
                                                                new XAttribute("Name", "FieldText" + i),
                                                                new XAttribute("IsVisible", "False"),
                                                                new XAttribute("HeaderText", "Text " + i)
                                                       ));
                                  //      string example =
                                     //       "<Telerik.WinControls.UI.GridViewTextBoxColumn Width='105' MinWidth='105' Name='FieldText4' IsVisible='False' HeaderText='Text 4' />";
                                        //  colTag.Add(s);



                                    }
                                    
                                    var dpONE1 = @"C:\Xml_documents\workstation_322\" + cid + "_" + lid + "_DONE_.xml";
                                    var dp = @"C:\Xml_documents\NESHER_BACKUP\" + cid + "_" + lid + "_DONE_.xml";

                                    xDoc.Save(dp);
                                        SaveToDb(dp, cid, lid);


                                }
                            }
                        }








                    }
                    xmlDal.SaveChanges();
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message + " " + cid, lid);
            }

            MessageBox.Show("END");
        }

        private void SaveToDb(string dp, string cid, string lid)
        {

            var client = decimal.Parse(cid);
            var lab = decimal.Parse(lid);

            var c = from item in allXmls
                    where item.TableName == "CLIENT_SAMPLE_DETAILS"
                          & item.EntityId == client
                          & item.LAB_ID == lab
                    select item;

            var current = c.FirstOrDefault();
            if (current != null)
            {
                MemoryStream ms = new MemoryStream();
                XDocument xDoc = XDocument.Load(dp);
                xDoc.Save(ms);
                byte[] blobValue = ms.ToArray();
                current.XmlData = blobValue;
            }
           

        }

        private void button1_Click(object sender, EventArgs e)
        {
            //int labid = 1;
            ////שומר את העיצוב והעמודות עבור הלקוח       
            //var xmlDal = new MockDataLayer();
            //xmlDal.Connect();





            //MemoryStream ms = new MemoryStream();
            ////  xmlDal.GetXmlStorage();


            //XDocument xDoc = XDocument.Load("C:\\popay.xml");
            //xDoc.Save(ms);
            //byte[] bt = ms.ToArray();
            //var clientId = 3884;
            //// var newLayoutXml = UiHelperMethods.ConvertGridToByteArrray(gridSamples);
            //var xs = xmlDal.GetXmlStorage("CLIENT_SAMPLE_DETAILS", clientId, labid);
            //if (xs != null)
            //{
            //    xs.XmlData = bt;
            //}
            //xmlDal.SaveChanges();
            //xmlDal.Close();

            //string strconn = "DATA SOURCE=tst1;PASSWORD=lims;USER ID=LIMS";

        }
    }

}



