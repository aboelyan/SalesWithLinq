using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SalesWithLinq.Forms
{
    public partial class frm_StoresList : XtraForm
    {
        public frm_StoresList()
        {
            InitializeComponent();
        }

        private void frm_StoresList_Load(object sender, EventArgs e)
        {

            RefreshData();

            gridView1.OptionsBehavior.Editable = false; // عدم السماح بالتعديلات 
            gridView1.Columns["ID"].Visible = false;
            gridView1.Columns["Name"].Caption = "الاسم";
            gridView1.DoubleClick += GridView1_DoubleClick;
        }

        private void GridView1_DoubleClick(object sender, EventArgs e)
        {
            int SoteID = 0;
            SoteID =Convert.ToInt32( gridView1.GetFocusedRowCellValue("ID"));
            frm_Stores frm = new frm_Stores(SoteID);
            frm_Main.OpenForm(frm, true); 
            RefreshData();
        }

        private void barButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            RefreshData();
        }
        void RefreshData()
        {
            var db = new DAL.dbDataContext();
            gridControl1.DataSource = db.Stores.Select(x=>new { x.ID , x.Name  });

        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frm_Stores frm = new frm_Stores(); 
            frm_Main.OpenForm(frm, true);

            RefreshData();
        }
    }
}
