using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Diagnostics;
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
        ArrayList backTrack = new ArrayList();

        WebClient client = new WebClient();
        JavaScriptSerializer serializer = new JavaScriptSerializer();

        string selectedAction;
        dynamic sitesArray;
        dynamic browseContent;
        String log;
        String id;
        Boolean goToHome = false;
        Boolean browsingHome = true;

        public SelectSites (string action)
        {
            InitializeComponent();
            this.selectedAction = action;

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            client.UseDefaultCredentials = true;
        }

        private void SelectSites_Load(object sender, EventArgs e)
        {
            log = "";
            String sitesJSON = client.DownloadString("https://documents.i.opw.ie/share/proxy/alfresco/api/people/chiribest/sites/");
            sitesArray = serializer.DeserializeObject(sitesJSON);
            listBox1.Items.Add("Home");
            listBox1.Items.Add("Shared");
            log += "Load_JSON: " + sitesJSON + "\n";

            foreach (dynamic d in sitesArray)
            {
                listBox1.Items.Add(d["title"]);
                log += "Load_dynamic: " + d["title"] + "\n";
            }

            Debug.WriteLine(log);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            log = "";

            if (!button1.Text.Equals("Back"))
            {
                if (listBox1.GetItemText(listBox1.SelectedItem).Equals("Home"))
                {
                    browseContent = serializer.DeserializeObject(client.DownloadString("https://documents.i.opw.ie/alfresco/api/-default-/public/alfresco/versions/1/nodes/-my-/children"));
                }
                else if (listBox1.GetItemText(listBox1.SelectedItem).Equals("Shared"))
                {
                    browseContent = serializer.DeserializeObject(client.DownloadString("https://documents.i.opw.ie/alfresco/api/-default-/public/alfresco/versions/1/nodes/-shared-/children"));
                }
                else
                {
                    if (!browsingHome)
                    {
                        foreach (dynamic d in browseContent["list"]["entries"])
                        {
                            if (d["entry"]["name"].Equals(listBox1.GetItemText(listBox1.SelectedItem)))
                            {
                                backTrack.Add(d["entry"]["parentId"]);
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
            }            
            else if (backTrack.Count > 0)
            {
                browseContent = serializer.DeserializeObject(client.DownloadString(String.Format("https://documents.i.opw.ie/alfresco/api/-default-/public/alfresco/versions/1/nodes/{0}/children", backTrack[backTrack.Count - 1])));
                backTrack.RemoveAt(backTrack.Count - 1);
            }
            else
            {
                goToHome = true;
            }

            listBox1.Items.Clear();

            if (!goToHome && !button1.Text.Equals("Select file"))
            {
                foreach (dynamic d in browseContent["list"]["entries"])
                {
                    listBox1.Items.Add(d["entry"]["name"]);
                }

                browsingHome = false;
            }
            else if (button1.Text.Equals("Select file"))
            {
                Process.Start("https://documents.i.opw.ie/alfresco/api/-default-/public/alfresco/versions/1/nodes/" + id + "/content");
            }
            else
            {
                listBox1.Items.Clear();
                listBox1.Items.Add("Home");
                listBox1.Items.Add("Shared");

                foreach (dynamic d in sitesArray)
                {
                    listBox1.Items.Add(d["title"]);
                }

                goToHome = false;
                browsingHome = true;
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

            button1.Text = "Back";
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
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
                if (!browsingHome)
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
                else
                {
                    button1.Text = "Open Site";
                }
            }
        }
    }
}
