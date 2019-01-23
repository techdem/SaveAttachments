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
        ArrayList shortNames = new ArrayList();
        ArrayList nodeIDs = new ArrayList();

        WebClient client = new WebClient();
        JavaScriptSerializer serializer = new JavaScriptSerializer();

        string selectedAction;
        dynamic sitesArray;
        dynamic browseContent;

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
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            String url;
            String id = "";

            if (listBox1.GetItemText(listBox1.SelectedItem).Equals("Home"))
            {
                url = "https://documents.i.opw.ie/share/page/user/chiribest/dashboard";
                browseContent = serializer.DeserializeObject(client.DownloadString("https://documents.i.opw.ie/alfresco/api/-default-/public/alfresco/versions/1/nodes/-my-/children"));                
            }
            else if (listBox1.GetItemText(listBox1.SelectedItem).Equals("Shared"))
            {
                url = "https://documents.i.opw.ie/share/page/context/shared/sharedfiles";
                browseContent = serializer.DeserializeObject(client.DownloadString("https://documents.i.opw.ie/alfresco/api/-default-/public/alfresco/versions/1/nodes/-shared-/children"));
            }
            else
            {
                //url = "http://documents.i.opw.ie/share/page/site/" + shortNames[listBox1.SelectedIndex - 2];

                if (browseContent != null)
                {
                    foreach (dynamic d in browseContent["list"]["entries"])
                    {
                        if (d["entry"]["name"].Equals(listBox1.GetItemText(listBox1.SelectedItem)))
                        {
                            id = d["entry"]["id"];
                        }
                    }
                }
                else
                {
                    foreach (dynamic d in sitesArray)
                    {
                        if (d["title"].Equals(listBox1.GetItemText(listBox1.SelectedItem)))
                        {
                            id = d["node"].Substring(d["node"].IndexOf("Store/") + 6);
                        }
                    }
                }

                browseContent = serializer.DeserializeObject(client.DownloadString(String.Format("https://documents.i.opw.ie/alfresco/api/-default-/public/alfresco/versions/1/nodes/{0}/children", id)));
            }

            listBox1.Items.Clear();

            foreach (dynamic d in browseContent["list"]["entries"])
            {
                listBox1.Items.Add(d["entry"]["name"]);
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
            if (browseContent != null)
            {
                if (listBox1.GetItemText(listBox1.SelectedItem).Equals("Home"))
                {
                    button1.Text = "Open Home";
                }
                else if (listBox1.GetItemText(listBox1.SelectedItem).Equals("Shared"))
                {
                    button1.Text = "Open Shared";
                }
                else
                {
                    foreach (dynamic d in browseContent["list"]["entries"])
                    {
                        if (d["entry"]["name"].Equals(listBox1.GetItemText(listBox1.SelectedItem)) && d["entry"]["nodeType"].Equals("cm:folder"))
                        {
                            button1.Text = "Open folder";
                        }
                        else if (d["entry"]["name"].Equals(listBox1.GetItemText(listBox1.SelectedItem)) && d["entry"]["nodeType"].Equals("cm:content"))
                        {
                            button1.Text = "Select file";
                        }
                    }
                }
            }
        }
    }
}
