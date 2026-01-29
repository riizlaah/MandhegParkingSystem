using MandhegParkingSystem.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace MandhegParkingSystem.Views
{
    /// <summary>
    /// Interaction logic for Dashboard.xaml
    /// </summary>
    public partial class Dashboard : Window
    {
        MandhegParkingSystemContext DBC;
        MainWindow mainWindow;
        public Employee employee;
        bool _logout = false;
        DispatcherTimer timer;
        public Dashboard(MandhegParkingSystemContext DBC, MainWindow mWindow, Employee employee)
        {
            this.DBC = DBC;
            this.employee = employee;
            this.mainWindow = mWindow;
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += clockTick;
            InitializeComponent();
            timer.Start();
            clockTick(null, null);
            greetLb.Text = $"Welcome, {employee.Name}";

        }
        private void clockTick(object sender, EventArgs e)
        {
            dateTimeLb.Text = DateTime.Now.ToString("dddd, dd MM yyyy, HH:mm:ss");
        }
        protected override void OnClosed(EventArgs e)
        {
            if(!_logout)
            {
                mainWindow.Close();
            }
            base.OnClosed(e);
        }

        private void Logout(object sender, RoutedEventArgs e)
        {
            _logout = true;
            mainWindow.Show();
            Close();
        }

        private void OnmasterMember(object sender, RoutedEventArgs e)
        {
            Hide();
            MasterMember MasterMember = new MasterMember(DBC, this);
            MasterMember.Show();
        }

        private void OnMasterVehicle(object sender, RoutedEventArgs e)
        {
            Hide();
            MasterVehicle masterVehicle = new MasterVehicle(DBC, this);
            masterVehicle.Show();
        }

        private void OnParkingPayment(object sender, RoutedEventArgs e)
        {
            Hide();
            ParkingPayment parkingPayment = new ParkingPayment(DBC, this);
            parkingPayment.Show();
        }
    }
}
