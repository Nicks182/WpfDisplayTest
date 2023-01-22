using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfDisplayTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        StackPanel G_Pnl_Display = null;
        TextBlock G_Txt_Display = null;
        Button G_Btn_Display = null;

        List<cl_MyDisplay> G_Displays = new List<cl_MyDisplay>();

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WindowsDisplayAPI.DisplayConfig.PathDisplayTarget[] L_DisplayTargets = WindowsDisplayAPI.DisplayConfig.PathDisplayTarget.GetDisplayTargets();

            WindowsDisplayAPI.Display L_Display = null;

            WindowsDisplayAPI.DisplayConfig.PathDisplayTarget L_Target = null;
            for (int i = 0; i < L_DisplayTargets.Count(); i++)
            {
                L_Target = L_DisplayTargets[i];
                L_Display = WindowsDisplayAPI.Display.GetDisplays().Where(d => d.DevicePath.Equals(L_Target.DevicePath)).FirstOrDefault();
                
                if (L_Display != null)
                {
                    G_Displays.Add(new cl_MyDisplay
                    {
                        DevicePath_Base64 = L_Display.DevicePath.EncodeBase64(),
                        FriendlyName = _GetFirendlyName(L_Target, L_Display),
                        Pos_X = L_Display.DisplayScreen.SavedSetting.Position.X,
                        Pos_Y = L_Display.DisplayScreen.SavedSetting.Position.Y,
                        Res_X = L_Display.DisplayScreen.SavedSetting.Resolution.Width,
                        Res_Y = L_Display.DisplayScreen.SavedSetting.Resolution.Height,
                    });
                }
            }


            foreach(var L_dis in G_Displays)
            {
                G_Txt_Display = new TextBlock();
                G_Txt_Display.Text = L_dis.FriendlyName;

                G_Btn_Display = new Button();
                G_Btn_Display.Content = "Off";
                G_Btn_Display.Tag = L_dis.DevicePath_Base64;
                G_Btn_Display.Click += G_Btn_Display_Click;

                G_Pnl_Display = new StackPanel();
                G_Pnl_Display.Orientation = Orientation.Horizontal;
                G_Pnl_Display.Margin = new Thickness(10);
                G_Pnl_Display.Children.Add(G_Txt_Display);
                G_Pnl_Display.Children.Add(G_Btn_Display);

                Pnl_Displays.Children.Add(G_Pnl_Display);
            }
        }

        private void G_Btn_Display_Click(object sender, RoutedEventArgs e)
        {
            string L_DevicePath_Base64 = (sender as Button).Tag.ToString();

            cl_MyDisplay L_cl_MyDisplay = G_Displays.Where(d => d.DevicePath_Base64 == L_DevicePath_Base64).FirstOrDefault();

            List<WindowsDisplayAPI.Display> L_Displays = WindowsDisplayAPI.Display.GetDisplays().ToList();
            List<WindowsDisplayAPI.UnAttachedDisplay> L_DeadDisplays = WindowsDisplayAPI.UnAttachedDisplay.GetUnAttachedDisplays().ToList();

            List<string> L_Display_Active_Names = new List<string>();
            foreach(var ActiveDisplay in L_Displays)
            {
                L_Display_Active_Names.Add("Active == " + ActiveDisplay.ScreenName);
            }

            foreach (var DeadDisplay in L_DeadDisplays)
            {
                L_Display_Active_Names.Add("Disabled == " + DeadDisplay.ScreenName);
            }




            WindowsDisplayAPI.Display L_Display = WindowsDisplayAPI.Display.GetDisplays().Where(d => d.DevicePath.EncodeBase64().Equals(L_DevicePath_Base64)).FirstOrDefault();
            WindowsDisplayAPI.UnAttachedDisplay L_DeadDisplay = WindowsDisplayAPI.UnAttachedDisplay.GetUnAttachedDisplays().Where(d => d.DevicePath.EncodeBase64().Equals(L_DevicePath_Base64)).FirstOrDefault();

            if (L_Display != null)
            {
                L_Display.DisplayScreen.Disable(true);

                (sender as Button).Content = "On";
            }
            else
            {


                if (L_DeadDisplay != null)
                {
                    WindowsDisplayAPI.DisplaySetting L_Setting = new WindowsDisplayAPI.DisplaySetting(
                            new System.Drawing.Size(L_cl_MyDisplay.Res_X, L_cl_MyDisplay.Res_Y),
                            new System.Drawing.Point(L_cl_MyDisplay.Pos_X, L_cl_MyDisplay.Pos_Y),
                            L_DeadDisplay.DisplayScreen.GetPreferredSetting().Frequency);


                    L_DeadDisplay.DisplayScreen.Enable(L_Setting, true);

                }

                (sender as Button).Content = "Off";
            }

        }

        private string _GetFirendlyName(WindowsDisplayAPI.DisplayConfig.PathDisplayTarget L_Target, WindowsDisplayAPI.Display L_Display)
        {
            if (string.IsNullOrEmpty(L_Target.FriendlyName) == false)
            {
                return L_Target.FriendlyName;
            }

            return L_Display.DisplayName;
        }

    }


    public class cl_MyDisplay
    {
        public string DevicePath_Base64 { get; set; }
        public string FriendlyName { get; set; }
        public int Pos_X { get; set; }
        public int Pos_Y { get; set; }
        public int Res_X { get; set; }
        public int Res_Y { get; set; }
    }

    public static class StringExtensions
    {

        public static string EncodeBase64(this string plainText)
        {
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }
    }
}
