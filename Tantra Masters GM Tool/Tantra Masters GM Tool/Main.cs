using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Tantra_Masters_GM_Tool
{
    public partial class Main : Form
    {
        public SqlConnection conn;
        public InventoryItemAPI inventoryItemAPI;
        public string user;
        ItemEdit itemEdit;
        public Main()
        {
            InitializeComponent();
            string connetionString = @"Data Source=15.235.156.62;Initial Catalog=Masters;User ID=sa;Password=Tantra1!";
            conn = new SqlConnection(connetionString);
            conn.Open();
            conn.Close();
            foreach (Control ctrl in tableLayout.Controls)
            {
                if (ctrl is Button)
                {
                    ctrl.Click += btn_Item_Click;
                    ctrl.Visible = false;
                }
            }            
        }

        public void ReloadItemData(int ID, string data)
        {
            inventoryItemAPI.inventoryId[ID] = new InventoryId();
            inventoryItemAPI.inventoryId[ID] = JsonConvert.DeserializeObject<InventoryId>(data);
            inventoryItemAPI.inventoryId[ID].rawText = data;
            LoadEditorData(itemEdit, ID);
        }

        private void btn_userSearch_Click(object sender, EventArgs e)
        {
            user = userTxtBox.Text;
            string select = "SELECT * FROM inventory where characterName='" + userTxtBox.Text + "'";
            SqlDataAdapter dataAdapter = new SqlDataAdapter(select, conn);
            var ds = new DataSet();
            dataAdapter.Fill(ds);
            dgv1.ReadOnly = true;
            dgv1.DataSource = ds.Tables[0];
            inventoryItemAPI = new InventoryItemAPI();
            inventoryItemAPI.inventoryId = new Dictionary<int,InventoryId>();
            for (int i = 3; i < dgv1.ColumnCount; i++)
            {
                InventoryId inventoryId = new InventoryId();
                inventoryId = JsonConvert.DeserializeObject<InventoryId>(dgv1[i, 0].Value.ToString());

                if (dgv1[i, 0].Value.ToString() != null)
                {
                    
                    inventoryItemAPI.inventoryId.Add(i - 2, inventoryId);
                    string btnName = "btn_item" + (i - 2).ToString();
                    foreach (Control ctrl in tableLayout.Controls)
                    {
                        if (ctrl is Button)
                        {
                            ctrl.Visible = true;
                            if (ctrl.Name == btnName)
                            {
                                if (inventoryId == null)
                                {
                                    ctrl.Text = "None";
                                    ctrl.Enabled = false;
                                }
                                else
                                {
                                    ctrl.Text = inventoryId.itemName;
                                    inventoryItemAPI.inventoryId[i-2].rawText = dgv1[i, 0].Value.ToString();
                                }
                                break;
                            }
                        }
                    }
                }
                else
                {
                    inventoryItemAPI.inventoryId.Add(i - 2, new InventoryId());
                }
            };
            conn.Close();
        }

        private void LoadEditor(int ID)
        {
            ItemEdit newItemEdit = new ItemEdit();
            itemEdit = newItemEdit;
            LoadEditorData(itemEdit, ID);
            itemEdit.ShowDialog();
        }

        private void LoadEditorData(ItemEdit itemEdit, int ID)
        {
            itemEdit.currentID = ID;
            itemEdit.main = this;
            itemEdit.itemId.Text = inventoryItemAPI.inventoryId[ID].itemId.ToString();
            itemEdit.itemName.Text = inventoryItemAPI.inventoryId[ID].itemName;
            itemEdit.itemGrade.Text = inventoryItemAPI.inventoryId[ID].grade;
            itemEdit.rawText.Text = inventoryItemAPI.inventoryId[ID].rawText;
            itemEdit.rawText.ScrollBars = ScrollBars.Vertical;

            itemEdit.flatStatHP.Text = inventoryItemAPI.inventoryId[ID].stats.flat.hp.ToString();
            itemEdit.flatStatTP.Text = inventoryItemAPI.inventoryId[ID].stats.flat.tp.ToString();
            itemEdit.flatStatPatk.Text = inventoryItemAPI.inventoryId[ID].stats.flat.patk.ToString();
            itemEdit.flatStatMatk.Text = inventoryItemAPI.inventoryId[ID].stats.flat.matk.ToString();
            itemEdit.flatStatPdef.Text = inventoryItemAPI.inventoryId[ID].stats.flat.pdef.ToString();
            itemEdit.flatStatMdef.Text = inventoryItemAPI.inventoryId[ID].stats.flat.mdef.ToString();
            itemEdit.flatStatHit.Text = inventoryItemAPI.inventoryId[ID].stats.flat.hit.ToString();
            itemEdit.flatStatDodge.Text = inventoryItemAPI.inventoryId[ID].stats.flat.dodge.ToString();
            itemEdit.flatStatCrit.Text = inventoryItemAPI.inventoryId[ID].stats.flat.crit.ToString();
            itemEdit.flatStatCritEva.Text = inventoryItemAPI.inventoryId[ID].stats.flat.critEva.ToString();

            itemEdit.perStatHP.Text = inventoryItemAPI.inventoryId[ID].stats.percent.hp.ToString() + "%";
            itemEdit.perStatTP.Text = inventoryItemAPI.inventoryId[ID].stats.percent.tp.ToString() + "%";
            itemEdit.perStatPatk.Text = inventoryItemAPI.inventoryId[ID].stats.percent.patk.ToString() + "%";
            itemEdit.perStatMatk.Text = inventoryItemAPI.inventoryId[ID].stats.percent.matk.ToString() + "%";
            itemEdit.perStatPdef.Text = inventoryItemAPI.inventoryId[ID].stats.percent.pdef.ToString() + "%";
            itemEdit.perStatMdef.Text = inventoryItemAPI.inventoryId[ID].stats.percent.mdef.ToString() + "%";
            itemEdit.perStatCritBoost.Text = inventoryItemAPI.inventoryId[ID].stats.percent.critDmgBoost.ToString() + "%";
            itemEdit.perStatCritReduc.Text = inventoryItemAPI.inventoryId[ID].stats.percent.critDmgReduc.ToString() + "%";
            itemEdit.perStatDropChance.Text = inventoryItemAPI.inventoryId[ID].stats.percent.dropChance.ToString() + "%";
        }

        private void btn_Item_Click(object sender, EventArgs e)
        {
            var button = sender as Button;
            LoadEditor(int.Parse(button.Name.Substring(8)));
        }
    }

}
