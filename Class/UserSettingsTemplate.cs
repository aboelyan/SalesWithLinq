using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesWithLinq.Class
{
    public   class UserSettingsTemplate
    {
        int ProfileID { get; set; }
        public UserSettingsTemplate (int profileid)
        {
            ProfileID = profileid;
            General = new GeneralSettings(ProfileID);
            Invoices = new InvoicesSettings(ProfileID);
            Sales = new SalesSettings(ProfileID);
            Purchase = new PurchaseSettings(ProfileID);
        }
        public GeneralSettings General { get; set; }
        public InvoicesSettings   Invoices { get; set; }
        public SalesSettings   Sales { get; set; }
        public PurchaseSettings   Purchase { get; set; }

        public static string GetPropCaption(string propName)
        {
            UserSettingsTemplate ins;
            switch (propName )
            {
                case nameof(ins.General ):return "اعدادات عامه";
                case nameof(ins.Invoices): return "اعدادات الفواتير";
                case nameof(ins.Sales): return "اعدادات البيع";
                case nameof(ins.Purchase): return "اعدادات الشراء";
                case nameof(ins.General.DefualtRawStore): return "مخزن الافتراضي للخامات";
                case nameof(ins.General.DefaultStore): return "المخزن الافتراضي";
                case nameof(ins.General.DefualtDrawer): return "الخزينه الافتراضيه";
                case nameof(ins.General.DefualtCustomer): return "العميل الافتراضي";
                case nameof(ins.General.DefaultVendor): return "المورد الافتراضي";
                case nameof(ins.General.CanChangeStore): return "السماح بتغيير المخزن";
                case nameof(ins.General.CanChangeDrawer): return "السماح بتغيير الخزنه";
                case nameof(ins.General.CanChangeCustomer): return "السماح بتغيير العميل";
                case nameof(ins.General.CanChangeVendor): return "السماح بتغيير المورد";
                case nameof(ins.General.CanViewDocumentHistory): return "بامكانه عرض سجل التغييرات";
                case nameof(ins.Invoices.CanChangeTax): return "السماح بتغيير الضريبه";
                case nameof(ins.Invoices.CanDeleteItemsInInvoices): return "السماح بحذف الاصناف من الفاتوره";
                case nameof(ins.Sales .CanChangePaidInSales ): return "السماح بتغيير المبلغ المدفوع";
                case nameof(ins.Sales .CanNotPostToStoreInSales): return "انشاء فواتير بدون صرف ";
                case nameof(ins.Sales.DefaultPayMethodInSales): return "نوع السداد الافتراضي";
                case nameof(ins.Sales.WhenSellingToACustomerExeededMaxCredit): return "عند البيع لعميل تجاوز حد الائتمان";
                case nameof(ins.Sales.CanChangeItemPriceInSales): return "السماح بتغيير سعر الصنف";
                case nameof(ins.Invoices .WhenSellingItemReachedReOrderLevel): return "عند صرف صنف وصل رصيده الي حد الطلب ";
                case nameof(ins.Invoices.WhenSellingItemWithQtyMoreThanAvailableQty): return "عند صرف كميه من صنف اكثر من المتاح";
                case nameof(ins.Sales  .WhenSellingItemWithPriceLowerThanCostPrice): return "عند البيع بسعر اقل من سعر التكلفه";
                case nameof(ins.Sales .MaxDiscountInInvoice): return "اقصي خصم مسموح للفاتوره";
                case nameof(ins.Sales.MaxDiscountPerItem): return "اقصي خصم مسموح للصنف ";
                case nameof(ins.Sales.CanSellToVendors): return "السماح بالبيع للموردين";
                case nameof(ins.Sales.CanChangeSalesInvoiceDate): return "السماح بتغيير التاريخ";
                case nameof(ins.Sales.CanChangeQtyInSales): return "السماح بتغيير الكميه";
                case nameof(ins.Sales.HideCostInSales): return "اخفاء التكلفه من الفاتوره";
                case nameof(ins.Purchase .CanChangeItemPriceInPurchase): return "السماح بتغيير سعر الشراء";
                case nameof(ins.Purchase.CanBuyFromCustomers): return "السماح بالشراء من العملاء";
                case nameof(ins.Purchase.CanChangePurchaseInvoiceDate): return "السماح بتغيير التاريخ";
                default: return  "$" + propName + "$";
            }

        }
        public static BaseEdit GetPropertyControl(string propName , object propertyValue)
        {
            UserSettingsTemplate ins;
            BaseEdit edit = null;
            switch (propName)
            {
                 
                case nameof(ins.General .CanChangeStore):
                case nameof(ins.General.CanChangeDrawer):
                case nameof(ins.General.CanChangeCustomer):
                case nameof(ins.General.CanChangeVendor):
                case nameof(ins.General.CanViewDocumentHistory ):
                case nameof(ins.Invoices .CanChangeTax):
                case nameof(ins.Invoices .CanDeleteItemsInInvoices):
                case nameof(ins.Sales .CanChangePaidInSales):
                case nameof(ins.Sales .CanNotPostToStoreInSales):
                case nameof(ins.Sales.CanChangeItemPriceInSales):
                case nameof(ins.Sales.CanSellToVendors):
                case nameof(ins.Sales.CanChangeSalesInvoiceDate):
                case nameof(ins.Sales.CanChangeQtyInSales):
                case nameof(ins.Sales.HideCostInSales):
                case nameof(ins.Purchase.CanChangeItemPriceInPurchase):
                case nameof(ins.Purchase.CanBuyFromCustomers):
                case nameof(ins.Purchase.CanChangePurchaseInvoiceDate):
                    edit = new ToggleSwitch ();
                    ((ToggleSwitch)edit).Properties.OnText = "نعم";
                    ((ToggleSwitch)edit).Properties.OffText = "لا";
                    break;

                case nameof(ins.General.DefaultStore ):
                case nameof(ins.General.DefualtRawStore ):
                    edit = new LookUpEdit ();
                    ((LookUpEdit)edit).IntializeData(Session.Store); 
                    break;
                case nameof(ins.General.DefualtDrawer):
                    edit = new LookUpEdit();
                    ((LookUpEdit)edit).IntializeData(Session.Drawer ); 
                    break;
                case nameof(ins.General.DefualtCustomer ):
                    edit = new LookUpEdit();
                    ((LookUpEdit)edit).IntializeData(Session.Customers );
                    break;
                case nameof(ins.General.DefaultVendor ):
                    edit = new LookUpEdit();
                    ((LookUpEdit)edit).IntializeData(Session.Vendors);
                    break;
                case nameof(ins.Sales .DefaultPayMethodInSales):
                    edit = new LookUpEdit();
                    ((LookUpEdit)edit).IntializeData(Master.PayMethodsList);
                    break;
                case nameof(ins.Sales.WhenSellingToACustomerExeededMaxCredit):
                case nameof(ins.Invoices  .WhenSellingItemReachedReOrderLevel):
                case nameof(ins.Invoices.WhenSellingItemWithQtyMoreThanAvailableQty):
                    edit = new LookUpEdit();
                    ((LookUpEdit)edit).IntializeData(Master.WarningLevelsList);
                    break;
                case nameof(ins.Sales.MaxDiscountInInvoice):
                case nameof(ins.Sales.MaxDiscountPerItem):
                    edit = new SpinEdit ();
                     ((SpinEdit)edit).Properties  .Increment = 0.01m;
                     ((SpinEdit)edit).Properties.Mask.EditMask = "p";
                     ((SpinEdit)edit).Properties.Mask.UseMaskAsDisplayFormat = true;
                     ((SpinEdit)edit).Properties.MaxValue = 1;
                    
                    break;
                default:
                    break;
            }

            if(edit != null)
            {
                edit.Name = propName;
                edit.Properties.NullText = "";
                edit.EditValue = propertyValue;
            }
            return edit;
        }
    }
    public class GeneralSettings
    {
        int ProfileID { get; set; }
        public GeneralSettings (int profileid)
        {
            ProfileID = profileid; 
        }
        public int DefualtRawStore { get { return Master.FromByteArray<int>(Master.GetPropertyValue(Master.GetCallerName(), ProfileID)); } }
        public int DefaultStore { get { return Master.FromByteArray<int>(Master.GetPropertyValue(Master.GetCallerName(), ProfileID)); } }
        public int DefualtDrawer { get { return Master.FromByteArray<int>(Master.GetPropertyValue(Master.GetCallerName(), ProfileID)); } }
        public int DefualtCustomer { get { return Master.FromByteArray<int>(Master.GetPropertyValue(Master.GetCallerName(), ProfileID)); } }
        public int DefaultVendor { get { return Master.FromByteArray<int>(Master.GetPropertyValue(Master.GetCallerName(), ProfileID)); } }
        public bool CanChangeStore { get { return Master.FromByteArray<bool>(Master.GetPropertyValue(Master.GetCallerName(), ProfileID)); } }
        public bool CanChangeDrawer { get { return Master.FromByteArray<bool>(Master.GetPropertyValue(Master.GetCallerName(), ProfileID)); } }
        public bool CanChangeCustomer { get { return Master.FromByteArray<bool>(Master.GetPropertyValue(Master.GetCallerName(), ProfileID)); } }
        public bool CanChangeVendor { get { return Master.FromByteArray<bool>(Master.GetPropertyValue(Master.GetCallerName(), ProfileID)); } }
        public bool CanViewDocumentHistory { get { return Master.FromByteArray<bool>(Master.GetPropertyValue(Master.GetCallerName(), ProfileID)); } }

    }
    public class InvoicesSettings
    {
        int ProfileID { get; set; }
        public InvoicesSettings(int profileid)
        {
            ProfileID = profileid;
        }
        public bool CanChangeTax { get { return Master.FromByteArray<bool>(Master.GetPropertyValue(Master.GetCallerName(), ProfileID)); } }
        public bool CanDeleteItemsInInvoices { get { return Master.FromByteArray<bool>(Master.GetPropertyValue(Master.GetCallerName(), ProfileID)); } }
        public Master.WarningLevels WhenSellingItemReachedReOrderLevel { get { return Master.FromByteArray<Master.WarningLevels>(Master.GetPropertyValue(Master.GetCallerName(), ProfileID)); } }
        public Master.WarningLevels WhenSellingItemWithQtyMoreThanAvailableQty { get { return Master.FromByteArray<Master.WarningLevels>(Master.GetPropertyValue(Master.GetCallerName(), ProfileID)); } }

    }

    public class SalesSettings
    {
        int ProfileID { get; set; }
        public SalesSettings(int profileid)
        {
            ProfileID = profileid;

        }
        public bool CanChangePaidInSales { get { return Master.FromByteArray<bool>(Master.GetPropertyValue(Master.GetCallerName(), ProfileID)); } }
        public bool CanNotPostToStoreInSales { get { return Master.FromByteArray<bool>(Master.GetPropertyValue(Master.GetCallerName(), ProfileID)); } }
        public Master.PayMethods DefaultPayMethodInSales { get { return Master.FromByteArray<Master.PayMethods>(Master.GetPropertyValue(Master.GetCallerName(), ProfileID)); } }
        public Master.WarningLevels WhenSellingToACustomerExeededMaxCredit { get { return Master.FromByteArray<Master.WarningLevels>(Master.GetPropertyValue(Master.GetCallerName(), ProfileID)); } }
        public bool CanChangeItemPriceInSales { get { return Master.FromByteArray<bool>(Master.GetPropertyValue(Master.GetCallerName(), ProfileID)); } }
          public Master.WarningLevels WhenSellingItemWithPriceLowerThanCostPrice { get { return Master.FromByteArray<Master.WarningLevels>(Master.GetPropertyValue(Master.GetCallerName(), ProfileID)); } }
        public decimal  MaxDiscountInInvoice { get { return Master.FromByteArray<decimal>(Master.GetPropertyValue(Master.GetCallerName(), ProfileID)); } }
        public decimal MaxDiscountPerItem { get { return Master.FromByteArray<decimal>(Master.GetPropertyValue(Master.GetCallerName(), ProfileID)); } }
        public bool CanSellToVendors { get { return Master.FromByteArray<bool>(Master.GetPropertyValue(Master.GetCallerName(), ProfileID)); } }
        public bool CanChangeSalesInvoiceDate { get { return Master.FromByteArray<bool>(Master.GetPropertyValue(Master.GetCallerName(), ProfileID)); } }
        public bool CanChangeQtyInSales { get { return Master.FromByteArray<bool>(Master.GetPropertyValue(Master.GetCallerName(), ProfileID)); } }
        public bool HideCostInSales { get { return Master.FromByteArray<bool>(Master.GetPropertyValue(Master.GetCallerName(), ProfileID)); } }

    }
    public class PurchaseSettings
    {
        int ProfileID { get; set; }
        public PurchaseSettings(int profileid)
        {
            ProfileID = profileid;

        }
        public bool CanChangeItemPriceInPurchase { get; set; }
        public bool CanBuyFromCustomers { get; set; }
        public bool CanChangePurchaseInvoiceDate { get; set; }

    }

    }
