using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace URTrackerCrack
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.tbMachine.Text = MachineCode.GetMachineCode();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            
            var res = decode(tbCode.Text.Trim(), this.tbMachine.Text);
            this.tbRes.Text = res;
        }

        private string decode(string code,string machineCode)
        {
            byte[] rgbIV = new byte[] { 18, 52, 86, 120, 144, 171, 205, 239 };
            byte[] mc = Encoding.UTF8.GetBytes(machineCode);
            DESCryptoServiceProvider cryptoServiceProvider = new DESCryptoServiceProvider();
            byte[] buffer = Convert.FromBase64String(code);
            var memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, cryptoServiceProvider.CreateDecryptor(mc, rgbIV), CryptoStreamMode.Write);
            cryptoStream.Write(buffer, 0, buffer.Length);
            cryptoStream.FlushFinalBlock();
            var res = RegAckMsg.ToResgAckMsg(Encoding.UTF8.GetString(memoryStream.ToArray()));
            return res.CustomerName;
        }

        private string encode(RegAckMsg msg,string machineCode)
        {
            byte[] IV = new byte[] { 18, 52, 86, 120, 144, 171, 205, 239 };
            byte[] mc = Encoding.UTF8.GetBytes(machineCode);

            DESCryptoServiceProvider cryptoServiceProvider = new DESCryptoServiceProvider();
            var code = msg.toBase64();
            byte[] buffer = Encoding.Default.GetBytes(code);
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            CryptoStream cs = new CryptoStream(ms, cryptoServiceProvider.CreateEncryptor(mc,IV), CryptoStreamMode.Write);
            cs.Write(buffer, 0, buffer.Length); //写
            cs.FlushFinalBlock();
            return Convert.ToBase64String(ms.ToArray());
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            RegAckMsg msg = new RegAckMsg()
            {
                AllowedUserCount = 1000,
                CheckMessage = "",
                CheckResult = 0,
                CustomerName = "中山大学低碳科技与经济研究中心",
                ExpireDate = new DateTime(2300, 1, 1),
                FunctionLevel = 2,
                NextCheckTime = new DateTime(2099, 1, 1)
            };
            this.tbRes.Text = encode(msg, this.tbMachine.Text);
        }
    }
}
