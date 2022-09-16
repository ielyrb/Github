using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Tantra_Masters_GM_Tool
{
    public partial class ItemEdit : Form
    {
        public int currentID;
        public Main main;
        SqlConnection conn;
        public ItemEdit()
        {
            InitializeComponent();
            string connetionString = @"Data Source=15.235.156.62;Initial Catalog=Masters;User ID=sa;Password=Tantra1!";
            conn = new SqlConnection(connetionString);
        }

        private void btn_Back_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btn_ItemSave_Click(object sender, EventArgs e)
        {
            conn.Open();
            using (SqlCommand cmd = conn.CreateCommand())
            {
                string columnName = "item" + currentID.ToString();
                string query = "UPDATE inventory set " + columnName + "='" + rawText.Text + "' where characterName='" + main.user + "'";
                cmd.CommandText = query;
                cmd.ExecuteNonQuery();
                main.ReloadItemData(currentID, rawText.Text);
            }
            conn.Close();
        }
    }
}
