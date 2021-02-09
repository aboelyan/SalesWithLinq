using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TableDependency.SqlClient;
using TableDependency.SqlClient.Base.Abstracts;
using TableDependency.SqlClient.Base.Enums;
using TableDependency.SqlClient.Base.EventArgs;
 
namespace SalesWithLinq.Class
{
   public static  class DataBaseWatcher
    {
        public static SqlTableDependency<DAL.Product> Products;

        public  static void Products_Changed(object sender , RecordChangedEventArgs<DAL.Product > e)
        {

            Application.OpenForms[0].Invoke(new Action(() =>
            {
                if (e.ChangeType == ChangeType.Insert)
                {
                    Session.Products.Add(e.Entity);
                    Session.ProductsView.Add(  Session.ProductViewClass.GetProduct(e.Entity.ID));
                }
                else if (e.ChangeType == ChangeType.Update)
                {
                    var index = Session.Products.IndexOf(Session.Products.Single(x => x.ID == e.Entity.ID));
                    Session.Products.Remove(Session.Products.Single(x => x.ID == e.Entity.ID));
                    Session.Products.Insert  (index, e.Entity);

                    var viewIndex = Session.ProductsView .IndexOf(Session.ProductsView .Single(x => x.ID == e.Entity.ID));
                    Session.ProductsView.Remove(Session.ProductsView.Single(x => x.ID == e.Entity.ID));
                    Session.ProductsView.Add(Session.ProductViewClass.GetProduct(e.Entity.ID));

                }
                else if (e.ChangeType == ChangeType.Delete )
                {
                    Session.Products.Remove(Session.Products.Single(x => x.ID == e.Entity.ID));
                    Session.ProductsView.Remove(Session.ProductsView.Single(x => x.ID == e.Entity.ID));
                }
            } ));  
        }


        public class CustomersAndVendors : DAL.CustomersAndVendor { }
        public static SqlTableDependency<CustomersAndVendors> Vendors;
        public static  void VendorsChanged( object sender , RecordChangedEventArgs<CustomersAndVendors> e )
        {
            Application.OpenForms[0].Invoke(new Action(() =>
            {
                switch (e.ChangeType)
                {
                    case ChangeType.None:
                        break;
                    case ChangeType.Delete:
                        Session.Vendors.Remove(Session.Vendors.Single(x=>x.ID == e.Entity.ID ));
                        break;
                    case ChangeType.Insert:
                        Session.Vendors.Add(e.Entity);
                        break;
                    case ChangeType.Update:
                        int index = Session.Vendors.IndexOf(Session.Vendors.Single(x => x.ID == e.Entity.ID));
                        Session.Vendors.Remove(Session.Vendors.Single(x => x.ID == e.Entity.ID));
                        Session.Vendors.Add(e.Entity);
                        break;
                    default:
                        break;
                }

            })); 
        }
        public class VendorsOnly : ITableDependencyFilter
        {
            public string Translate()
            {
                return "[IsCustomer] = 0";
            }
        }

        ////////////////////////
        public static SqlTableDependency<CustomersAndVendors> Customers;
        public static void CustomerChanged(object sender, RecordChangedEventArgs<CustomersAndVendors> e)
        {
            
            Application.OpenForms[0].Invoke(new Action(() =>
            {
                switch (e.ChangeType)
                {
                    case ChangeType.None:
                        break;
                    case ChangeType.Delete:
                        Session.Customers.Remove(Session.Customers.Single(x => x.ID == e.Entity.ID));
                        break;
                    case ChangeType.Insert:
                        Session.Customers.Add(e.Entity);
                        break;
                    case ChangeType.Update:
                        int index = Session.Customers.IndexOf(Session.Customers.Single(x => x.ID == e.Entity.ID));
                        Session.Customers.Remove(Session.Customers.Single(x => x.ID == e.Entity.ID));
                        Session.Customers.Add(e.Entity);
                        break;
                    default:
                        break;
                }

            }));
            
        }
        public class CustomersOnly : ITableDependencyFilter
        {
            public string Translate()
            {
                return "[IsCustomer] = 'true'";
            }
        }






    }
}
