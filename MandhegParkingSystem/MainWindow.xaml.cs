using MandhegParkingSystem.Models;
using MandhegParkingSystem.Views;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MandhegParkingSystem
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    
    public partial class MainWindow : Window
    {
        public MandhegParkingSystemContext DBC = new MandhegParkingSystemContext();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void TryLogin(object sender, RoutedEventArgs e)
        {
            if(!Regex.IsMatch(empId.Text, @"^\d+$")|| password.Password.Length == 0)
            {
                return;
            }
            var emp = DBC.Employees.FirstOrDefault(e => e.Id == Convert.ToInt32(empId.Text));
            if(emp == null)
            {
                MessageBox.Show("Login failed!");
                return;
            }
            using (SHA256 sha256 = SHA256.Create())
            {
                if(!verifyHash(sha256, password.Password, emp.Password))
                {
                    MessageBox.Show("Login failed!");
                    return;
                }
                empId.Text = "";
                password.Password = "";
                var dashboard = new Dashboard(DBC, this, emp);
                Hide();
                dashboard.Show();
            }
            
        }
        private string getHash(HashAlgorithm algo, string text)
        {
            byte[] bytes = algo.ComputeHash(Encoding.UTF8.GetBytes(text));
            var sBuilder = new StringBuilder();
            foreach (var item in bytes)
            {
                sBuilder.Append(item.ToString("x2"));
            }
            return sBuilder.ToString();
        }
        private bool verifyHash(HashAlgorithm algo, string input, string hashedText)
        {
            var hashOutput = getHash(algo, input);
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;
            return comparer.Compare(hashOutput, hashedText) == 0;
        }
    }
}