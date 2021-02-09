using DevExpress.LookAndFeel;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Navigation;
using DevExpress.XtraEditors;
using SalesWithLinq.Class;
using SalesWithLinq.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace SalesWithLinq.Forms
{
    public partial class frm_Main : DevExpress.XtraBars.FluentDesignSystem.FluentDesignForm
    {

        public static frm_Main _instance;
        public static frm_Main Instance { get
            {
                if (_instance == null)
                    _instance = new frm_Main();

                return _instance;
            }
        }
        public frm_Main()
        {
            InitializeComponent();
            accordionControl1.ElementClick += AccordionControl1_ElementClick;

        }

        private void AccordionControl1_ElementClick(object sender, DevExpress.XtraBars.Navigation.ElementClickEventArgs e)
        {
            var tag = e.Element.Tag as string;
            if (tag != string.Empty)
            {
                OpenFormByName(tag);
            }
        }

        public static void OpenFormByName(string name)
        {

            Form frm = null;


            switch (name)
            {
                case "frm_Vendor":
                    frm = new frm_CustomerVendor(false);
                    break;
                case "frm_Customer":
                    frm = new frm_CustomerVendor(true);
                    break;

                case "frm_VendorList":
                    frm = new frm_CustomerVendorList(false);
                    break;
                case "frm_CustomerList":
                    frm = new frm_CustomerVendorList(true);
                    break;
                case "frm_PurchaseInvoice":
                    frm = new frm_Invoice(Class.Master.InvoiceType.Purchase);
                    break;
                default:
                    var ins = Assembly.GetExecutingAssembly().GetTypes().FirstOrDefault(x => x.Name == name);
                    if (ins != null)
                    {
                        frm = Activator.CreateInstance(ins) as Form;
                        if (Application.OpenForms[frm.Name] != null)
                        {
                            frm = Application.OpenForms[frm.Name];
                        }
                        else
                        {
                        //    frm.Show();
                        }

                        frm.BringToFront();

                    }
                    break;
            }

            if (frm != null)
            {
                 frm.Name = name;

                OpenForm( frm);
            }
        }
        public static void  OpenForm(Form frm , bool OpenInDialog = false )
        {
            if(Session.User.UserType == (byte)Master.UserType.Admin)
            {
                frm.Show();
                return; 
            }
            var screen = Session.ScreensAccesses.SingleOrDefault(x => x.ScreenName == frm.Name);
            if(screen != null)
            {
                if(screen.CanOpen == true )
                {
                    if (OpenInDialog)
                        frm.ShowDialog();
                    else  
                        frm.Show();
                    return;
                }
                else
                {
                    XtraMessageBox.Show(
           text: "غير مصرح لك ",
           caption: "",
           icon: MessageBoxIcon.Error,
           buttons: MessageBoxButtons.OK
           );
                    return;
                }

            }
            


        }

        private void frm_Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            Settings.Default.SkinName = UserLookAndFeel.Default.SkinName;
            Settings.Default.PaletteName = UserLookAndFeel.Default.ActiveSvgPaletteName;
            Settings.Default.Save();
            Application.Exit();
        }

        private void frm_Main_Load(object sender, EventArgs e)
        {
            UserLookAndFeel.Default.SkinName = Settings.Default.SkinName .ToString();
            UserLookAndFeel.Default.SetSkinStyle(Settings.Default.SkinName.ToString(), Settings.Default.PaletteName.ToString());
            DevExpress.XtraEditors.WindowsFormsSettings.DefaultFont = Settings.Default.SystemFont;
            accordionControl1.Elements.Clear();
            var screens = Class.Session.ScreensAccesses.Where(x=>x.CanShow == true || Session.User.UserType == (byte)Master.UserType.Admin);
            screens.Where(s=>s.ParentScreenID == 0 ).ToList().ForEach(s =>
            {
                AccordionControlElement elm = new AccordionControlElement();
                elm.Text  = s.ScreenCaption;
                elm.Tag = s.ScreenName;
                elm.Name = s.ScreenName;
                elm.Style = ElementStyle.Group;
                accordionControl1.Elements.Add(elm);
                AddAccordionElement(elm, s.ScreenID);

            });

        }
        void AddAccordionElement(AccordionControlElement parent ,int parentID)
        {
            var screens = Class.Session.ScreensAccesses.Where(x => x.CanShow == true || Session.User.UserType == (byte)Master.UserType.Admin);

            screens.Where(s => s.ParentScreenID == parentID).ToList().ForEach(s =>
            {
              AccordionControlElement elm = new AccordionControlElement();
                elm.Text = s.ScreenCaption;
                elm.Tag = s.ScreenName;
                elm.Name = s.ScreenName;
                elm.Style = ElementStyle.Item ;
                parent.Elements.Add(elm);
            });
             
        }
        private void barButtonItem2_ItemClick(object sender, ItemClickEventArgs e)
        {
            using (FontDialog dialog = new FontDialog())
            {
                dialog.Font = DevExpress.XtraEditors.WindowsFormsSettings.DefaultFont;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    DevExpress.XtraEditors.WindowsFormsSettings.DefaultFont = dialog.Font;
                    Settings.Default.SystemFont = dialog.Font;
                    Settings.Default.Save();
                }
            }
        }
    }
}
