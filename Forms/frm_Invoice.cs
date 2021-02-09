using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraReports.UI;
using SalesWithLinq.Class;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SalesWithLinq.Forms
{
    public partial class frm_Invoice : frm_Master 
    {
        DAL.InvoiceHeader Invoice;
        Master.InvoiceType Type;
        DAL.dbDataContext generalDB;

        RepositoryItemGridLookUpEdit repoItems;
        RepositoryItemLookUpEdit repoItemsAll;
        RepositoryItemLookUpEdit repoUOM;
        RepositoryItemLookUpEdit repoStores;
        public frm_Invoice(Master.InvoiceType _Type )
        {
            InitializeComponent();
            gridView1.ValidateRow += GridView1_ValidateRow;
            gridView1.InvalidRowException += GridView1_InvalidRowException; 
            lkp_PartType.EditValueChanged += Lkp_PartType_EditValueChanged; 
            Type = _Type;
            RefreshData();
            New();
        }

     
        DAL.InvoiceDetail detailsInstance = new DAL.InvoiceDetail();
        private void frm_Invoice_Load(object sender, EventArgs e)
        {
            switch (Type)
            {
                case Master.InvoiceType.Purchase:
                    this.Text = "فاتوره مشتريات ";
                    chk_PostToStore.Enabled = false;
                    chk_PostToStore.Checked = true;
                    break;
                case Master.InvoiceType.Sales:
                    break;
                case Master.InvoiceType.PurchaseReturn:
                    break;
                case Master.InvoiceType.SalesReturn:
                    break;
                default:
                    break;
            
            }

            btn_Print.Visibility = DevExpress.XtraBars.BarItemVisibility.Always ;
            lkp_PartType.IntializeData(Class.Master.PartTypesList);
            lkp_PartType.Properties.PopulateColumns();
            lkp_PartType.Properties.Columns["ID"].Visible = false;
            glkp_PartID.Properties.Buttons.Add(new EditorButton(ButtonPredefines.Plus));
            glkp_PartID.ButtonClick += Lkp_PartType_ButtonClick;
            glkp_PartID.Properties .ValidateOnEnterKey = true;
            glkp_PartID.Properties.AllowNullInput = DefaultBoolean.False;
            glkp_PartID.Properties.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFitResizePopup;
            glkp_PartID.Properties.ImmediatePopup = true;
            glkp_PartID.Properties.ButtonClick += RepoItems_ButtonClick;
            var PartIDView = glkp_PartID.Properties.View;
            PartIDView.FocusRectStyle = DrawFocusRectStyle.RowFullFocus;
            PartIDView.OptionsSelection.UseIndicatorForSelection = true;
            PartIDView.OptionsView.ShowAutoFilterRow = true;
            PartIDView.OptionsView.ShowFilterPanelMode = ShowFilterPanelMode.ShowAlways;
            PartIDView.PopulateColumns(glkp_PartID.Properties .DataSource);
            //PartIDView.Columns["IsActive"].Visible = false;
            //PartIDView.Columns["Type"].Visible = false;


            gridView1.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Top;

            gridView1.Columns[nameof(detailsInstance.ID)].Visible = false;
            gridView1.Columns[nameof(detailsInstance.InoiceID)].Visible = false;

            repoUOM.IntializeData(Session.UnitNames, gridView1.Columns[nameof(detailsInstance.ItemUnitID)], gridControl1);
            repoStores.IntializeData(Session.Store, gridView1.Columns[nameof(detailsInstance.StoreID)], gridControl1);

            repoItems = new RepositoryItemGridLookUpEdit();
            repoItems.IntializeData(Session.ProductsView.Where(x => x.IsActive == true), gridView1.Columns[nameof(detailsInstance.ItemID)], gridControl1);
            repoItems.ValidateOnEnterKey = true;
            repoItems.AllowNullInput = DefaultBoolean.False;
            repoItems.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFitResizePopup;
            repoItems.ImmediatePopup = true;
            repoItems.Buttons.Add(new EditorButton(ButtonPredefines.Plus));
            repoItems.ButtonClick += RepoItems_ButtonClick;
            var repoView = repoItems.View;
            repoView.FocusRectStyle = DrawFocusRectStyle.RowFullFocus;
            repoView.OptionsSelection.UseIndicatorForSelection = true;
            repoView.OptionsView.ShowAutoFilterRow = true;
            repoView.OptionsView.ShowFilterPanelMode = ShowFilterPanelMode.ShowAlways;
            repoView.PopulateColumns(repoItems.DataSource);
            repoView.Columns["IsActive"].Visible = false;
            repoView.Columns["Type"].Visible = false;

            repoView.Columns["ID"].Caption = "كود";
            repoView.Columns["Name"].Caption = "الاسم";
            repoView.Columns["Descreption"].Caption = "الوصف";
            repoView.Columns["CategoryName"].Caption = "الفئه";
            repoItemsAll.IntializeData(Session.ProductsView, gridView1.Columns[nameof(detailsInstance.ItemID)], gridControl1);

            RepositoryItemSpinEdit spinEdit = new RepositoryItemSpinEdit();
            gridView1.Columns[nameof(detailsInstance.TotalPrice)].ColumnEdit = spinEdit;
            gridView1.Columns[nameof(detailsInstance.Price)].ColumnEdit = spinEdit;
            gridView1.Columns[nameof(detailsInstance.ItemQty)].ColumnEdit = spinEdit;
            gridView1.Columns[nameof(detailsInstance.DiscountValue)].ColumnEdit = spinEdit;

            RepositoryItemSpinEdit spinRatioEdit = new RepositoryItemSpinEdit();
            gridView1.Columns[nameof(detailsInstance.Discount)].ColumnEdit = spinRatioEdit;

            spinRatioEdit.Increment = 0.01m;
            spinRatioEdit.Mask.EditMask = "p";
            spinRatioEdit.Mask.UseMaskAsDisplayFormat = true;
            spinRatioEdit.MaxValue = 1;

            gridControl1.RepositoryItems.Add(spinRatioEdit);
            gridControl1.RepositoryItems.Add(spinEdit);
            gridView1.Columns[nameof(detailsInstance.TotalPrice)].OptionsColumn.AllowFocus = false;


            gridView1.Columns.Add(new GridColumn() { Name = "clmCode", FieldName = "Code", Caption = "الكود", UnboundType = DevExpress.Data.UnboundColumnType.String });
            gridView1.Columns.Add(new GridColumn()
            {
                Name = "clmIndex",
                FieldName = "Index",
                Caption = "مسلسل",
                UnboundType = DevExpress.Data.UnboundColumnType.Integer,
                MaxWidth = 30
            });

            gridView1.Columns[nameof(detailsInstance.ItemID)].Caption = "الصنف";
            gridView1.Columns[nameof(detailsInstance.CostValue)].Caption = "سعر التكلفه";
            gridView1.Columns[nameof(detailsInstance.Discount)].Caption = "ن.خصم";
            gridView1.Columns[nameof(detailsInstance.DiscountValue)].Caption = "ق.خصم";
            gridView1.Columns[nameof(detailsInstance.ItemQty)].Caption = "الكميه";
            gridView1.Columns[nameof(detailsInstance.ItemUnitID)].Caption = "الوحده";
            gridView1.Columns[nameof(detailsInstance.Price)].Caption = "السعر";
            gridView1.Columns[nameof(detailsInstance.StoreID)].Caption = "المخزن";
            gridView1.Columns[nameof(detailsInstance.TotalCostValue)].Caption = "اجمالي التكلفه";
            gridView1.Columns[nameof(detailsInstance.TotalPrice)].Caption = "اجمالي السعر";

            gridView1.Columns["Index"].OptionsColumn.AllowFocus = false;
            gridView1.Columns[nameof(detailsInstance.TotalCostValue)].OptionsColumn.AllowFocus = false;
            gridView1.Columns[nameof(detailsInstance.CostValue)].OptionsColumn.AllowFocus = false;

            gridView1.Columns["Index"].VisibleIndex = 0;
            gridView1.Columns["Code"].VisibleIndex = 1;
            gridView1.Columns[nameof(detailsInstance.ItemID)].MinWidth = 125;
            gridView1.Columns[nameof(detailsInstance.ItemID)].VisibleIndex = 2;
            gridView1.Columns[nameof(detailsInstance.ItemUnitID)].VisibleIndex = 3;
            gridView1.Columns[nameof(detailsInstance.ItemQty)].VisibleIndex = 4;
            gridView1.Columns[nameof(detailsInstance.Price)].VisibleIndex = 5;
            gridView1.Columns[nameof(detailsInstance.Discount)].VisibleIndex = 6;
            gridView1.Columns[nameof(detailsInstance.DiscountValue)].VisibleIndex = 7;
            gridView1.Columns[nameof(detailsInstance.TotalPrice)].VisibleIndex = 8;
            gridView1.Columns[nameof(detailsInstance.CostValue)].VisibleIndex = 9;
            gridView1.Columns[nameof(detailsInstance.TotalCostValue)].VisibleIndex = 10;
            gridView1.Columns[nameof(detailsInstance.StoreID)].VisibleIndex = 11;

            gridView1.Appearance.EvenRow.BackColor = Color.FromArgb(200, 255, 249, 196);
            gridView1.OptionsView.EnableAppearanceEvenRow = true;

            gridView1.Appearance.OddRow.BackColor = Color.WhiteSmoke;
            gridView1.OptionsView.EnableAppearanceOddRow = true;

            RepositoryItemButtonEdit buttonEdit = new RepositoryItemButtonEdit();
            gridControl1.RepositoryItems.Add(buttonEdit);
            buttonEdit.Buttons.Clear();
            buttonEdit.Buttons.Add(new EditorButton(ButtonPredefines.Delete));
            buttonEdit.ButtonClick += ButtonEdit_ButtonClick;
            GridColumn clmnDelete = new GridColumn()
            {
                Name = "clmnDelete",
                Caption = "",
                FieldName = "Delete",
                ColumnEdit = buttonEdit,
                VisibleIndex = 100,
                Width = 15
            };
            buttonEdit.TextEditStyle = TextEditStyles.HideTextEditor;
            gridView1.Columns.Add(clmnDelete);



            #region Events
            spn_DiscountValue.Enter += new System.EventHandler(this.spn_DiscountValue_Enter);
            spn_DiscountValue.Leave += Spn_DiscountValue_Leave;
            spn_DiscountValue.EditValueChanged += Spn_DiscountValue_EditValueChanged;
            spn_DiscountRation.EditValueChanged += Spn_DiscountValue_EditValueChanged;
            spn_TaxValue.Enter += Spn_TaxValue_Enter;
            spn_TaxValue.Leave += Spn_TaxValue_Leave;
            spn_TaxValue.EditValueChanged += Spn_TaxValue_EditValueChanged;
            spn_Tax.EditValueChanged += Spn_TaxValue_EditValueChanged;
            spn_TaxValue.EditValueChanged += Spn_EditValueChanged;
            spn_DiscountValue.EditValueChanged += Spn_EditValueChanged;
            spn_Expences.EditValueChanged += Spn_EditValueChanged;
            spn_Total.EditValueChanged += Spn_EditValueChanged;
            spn_Paid.EditValueChanged += Spn_Paid_EditValueChanged;
            spn_Net.EditValueChanged += Spn_Paid_EditValueChanged;
            spn_Net.EditValueChanging += Spn_Net_EditValueChanging;

            spn_Net.DoubleClick += Spn_Net_MouseDoubleClick;

            lkp_Branch.EditValueChanging += Lkp_Branch_EditValueChanging;
            gridView1.CustomRowCellEditForEditing += GridView1_CustomRowCellEditForEditing;
            gridView1.CellValueChanged += GridView1_CellValueChanged;
            gridView1.RowCountChanged += GridView1_RowCountChanged;
            gridView1.RowUpdated += GridView1_RowUpdated;
            gridView1.CustomUnboundColumnData += GridView1_CustomUnboundColumnData;
            gridView1.CellValueChanging += GridView1_CellValueChanging;
            gridControl1.ProcessGridKey += GridControl1_ProcessGridKey;

            this.Activated += Frm_Invoice_Activated;
            this.KeyPreview = true;
            this.KeyDown += Frm_Invoice_KeyDown;

            #endregion

        }

     

        private void GridView1_CellValueChanging(object sender, CellValueChangedEventArgs e)
        {
            if(e.Column.FieldName == "ItemID")
            {
                var row = gridView1.GetRow(e.RowHandle) as DAL.InvoiceDetail;
                if (row != null)
                {
                    if (row.ItemID != 0 && e.Value.Equals(row.ItemID) == false)
                        row.ItemUnitID = 0;
                }
            }
           
        }

        private void RepoItems_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            if (e.Button.Kind == ButtonPredefines.Plus)
                frm_Main.OpenFormByName(nameof(frm_Product));
        }

        #region GridViewStuff

        private void Frm_Invoice_KeyDown(object sender, KeyEventArgs e)
        {
           if(e.KeyCode == Keys.F5)
            {
                MoveFocuseToGrid(e.Modifiers  == Keys.Shift );
            }
            else if(e.KeyCode == Keys.F6)
            {
                //TODO Go to Product by index 
            }
            else if (e.KeyCode == Keys.F7) 
                glkp_PartID.Focus(); 
            else if (e.KeyCode == Keys.F8) 
                txt_Code .Focus(); 
            else if (e.KeyCode == Keys.F9) 
                spn_DiscountValue .Focus();
            else if (e.KeyCode == Keys.F10)
                spn_TaxValue.Focus();
            else if (e.KeyCode == Keys.F11)
                spn_Expences.Focus();
            else if (e.KeyCode == Keys.F12)
                spn_Paid.Focus();

        }

        private void ButtonEdit_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            GridView view = ((GridControl)((ButtonEdit)sender).Parent).MainView as GridView;
            if(view.FocusedRowHandle >= 0)
            {
                view.DeleteSelectedRows();
            }
        }

        private void Frm_Invoice_Activated(object sender, EventArgs e)
        {
            MoveFocuseToGrid();
        }

        private void GridView1_InvalidRowException(object sender, InvalidRowExceptionEventArgs e)
        {
            if (e.Row == null || (e.Row as DAL.InvoiceDetail).ItemID == 0)
            {
                gridView1.DeleteRow(e.RowHandle);
                //e.ExceptionMode = DevExpress.XtraEditors.Controls.ExceptionMode.Ignore ;
            }
                e.ExceptionMode = DevExpress.XtraEditors.Controls.ExceptionMode.Ignore ;
        }

        private void GridView1_ValidateRow(object sender, ValidateRowEventArgs e)
        {

            var row = e.Row as DAL.InvoiceDetail;
            if(row == null || row .ItemID == 0)
            {
                e.Valid = false;
              
            }
        }

        private void GridControl1_ProcessGridKey(object sender, KeyEventArgs e)
        {
            GridControl control = sender as GridControl;
            if (control == null) return;
            GridView view = control.FocusedView as GridView;
            if (view == null) return;
            if (view.FocusedColumn == null) return;
            if (e.KeyCode == Keys.Return)
            {
                string focusedColumn = view.FocusedColumn.FieldName;
                if (view.FocusedColumn .FieldName == "Code" || view.FocusedColumn.FieldName == "ItemID")
                {
                    GridControl1_ProcessGridKey(sender, new KeyEventArgs(Keys.Tab));
                }
                if (view.FocusedRowHandle < 0)
                {
                    view.AddNewRow();
                    view.FocusedColumn =view.Columns[focusedColumn];
                }
                e.Handled = true;

            }
            else if(e.KeyCode == Keys.Tab)
            {
                view.FocusedColumn = view.VisibleColumns[view.FocusedColumn.VisibleIndex  - 1];
                e.Handled = true;

            }
            //GridView focusedView = (sender as GridControl).FocusedView as GridView;
            //if (e.KeyData == Keys.F10  && focusedView.FocusedRowHandle == GridControl.NewItemRowHandle && focusedView.RowCount == 0)
            //{
            //    focusedView.FocusedRowHandle = GridControl.InvalidRowHandle;
            //    e.Handled = true;
            //}




        }

        string enteredCode;
        private void GridView1_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            if (e.Column.FieldName == "Code")
            {
                if (e.IsSetData)
                {
                    enteredCode = e.Value.ToString();
                }
                else if (e.IsGetData)
                {

                    e.Value = enteredCode;
                }
            }
            else if (e.Column.FieldName == "Index")
                e.Value = gridView1.GetVisibleRowHandle(e.ListSourceRowIndex)+1;
        }

     
        private void GridView1_RowUpdated(object sender, RowObjectEventArgs e)
        {
            var items = gridView1.DataSource as Collection<DAL.InvoiceDetail>;
            if (items == null)
                spn_Total.EditValue = 0;
            else
                spn_Total.EditValue = items.Sum(x => x.TotalPrice);
        }
        int CurrentRowsCount = 0;
        private void GridView1_RowCountChanged(object sender, EventArgs e)
        {

            if (CurrentRowsCount < gridView1.RowCount)
            {
                var rows = (gridView1.DataSource as Collection<DAL.InvoiceDetail>);
                var lastRow = rows.Last();
                var row = rows.FirstOrDefault(x => x.ItemID == lastRow.ItemID && x.ItemUnitID == lastRow.ItemUnitID && x != lastRow );
              
                if (row != null)
                {
                    row.ItemQty += lastRow.ItemQty;
                    GridView1_CellValueChanged(sender,
                        new CellValueChangedEventArgs(gridView1.FindRowHandelByRowObject(row),
                        gridView1.Columns[nameof(row.ItemQty )],row.ItemQty));
                    rows.Remove(lastRow);
                }
            }

            CurrentRowsCount = gridView1.RowCount;    
            GridView1_RowUpdated(sender, null);
        }
       
        private void GridView1_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            
            var row = gridView1.GetRow(e.RowHandle) as DAL.InvoiceDetail;
            if (row == null)
                return;
            Session.ProductViewClass itemv = null;
            Session.ProductViewClass.ProductUOMView unitv = null;
            if (e.Column.FieldName == "Code")
            {
                string ItemCode = e.Value.ToString() ; 
                if(Session.GlobalSettings.ReadFormScaleBarcode &&
                    ItemCode.Length == Session.GlobalSettings.BarcodeLength &&
                    ItemCode.StartsWith(Session.GlobalSettings.ScaleBarcodePrefix))
                {
                    var itemCodeString = e.Value.ToString()
                        .Substring(Session.GlobalSettings.ScaleBarcodePrefix.Length,
                        Session.GlobalSettings.ProductCodeLength);
                    ItemCode = Convert.ToInt32(itemCodeString).ToString();
                    string Readvalue = e.Value.ToString().Substring(
                        Session.GlobalSettings.ScaleBarcodePrefix.Length +
                        Session.GlobalSettings.ProductCodeLength) ;
                    if (Session.GlobalSettings.IgnoreCheckDigit)
                        Readvalue = Readvalue.Remove(Readvalue.Length - 1, 1);
                    double  value = Convert.ToDouble (Readvalue);
                    value = value / (Math.Pow(10, Session.GlobalSettings.DivideValueBy));
                    if (Session.GlobalSettings.ReadMode == Session.GlobalSettings.ReadValueMode.Weight)
                        row.ItemQty = value;
                    else if(Session.GlobalSettings.ReadMode == Session.GlobalSettings.ReadValueMode.Price )
                    {

                        itemv = Session.ProductsView.FirstOrDefault(x => x.Units.Select(u => u.Barcode).Contains(ItemCode));
                        if (itemv != null)
                        { 
                            unitv = itemv.Units.First(x => x.Barcode == ItemCode); 
                            switch (Type)
                            {
                                case Master.InvoiceType.PurchaseReturn:
                                case Master.InvoiceType.Purchase:
                                    row.ItemQty = value / unitv.BuyPrice;
                                    break;
                                case Master.InvoiceType.Sales:
                                case Master.InvoiceType.SalesReturn:
                                    row.ItemQty = value / unitv.SellPrice;

                                    break;
                                default:
                                    break;
                            }
                        }

                    }
                    



                }

     
                if(itemv == null )
                   itemv = Session.ProductsView.FirstOrDefault(x => x.Units.Select(u => u.Barcode).Contains(ItemCode));
                if(itemv != null)
                {
                    row.ItemID = itemv.ID;
                    if(unitv == null )
                        unitv = itemv.Units.First(x => x.Barcode == ItemCode);
                    row.ItemUnitID = unitv.UnitID;


                    GridView1_CellValueChanged(sender,
                        new CellValueChangedEventArgs(
                            e.RowHandle, gridView1.Columns[nameof(detailsInstance.ItemID)], row.ItemID));
                    
                    GridView1_CellValueChanged(sender,
                      new CellValueChangedEventArgs(
                          e.RowHandle, gridView1.Columns[nameof(detailsInstance.ItemUnitID )], row.ItemUnitID ));

                    enteredCode = string.Empty;
                    return;
                }
                enteredCode = string.Empty;

            }
            ///////////////
            if (row.ItemID == 0)
                return; 
            itemv = Session.ProductsView.Single(x => x.ID == row.ItemID);
        
            if(row.ItemUnitID == 0)
            {
                row.ItemUnitID = itemv.Units.First().UnitID;
                GridView1_CellValueChanged(sender, new CellValueChangedEventArgs(e.RowHandle, gridView1.Columns[nameof (detailsInstance.ItemUnitID)], row.ItemUnitID));
            }
            unitv = itemv.Units.Single(x => x.UnitID == row.ItemUnitID);
            ///////////////
            switch (e.Column.FieldName)
            {
                case nameof(detailsInstance.ItemID ):
                    if(row.ItemID == 0)
                    {
                        gridView1.DeleteRow(e.RowHandle);
                        return;
                    }
                    if(row.StoreID == 0 && lkp_Branch.IsEditValueValidAndNotZero ())
                        row.StoreID = Convert.ToInt32(lkp_Branch.EditValue ) ;
                    break;
                case nameof(detailsInstance.ItemUnitID):
                    if (Type == Master.InvoiceType.Purchase || Type == Master.InvoiceType.PurchaseReturn)
                        row.Price = unitv.BuyPrice;
                    if (row.ItemQty == 0)
                        row.ItemQty = 1;

                    GridView1_CellValueChanged(sender, new CellValueChangedEventArgs(e.RowHandle, gridView1.Columns[nameof(detailsInstance.Price)], row.Price));


                    break; 
                case nameof(detailsInstance.Price):
                case nameof(detailsInstance.Discount):
                case nameof(detailsInstance.ItemQty):
                    row.DiscountValue = row.Discount * (row.ItemQty * row.Price);
                    GridView1_CellValueChanged(sender, new CellValueChangedEventArgs(e.RowHandle, gridView1.Columns[nameof(detailsInstance.DiscountValue)], row.DiscountValue));
                    break;
                case nameof(detailsInstance.DiscountValue):
                    if (gridView1.FocusedColumn.FieldName == nameof(detailsInstance.DiscountValue))
                        row.Discount = row.DiscountValue / (row.ItemQty * row.Price);
                    row.TotalPrice = (row.ItemQty * row.Price) - row.DiscountValue;
                    row.CostValue = row.TotalPrice / row.ItemQty;
                    row.TotalCostValue = row.TotalPrice;
                    break; 
                default:
                    break;
            }

        }

        private void GridView1_CustomRowCellEditForEditing(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            if(e.Column .FieldName == "ItemUnitID")
            {
                RepositoryItemLookUpEdit repo = new RepositoryItemLookUpEdit();
                repo.NullText = "";
                e.RepositoryItem = repo;
                var row = gridView1.GetRow(e.RowHandle) as  DAL.InvoiceDetail ;
                if (row == null)  
                    return; 
                var item = Session.ProductsView.SingleOrDefault(x => x.ID == row.ItemID);
                if (item == null) 
                    return; 
                repo.DataSource = item.Units;
                repo.ValueMember = "UnitID";
                repo.DisplayMember = "UnitName";


             
            }else if(e.Column .FieldName == nameof(detailsInstance.ItemID))
            {
                e.RepositoryItem = repoItems;
            }
        }
        #endregion


        private void Lkp_Branch_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
        {
            var items = gridView1.DataSource as Collection<DAL.InvoiceDetail>;
            if (e.OldValue is int && e.NewValue is int)
            {

                foreach (var row in items)
                {

                    if (row.StoreID == Convert.ToInt32(e.OldValue))
                        row.StoreID = Convert.ToInt32(e.NewValue);
                }
            }
        }


        private void Lkp_PartType_EditValueChanged(object sender, EventArgs e)
        {
            if (lkp_PartType.IsEditValueOfTypeInt())
            {
                int partType = Convert.ToInt32(lkp_PartType.EditValue);
                if (partType == (int)Master.PartType.Customer)
                    glkp_PartID.IntializeData(Session.Customers);
                else if (partType == (int)Master.PartType.Vendor)
                    glkp_PartID.IntializeData(Session.Vendors);
            }

        }

       
        #region SpenEditCalculations

        private void Spn_Net_MouseDoubleClick(object sender, EventArgs e)
        {
            spn_Paid.EditValue = spn_Net.EditValue;
        }

        private void Spn_Net_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
        {

            if (Convert.ToDouble(e.OldValue) == Convert.ToDouble(spn_Paid.EditValue))
                spn_Paid.EditValue = e.NewValue;
        }

        private void Spn_Paid_EditValueChanged(object sender, EventArgs e)
        {
            var net = Convert.ToDouble(spn_Net.EditValue);
            var paid = Convert.ToDouble(spn_Paid .EditValue );
            spn_Remaing.EditValue = net - paid;

        }

        private void Spn_EditValueChanged(object sender, EventArgs e)
        {

            var total = Convert.ToDouble(spn_Total.EditValue);
            var tax = Convert.ToDouble(spn_TaxValue.EditValue );
            var discount = Convert.ToDouble(spn_DiscountValue.EditValue);
            var expences = Convert.ToDouble(spn_Expences.EditValue);

            spn_Net.EditValue = (total + tax - discount + expences);
        }
 

        private void Spn_TaxValue_EditValueChanged(object sender, EventArgs e)
        {
            var total = Convert.ToDouble(spn_Total.EditValue);
            var val = Convert.ToDouble(spn_TaxValue.EditValue);
            var ratio = Convert.ToDouble(spn_Tax.EditValue);

            if (IsTaxValueFocused)
                spn_Tax.EditValue = (val / total); 
            else
                spn_TaxValue.EditValue = total * ratio; 
        }

        Boolean IsTaxValueFocused;
        private void Spn_TaxValue_Leave(object sender, EventArgs e)
        {
            IsTaxValueFocused = false;
        }

        private void Spn_TaxValue_Enter(object sender, EventArgs e)
        {
            IsTaxValueFocused = true ;

        }

        private void Spn_DiscountValue_EditValueChanged(object sender, EventArgs e)
        {
            var total = Convert.ToDouble(spn_Total.EditValue );
            var discountVal =  Convert.ToDouble(spn_DiscountValue.EditValue );
            var discountRation = Convert.ToDouble(spn_DiscountRation.EditValue );

            if (IsDiscountValueFocused)
            {
                spn_DiscountRation.EditValue = (discountVal / total);
            }
            else
            {
                spn_DiscountValue.EditValue = total * discountRation;
            }
        }

        private void Lkp_PartType_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            if(e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Plus)
            {
                using (var frm = new frm_CustomerVendor(Convert.ToInt32(lkp_PartType.EditValue) == (int)Master.PartType.Customer))
                {
                    frm.ShowDialog();
                    RefreshData();
                }
            } 
        }
        Boolean IsDiscountValueFocused;
        private void spn_DiscountValue_Enter(object sender, EventArgs e)
        {
            IsDiscountValueFocused = true;
        }
        private void Spn_DiscountValue_Leave(object sender, EventArgs e)
        {
            IsDiscountValueFocused = false ;
        }
        #endregion
     
        
        void MoveFocuseToGrid( bool FocuseToItem = false  )
        {
            this.gridControl1.Focus();
            gridView1.FocusedRowHandle = GridControl.InvalidRowHandle;
            gridView1.FocusedColumn = FocuseToItem ? gridView1.Columns["ItemID"]: gridView1.Columns["Code"];
            gridView1.AddNewRow();
            gridView1.UpdateCurrentRow();

        }
        public override void RefreshData()
        {
            lkp_Branch.IntializeData(Session.Store);
            lkp_Drawer.IntializeData(Session.Drawer);
            base.RefreshData();
        }
        public override bool IsDataVailde()
        {
            int NumberOfErrors = 0;
            if(gridView1.RowCount == 0 )
            {
                NumberOfErrors++;
                XtraMessageBox.Show(
                    text: "برجاء ادخال صنف واحد علي الاقل",
                    caption: "",
                    buttons: MessageBoxButtons.OK,
                    icon: MessageBoxIcon.Error);
            }
            NumberOfErrors += txt_Code.IsTextVailde() ? 0 : 1;
            NumberOfErrors += lkp_PartType.IsEditValueValid() ? 0 : 1;
            NumberOfErrors += lkp_Drawer.IsEditValueValid() ? 0 : 1;
            NumberOfErrors += lkp_Branch.IsEditValueValid() ? 0 : 1;
            NumberOfErrors += glkp_PartID.IsEditValueValidAndNotZero() ? 0 : 1;
            NumberOfErrors += dt_Date.IsDateVailde() ? 0 : 1;
            if (chk_PostToStore.Checked)
            {
                NumberOfErrors += dt_PostDate.IsDateVailde() ? 0 : 1;
                layoutControlGroup3.Expanded = true;
            }
            return (NumberOfErrors == 0);

        }
        string GetNewIvoiceCode()
        {
            string maxCode;
            using (var db = new DAL.dbDataContext())
            {
                maxCode = db.InvoiceHeaders.Where(x=> x.InvoiceType == (int)Type) .Select(x => x.Code).Max();
            }

            return Master.GetNextNumberInString(maxCode);
        }
        public override void New()
        {
            Invoice = new DAL.InvoiceHeader()
            {
                Darwer = Session.Defualts.Drawer,
                Date = DateTime.Now,
                PostedToStore = true,
                PostDate = DateTime.Now,
                Code = GetNewIvoiceCode(),
            };
            switch (Type)
            {
                case Master.InvoiceType.PurchaseReturn:
                case Master.InvoiceType.Purchase:
                    Invoice.PartType = (int)Master.PartType.Vendor;
                    Invoice.PartID = Session.Defualts.Vendor;
                    Invoice.Branch = Session.Defualts.RawStore;
                    break;
                case Master.InvoiceType.SalesReturn:
                case Master.InvoiceType.Sales:
                    Invoice.PartType = (int)Master.PartType.Customer ;
                    Invoice.PartID = Session.Defualts.Customer ;
                    Invoice.Branch = Session.Defualts.Store ; 
                    break; 
                default:
                    throw new  NotImplementedException ();
            }
         
          
          
            base.New();
            MoveFocuseToGrid();

        }
  
        public override void GetData()
        {
            lkp_Branch.EditValue = Invoice.Branch;
            lkp_Drawer.EditValue = Invoice.Darwer;
            lkp_PartType.EditValue = Invoice.PartType;
            glkp_PartID.EditValue = Invoice.PartID;
            txt_Code.Text = Invoice.Code;
            dt_Date.DateTime = Invoice.Date;
            dt_DelivaryDate.EditValue  = Invoice.DelivaryDate;
            dt_PostDate.EditValue = Invoice.PostDate;
            memoEme_ShoppingAddress.Text = Invoice.ShippingAddress;
            me_Notes.Text = Invoice.Notes;
            chk_PostToStore.Checked = Invoice.PostedToStore;
            spn_DiscountRation.EditValue = Invoice.DiscountRation;
            spn_DiscountValue.EditValue = Invoice.DiscountValue;
            spn_Expences.EditValue = Invoice.Expences;
            spn_Net.EditValue = Invoice.Net;
            spn_Paid.EditValue = Invoice.Paid;
            spn_Remaing.EditValue = Invoice.Remaing;
            spn_Tax.EditValue = Invoice.Tax;
            spn_TaxValue.EditValue = Invoice.TaxValue;
            spn_Total.EditValue = Invoice.Total;
            generalDB = new DAL.dbDataContext();
            gridControl1.DataSource = generalDB.InvoiceDetails.Where(x => x.InoiceID == Invoice.ID);

            base.GetData();
        }
        public override void SetData()
        {
            Invoice.Branch = Convert.ToInt32(lkp_Branch.EditValue);
            Invoice.Darwer = Convert.ToInt32(lkp_Drawer.EditValue);
            Invoice.PartType = Convert.ToByte(lkp_PartType.EditValue);
            Invoice.PartID = Convert.ToInt32(glkp_PartID.EditValue);
            Invoice.Code = txt_Code.Text;
            Invoice.Date = dt_Date.DateTime;
            Invoice.DelivaryDate = dt_DelivaryDate.EditValue as DateTime?;
            Invoice.PostDate = dt_PostDate.EditValue as DateTime?;
            Invoice.ShippingAddress = memoEme_ShoppingAddress.Text;
            Invoice.Notes = me_Notes.Text;
            Invoice.PostedToStore = chk_PostToStore.Checked;
            Invoice.DiscountRation = Convert.ToDouble(spn_DiscountRation.EditValue);
            Invoice.DiscountValue = Convert.ToDouble(spn_DiscountValue.EditValue);
            Invoice.Expences = Convert.ToDouble(spn_Expences.EditValue);
            Invoice.Net = Convert.ToDouble(spn_Net.EditValue);
            Invoice.Paid = Convert.ToDouble(spn_Paid.EditValue);
            Invoice.Remaing = Convert.ToDouble(spn_Remaing.EditValue);
            Invoice.Tax = Convert.ToDouble(spn_Tax.EditValue);
            Invoice.TaxValue = Convert.ToDouble(spn_TaxValue.EditValue);
            Invoice.Total = Convert.ToDouble(spn_Total.EditValue);
            Invoice.InvoiceType = (byte)Type;
            base.SetData();
        }
        public override void Save()
        {
            gridView1.UpdateCurrentRow();
            var db = new DAL.dbDataContext();
            if(Invoice.ID == 0)
            {
                db.InvoiceHeaders.InsertOnSubmit(Invoice);
            }else
            {
                db.InvoiceHeaders.Attach(Invoice);
            }
            SetData();

            var items = (Collection<DAL.InvoiceDetail>)gridView1.DataSource ;


            if(Invoice.Expences > 0)
            {
                var totalPrice = items.Sum(x => x.TotalPrice);
                var totalQTY = items.Sum(x => x.ItemQty);
                var ByPriceUnit = Invoice.Expences / totalPrice;// السعر لكل جنيه واحد  
                var ByQtyUnit = Invoice.Expences / totalQTY;    //السعر لكل قطعه واحده من الوحده المختاره  
                XtraDialogArgs args = new XtraDialogArgs();
                UserControls.CostDistributionOption distributionOption = new UserControls.CostDistributionOption();
                args.Caption = "";
                args.Content = distributionOption;
                ((XtraBaseArgs)args).Buttons = new DialogResult[]
                {
                DialogResult.OK
                };
                args.Showing += Args_Showing;

                XtraDialog.Show(args);
                  

                foreach (var row in items)
                {

                    if (distributionOption.SelectedOption == Master.CostDistributionOptions.ByPrice)
                        row.CostValue = (row.TotalPrice / row.ItemQty  )+ (ByPriceUnit * row.Price); // توزيع بالسعر
                    else
                        row.CostValue = (row.TotalPrice / row.ItemQty) + ( ByQtyUnit); // توزيع بالكميه 

                    row.TotalCostValue = row.ItemQty * row.CostValue;
                }
            }else
            {
                foreach (var row in items)
                {
                    row.CostValue = row.TotalPrice / row.ItemQty;
                    row.TotalCostValue = row.TotalPrice;
                }

            }
            var msg = $"   {Invoice.ID} لعميل {glkp_PartID.Text} فاتوره مبيعات رقم ";

            #region Journals


            // فاتوره بقيمه بضاعه 1000
            // ضريبه 120
            // مصروف نقل 100
            // خصم 50 

            //  1170  الصافي 


            //         حساب المخزون           - مدين - 1100
            //         حساب ضريبه مضافه       - مدين -  120   
            // حساب المورد            - دائن -         1220


            //          حساب المورد          - مدين - 50 
            //     حساب خصم نقدي مكتسب  - دائن -      50


            //        حساب المورد         -مدين - 1000  
            //   حساب الخذنه         -دائن -      1000 
            db.Journals.DeleteAllOnSubmit(db.Journals.Where(x => x.SourceType == (byte)Type && x.SourceID == Invoice.ID));
            db.SubmitChanges();
            var partAccountID = db.CustomersAndVendors.Single(x => x.ID == Invoice.PartID).AccountID ;
            var store = db.Stores.Single(x => x.ID == Invoice.Branch);
            var drawer = db.Drawers .Single(x => x.ID == Invoice.Darwer);
            db.Journals.InsertOnSubmit(new DAL.Journal() // Part
            {
                AccountID = partAccountID,
                Code = 54545,
                Credit = Invoice.Total + Invoice.TaxValue + Invoice.Expences,
                Debit = 0,
                InsertDate = Invoice.Date,
                Notes = msg,
                SourceID = Invoice.ID,
                SourceType = (byte)Type, 
            });
            db.Journals.InsertOnSubmit(new DAL.Journal() // Store Inventory
            {
                AccountID = store.InventoryAccountID ,
                Code = 54545,
                Credit = 0,
                Debit =  Invoice.Total + Invoice.Expences ,
                InsertDate = Invoice.Date,
                Notes = msg,
                SourceID = Invoice.ID,
                SourceType = (byte)Type,
            });

           if( Invoice.Tax > 0)
            db.Journals.InsertOnSubmit(new DAL.Journal() // 
            {
                AccountID = Session.Defualts .PurchaseTax ,
                Code = 54545,
                Credit = 0,
                Debit = Invoice.TaxValue  ,
                InsertDate = Invoice.Date,
                Notes = msg + " - ضريبه مضافه ",
                SourceID = Invoice.ID,
                SourceType = (byte)Type,
            });
            //if (Invoice.Expences  > 0)
            //    db.Journals.InsertOnSubmit(new DAL.Journal() // 
            //    {
            //        AccountID = Session.Defualts.PurchaseExpences,
            //        Code = 54545,
            //        Credit = 0,
            //        Debit = Invoice.Expences,
            //        InsertDate = Invoice.Date,
            //        Notes = msg + " -   مصروفات شراء",
            //        SourceID = Invoice.ID,
            //        SourceType = (byte)Type,
            //    });

            if(Invoice.DiscountValue > 0)
            {
                db.Journals.InsertOnSubmit(new DAL.Journal() // Store Tax
                {
                    AccountID = Session.Defualts.DiscountReceivedAccountID,
                    Code = 54545,
                    Credit = Invoice.DiscountValue,
                    Debit = 0,
                    InsertDate = Invoice.Date,
                    Notes = msg + " -   خصم شراء",
                    SourceID = Invoice.ID,
                    SourceType = (byte)Type,
                });
                db.Journals.InsertOnSubmit(new DAL.Journal() // Store Tax
                {
                    AccountID = partAccountID,
                    Code = 54545,
                    Credit = 0,
                    Debit = Invoice.DiscountValue,
                    InsertDate = Invoice.Date,
                    Notes = msg + " -   خصم شراء",
                    SourceID = Invoice.ID,
                    SourceType = (byte)Type,
                });

            }


            if (Invoice.Paid > 0)
            { 
                db.Journals.InsertOnSubmit(new DAL.Journal() //
                {
                    AccountID = drawer.AcoountID,
                    Code = 54545,
                    Credit = Invoice.Paid,
                    Debit = 0,
                    InsertDate = Invoice.Date,
                    Notes = msg + " - سداد",
                    SourceID = Invoice.ID,
                    SourceType = (byte)Type,
                });
                db.Journals.InsertOnSubmit(new DAL.Journal() //
                {
                    AccountID = partAccountID,
                    Code = 54545,
                    Credit = 0,
                    Debit = Invoice.Paid,
                    InsertDate = Invoice.Date,
                    Notes = msg + " - سداد",
                    SourceID = Invoice.ID,
                    SourceType = (byte)Type,
                });
            }
            #endregion

       
            foreach (var row in items)
                row.InoiceID = Invoice.ID; 
            generalDB.SubmitChanges();
            db.StoreLogs.DeleteAllOnSubmit(db.StoreLogs.Where(x => x.SourceType == (byte)Type && x.SourceID == Invoice.ID));
            db.SubmitChanges();
            if (Invoice.PostedToStore)
                foreach (var row in items)
                {
                    var unitView = Session.ProductsView.Single(x => x.ID == row.ItemID).Units.Single(x => x.UnitID == row.ItemUnitID);
                    db.StoreLogs.InsertOnSubmit(new DAL.StoreLog()
                    {
                        ProductID = row.ItemID,
                        InsertTime = Invoice.PostDate.Value,
                        SourceID = Invoice.ID,
                        SourceType = (byte)Type,
                        Notes = msg,
                        IsInTransaction = true,
                        StoreID = row.StoreID,
                        Qty = row.ItemQty * unitView.Factor,
                        CostValue = row.CostValue / unitView.Factor

                    }); 
                } 
            db.SubmitChanges();
            base.Save();
        }

        private void Args_Showing(object sender, XtraMessageShowingArgs e)
        {

            e.Form.ControlBox = false;
            e.Form.Height = 150;
            e.Buttons[DialogResult.OK].Text = "متابعه وحفظ";
        }
        public override void Print()
        {
            using (var db = new DAL.dbDataContext ())
            {
                var invoice = (from inv in db.InvoiceHeaders
                               join str in db.Stores on inv.Branch equals str.ID
                               from prt in db.CustomersAndVendors.Where(x => x.ID == inv.PartID).DefaultIfEmpty()
                               from drw in db.Drawers.Where(x => x.ID == inv.Darwer).DefaultIfEmpty()
                               where inv.ID == Invoice.ID
                               select new
                               {
                                   inv.ID,
                                   inv.Code,
                                   Store = str.Name,
                                   Drawer = drw.Name,
                                   PartName = prt.Name ,
                                   Phone = prt.Phone ,
                                   inv.Date,
                                   inv.DiscountValue,
                                   inv.Expences,
                                   InvoiceType =
                                   (inv.InvoiceType == (byte)Master.InvoiceType.Purchase) ? "فاتوره مشتريات " :
                                   (inv.InvoiceType == (byte)Master.InvoiceType.Sales) ? "فاتوره مبيعات " :
                                   (inv.InvoiceType == (byte)Master.InvoiceType.SalesReturn) ? "فاتوره مردود مبيعات " :
                                   (inv.InvoiceType == (byte)Master.InvoiceType.PurchaseReturn) ? "فاتوره مردود مشتريات " : "UnDefined",
                                   inv.Net,
                                   inv.Notes,
                                   inv.Paid,
                                   PartType =
                                   (inv.PartType == (byte)Master.PartType.Customer) ? "عميل" :
                                   (inv.PartType == (byte)Master.PartType.Vendor) ? "مورد" : "UnDefined",
                                   inv.Remaing,
                                   inv.TaxValue,
                                   inv.Total,
                                   ProductCount = db.InvoiceDetails.Where(x => x.InoiceID == inv.ID).Count(),
                                   Products = (
                                  from d in db.InvoiceDetails.Where(x => x.InoiceID == inv.ID)
                                  from p in db.Products.Where(x => x.ID == d.ItemID)
                                  from u in db.UnitNames.Where(x => x.ID == d.ItemUnitID).DefaultIfEmpty()
                                  select new
                                  {
                                      ProductName = p.Name,
                                      UnitName = u.Name,
                                      d.ItemQty,
                                      d.Price,
                                      d.TotalPrice,
                                  }).ToList()
                               }).ToList();

               Reporting.rpt_Invoice.Print(invoice);
            }
            base.Print();   
        }
    }
}
