using System;
using System.Collections.Generic;
using System.Linq; 
using System.Threading.Tasks;
using System.Windows.Forms;
using SalesWithLinq.Forms ;
using System.Reflection;
using DevExpress.LookAndFeel;
using SalesWithLinq.Properties;

namespace SalesWithLinq
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            UserLookAndFeel.Default.SkinName = Settings.Default.SkinName.ToString();
            UserLookAndFeel.Default.SetSkinStyle(Settings.Default.SkinName.ToString(), Settings.Default.PaletteName.ToString());
            new frm_Login().Show();
            Application.Run();
          //  Application.Run( frm_Main.Instance );

        }
      
    }
}
