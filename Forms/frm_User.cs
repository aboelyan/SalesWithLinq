using Liphsoft.Crypto.Argon2;
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
    public partial class frm_User : frm_Master 
    {
        DAL.User user;
        public frm_User()
        {
            InitializeComponent();
            RefreshData();
            New();
        }

        public frm_User(int id )
        {
            InitializeComponent();
            RefreshData();
            using (var db = new DAL.dbDataContext ())
            {
                user = db.Users.SingleOrDefault(x => x.ID == id);
                GetData();
            }
        }

        public override void RefreshData()
        {
            using (var db = new DAL.dbDataContext())
            {
                lkp_ScreenProfileID.IntializeData(db.UserAccessProfileNames.Select(x => new { x.ID ,x.Name }).ToList());
                lkp_SettingsProfileID.IntializeData(db.UserSettingsProfiles.Select(x => new { x.ID, x.Name }).ToList());
                lkp_UserType .IntializeData(Master.UserTypeList ); 
            }
            base.RefreshData(); 
        }
        public override void New()
        {
            user = new DAL.User();
            user.IsActive = true;
            base.New(); 
        }
        public override void GetData()
        {
            txt_Name.Text = user.Name;
            txt_UserName.Text = user.UserName;
            txt_PassWord.Text = user.Password;
            lkp_ScreenProfileID.EditValue = user.ScreenProfileID;
            lkp_SettingsProfileID.EditValue = user.SettingsProfileID;
            lkp_UserType.EditValue = user.UserType;
            toggleSwitch1.IsOn = user.IsActive;
            base.GetData();
        }
        public override void SetData()
        { 

            if(user.Password != txt_PassWord.Text)
            {
                var hasher = new PasswordHasher(); 
                string myhash = hasher.Hash(txt_PassWord.Text );
                txt_PassWord.Text = myhash;
            }

            user.Name = txt_Name.Text;
            user.Password = txt_PassWord.Text;
            user.UserName = txt_UserName.Text.Trim();
            user.ScreenProfileID =Convert.ToInt32( lkp_ScreenProfileID.EditValue );
            user.SettingsProfileID = Convert.ToInt32(lkp_SettingsProfileID.EditValue);
            user.UserType = Convert.ToByte(lkp_UserType.EditValue);
            user.IsActive = toggleSwitch1.IsOn;
            base.SetData();  
        }
        public override void Save()
        {
            using (var db = new DAL.dbDataContext())
            {

                if (user.ID == 0)
                {
                    db.Users.InsertOnSubmit(user);
                }
                else
                {
                    db.Users.Attach(user);
                }
                SetData();
                db.SubmitChanges();
                base.Save();
            } 
        }
        public override bool IsDataVailde()
        {
            int NumberOfErrors = 0;

            using (var db = new DAL.dbDataContext())
            { 
             if (db.Users.Where(x=>x.UserName.Trim() == txt_UserName.Text.Trim() 
                    && x.ID != user.ID ).Count () > 0)
                {
                    NumberOfErrors += 1;
                    txt_UserName.ErrorText = "هذا الاسم مسجل بالفعل";
                }
                if (db.Users.Where(x => x.Name .Trim() == txt_Name.Text.Trim()
                       && x.ID != user.ID).Count() > 0)
                {
                    NumberOfErrors += 1;
                    txt_Name.ErrorText = "هذا الاسم مسجل بالفعل";
                }

            }
            NumberOfErrors += txt_Name .IsTextVailde() ? 0 : 1;
            NumberOfErrors += txt_PassWord.IsTextVailde() ? 0 : 1;
            NumberOfErrors += txt_UserName.IsTextVailde() ? 0 : 1;

            
            NumberOfErrors += lkp_ScreenProfileID.IsEditValueValidAndNotZero() ? 0 : 1;
            NumberOfErrors += lkp_SettingsProfileID.IsEditValueValidAndNotZero() ? 0 : 1;
            NumberOfErrors += lkp_UserType.IsEditValueValidAndNotZero() ? 0 : 1;
            
            return (NumberOfErrors == 0);

            
        }
       
    }
}
