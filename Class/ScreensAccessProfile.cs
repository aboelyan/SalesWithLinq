using SalesWithLinq.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SalesWithLinq.Class
{
   public  class ScreensAccessProfile
    {
        public static int MaxID = 1;
      
        public ScreensAccessProfile(string name , ScreensAccessProfile parent = null  )
        {
            ScreenName = name;
            ScreenID = MaxID++;
            if (parent != null)
                ParentScreenID = parent.ScreenID;
            else ParentScreenID = 0;
            Actions = new List<Master.Actions>() {
                Master.Actions.Add ,
                Master.Actions.Edit ,
                Master.Actions.Delete ,
                Master.Actions.Print ,
                Master.Actions.Show ,
                Master.Actions.Open  ,
            };
        }

        public int ScreenID { get; set; }
        public int ParentScreenID { get; set; }
        public string  ScreenName { get; set; }
        public string ScreenCaption { get; set; }
        public bool  CanShow { get; set; }
        public bool CanOpen { get; set; }
        public bool CanAdd { get; set; }
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }
        public bool CanPrint { get; set; } 
        public List<Master.Actions> Actions { get; set; } 
    }
    public static class Screens
    {
        public static ScreensAccessProfile mainSettings = new ScreensAccessProfile("elm_MainSettings")
        {
            Actions = new List<Master.Actions>() { Master.Actions.Show }, 
            ScreenCaption = "البيانات"
        };
        public static ScreensAccessProfile CompanyInfo = new ScreensAccessProfile(nameof(frm_CompanyInfo), mainSettings) 
        {
            Actions = new List<Master.Actions>() { Master.Actions.Show , Master.Actions.Edit , Master.Actions.Open },
            ScreenCaption = "بيانات الشركه" };

        public static ScreensAccessProfile Customers = new ScreensAccessProfile("elm_Customers")
        {
            Actions = new List<Master.Actions>() { Master.Actions.Show },
            ScreenCaption = "العملاء" 
        };
        public static ScreensAccessProfile AddCustomer = new ScreensAccessProfile("frm_Customer",Customers)
        { ScreenCaption = "اضافه عميل" };
        public static ScreensAccessProfile ViewCustomer = new ScreensAccessProfile("frm_CustomerList", Customers)
        { ScreenCaption = "عرض العملاء " };

        public static ScreensAccessProfile Vendors = new ScreensAccessProfile("elm_Vendors")
        {
            Actions = new List<Master.Actions>() { Master.Actions.Show },
            ScreenCaption = "الموردين"
        };
        public static ScreensAccessProfile AddVendor = new ScreensAccessProfile("frm_Vendor", Vendors)
        { ScreenCaption = "اضافه مورد" };
        public static ScreensAccessProfile ViewVendors = new ScreensAccessProfile("frm_VendorList", Vendors)
        { ScreenCaption = "عرض الموردين" };

        public static ScreensAccessProfile Stores = new ScreensAccessProfile("elm_Stores")
        {
            Actions = new List<Master.Actions>() { Master.Actions.Show  },
            ScreenCaption = "الافرع"
        };
        public static ScreensAccessProfile AddStore = new ScreensAccessProfile(nameof(frm_Stores ), Stores)
        { ScreenCaption = "اضافه مخزن" };
        public static ScreensAccessProfile ViewCustomers = new ScreensAccessProfile(nameof(frm_StoresList ), Stores)
        { ScreenCaption = "عرض المخازن" };

        public static ScreensAccessProfile Drawers = new ScreensAccessProfile("elm_Drawers")
        {
            Actions = new List<Master.Actions>() { Master.Actions.Show },
            ScreenCaption = "الخزن"
        };
        public static ScreensAccessProfile AddDrawer = new ScreensAccessProfile(nameof(frm_Drawer), Drawers)
        { ScreenCaption = "اضافه خزنه" };
        public static ScreensAccessProfile ViewDrawers = new ScreensAccessProfile(nameof(frm_DrawerList), Drawers)
        { ScreenCaption = "عرض الخزن" };

        public static ScreensAccessProfile Products = new ScreensAccessProfile("elm_Product")
        {
            Actions = new List<Master.Actions>() { Master.Actions.Show },
            ScreenCaption = "الاصناف"
        };
        public static ScreensAccessProfile AddProduct = new ScreensAccessProfile(nameof(frm_Product), Products)
        { ScreenCaption = "اضافه صنف" };
        public static ScreensAccessProfile ViewProducts = new ScreensAccessProfile(nameof(frm_ProductList ), Products)
        { ScreenCaption = "عرض الاصناف" };

        public static ScreensAccessProfile Purchases = new ScreensAccessProfile("elm_Purchases")
        {
            Actions = new List<Master.Actions>() { Master.Actions.Show },
            ScreenCaption = "المشتريات"
        };
        public static ScreensAccessProfile AddPurchaseInvoice = new ScreensAccessProfile("frm_PurchaseInvoice", Purchases)
        { ScreenCaption = "اضافه فاتوره مشتريات" };

        public static ScreensAccessProfile Settings = new ScreensAccessProfile("elm_Settings")
        {
            Actions = new List<Master.Actions>() { Master.Actions.Show },
            ScreenCaption = "الاعدادت"
        };
        public static ScreensAccessProfile AddUserSettingProfile = new ScreensAccessProfile(nameof(frm_UserSettingsProfile ), Settings)
        { ScreenCaption = "اضافه نموذج اعدادات" };
        public static ScreensAccessProfile ViweUserSettingProfile = new ScreensAccessProfile(nameof(frm_UserSettingsProfileList), Settings)
        { ScreenCaption = "عرض نماذج الاعدادت" };

        public static ScreensAccessProfile AddAccessProfile = new ScreensAccessProfile(nameof(frm_AccessProfile ), Settings)
        { ScreenCaption = "اضافه نموذج صلاحيه وصول" };
        public static ScreensAccessProfile ViweAccessProfile = new ScreensAccessProfile(nameof(frm_AccessProfileList ), Settings)
        { ScreenCaption = "عرض نماذج صلاحيات الوصول" };
        public static ScreensAccessProfile AddUser = new ScreensAccessProfile(nameof(frm_User ), Settings)
        { ScreenCaption = "اضافه مستخدم" };
        public static ScreensAccessProfile ViewUsers = new ScreensAccessProfile(nameof(frm_UserList), Settings)
        { ScreenCaption = "عرض المستخدمين" };


        public static List<ScreensAccessProfile> GetScreens { 
        
            get
            {
                Type t = typeof(Screens);
                FieldInfo[] fields = t.GetFields(BindingFlags.Public | BindingFlags.Static);

                var list = new List<ScreensAccessProfile>();
                foreach (var item in fields )
                {
                    var obj = item.GetValue(null);
                    if(obj != null && obj.GetType() == typeof(ScreensAccessProfile ))
                    list.Add((ScreensAccessProfile )obj);
                }

                return list;

            }
        }

    }
}
