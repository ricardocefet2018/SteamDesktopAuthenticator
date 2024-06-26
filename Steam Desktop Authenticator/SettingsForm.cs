﻿using System;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Steam_Desktop_Authenticator
{
    public partial class SettingsForm : Form
    {
        Manifest manifest;
        bool fullyLoaded = false;

        public SettingsForm()
        {
            InitializeComponent();

            // Get latest manifest
            manifest = Manifest.GetManifest(true);

            chkPeriodicChecking.Checked = manifest.PeriodicChecking;
            numPeriodicInterval.Value = manifest.PeriodicCheckingInterval;
            chkShowPopup.Checked = manifest.ShowAPopup;
            chkCheckAll.Checked = manifest.CheckAllAccounts;
            chkConfirmMarket.Checked = manifest.AutoConfirmMarketTransactions;
            chkConfirmTrades.Checked = manifest.AutoConfirmTrades;
            chkConfirmTradesFromFile.Checked = manifest.AutoConfirmTradesFromFile;
            txtTradeListFilePath.Text = manifest.TradeListFilePath;

            SetControlsEnabledState(chkPeriodicChecking.Checked);

            fullyLoaded = true;
        }

        private void SetControlsEnabledState(bool enabled)
        {
            numPeriodicInterval.Enabled = chkCheckAll.Enabled =
                chkConfirmMarket.Enabled = chkConfirmTrades.Enabled = 
                chkConfirmTradesFromFile.Enabled = chkShowPopup.Enabled = enabled;

            button1.Enabled = chkConfirmTradesFromFile.Checked;
        }

        private bool ShowWarning(CheckBox affectedBox)
        {
            if (!fullyLoaded) return affectedBox.Checked;

            var result = MessageBox.Show("Warning: enabling this will severely reduce the security of your items! Use of this option is at your own risk. Would you like to continue?", "Warning!", MessageBoxButtons.YesNo);
            if (result == DialogResult.No)
            {
                affectedBox.Checked = false;
            }
            return affectedBox.Checked;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            manifest.PeriodicChecking = chkPeriodicChecking.Checked;
            manifest.PeriodicCheckingInterval = (int)numPeriodicInterval.Value;
            manifest.ShowAPopup = chkShowPopup.Checked;
            manifest.CheckAllAccounts = chkCheckAll.Checked;
            manifest.AutoConfirmMarketTransactions = chkConfirmMarket.Checked;
            manifest.AutoConfirmTrades = chkConfirmTrades.Checked;
            manifest.AutoConfirmTradesFromFile = chkConfirmTradesFromFile.Checked;
            manifest.TradeListFilePath = txtTradeListFilePath.Text;

            manifest.Save();
            this.Close();
        }

        private void chkPeriodicChecking_CheckedChanged(object sender, EventArgs e)
        {
            SetControlsEnabledState(chkPeriodicChecking.Checked);
        }

        private void chkConfirmMarket_CheckedChanged(object sender, EventArgs e)
        {
            if (chkConfirmMarket.Checked)
                ShowWarning(chkConfirmMarket);
        }

        private void chkConfirmTrades_CheckedChanged(object sender, EventArgs e)
        {
            if (chkConfirmTrades.Checked)
                ShowWarning(chkConfirmTrades);
        }

        private void chkConfirmTradesFromFile_CheckedChanged(object sender, EventArgs e)
        {
            if (chkConfirmTradesFromFile.Checked)
            {
                var shouldContinue = ShowWarning(chkConfirmTradesFromFile);
                if (!shouldContinue) return;
            }

            button1.Enabled = chkConfirmTradesFromFile.Checked;
            return;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (chkConfirmTradesFromFile.Checked)
            {

                using (OpenFileDialog ofd = new OpenFileDialog())
                {
                    ofd.InitialDirectory = Manifest.GetExecutableDir();
                    ofd.Filter = "json files (*.json)|*.json";
                    ofd.RestoreDirectory = true;

                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        txtTradeListFilePath.Text = ofd.FileName;
                    }
                }
            }
        }
    }
}
