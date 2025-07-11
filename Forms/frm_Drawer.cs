﻿using System;
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
    public partial class frm_Drawer : frm_Master
    {
        DAL.Drawer  drawer ;
        public frm_Drawer()
        {
            InitializeComponent();
            New();
        }
        public frm_Drawer(int id)
        {
            InitializeComponent();
            LoadDrawer(id);
        }
        void LoadDrawer(int id)
        {
            using (var db =new DAL.dbDataContext ())
            {
                drawer = db.Drawers.Single(x => x.ID == id);
                GetData();
            }
        }

        public override void New()
        {
            drawer = new DAL.Drawer();
            base.New();
        }
        public override void GetData()
        {
            txt_Name .Text = drawer.Name;
            base.GetData();
        }
        public override void Save()
        {
            if (txt_Name.Text.Trim() == string.Empty)
            {
                txt_Name.ErrorText = "برجاء ادخال اسم الخزنه";
                return;
            }
            var db = new DAL.dbDataContext();

            DAL.Account account;

            if (drawer.ID == 0)
            {
                account = new DAL.Account();
                db.Drawers.InsertOnSubmit(drawer);
                db.Accounts.InsertOnSubmit(account);
            }
            else
            {
                db.Drawers.Attach(drawer);
                account = db.Accounts.Single(s => s.ID == drawer.AcoountID ) ;
            }


            SetData();
            account.Name = drawer.Name;
            db.SubmitChanges();
            drawer.AcoountID = account.ID;
            db.SubmitChanges();


            base.Save();    
        }

        public override void SetData()
        {
            drawer.Name = txt_Name.Text;
            base.SetData();
        }

        //public override void Save()
        //{
        //    if (textEdit1.Text.Trim() == string.Empty)
        //    {
        //        textEdit1.ErrorText = "برجاء ادخال اسم الخزنه";
        //        return;
        //    }
        //    var db = new DAL.dbDataContext();
        //    DAL.Account account = new DAL.Account() ;
        //    if (drawer.ID == 0)
        //    {
        //        db.Accounts.InsertOnSubmit(account);
        //        db.Drawers.InsertOnSubmit(drawer);
        //    }
        //    else
        //    {
        //        account = db.Accounts.Single(x => x.ID == drawer.AcoountID);
        //        
        //        db.Drawers.Attach(drawer);

        //    }
        //    SetData();
        //    account.Name = textEdit1.Text;
        //    db.SubmitChanges();

        //    drawer.AcoountID = account.ID;

        //    base.Save();
        //}
        //public override void New()
        //{
        //    drawer = new DAL.Drawer();
        //     base.New();
        //}
    }
}
