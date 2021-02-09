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
    public partial class frm_Master : XtraForm
    {
        bool IsNew;
        public static  string ErrorText { get {
                 
                return "هذا الحقل مطلوب";
            } 
        }
        public frm_Master()
        {
            InitializeComponent(); 
        }
        public virtual void Save()
        {
            XtraMessageBox.Show("تم الحفظ بنجاح");
            RefreshData();
            IsNew = false;

        }
        public virtual void New()
        {
            GetData();
            IsNew = true;
        }
        public virtual void Delete()
        {

        }
        public virtual void Print()
        {

        }

        public virtual void GetData()
        {
             
        }
        public virtual void SetData()
        {

        }
        public virtual void RefreshData()
        {

        }
        public virtual bool  IsDataVailde()
        {
            return true;
        }
        private void btn_Save_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (CheckActionAuthorization(this.Name,IsNew? Master.Actions.Add: Master.Actions.Edit  ))
                if (IsDataVailde())
                Save();
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //Check if user can add 
            New();
        }

        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (CheckActionAuthorization(this.Name, Master.Actions.Delete ))
                Delete();
        }
        private void barButtonItem1_ItemClick_1(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (CheckActionAuthorization(this.Name, Master.Actions.Print))
               Print();
        }
        public static bool  CheckActionAuthorization(string formName , Master.Actions actions,DAL.User user =null )
        {
            if (user == null)
                user = Session.User;

            if (user.UserType == (byte)Master.UserType.Admin)
                return true;
            else
            {
                var screen = Session.ScreensAccesses.SingleOrDefault(x => x.ScreenName == formName);
                bool flag = true;
                if(screen != null)
                {
                    switch (actions)
                    { 
                        case Master.Actions.Add:
                            flag = screen.CanAdd;
                            break;
                        case Master.Actions.Edit:
                            flag = screen.CanEdit ;

                            break;
                        case Master.Actions.Delete:
                            flag = screen.CanDelete ;

                            break;
                        case Master.Actions.Print:
                            flag = screen.CanPrint ;

                            break;
                        default:
                            break;
                    }
                }
                if(flag == false)
                {
                    XtraMessageBox.Show(
         text: "غير مصرح لك ",
         caption: "",
         icon: MessageBoxIcon.Error,
         buttons: MessageBoxButtons.OK
         ); 
                }
                return flag;
            }


        }
        private void frm_Master_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void frm_Master_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F1)
            {
                btn_Save.PerformClick();
            }
            if (e.KeyCode == Keys.F2)
            {
                btn_New .PerformClick();
            }
            if (e.KeyCode == Keys.F3)
            { 
                btn_Print .PerformClick();
            }
            if (e.KeyCode == Keys.F4)
            {
               btn_Delete.PerformClick() ;
            }
        }

       
    }
}
