using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Tools.Ribbon;
using Microsoft.Office.Interop.Outlook;
using System.IO;
using System.Windows.Forms;
using System.Net;
using System.Web.Script.Serialization;
using System.Collections;

namespace SaveAttachments
{
    public partial class RibbonMain
    {
        private void RibbonMain_Load(object sender, RibbonUIEventArgs e)
        {

        }

        private void button1_Click(object sender, RibbonControlEventArgs e)
        {
            //string myURL = "http://documents.i.opw.ie";
            //System.Diagnostics.Process.Start(myURL);

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            WebClient client = new WebClient();
            client.UseDefaultCredentials = true;
            string sitesInput = client.DownloadString("https://documents.i.opw.ie/share/proxy/alfresco/api/people/chiribest/sites/");
            var serializer = new JavaScriptSerializer();
            dynamic sitesArray = serializer.DeserializeObject(sitesInput);
            ArrayList userSites = new ArrayList();
            string testSites = "";

            foreach (dynamic s in sitesArray) {
                userSites.Add(s["title"]);
                testSites += "\n"+s["title"];
            }

            MessageBox.Show(testSites, "QAA");
        }

        private void button2_Click(object sender, RibbonControlEventArgs e)
        {
            var context = e.Control.Context as Inspector;

            try
            {
                var application = Globals.ThisAddIn.Application;
                Selection selection = application.ActiveExplorer().Selection;
                var mailItem = selection[1] as MailItem;

                if (mailItem != null)
                {
                    if (mailItem.Attachments.Count > 0)
                    {
                        FolderBrowserDialog pathDialog = new FolderBrowserDialog();

                        if (pathDialog.ShowDialog() == DialogResult.OK)
                        {
                            foreach (Attachment item in mailItem.Attachments)
                            {
                                item.SaveAsFile(Path.Combine(pathDialog.SelectedPath, item.FileName));
                            }

                            MessageBox.Show("Attachments downloaded to folder!", "QAA");
                        }
                        else
                        {
                            MessageBox.Show("Invalid Path...", "QAA");
                        }                        
                    }
                    else
                    {
                        MessageBox.Show("No attachments found!", "QAA");
                    }
                }
            }

            catch (NullReferenceException ex)
            {
                MessageBox.Show("Please select an e-mail first...\n\nException:\n " + ex, "QAA");
            }
        }
    }
}
