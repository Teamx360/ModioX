﻿using System;
using System.Windows.Forms;
using DarkUI.Forms;

namespace ModioX.Forms
{
    public partial class DataViewDialog : DarkForm
    {
        /// <summary>
        ///     Initialize application form
        /// </summary>
        public DataViewDialog()
        {
            InitializeComponent();
        }

        private void DataViewDialog_Load(object sender, EventArgs e)
        {
        }

        private void DataViewDialog_Scroll(object sender, ScrollEventArgs e)
        {
            PanelDetails.Update(); // Removes lag when scrolling
        }
    }
}