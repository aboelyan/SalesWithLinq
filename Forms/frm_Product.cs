using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Grid;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static SalesWithLinq.Class.Master;

namespace SalesWithLinq.Forms
{

    public partial class frm_Product : frm_Master
    {
        DAL.Product product;
        RepositoryItemLookUpEdit repoUOM = new RepositoryItemLookUpEdit();
        DAL.dbDataContext sdb = new DAL.dbDataContext();
        public frm_Product()
        {
            InitializeComponent();
            RefreshData();
            New();
        }
        public frm_Product( int id )
        {
            InitializeComponent();
            RefreshData();
            LoadProduct(id);
        }
        void LoadProduct(int id)
        {
            using (var db = new DAL.dbDataContext())
            {
                product = db.Products.Single(x => x.ID == id);

            }
            this.Text = string.Format("بيانات صنف : {0}", product.Name);
            GetData();
        }
        public override void New()
        {
            product = new DAL.Product() {
                Code = GetNewProductCode()
            , IsActive = true,
            };
            var db = new DAL.dbDataContext();
            var categ = db.ProductCategories
                    .Where(x => db.ProductCategories.Where(w => w.ParentID == x.ID).Count() == 0).FirstOrDefault();
            if (categ != null )
                product.CategoryID =  categ .ID; 
            base.New();
            this.Text = "اضافه صنف جديد";
            var data = gridView1.DataSource as BindingList<DAL.ProductUnit>;
          
            if (db.UnitNames.Count () == 0)
            {
                db.UnitNames.InsertOnSubmit(new DAL.UnitName() { Name = "قطعه" });
                db.SubmitChanges();
                RefreshData();
            }
            data.Add(new DAL.ProductUnit() { Factor = 1, UnitID = db.UnitNames.First().ID  , Barcode = GetNewBarcode()});
        }


        public override void GetData()
        {
            txt_Code.Text = product.Code;
            txt_Name.Text = product.Name;
            lkp_Category.EditValue = product.CategoryID;
            lkp_Type.EditValue = product.Type;
            memoEdit1.Text = product.Descreption;
            checkEdit1.Checked = product.IsActive;
            if (product.Image != null)
                pictureEdit1.Image = GetImageFromByteArray(product.Image.ToArray());
            else
                pictureEdit1.Image = null;
            gridControl1.DataSource = sdb.ProductUnits.Where(x => x.ProductID == product.ID);
            base.GetData();
        }
        public override void SetData()
        {
            product.CategoryID = Convert.ToInt32(lkp_Category.EditValue);
            product.Code = txt_Code.Text;
            product.Name = txt_Name.Text;
            product.Type = Convert.ToByte(lkp_Type.EditValue);
            product.IsActive = checkEdit1.Checked;
            product.Image =GetByteFromImage( pictureEdit1.Image);
            base.SetData();
        }

        bool ValdiateData()
        {
            if (lkp_Category.EditValue is int == false || Convert.ToInt32(lkp_Category.EditValue ) <=0 )
            {
                lkp_Category.ErrorText = ErrorText;
                return false;
            }
            if (lkp_Type.EditValue is byte == false)
            {
                lkp_Type.ErrorText = ErrorText;
                return false;
            }
            if (txt_Name.Text.Trim() == string.Empty)
            {
                txt_Name.ErrorText = ErrorText;
                return false;
            }
            if (txt_Code.Text.Trim() == string.Empty)
            {
                txt_Code.ErrorText = ErrorText;
                return false;
            }
            var db = new DAL.dbDataContext();
            if (db.Products.Where(x => x.ID != product.ID && x.Name.Trim() == txt_Name.Text.Trim()).Count() > 0)
            {
                txt_Name.ErrorText = "هذا الاسم مسجل بالفعل";
                return false;
            }
            if (db.Products.Where(x => x.ID != product.ID && x.Code.Trim() == txt_Code.Text.Trim()).Count() > 0)
            {
                txt_Code.ErrorText = "هذا الكود مسجل بالفعل";
                return false;
            }

            return true;
        }
        public override void Save()
        {
            gridView1.UpdateCurrentRow();
            if (ValdiateData() == false)
                return;
            var db = new DAL.dbDataContext();

            if (product.ID == 0)
                db.Products.InsertOnSubmit(product);
            else
                db.Products.Attach(product);

            SetData();
            db.SubmitChanges();
            var data = gridView1.DataSource as BindingList<DAL.ProductUnit >;
            foreach (var item in data)
            {
                item.ProductID = product.ID;
                if (string.IsNullOrEmpty(item.Barcode) )
                    item.Barcode = "";
            }
            sdb.SubmitChanges();
            base.Save();
            this.Text = string.Format("بيانات صنف : {0}", product.Name);
        }
        Byte[] GetByteFromImage(Image image)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                try
                {
                    image.Save(stream, ImageFormat.Jpeg);
                    return stream.ToArray();
                }catch 
                {
                    return stream.ToArray();
                }
               
            }
        }
        Image GetImageFromByteArray(Byte[] ByteArray)
        {
            Image img;
            try
            {
                Byte[] imgbyte = ByteArray;
                MemoryStream stream = new MemoryStream(imgbyte, false);
                img = Image.FromStream(stream);
            }catch { img = null; }
            return img; 
        }
        public override void RefreshData()
        {
            //var db = new DAL.dbDataContext();
            //lkp_Category.Properties.DataSource = db.ProductCategories;
            using (var db = new DAL.dbDataContext())
            {
                lkp_Category.Properties.DataSource = db.ProductCategories
                    .Where(x => db.ProductCategories.Where(w => w.ParentID == x.ID).Count() == 0).ToList();
                repoUOM.DataSource = db.UnitNames.ToList();
            }
            base.RefreshData();
        }
        DAL.ProductUnit ins = new DAL.ProductUnit();
        private void frm_Product_Load(object sender, EventArgs e)
        {
            lkp_Category.Properties.DisplayMember = "Name";
            lkp_Category.Properties.ValueMember = "ID";
            lkp_Category.ProcessNewValue += Lkp_Category_ProcessNewValue;
            lkp_Category.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;

            lkp_Type.Properties.DataSource = ProductTypesList;
            lkp_Type.Properties.DisplayMember = "Name";
            lkp_Type.Properties.ValueMember = "ID";

            gridView1.OptionsView.ShowGroupPanel = false;
            gridView1.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Top;
             
            gridView1.Columns[nameof(ins.ID)].Visible = false;
            gridView1.Columns[nameof(ins.ProductID)].Visible = false;
            RepositoryItemCalcEdit calcEdit = new RepositoryItemCalcEdit();
           
            gridControl1.RepositoryItems.Add(calcEdit);
            gridControl1.RepositoryItems.Add(repoUOM);

            gridView1.Columns[nameof(ins.SellPrice)].ColumnEdit = calcEdit;
            gridView1.Columns[nameof(ins.BuyPrice)].ColumnEdit = calcEdit;
            gridView1.Columns[nameof(ins.SellDiscount)].ColumnEdit = calcEdit;
            gridView1.Columns[nameof(ins.Factor)].ColumnEdit = calcEdit;
            gridView1.Columns[nameof(ins.UnitID)].ColumnEdit = repoUOM;

            gridView1.Columns[nameof(ins.Barcode)].Caption = "الباركود";
            gridView1.Columns[nameof(ins.BuyPrice)].Caption = "سعر الشراء";
            gridView1.Columns[nameof(ins.Factor)].Caption = "معامل التحويل";
            gridView1.Columns[nameof(ins.SellDiscount)].Caption = "خصم البيع";
            gridView1.Columns[nameof(ins.SellPrice)].Caption = "سعر البيع";
            gridView1.Columns[nameof(ins.UnitID)].Caption = "اسم الوحده";

            repoUOM.ValueMember = "ID";
            repoUOM.DisplayMember = "Name";
            repoUOM.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;

            repoUOM.ProcessNewValue += RepoUOM_ProcessNewValue;

            gridView1.ValidateRow += GridView1_ValidateRow;
            gridView1.InvalidRowException += GridView1_InvalidRowException;
            gridView1.FocusedRowChanged += GridView1_FocusedRowChanged;
            gridView1.CustomRowCellEditForEditing += GridView1_CustomRowCellEditForEditing;
      
        }

        private void GridView1_CustomRowCellEditForEditing(object sender, CustomRowCellEditEventArgs e)
        {
           if(e.Column.FieldName== nameof(ins.UnitID))
            {
                var ids = ((Collection<DAL.ProductUnit>)gridView1.DataSource).Select(x => x.UnitID).ToList();
                RepositoryItemLookUpEdit repo = new RepositoryItemLookUpEdit();
                using (var db = new DAL.dbDataContext())
                {
                    var currentID = (Int32?)e.CellValue;
                    ids.Remove(currentID??0 );
                    repo.DataSource = db.UnitNames.Where(x => ids.Contains(x.ID) == false).ToList();
                    repo.ValueMember = "ID";
                    repo.DisplayMember = "Name";
                    repo.PopulateColumns();
                    repo.Columns["ID"].Visible = false;
                    repo.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                    repo.ProcessNewValue += RepoUOM_ProcessNewValue;
                    e.RepositoryItem = repo;
                }
                // select * from xxx where id not  in(5,65,6.56)
            }
        }

        private void GridView1_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            
            gridView1.Columns[nameof(ins.Factor)].OptionsColumn.AllowEdit = !(e.FocusedRowHandle == 0);
            
        }

        private void GridView1_InvalidRowException(object sender, DevExpress.XtraGrid.Views.Base.InvalidRowExceptionEventArgs e)
        {
            e.ExceptionMode = DevExpress.XtraEditors.Controls.ExceptionMode.NoAction;
        }

        private void GridView1_ValidateRow(object sender, DevExpress.XtraGrid.Views.Base.ValidateRowEventArgs e)
        { 
            var row = e.Row as DAL.ProductUnit ;
            var view = sender as GridView;
            if (row == null)
                return;
            if(row.Factor <= 1 && e.RowHandle != 0)
            {
                e.Valid = false;
                view.SetColumnError(view.Columns[nameof(row.Factor)], "يجب ان تكون القيمه اكبر من 1 ");
            }
            if (row.UnitID  <= 0 )
            {
                e.Valid = false;
                view.SetColumnError(view.Columns[nameof(row.UnitID)], ErrorText);
            }
            if(CheckIfBarcodeExist(row.Barcode , prdID : product.ID))
            {
                e.Valid = false;
                view.SetColumnError(view.Columns[nameof(row.Barcode)], " هذا الكود موجود بالفعل ");
            }

        }

        private void RepoUOM_ProcessNewValue(object sender, DevExpress.XtraEditors.Controls.ProcessNewValueEventArgs e)
        {
            if(e.DisplayValue is string value&& value.Trim () != string.Empty)
            {
                var NewObject = new DAL.UnitName() { Name = value.Trim() };
                using (DAL.dbDataContext db = new DAL.dbDataContext())
                {
                    db.UnitNames.InsertOnSubmit(NewObject);
                    db.SubmitChanges();
                }
                ((List<DAL.UnitName>)repoUOM.DataSource).Add(NewObject);
                ((List<DAL.UnitName>)(((LookUpEdit)sender ).Properties.DataSource)).Add(NewObject);
                e.Handled = true;
                 
            }
        }

     
         
        private void Lkp_Category_ProcessNewValue(object sender, DevExpress.XtraEditors.Controls.ProcessNewValueEventArgs e)
        {
            if(e.DisplayValue is string st&&st.Trim() != string.Empty)
            {
                var newObject =new DAL.ProductCategory() {  Name = st, ParentID = 0 , Number = "0"};
                using (var db = new DAL.dbDataContext())
                {
                    db.ProductCategories.InsertOnSubmit(newObject);
                    db.SubmitChanges();
                }
               ((List<DAL.ProductCategory>)lkp_Category.Properties.DataSource).Add(newObject);
                e.Handled = true;
          
            }
        }
        string GetNewProductCode()
        {
            string  maxCode;
            using (var db = new DAL.dbDataContext())
            {
                maxCode = db.Products.Select(x => x.Code).Max(); 
            }
             
            return  GetNextNumberInString(maxCode);
        }
        string GetNewBarcode()
        {
            string maxCode;
            using (var db = new DAL.dbDataContext())
            {
                maxCode = db.ProductUnits .Select(x => x.Barcode ).Max();
            }

            return GetNextNumberInString(maxCode);
        }
      
        Boolean CheckIfBarcodeExist(string barcode , int prdID )
        {
            using (var db = new DAL.dbDataContext()) 
                return  db.ProductUnits.Where(x => x.Barcode == barcode && x.ProductID != prdID  ).Count () > 0;
            
        }

    }
}


