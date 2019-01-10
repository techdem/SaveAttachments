using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Tools.Ribbon;
using Microsoft.Office.Interop.Outlook;
using System.IO;
using System.Windows.Forms;

namespace SaveAttachments
{
    public partial class RibbonMain
    {
        private void RibbonMain_Load(object sender, RibbonUIEventArgs e)
        {

        }

        private void button1_Click(object sender, RibbonControlEventArgs e)
        {
            string myURL = "http://documents.i.opw.ie";
            System.Diagnostics.Process.Start(myURL);
        }

        private void button2_Click(object sender, RibbonControlEventArgs e)
        {
            var context = e.Control.Context as Inspector;
            

            try
            {
                var mailItem = context.CurrentItem as MailItem;

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

                            MessageBox.Show("Attachments downloaded to folder!");
                        }
                        else
                        {
                            MessageBox.Show("Invalid Path...");
                        }                        
                    }
                    else
                    {
                        MessageBox.Show("No attachments found!");
                    }
                }
            }

            catch (NullReferenceException ex)
            {
                MessageBox.Show("Please select an e-mail first...\n\nException:\n " + ex);
            }
        }
    }
}
