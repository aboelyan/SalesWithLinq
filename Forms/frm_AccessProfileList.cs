using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.Utils;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;

namespace SalesWithLinq.Forms
{
    public partial class frm_AccessProfileList : frm_Master 
    {
      
        public frm_AccessProfileList()
        {
            InitializeComponent();
            gridView1.OptionsBehavior.Editable = false;
            gridView1.DoubleClick += GridView1_Click;
            RefreshData();
            gridView1.Columns["ID"].Visible = false;
            btn_Delete.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            btn_Save.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
        }
        public override void RefreshData()
        {
            using (var db = new DAL.dbDataContext())
            {
                gridControl1.DataSource = db.UserAccessProfileNames .ToList();

            }
            base.RefreshData();
        }
        public override void New()
        {
            frm_Main.OpenFormByName(nameof(frm_AccessProfile));
            base.New();
        }
        private void GridView1_Click(object sender, EventArgs e)
        {
            DXMouseEventArgs ea = e as DXMouseEventArgs;
            GridView view = sender as GridView;
            GridHitInfo info = view.CalcHitInfo(ea.Location);
            if (info.InRow || info.InRowCell)
            {
                var frm = new frm_AccessProfile(Convert.ToInt32(view.GetFocusedRowCellValue("ID")));
                frm_Main.OpenForm( frm ,true );
                RefreshData();
            }
        }
    }
}