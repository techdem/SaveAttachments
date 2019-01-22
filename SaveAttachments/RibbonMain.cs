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
            SelectSites displaySites = new SelectSites("download");
            displaySites.Show();
        }

        private void button2_Click(object sender, RibbonControlEventArgs e)
        {
            SelectSites displaySites = new SelectSites("upload");
            displaySites.Show();
        }
    }
}
