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
    public partial class frm_ProductCategory : frm_Master 
    {
        DAL.ProductCategory  category ; 
        public frm_ProductCategory()
        {
            InitializeComponent();
            New();
        }
        public override void New()
        {
            category = new DAL.ProductCategory();
            base.New();
        }
        public override void GetData()
        {
            textEdit1.Text = category.Name;
            lookUpEdit1.EditValue = category.ParentID;
            base.GetData();
        }
        public override void SetData()
        {
            category.Name = textEdit1.Text;
            category.ParentID = (lookUpEdit1.EditValue as int?) ?? 0;
            category.Number = "0";
            base.SetData();
        }
        bool IsDataValid()
        {
            if (textEdit1.Text.Trim() == string.Empty)
            {
                textEdit1.ErrorText = "هذا الحقل مطلوب";
                return false;
            }
            var db = new DAL.dbDataContext();
            var oldObj = db.ProductCategories .Where(x => x.Name.Trim() == textEdit1.Text.Trim() && 
            x.ID != category .ID);
            if (oldObj.Count() > 0)
            {
                textEdit1.ErrorText = "هذا الاسم مسجل مسبقا";
                return false;
            }


            return true;
        }
        public override void Save()
        {
            if (IsDataValid() == false)
                return;
            var db = new DAL.dbDataContext();
            if (category .ID == 0)
            {
                db.ProductCategories.InsertOnSubmit(category);
            }
            else
            {
                db.ProductCategories .Attach(category);
            }
            SetData(); 
            db.SubmitChanges(); 
            base.Save();
        }
        
        private void frm_ProductCategory_Load(object sender, EventArgs e)
        {
           
            RefreshData();
            lookUpEdit1.Properties.DisplayMember = nameof(category.Name);
            lookUpEdit1.Properties.ValueMember = nameof(category.ID);
            treeList1.ParentFieldName = nameof(category.ParentID);
            treeList1.KeyFieldName = nameof(category.ID);
            treeList1.OptionsBehavior.Editable = false;
            treeList1.Columns[nameof(category.Number)].Visible = false;
            treeList1.Columns[nameof(category.Name)].Caption = "الاسم";
            treeList1.FocusedNodeChanged += TreeList1_FocusedNodeChanged;
        }

        private void TreeList1_FocusedNodeChanged(object sender, DevExpress.XtraTreeList.FocusedNodeChangedEventArgs e)
        {
            int id = 0;
            if (int.TryParse(e.Node.GetValue("ID").ToString(), out id))
            {
                var db = new DAL.dbDataContext();
                category = db.ProductCategories.Single(x => x.ID == id);
                GetData();
            }
        }

        public override void RefreshData()
        {
            var db = new DAL.dbDataContext();
            var groups = db.ProductCategories;
            lookUpEdit1.Properties.DataSource = groups;


            treeList1.DataSource = groups;
            treeList1.ExpandAll();
            base.RefreshData();
        }
    }
}
