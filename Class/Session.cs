using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SalesWithLinq.Class.DataBaseWatcher;
using static SalesWithLinq.Class.Master;

namespace SalesWithLinq.Class
{
    public static class Session
    {

        //Test

        public static class Defualts
        {
            public static int Drawer { get => 1; }
            public static int Customer { get => 1; }
            public static int Vendor { get => 2; }
            public static int Store { get => 1003; }
            public static int RawStore { get => 1003; }
            public static int DiscountAllowedAccountID { get => 1010; }
            public static int DiscountReceivedAccountID { get => 1009; }
            public static int SalesTax { get => 1019; }
            public static int PurchaseTax { get => 1020; }
            public static int PurchaseExpences { get => 1021; }
        }

        private static DAL.CompanyInfo companyInfo;
        public static DAL.CompanyInfo CompanyInfo
        {
            get
            {
                if (companyInfo == null)
                {
                    using (var db = new DAL.dbDataContext())
                    {
                        companyInfo = db.CompanyInfos.First();
                    }
                }
                return companyInfo;

            }
        }
        public static class CurrentSettings
        {
            public static PrintMode InvoicePrintMode { get => PrintMode.ShowPreview; }


        }
        public static UserSettingsTemplate _userSettings;
        public static UserSettingsTemplate UserSettings
        {
            get
            {

                if (_userSettings == null)
                    _userSettings = new UserSettingsTemplate(User.SettingsProfileID);
                return _userSettings;

            }
        }
        public static class GlobalSettings
        {
            public static Boolean ReadFormScaleBarcode { get => true; }

            public static string ScaleBarcodePrefix { get => "21"; }
            public static byte ProductCodeLength { get => 5; }
            public static byte BarcodeLength { get => 13; }
            public static byte ValueCodeLength { get => 5; }
            public static ReadValueMode ReadMode { get => ReadValueMode.Price; }
            public static Boolean IgnoreCheckDigit { get => true; }
            public static byte DivideValueBy { get => 2; }

            public enum ReadValueMode
            {
                Weight,
                Price,
            }
        }
        private static BindingList<DAL.UnitName> unitNames;
        public static BindingList<DAL.UnitName> UnitNames
        {
            get
            {

                if (unitNames == null)
                {
                    using (var db = new DAL.dbDataContext())
                    {
                        unitNames = new BindingList<DAL.UnitName>(db.UnitNames.ToList());
                    }
                }
                return unitNames;
            }

        }

        private static BindingList<DAL.Product> _products;
        public static BindingList<DAL.Product> Products
        {
            get
            {

                if (_products == null)
                {

                    using (var db = new DAL.dbDataContext())
                    {
                        _products = new BindingList<DAL.Product>(db.Products.ToList());
                    }
                    DataBaseWatcher.Products = new TableDependency.SqlClient.SqlTableDependency<DAL.Product>(Properties.Settings.Default.SalesDBConnectionString);
                    DataBaseWatcher.Products.OnChanged += DataBaseWatcher.Products_Changed;
                    DataBaseWatcher.Products.Start();
                }
                return _products;
            }
        }


        private static BindingList<ProductViewClass> productViewClasses;
        public static BindingList<ProductViewClass> ProductsView
        {
            get
            {
                if (productViewClasses == null)
                {
                    using (var db = new DAL.dbDataContext())
                    {
                        var data = from pr in Session.Products
                                   join cg in db.ProductCategories on pr.CategoryID equals cg.ID
                                   select new ProductViewClass
                                   {
                                       ID = pr.ID,
                                       Code = pr.Code,
                                       Name = pr.Name,
                                       CategoryName = cg.Name,
                                       Descreption = pr.Descreption,
                                       IsActive = pr.IsActive,
                                       Type = pr.Type,
                                       Units = (from u in db.ProductUnits
                                                where u.ProductID == pr.ID
                                                join un in db.UnitNames on u.UnitID equals un.ID
                                                select new ProductViewClass.ProductUOMView
                                                {
                                                    UnitID = u.UnitID,
                                                    UnitName = un.Name,
                                                    Factor = u.Factor,
                                                    SellPrice = u.SellPrice,
                                                    BuyPrice = u.BuyPrice,
                                                    Barcode = u.Barcode,
                                                }).ToList()
                                   };
                        productViewClasses = new BindingList<ProductViewClass>(data.ToList());
                    }
                }
                return productViewClasses;
            }
        }

        public class ProductViewClass
        {
            public static ProductViewClass GetProduct(int id)
            {
                ProductViewClass obj;
                using (var db = new DAL.dbDataContext())
                {
                    var data = from pr in Session.Products
                               where pr.ID == id
                               join cg in db.ProductCategories on pr.CategoryID equals cg.ID
                               select new ProductViewClass
                               {
                                   ID = pr.ID,
                                   Code = pr.Code,
                                   Name = pr.Name,
                                   CategoryName = cg.Name,
                                   Descreption = pr.Descreption,
                                   IsActive = pr.IsActive,
                                   Type = pr.Type,
                                   Units = (from u in db.ProductUnits
                                            where u.ProductID == pr.ID
                                            join un in db.UnitNames on u.UnitID equals un.ID
                                            select new ProductViewClass.ProductUOMView
                                            {
                                                UnitID = u.UnitID,
                                                UnitName = un.Name,
                                                Factor = u.Factor,
                                                SellPrice = u.SellPrice,
                                                BuyPrice = u.BuyPrice,
                                                Barcode = u.Barcode,
                                            }).ToList()
                               };
                    obj = data.First();
                };
                return obj;
            }
            public int ID { get; set; }
            public string Code { get; set; }
            public string Name { get; set; }
            public string CategoryName { get; set; }
            public string Descreption { get; set; }
            public Boolean IsActive { get; set; }
            public byte Type { get; set; }
            public List<ProductUOMView> Units { get; set; }
            public class ProductUOMView
            {
                public int UnitID { get; set; }
                public string UnitName { get; set; }
                public Double Factor { get; set; }
                public Double SellPrice { get; set; }
                public Double BuyPrice { get; set; }
                public string Barcode { get; set; }
            }
        }

        private static BindingList<DAL.CustomersAndVendor> _vendors;
        public static BindingList<DAL.CustomersAndVendor> Vendors
        {
            get
            {
                if (_vendors == null)
                {
                    using (var db = new DAL.dbDataContext())
                    {
                        _vendors = new BindingList<DAL.CustomersAndVendor>(db.CustomersAndVendors.Where(x => x.IsCustomer == false).ToList());
                    }
                    DataBaseWatcher.Vendors =
                        new TableDependency.SqlClient.SqlTableDependency<CustomersAndVendors>(Properties.Settings.Default.SalesDBConnectionString,
                        filter: new DataBaseWatcher.VendorsOnly());
                    DataBaseWatcher.Vendors.OnChanged += DataBaseWatcher.VendorsChanged;
                    DataBaseWatcher.Vendors.Start();
                }
                return _vendors;
            }
        }

        private static BindingList<DAL.Drawer> _drawer;
        public static BindingList<DAL.Drawer> Drawer
        {
            get
            {
                if (_drawer == null)
                {
                    using (var db = new DAL.dbDataContext())
                    {
                        _drawer = new BindingList<DAL.Drawer>(db.Drawers.ToList());
                    }
                }
                return _drawer;
            }
        }

        private static BindingList<DAL.Store> _store;
        public static BindingList<DAL.Store> Store
        {
            get
            {
                if (_store == null)
                {
                    using (var db = new DAL.dbDataContext())
                    {
                        _store = new BindingList<DAL.Store>(db.Stores.ToList());
                    }
                }
                return _store;
            }
        }

        //private static BindingList<DAL.UserSettingsProfileProperty > _profileProperties;
        //public static BindingList<DAL.UserSettingsProfileProperty> ProfileProperties
        //{
        //    get
        //    {
        //    
        //        if (_profileProperties == null)
        //        {
        //            using (var db = new DAL.dbDataContext())
        //            {
        //                _profileProperties = new BindingList<DAL.UserSettingsProfileProperty>(db.UserSettingsProfileProperties .ToList());
        //            }
        //        }
        //        return _profileProperties;
        //    }
        //}
        private static DAL.User _user;
        public static DAL.User User { get => _user; }

        public static void SetUser(DAL.User user)
        {
            _user = user;
            using (DAL.dbDataContext db = new DAL.dbDataContext())
            {
                _screensAccesses = (from s in Class.Screens.GetScreens
                                    from d in db.UserAccessProfileDetails
                                    .Where(x => x.ProfileID == user.ScreenProfileID && x.ScreenID == s.ScreenID).DefaultIfEmpty()
                                    select new Class.ScreensAccessProfile(s.ScreenName)
                                    {
                                        CanAdd = (d == null) ? true : d.CanAdd,
                                        CanDelete = (d == null) ? true : d.CanDelete,
                                        CanEdit = (d == null) ? true : d.CanEdit,
                                        CanOpen = (d == null) ? true : d.CanOpen,
                                        CanPrint = (d == null) ? true : d.CanPrint,
                                        CanShow = (d == null) ? true : d.CanShow,
                                        Actions = s.Actions,
                                        ScreenName = s.ScreenName,
                                        ScreenCaption = s.ScreenCaption,
                                        ScreenID = s.ScreenID,
                                        ParentScreenID = s.ParentScreenID,
                                    }).ToList();
            }
        }
        private static BindingList<DAL.CustomersAndVendor> _customer;
        public static BindingList<DAL.CustomersAndVendor> Customers
        {
            get
            {
                if (_customer == null)
                {
                    using (var db = new DAL.dbDataContext())
                    {
                        _customer = new BindingList<DAL.CustomersAndVendor>(db.CustomersAndVendors.Where(x => x.IsCustomer == true).ToList());
                    }

                    DataBaseWatcher.Customers =
                        new TableDependency.SqlClient.SqlTableDependency<CustomersAndVendors>(Properties.Settings.Default.SalesDBConnectionString,
                        filter: new DataBaseWatcher.CustomersOnly());
                    DataBaseWatcher.Customers.OnChanged += DataBaseWatcher.CustomerChanged;
                    DataBaseWatcher.Customers.Start();
                }
                return _customer;
            }
        }

        private static List<ScreensAccessProfile> _screensAccesses;
        public static List<ScreensAccessProfile> ScreensAccesses
        {

            get {

                //if (User.UserType == (byte)Master.UserType.Admin)
                //    return Screens.GetScreens;
                //else
                    return _screensAccesses;
            }


        }
    }
}
