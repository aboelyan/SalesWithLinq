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
using DevExpress.XtraLayout;
using SalesWithLinq.Class;
using DevExpress.Utils.Animation;

namespace SalesWithLinq.Forms
{
    public partial class frm_UserSettingsProfile : frm_Master 
    {
        DAL.UserSettingsProfile profile;
        List<BaseEdit> editors;
        public frm_UserSettingsProfile()
        {
            InitializeComponent();
            accordionControl1.ElementClick += AccordionControl1_ElementClick;
            New();
        }
        public frm_UserSettingsProfile(int id)
        {
            InitializeComponent();
            accordionControl1.ElementClick += AccordionControl1_ElementClick;
            using (var db = new DAL.dbDataContext ())
            {
                profile = db.UserSettingsProfiles.Single(x => x.ID == id);
                textEdit1.Text = profile.Name;
                GetData();

            }
        }

        private void AccordionControl1_ElementClick(object sender, DevExpress.XtraBars.Navigation.ElementClickEventArgs e)
        {
          var indx =  accordionControl1.Elements.IndexOf(e.Element);
            xtraTabControl1.SelectedTabPageIndex = indx;
        }
        public override void New()
        {
            profile = new DAL.UserSettingsProfile();
            textEdit1.Text = profile.Name; 
        }
        public override void GetData()
        {

            editors = new List<BaseEdit>();
            Class.UserSettingsTemplate settings = new Class.UserSettingsTemplate(profile.ID );
            accordionControl1.Elements.Clear();
            xtraTabControl1.TabPages.Clear();
            accordionControl1.AllowItemSelection = true;

            var catlog = settings.GetType().GetProperties();
            foreach (var item in catlog)
            {
                accordionControl1.Elements.Add(new DevExpress.XtraBars.Navigation.AccordionControlElement()
                {
                    Name = item.Name,
                    Text = Class.UserSettingsTemplate.GetPropCaption(item.Name),
                    Style = DevExpress.XtraBars.Navigation.ElementStyle.Item,
                });

                var page = new DevExpress.XtraTab.XtraTabPage()
                { Name = item.Name, Text = Class.UserSettingsTemplate.GetPropCaption(item.Name) };
                xtraTabControl1.TabPages.Add(page);
                LayoutControl lc = new LayoutControl();
                EmptySpaceItem empty1 = new EmptySpaceItem();
                EmptySpaceItem empty2 = new EmptySpaceItem();
                //empty2.SizeConstraintsType = SizeConstraintsType.Custom;
                //empty2.MaxSize = new Size(600, 25);
                //empty2.MinSize = new Size(150, 1);
                lc.AddItem(empty1);
                lc.AddItem(empty2, empty1, DevExpress.XtraLayout.Utils.InsertType.Left);


                var props = item.GetValue(settings).GetType().GetProperties();
                foreach (var prop in props)
                {
                    BaseEdit edit = Class.UserSettingsTemplate.GetPropertyControl(prop.Name, prop.GetValue(item.GetValue(settings)));
                    if (edit != null)
                    {
                        var layoutItem = lc.AddItem("", edit,empty2 , DevExpress.XtraLayout.Utils.InsertType.Top );
                        layoutItem.TextVisible = true;
                        layoutItem.Text = Class.UserSettingsTemplate.GetPropCaption(prop.Name);
                        layoutItem.SizeConstraintsType = SizeConstraintsType.Custom;
                        layoutItem.MaxSize = new Size(700,25);
                        layoutItem.MinSize = new Size(250, 25);
                        editors.Add(edit); 
                    }
                }

                lc.Dock = DockStyle.Fill;
                page.Controls.Add(lc);
            }
        }
        public override bool IsDataVailde()
        {
            int flag = 0;
            if (textEdit1.Text.Trim() == string.Empty)
            {
                flag++;
                textEdit1.ErrorText = ErrorText;

            }

            editors.ForEach(e =>
            {
                if (e.GetType() == typeof (LookUpEdit) && ((LookUpEdit)e).Properties.DataSource .GetType() != typeof (List<Master.ValueAndID> ))
                      flag  += ((LookUpEdit)e).IsEditValueValidAndNotZero() ? 0 : 1; 
                // else if (e.GetType() == typeof(LookUpEdit) && ((LookUpEdit)e).Properties.DataSource.GetType() != typeof(Master.ValueAndID))
                //{

                //}
                
            });
            return (flag == 0);
        }
        public override void Save()
        {
            var db = new DAL.dbDataContext();
            if(profile .ID == 0)
            {
                db.UserSettingsProfiles.InsertOnSubmit (profile);
            }
            else
            {
                db.UserSettingsProfiles.Attach(profile);
            }
            profile.Name = textEdit1.Text;
            db.SubmitChanges();
            db.UserSettingsProfileProperties.DeleteAllOnSubmit(
            db.UserSettingsProfileProperties.Where(x => x.ProfileID == profile.ID));
            db.SubmitChanges();
            editors.ForEach(e =>
            {
                db.UserSettingsProfileProperties.InsertOnSubmit(new DAL.UserSettingsProfileProperty()
                {
                    ProfileID = profile.ID,
                    PropertyName = e.Name,
                    PropertyValue = Master.ToByteArray<object>(e.EditValue)
                });
            });
            db.SubmitChanges();

            base.Save();
        }
        private void frm_UserSettingsProfile_Load(object sender, EventArgs e)
        {
            accordionControl1.AnimationType = DevExpress.XtraBars.Navigation.AnimationType.Simple;
            accordionControl1.Appearance.Item.Hovered.Font =
                new Font(accordionControl1.Appearance.Item.Hovered.Font, FontStyle.Bold);
            accordionControl1.Appearance.Item.Hovered.Options.UseFont = true;
            accordionControl1.Appearance.Item.Pressed.Font =
               new Font(accordionControl1.Appearance.Item.Pressed .Font, FontStyle.Bold);
            accordionControl1.Appearance.Item.Pressed.Options.UseFont = true;
            accordionControl1.ScrollBarMode = DevExpress.XtraBars.Navigation.ScrollBarMode.Hidden;
            accordionControl1.OptionsMinimizing.AllowMinimizeMode = DevExpress.Utils.DefaultBoolean.False;
            xtraTabControl1.ShowTabHeader = DevExpress.Utils.DefaultBoolean.False ;
            xtraTabControl1.Transition.AllowTransition = DevExpress.Utils.DefaultBoolean.True;
            xtraTabControl1.Transition.EasingMode = DevExpress.Data.Utils.EasingMode.EaseInOut;
            SlideFadeTransition trans = new SlideFadeTransition();
            trans.Parameters.EffectOptions = PushEffectOptions.FromBottom;
            xtraTabControl1.Transition.TransitionType = trans;
            xtraTabControl1.SelectedPageChanging += XtraTabControl1_SelectedPageChanging;
        }

        private void XtraTabControl1_SelectedPageChanging(object sender, DevExpress.XtraTab.TabPageChangingEventArgs e)
        {
            SlideFadeTransition trans = new SlideFadeTransition();
            var currenPage = xtraTabControl1.TabPages.IndexOf(e.Page);
            var PrevPage = xtraTabControl1.TabPages.IndexOf(e.PrevPage );
            if (currenPage > PrevPage)
                trans.Parameters.EffectOptions = PushEffectOptions.FromBottom;
            else
                trans.Parameters.EffectOptions = PushEffectOptions.FromTop ;
            xtraTabControl1.Transition.TransitionType = trans;
        }
    }
}