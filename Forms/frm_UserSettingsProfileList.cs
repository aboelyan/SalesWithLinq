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
    public partial class frm_UserSettingsProfileList : frm_Master 
    {
        public frm_UserSettingsProfileList()
        {
            InitializeComponent();
            gridView1.OptionsBehavior.Editable = false;
            gridView1.DoubleClick  += GridView1_Click;
            RefreshData();
            gridView1.Columns["ID"].Visible = false;
            btn_Delete.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            btn_Save.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
        }
        public override void RefreshData()
        {
            using(var db = new DAL.dbDataContext())
            {
                gridControl1 .DataSource = db.UserSettingsProfiles.ToList();

            }
            base.RefreshData();
        }
        public override void New()
        {
            var frm = new  frm_UserSettingsProfile();
            frm_Main.OpenForm(frm);
        }
        private void GridView1_Click(object sender, EventArgs e)
        {
            DXMouseEventArgs ea = e as DXMouseEventArgs;
            GridView view = sender as GridView;
            GridHitInfo info = view.CalcHitInfo(ea.Location);
            if (info.InRow || info.InRowCell)
            {
                var frm = new frm_UserSettingsProfile (Convert.ToInt32(view.GetFocusedRowCellValue("ID")));
                frm_Main.OpenForm(frm, true);

                RefreshData();
            }
        }
    }
}