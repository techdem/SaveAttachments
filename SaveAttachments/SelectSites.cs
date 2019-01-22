using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace SaveAttachments
{
    public partial class SelectSites : Form
    {
        string selectedAction;
        dynamic sitesArray;
        dynamic browseContent;

        ArrayList shortNames = new ArrayList();
        ArrayList nodeIDs = new ArrayList();

        WebClient client = new WebClient();
        JavaScriptSerializer serializer = new JavaScriptSerializer();

        public SelectSites (string action)
        {
            InitializeComponent();
            this.selectedAction = action;
        }

        private void SelectSites_Load(object sender, EventArgs e)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            client.UseDefaultCredentials = true;

            sitesArray = serializer.DeserializeObject(client.DownloadString("https://documents.i.opw.ie/share/proxy/alfresco/api/people/chiribest/sites/"));
            listBox1.Items.Add("Home");
            listBox1.Items.Add("Shared");

            foreach (dynamic d in sitesArray)
            {
                listBox1.Items.Add(d["title"]);
                shortNames.Add(d["shortName"]);
                nodeIDs.Add(d["node"].Substring(d["node"].IndexOf("Store/") + 6));
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            String url;

            if (listBox1.SelectedIndex == 0)
            {
                listBox1.Items.Clear();
                url = "https://documents.i.opw.ie/share/page/user/chiribest/dashboard";
                browseContent = serializer.DeserializeObject(client.DownloadString("https://documents.i.opw.ie/alfresco/api/-default-/public/alfresco/versions/1/nodes/-my-/children"));

                foreach (dynamic d in browseContent["list"]["entries"])
                {
                    listBox1.Items.Add(d["entry"]["name"]);
                }
            }
            else if (listBox1.SelectedIndex == 1)
            {
                url = "https://documents.i.opw.ie/share/page/context/shared/sharedfiles";
            }
            else
            {
                url = "http://documents.i.opw.ie/share/page/site/" + shortNames[listBox1.SelectedIndex - 2];
            }

            //switch (selectedAction)
            //{
            //    case "download":

            //        MessageBox.Show(nodeIDs[listBox1.SelectedIndex - 2].ToString());
            //        this.Close();

            //        break;
                
            //    case "upload":

            //        MessageBox.Show("Uploading to Alfresco");
            //        OpenFileDialog selectFile = new OpenFileDialog();

            //        if (selectFile.ShowDialog() == DialogResult.OK) {
            //            var response = client.UploadFile("https://documents.i.opw.ie/alfresco/api/-default-/public/alfresco/versions/1/nodes/-my-/children", "POST", selectFile.FileName);
            //        }

            //        break;
            //}
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
