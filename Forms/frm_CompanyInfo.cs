using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace SalesWithLinq.Forms
{
    public partial class frm_CompanyInfo : DevExpress.XtraEditors.XtraForm
    {
        public frm_CompanyInfo()
        {
            InitializeComponent();
            this.Load += Frm_CompanyInfo_Load;
        }

        private void Frm_CompanyInfo_Load(object sender, EventArgs e)
        {
            GetData();
             
        }
       
        private void btn_Save_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Save();
        }

        private void frm_CompanyInfo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F1)
            {
                Save();

            }
        }
        void Save()
        {
                if (txt_Name.Text.Trim() == string.Empty)
                {
                    txt_Name.ErrorText = "برجاء ادخال اسم الشركه";
                    return;
                }


            DAL.dbDataContext db = new DAL.dbDataContext();
            DAL.CompanyInfo info = db.CompanyInfos.FirstOrDefault();
            if(info == null)
            {
                info = new DAL.CompanyInfo();
                db.CompanyInfos.InsertOnSubmit(info); 
            }

            info.CompanyName = txt_Name.Text;
            info.Mobile = txt_Mobile.Text;
            info.Phone = txt_Phone.Text;
            info.Address = txt_Address.Text;
            db.SubmitChanges();
            XtraMessageBox.Show("تم الحفظ بنجاح");
        }
        void GetData()
        {
            DAL.dbDataContext db = new DAL.dbDataContext();
            DAL.CompanyInfo info = db.CompanyInfos.FirstOrDefault (); // =>  select top(1) * from [SalesDB].[dbo].[CompanyInfo]
            if (info == null) 
                return;

            txt_Name.Text = info.CompanyName;
            txt_Address.Text = info.Address;
            txt_Mobile.Text = info.Mobile;
            txt_Phone.Text = info.Phone;

        }
    }
}