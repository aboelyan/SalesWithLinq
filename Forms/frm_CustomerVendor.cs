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
    public partial class frm_CustomerVendor : frm_Master 
    {
        bool IsCustomer;
        DAL.CustomersAndVendor CusVnd;
        public frm_CustomerVendor(bool isCustomer)
        {
            InitializeComponent();
            this.Text = (IsCustomer) ? "عميل" : "مورد";
            this.Name = IsCustomer ? "frm_Customer" : "frm_Vendor";

            IsCustomer = isCustomer;
            New();
        }
        public frm_CustomerVendor(int id )
        {
            InitializeComponent();
            this.Text = (IsCustomer) ? "عميل" : "مورد";
            this.Name = IsCustomer ? "frm_Customer" : "frm_Vendor";
            LoadObject(id);
        }
        void  LoadObject(int id)
        {
            using (var db = new DAL.dbDataContext())
            {
                CusVnd = db.CustomersAndVendors .Single(x => x.ID == id);
                IsCustomer = CusVnd.IsCustomer;
                GetData();
            }
        }

        private void frm_CustomerVendor_Load(object sender, EventArgs e)
        {
         
        }
        public override void New()
        {
            CusVnd = new DAL.CustomersAndVendor();
            base.New();
        }
        public override void GetData()
        {
            txt_Name.Text = CusVnd.Name;
            txt_Mobile.Text = CusVnd.Mobile;
            txt_Phone.Text = CusVnd.Phone;
            txt_Address.Text = CusVnd.Address;
            txt_AccountID.Text = CusVnd.AccountID.ToString();
            base.GetData();
        }
        public override void SetData()
        {
            CusVnd.Name = txt_Name.Text;
            CusVnd.Mobile = txt_Mobile.Text;
            CusVnd.Phone = txt_Phone.Text;
            CusVnd.Address = txt_Address.Text;
            CusVnd.IsCustomer = IsCustomer;
            base.SetData();
        }
        bool IsDataValid()
        {
            if(txt_Name.Text.Trim() == string.Empty)
            {
                txt_Name.ErrorText = "هذا الحقل مطلوب";
                return false;
            }
            var db = new DAL.dbDataContext();
            var oldObj = db.CustomersAndVendors.Where(x => x.Name.Trim() == txt_Name.Text.Trim() &&
            x.IsCustomer == IsCustomer &&
            x.ID != CusVnd.ID);
            if(oldObj.Count() > 0)
            {
                txt_Name.ErrorText = "هذا الاسم مسجل مسبقا";
                return false;
            }


            return true;
        }
        public override void Save()
        {
            if (IsDataValid() == false)
                return;
            var db = new DAL.dbDataContext();
            DAL.Account account;
            if (CusVnd.ID == 0)
            {
                db.CustomersAndVendors.InsertOnSubmit(CusVnd);
                account = new DAL.Account();
                db.Accounts.InsertOnSubmit(account);
            }
            else
            {
                db.CustomersAndVendors.Attach(CusVnd);
                account = db.Accounts.Single(s => s.ID == CusVnd.AccountID );
            }


            SetData();
            account.Name = CusVnd.Name;
            db.SubmitChanges();
            CusVnd.AccountID = account.ID;
            db.SubmitChanges();
            base.Save();


 
        }
    }
}
