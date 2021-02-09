using DevExpress.XtraEditors;
using DevExpress.XtraSplashScreen;
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
    public partial class frm_Login : XtraForm
    {
        public frm_Login()
        {
            InitializeComponent();
        }

        private void btn_LogIn_Click(object sender, EventArgs e)
        {
            using (var db = new DAL.dbDataContext())
            {

               


                var userName = txt_UserName.Text;
                var passWord = txt_UserPassword.Text;


                var user = db.Users.SingleOrDefault(x => x.UserName == userName);
                if(user == null) 
                    goto  LogInFaild; 
                else
                { if(user.IsActive == false)
                    {
                        XtraMessageBox.Show(
                       text: "تم تعطيل هذا الحساب",
                   
                       caption: "",
                       icon: MessageBoxIcon.Exclamation ,
                       buttons: MessageBoxButtons.OK
                       );
                        return;
                    }
                    var passWordHash = user.Password;
                    var hasher = new Liphsoft.Crypto.Argon2.PasswordHasher();
                    if(hasher.Verify (passWordHash, passWord))
                    {

                        // Successfully loging 
                        this.Hide();
                        SplashScreenManager.ShowForm( parentForm: frm_Main.Instance ,typeof(StartSplash));

                        // put loading over here
                        Class.Session.SetUser(user);

                        Type t = typeof(Class.Session);
                        var propertys = t.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                        foreach (var item in propertys)
                        {
                            var obj = item.GetValue(null);
                        }
                        ///////////
                        frm_Main.Instance.Show(); 
                        this.Close();
                        SplashScreenManager.CloseForm();
                        return;
                
                        ///////////////////////////
                    }


                    else 
                        goto LogInFaild; 
                }

            }

        LogInFaild:
            XtraMessageBox.Show(
            text: "اسم المستخدم او كلمه السر غير صحيحه",
            caption: "",
            icon: MessageBoxIcon.Error,
            buttons: MessageBoxButtons.OK
            );
            return;

        }
    }
}
