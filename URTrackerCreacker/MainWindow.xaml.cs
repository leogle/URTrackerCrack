using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
using URTrackerCrack;

namespace URTrackerCreacker
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
            try
            {
                var name = this.tbName.Text;
                var count = int.Parse(tbCount.Text);
                var machineCode = MachineCode.GetMachineCode();
                RegAckMsg msg = new RegAckMsg()
                {
                    AllowedUserCount = count,
                    CheckMessage = "",
                    CheckResult = 0,
                    CustomerName = name,
                    ExpireDate = new DateTime(2300, 1, 1),
                    FunctionLevel = 2,
                    NextCheckTime = new DateTime(2099, 1, 1)
                };
                var code = encode(msg, machineCode);
                WriteToDb(this.tbConnect.Text.Trim(), code);
                MessageBox.Show("破解成功！！\r\n请重启IIS服务网站", "确认", MessageBoxButton.OK, MessageBoxImage.Information);
            }catch(Exception ex)
            {
                MessageBox.Show("破解失败：" + ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string decode(string code, string machineCode)
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

        private string encode(RegAckMsg msg, string machineCode)
        {
            byte[] IV = new byte[] { 18, 52, 86, 120, 144, 171, 205, 239 };
            byte[] mc = Encoding.UTF8.GetBytes(machineCode);

            DESCryptoServiceProvider cryptoServiceProvider = new DESCryptoServiceProvider();
            var code = msg.toBase64();
            byte[] buffer = Encoding.Default.GetBytes(code);
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            CryptoStream cs = new CryptoStream(ms, cryptoServiceProvider.CreateEncryptor(mc, IV), CryptoStreamMode.Write);
            cs.Write(buffer, 0, buffer.Length); //写
            cs.FlushFinalBlock();
            return Convert.ToBase64String(ms.ToArray());
        }

        private void WriteToDb(string connectstring,string code)
        {
            SqlConnection sqlConnection = new SqlConnection(connectstring);
            sqlConnection.Open();
            var command = sqlConnection.CreateCommand();
            command.CommandText = "\r\nIF EXISTS(SELECT [ConfigItem] FROM [Common_SystemConfig] WHERE [ConfigItem] = @ConfigItem)\r\nBEGIN\r\n\tUPDATE [Common_SystemConfig] SET\r\n\t\t[ConfigValue] = @ConfigValue\r\n\tWHERE\r\n\t\t[ConfigItem] = @ConfigItem\r\nEND\r\nELSE\r\nBEGIN\r\n\tINSERT INTO [Common_SystemConfig] (\r\n\t\t[ConfigItem],\r\n\t\t[ConfigValue]\r\n\t) VALUES (\r\n\t\t@ConfigItem,\r\n\t\t@ConfigValue\r\n\t)\r\nEND\r\n";
            command.CommandType = System.Data.CommandType.Text;
            command.Parameters.Add(new SqlParameter("@ConfigItem", "LocalRConfig"));
            command.Parameters.Add(new SqlParameter("@ConfigValue", code));
            command.ExecuteNonQuery();
            sqlConnection.Close();
        }
    }
}
