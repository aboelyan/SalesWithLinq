using DevExpress.XtraGrid.Views.Grid;
using SalesWithLinq.Class;
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
    public partial class frm_ProductList : frm_Master
    {
        public frm_ProductList()
        {
            InitializeComponent();
        }

        private void frm_ProductList_Load(object sender, EventArgs e)
        {

           // Session.ProductsView.ListChanged += ProductsView_ListChanged; 
            gridView1.OptionsBehavior.Editable = false;
            btn_Save.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            btn_Delete .Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            gridView1.DoubleClick += GridView1_DoubleClick;
            RefreshData();
            gridView1.CustomColumnDisplayText += GridView1_CustomColumnDisplayText;
            gridControl1.ViewRegistered += GridControl1_ViewRegistered;
            gridView1.OptionsDetail.ShowDetailTabs = false;

            var ins = new Session.ProductViewClass();
            //  gridView1.Columns[nameof(ins.CategoryName)].Caption = "الفئه";
            gridView1.Columns[nameof(ins.Code)].Caption = "الكود";
            gridView1.Columns[nameof(ins.Descreption)].Caption = "الوصف";
            gridView1.Columns[nameof(ins.IsActive)].Caption = "نشط";
            gridView1.Columns[nameof(ins.Name)].Caption = "الاسم";
            gridView1.Columns[nameof(ins.Type)].Caption = "النوع";
            gridView1.Columns[nameof(ins.ID)].Visible = false;


        }
 

        public override void New()
        {

            var frm = new frm_Product();
            frm_Main.OpenForm(frm); 

        }
        private void GridControl1_ViewRegistered(object sender, DevExpress.XtraGrid.ViewOperationEventArgs e)
        {
         
            if(e.View .LevelName == "UOM")
            {
                GridView view = e.View as GridView;
                view.OptionsView.ShowViewCaption = true;
                view.ViewCaption = "وحدات القياس ";
                view.Columns["UnitName"].Caption = "اسم الوحده";
                view.Columns["Factor"].Caption = "المعامل";
                view.Columns["SellPrice"].Caption = "سعر البيع";
                view.Columns["BuyPrice"].Caption = "سعر الشراء";
                view.Columns["Barcode"].Caption = "الباركود";
            }
        }

        private void GridView1_DoubleClick(object sender, EventArgs e)
        {
            int id = 0;
            if(int.TryParse (gridView1.GetFocusedRowCellValue("ID").ToString(),out id ) && id > 0)
            {
                var frm = new frm_Product(id);
                frm_Main.OpenForm(frm,true);
                
            }
        }

        private void GridView1_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
           if(e.Column.FieldName == "Type")
            {
                e.DisplayText = Master.ProductTypesList.Single(x => x.ID == Convert.ToInt32(e.Value)).Name;
            }
        }

        public override void RefreshData()
        {
           
            base.RefreshData(); 
            gridControl1.DataSource = Session.ProductsView;
          

        }

     
    }
}
