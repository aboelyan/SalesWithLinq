using DevExpress.XtraEditors;
using SalesWithLinq.Class;
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
    public partial class frm_Stores : frm_Master    
    {
        DAL.Store store;
        public frm_Stores()
        {
            InitializeComponent();
            New();
        }
        public frm_Stores(int id)
        {
            InitializeComponent();
            var db = new DAL.dbDataContext();

            store = db.Stores.Where(s => s.ID == id).First();
            GetData();
        }
        public override void Save()
        {
            if (textEdit1 .Text.Trim() == string.Empty)
            {
                textEdit1.ErrorText = "برجاء ادخال اسم الفرع";
                return;
            }
            DAL.Account SalesAccount = new DAL.Account();
            DAL.Account SalesReturnAccount = new DAL.Account();
            DAL.Account InventoryAccount = new DAL.Account();
            DAL.Account CostOfSoldGoodsAccont = new DAL.Account();
         


            var db = new DAL.dbDataContext();

            if(store.ID == 0)
            {
                db.Stores.InsertOnSubmit(store);

                db.Accounts.InsertOnSubmit(SalesAccount);
                db.Accounts.InsertOnSubmit(SalesReturnAccount);
                db.Accounts.InsertOnSubmit(InventoryAccount);
                db.Accounts.InsertOnSubmit(CostOfSoldGoodsAccont);

                store.DiscountAllowedAccountID = Session.Defualts.DiscountAllowedAccountID;
                store.DiscountReceivedAccountID = Session.Defualts.DiscountReceivedAccountID ;
            }
            else
            {
          
                db.Stores.Attach(store);

                SalesAccount = db.Accounts.Single(x => x.ID == store.SalesAccountID);
                SalesReturnAccount = db.Accounts.Single(x => x.ID == store.SalesReturnAccountID);
                InventoryAccount = db.Accounts.Single(x => x.ID == store.InventoryAccountID);
                CostOfSoldGoodsAccont = db.Accounts.Single(x => x.ID == store.CostOfSoldGoodsAccountID);

            } 



            SetData();
            SalesAccount.Name = store.Name + "- مبيعات";
            SalesReturnAccount.Name = store.Name + "- مردود مبيعات";
            InventoryAccount.Name = store.Name + "- المخزون";
            CostOfSoldGoodsAccont.Name = store.Name + "- تكلفه البضاعه المباعه";
            db.SubmitChanges();

            store.SalesAccountID = SalesAccount.ID;
            store.SalesReturnAccountID = SalesReturnAccount.ID;
            store.InventoryAccountID = InventoryAccount.ID;
            store.CostOfSoldGoodsAccountID = CostOfSoldGoodsAccont.ID;
           
            db.SubmitChanges();
            base.Save();
          
        }
       public override  void GetData()
        {
            textEdit1.Text = store.Name; 
        }
        public override void SetData()
        {
            store.Name = textEdit1.Text;
        }
        public override void New()
        {
            store = new DAL.Store();
            GetData();
        }


        public override void Delete()
        {
            var db = new DAL.dbDataContext();
            if(XtraMessageBox.Show(text: "هل تريد حذف المخزن",caption: "تاكيد الحذف",
                buttons: MessageBoxButtons.YesNo,
                icon: MessageBoxIcon.Question ) == DialogResult.Yes )
            {
                var log = db.StoreLogs.Where(x => x.SourceID == store.ID).Count();
                var accountLog = db.Journals.Where(x => x.AccountID == store.CostOfSoldGoodsAccountID ||
               x.AccountID == store.SalesAccountID ||
               x.AccountID == store.SalesReturnAccountID ||
               x.AccountID == store.InventoryAccountID
                ).Count() ;

                if(log + accountLog > 0)
                {
                    XtraMessageBox.Show(text: "عفوا لا يمكن حذف المخزن حيث ام استخادمه في النظام "
                        , caption: "", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Error);

                    return;
                }

                db.Stores.Attach(store);
                db.Stores.DeleteOnSubmit(store);
                db.Accounts.DeleteAllOnSubmit(db.Accounts .Where(x =>
                x.ID == store.CostOfSoldGoodsAccountID ||
               x.ID == store.SalesAccountID ||
               x.ID == store.SalesReturnAccountID ||
               x.ID == store.InventoryAccountID
                ));
                db.SubmitChanges();
                XtraMessageBox.Show("تم الحذف بنجاح");
                New();
            }
            

        }
    }
}
