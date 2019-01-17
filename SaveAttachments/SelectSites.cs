﻿using System;
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
        dynamic sitesArray;
        ArrayList shortNames = new ArrayList();

        public SelectSites()
        {
            InitializeComponent();
        }

        private void SelectSites_Load(object sender, EventArgs e)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            WebClient client = new WebClient();
            client.UseDefaultCredentials = true;
            string sitesInput = client.DownloadString("https://documents.i.opw.ie/share/proxy/alfresco/api/people/chiribest/sites/");
            var serializer = new JavaScriptSerializer();
            sitesArray = serializer.DeserializeObject(sitesInput);

            foreach (dynamic s in sitesArray)
            {
                listBox1.Items.Add(s["title"]);
                shortNames.Add(s["shortName"]);
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string siteURL = "http://documents.i.opw.ie/share/page/site/" + shortNames[listBox1.SelectedIndex];
            System.Diagnostics.Process.Start(siteURL);
        }
    }
}