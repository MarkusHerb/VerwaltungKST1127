using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VerwaltungKST1127.Auftragsverwaltung
{
    public partial class Form_TestansichtInforEinlesen : Form
    {
        public Form_TestansichtInforEinlesen()
        {
            InitializeComponent();
        }

        public void LoadData(DataTable data)
        {
            dgvEingleseneInforDaten.DataSource = data;
        }
    }
}
