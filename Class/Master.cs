using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using SalesWithLinq.Forms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SalesWithLinq.Class
{
   public static  class Master
    {
        public class ValueAndID
        {
            public int ID { get; set; }
            public string Name { get; set; }
        }
        public static  List<ValueAndID> ProductTypesList = new List<ValueAndID>() {
                new ValueAndID() { ID = 0, Name  ="مخزني" },
                new ValueAndID() { ID = 1, Name  ="خدمي" }
        
        };
       
        public enum ProductType
        {
            Inventory,
            Service
        }

        public static List<ValueAndID> InvoiceTypesList = new List<ValueAndID>() {
                new ValueAndID() { ID = (int)InvoiceType.Purchase , Name  ="مشتريات" },
                new ValueAndID() { ID = (int)InvoiceType.Sales  , Name  ="مبيعات" },
                new ValueAndID() { ID = (int)InvoiceType.PurchaseReturn  , Name  ="مردود مشتريات" },
                new ValueAndID() { ID = (int)InvoiceType.SalesReturn  , Name  ="مردود مبيعات" }, 
        };

        public enum InvoiceType
        {
            Purchase = SourceTypes.Purchase ,
            Sales = SourceTypes.Sales ,
            PurchaseReturn = SourceTypes.PurchaseReturn ,
            SalesReturn = SourceTypes.SalesReturn ,
        }


        public enum SourceTypes
        {
       
            Purchase,
            Sales,
            PurchaseReturn,
            SalesReturn, 
         
        }

        public enum CostDistributionOptions
        { 
            ByPrice,
            ByQty,  
        }
        public enum PrintMode
        {
            Direct,
            ShowPreview,
            ShowDialog,
        }




        public static List<ValueAndID> PartTypesList = new List<ValueAndID>() {
                new ValueAndID() { ID = 0, Name  ="مورد" },
                new ValueAndID() { ID = 1, Name  ="عميل" }

        };
        public enum PartType
        {
            Vendor,
            Customer,
        }

        public static List<ValueAndID> PayMethodsList = new List<ValueAndID>() {
                new ValueAndID() { ID = (int)PayMethods.Cash , Name  ="نقدي" },
                new ValueAndID() { ID = (int)PayMethods.Credit , Name  ="اجل" }

        };
        public enum PayMethods
        {
            Cash =1,
            Credit,
        }


        public static List<ValueAndID> UserTypeList = new List<ValueAndID>() {
                new ValueAndID() { ID = (int)UserType.Admin , Name  ="مدير نظام" },
                new ValueAndID() { ID = (int)UserType.User  , Name  ="دخول مخصص" }

        };
        public enum UserType
        {
            Admin = 1,
            User,
        }


        public static List<ValueAndID> WarningLevelsList = new List<ValueAndID>() {
                new ValueAndID() { ID = (int)WarningLevels.DoNotEnteript  , Name  ="عدم التداخل" },
                new ValueAndID() { ID = (int)WarningLevels.ShowWarning  , Name  ="تحذير" },
                new ValueAndID() { ID = (int)WarningLevels.Prevent  , Name  ="منع" },

        };
        public enum WarningLevels
        {
            DoNotEnteript =1,
            ShowWarning,
            Prevent,
        }

        public enum Actions
        {
            Show=1,
            Open,
            Add,
            Edit,
            Delete,
            Print,
        }

        public static  int FindRowHandelByRowObject(this GridView view, object row)
        {
            if(row !=null)
            {
                for (int i = 0; i < view.DataRowCount ; i++)
                {
                    if (row.Equals(view.GetRow(i)))
                        return i;
                }
            }
            return DevExpress.XtraGrid.GridControl.InvalidRowHandle;
        }
        public static bool  IsTextVailde(this TextEdit   txt)
        {
            if(txt.Text.Trim() == string.Empty)
            {
                txt.ErrorText = frm_Master.ErrorText;
                return false   ;
            }
            return true  ;
        }
        public static bool IsEditValueValid(this LookUpEditBase lkp)
        {
            if (lkp.IsEditValueOfTypeInt() == false )
            {
                lkp.ErrorText = frm_Master.ErrorText;
                return false;
            }
            return true;
        }

        public static bool IsEditValueValidAndNotZero(this LookUpEditBase lkp)
        {
            if (lkp.IsEditValueOfTypeInt() == false || Convert.ToInt32(lkp.EditValue ) == 0)
            {
                lkp.ErrorText = frm_Master.ErrorText;
                return false;
            }
            return true;
        }
        public static bool IsDateVailde(this DateEdit   dt)
        {
            if (dt.DateTime .Year < 1950)
            {
                dt.ErrorText = frm_Master.ErrorText;
                return false;
            }
            return true;
        }
        public static bool IsEditValueOfTypeInt(this LookUpEditBase edit)
        {
            var val = edit.EditValue;
            return (val is int || val is byte);
        }
        public static void IntializeData(this RepositoryItemLookUpEditBase repo, object dataSource ,GridColumn column,GridControl grid )
        {
            IntializeData(repo, dataSource, column,grid, "Name", "ID");
        }
        public static void IntializeData(this RepositoryItemLookUpEditBase repo, object dataSource, GridColumn column, GridControl grid, string displayMember, string valueMember)
        {
            if (repo == null) 
                 repo = new RepositoryItemLookUpEdit();
           

            repo.DataSource = dataSource;
            repo.DisplayMember = displayMember;
            repo.ValueMember = valueMember;
            repo.NullText = "";
            column.ColumnEdit = repo;
            if (grid != null)
                grid.RepositoryItems.Add(repo);
        }
        public static void IntializeData(this LookUpEdit lkp, object dataSource)
        {
            lkp.IntializeData( dataSource, "Name", "ID");
        }
        public static void IntializeData(this LookUpEdit lkp, object dataSource ,string displayMember , string valueMember)
        {
            lkp.Properties.DataSource = dataSource;
            lkp.Properties.DisplayMember = displayMember;
            lkp.Properties.ValueMember = valueMember;
            lkp.Properties.Columns.Clear();
            lkp.Properties.Columns.Add(new DevExpress.XtraEditors.Controls.LookUpColumnInfo()
            {
                FieldName = displayMember,

            });
            lkp.Properties.ShowHeader = false;
           // lkp.Properties.PopulateColumns(dataSource);
            //lkp.Properties.Columns[valueMember].Visible = false ; 
        }
        public static void IntializeData(this GridLookUpEdit lkp, object dataSource)
        {
            lkp.IntializeData(dataSource, "Name", "ID");
        }
        public static void IntializeData(this GridLookUpEdit  lkp, object dataSource, string displayMember, string valueMember)
        {
            lkp.Properties.DataSource = dataSource;
            lkp.Properties.DisplayMember = displayMember;
            lkp.Properties.ValueMember = valueMember;
      
        }

       public static  string GetNextNumberInString(string Number)
        {
            if (Number == string.Empty || Number == null)
                return "1";
            string str1 = "";
            foreach (Char c in Number)
                str1 = char.IsDigit(c) ? str1 + c.ToString() : "";
            if (str1 == string.Empty)
                return Number + "1";
            string str2 = str1.Insert(0, "1");
            str2 = (Convert.ToInt64 (str2) + 1).ToString();
            string str3 = str2[0] == '1' ? str2.Remove(0, 1) : str2.Remove(0, 1).Insert(0, "1");
            int indx = Number.LastIndexOf(str1);
            Number = Number.Remove(indx);
            Number = Number.Insert(indx, str3);
            return Number;

        }
        public static T FromByteArray<T>(byte[] data)
        {
            if (data == null)
                return default(T);
            BinaryFormatter formatter = new BinaryFormatter();
            using (MemoryStream stream = new MemoryStream(data))
            {
                object obj = formatter.Deserialize(stream);
                return (T)obj;
                //return  (T)formatter.Deserialize(stream);
            };
        }
        public static byte[] ToByteArray<T>(T obj)
        {
            if (obj == null)
                return null;
            BinaryFormatter formatter = new BinaryFormatter();
            using (MemoryStream stream = new MemoryStream())
            {
                 formatter.Serialize (stream ,obj );
                return stream.ToArray();
            };
        }
        public static byte[] GetPropertyValue(string propertyName , int profileID)
        {
            using(var db = new DAL.dbDataContext())
            {
                var prop = db.UserSettingsProfileProperties.SingleOrDefault(x => x.ProfileID == profileID &&
               x.PropertyName == propertyName);
                if (prop == null)
                    return null;
                return prop.PropertyValue.ToArray();
            }
        }

        public static string  GetCallerName([CallerMemberName] string callerName = "")
        {
            return callerName;
        }
    }
}
