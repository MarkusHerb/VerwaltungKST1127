using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VerwaltungKST1127.Material
{
    public partial class Form_Pw : Form
    {
        public string Passwort { get; private set; }

        public Form_Pw()
        {
            InitializeComponent();
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            Passwort = textBoxPw.Text;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void BtnCancle_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();   
        }
    }
}
