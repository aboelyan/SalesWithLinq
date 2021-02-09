using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using SalesWithLinq.Class;

namespace SalesWithLinq.Reporting
{
    public partial class rpt_Invoice : DevExpress.XtraReports.UI.XtraReport
    {
        Master.InvoiceType Type;
        public rpt_Invoice()
        {
            InitializeComponent();

            lbl_CompanyName.Text = Session.CompanyInfo.CompanyName;
            lbl_CompanyAddress.Text = Session.CompanyInfo.Address;
            lbl_CompanyPhone.Text = Session.CompanyInfo.Phone;
        }
        void BindData()
        {
            lbl_Code.DataBindings.Add("Text", this.DataSource, "Code");
            xrBarCode1.DataBindings.Add("Text", this.DataSource, "ID");
            xrTableCell14.DataBindings.Add("Text", this.DataSource, "Store");
            lbl_Date .DataBindings.Add("Text", this.DataSource, "Date");
            lbl_Discount.DataBindings.Add("Text", this.DataSource, "DiscountValue");
            lbl_Expences.DataBindings.Add("Text", this.DataSource, "Expences");
            lbl_InvoiceType.DataBindings.Add("Text", this.DataSource, "InvoiceType");
            lbl_Net.DataBindings.Add("Text", this.DataSource, "Net");
            lbl_Paid.DataBindings.Add("Text", this.DataSource, "Paid");
            lbl_PartName.DataBindings.Add("Text", this.DataSource, "PartName");
            lbl_PartType.DataBindings.Add("Text", this.DataSource, "PartType");
            xrTableCell11.DataBindings.Add("Text", this.DataSource, "Phone");
            lbl_Qty.DataBindings.Add("Text", this.DataSource, "ProductCount");
            lbl_Remining.DataBindings.Add("Text", this.DataSource, "Remaing");
            lbl_Tax.DataBindings.Add("Text", this.DataSource, "TaxValue");
            lbl_Total.DataBindings.Add("Text", this.DataSource, "Total");
            // lbl_NetText.DataBindings.Add("Text", this.DataSource, "InvoiceType");
            cell_Price.ExpressionBindings.Add(new ExpressionBinding("BeforePrint", "Text", "Price"));
            cell_Product.ExpressionBindings.Add(new ExpressionBinding("BeforePrint", "Text", "ProductName"));
            cell_Qty.ExpressionBindings.Add(new ExpressionBinding("BeforePrint", "Text", "ItemQty"));
            cell_Unit.ExpressionBindings.Add(new ExpressionBinding("BeforePrint", "Text", "UnitName"));
            Cell_Total.ExpressionBindings.Add(new ExpressionBinding("BeforePrint", "Text", "TotalPrice"));
  
        }
        /*
              select new
                                  {
                                      ProductName = p.Name,
                                      UnitName = u.Name,
                                      d.ItemQty,
                                      d.Price,
                                      d.TotalPrice,
                                  }).ToList()
                               }).FirstOrDefault();
             */
        public static void Print(object ds)
        {
            Reporting.rpt_Invoice rpt = new Reporting.rpt_Invoice();
            rpt.DataSource = ds;
            rpt.DetailReport.DataSource = rpt.DataSource;
            rpt.DetailReport.DataMember = "Products";
            rpt.BindData();

            switch (Session.CurrentSettings.InvoicePrintMode)
            {
                case Master.PrintMode.Direct: 
                    rpt.Print();
                    break;
                case Master.PrintMode.ShowPreview:
                    rpt.ShowPreview();  
                    break;
                case Master.PrintMode.ShowDialog:
                    rpt.PrintDialog ();
                    break;
                default:
                    break;
            } 
        }

        private void lbl_NetText_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            lbl_NetText.Text = NumberToText.Utils.ConvertMoneyToArabicText(lbl_Net.Text);
        }
        int index = 1;
        private void cell_Index_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            cell_Index.Text = (index++).ToString();
        }
    }
}
