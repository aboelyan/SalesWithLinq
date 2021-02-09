using DevExpress.XtraEditors;
using DevExpress.XtraLayout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SalesWithLinq.Class.Master;

namespace SalesWithLinq.UserControls
{
    class CostDistributionOption :XtraUserControl
    {
        public RadioGroup group = new RadioGroup();
        public CostDistributionOptions SelectedOption { get {

                if (group.EditValue != null)
                    return ((CostDistributionOptions)group.EditValue);
                else return CostDistributionOptions.ByQty;
            } }
        public CostDistributionOption()
        {
            LayoutControl lc = new LayoutControl();
            lc.Dock = System.Windows.Forms.DockStyle.Fill;
            group.Properties.Items.AddRange(new DevExpress.XtraEditors.Controls.RadioGroupItem[]
            {
                new DevExpress.XtraEditors.Controls.RadioGroupItem(CostDistributionOptions.ByPrice ,"حسب السعر"),
                new DevExpress.XtraEditors.Controls.RadioGroupItem(CostDistributionOptions.ByQty ,"حسب الكميه")
            });
            group.SelectedIndex = 1;
            lc.AddItem("طريقه توزيع المصاريف علي الاصناف", group).TextLocation = DevExpress.Utils.Locations.Top ;
            this.Controls.Add(lc);
            this.Dock = System.Windows.Forms.DockStyle.Top ;
            this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
           
            this.Height = 80;
        }
    }
}
