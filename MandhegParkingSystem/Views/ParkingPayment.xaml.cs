using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MandhegParkingSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace MandhegParkingSystem.Views
{
    /// <summary>
    /// Interaction logic for MasterMember.xaml
    /// </summary>
    public partial class ParkingPayment : Window
    {
        MandhegParkingSystemContext DBC;
        Dashboard dashboard;
        Vehicle? currVehicle;
        HourlyRate HRate;
        public ParkingPayment(MandhegParkingSystemContext context, Dashboard dashb)
        {
            DBC = context;
            dashboard = dashb;
            InitializeComponent();
            vehicleType.ItemsSource = DBC.VehicleTypes.Include(v => v.HourlyRates).ThenInclude(h => h.Membership).ToList();
            vehicleType.SelectedIndex = 0;
            clearFields();
            OnVehicleTypeChanged(null, null);
        }

        private void OnLPlateChanged(object sender, TextChangedEventArgs e)
        {
            var src = licensePlate.Text.Trim();
            if (src.Length == 0 || string.IsNullOrWhiteSpace(src)) return;
            var vehicle = DBC.Vehicles.Include(v => v.Member).ThenInclude(m => m.Membership).Where(v => v.LicensePlate == src).FirstOrDefault();
            if (vehicle != null)
            {
                vehicleType.SelectedValue = vehicle.VehicleTypeId;
                vehicleType.IsEnabled = false;
                owner.Text = vehicle.Member.Name;
                memberType.Text = vehicle.Member.Membership.Name;
                currVehicle = vehicle;
            } else
            {
                currVehicle = null;
                vehicleType.SelectedIndex = 0;
                vehicleType.IsEnabled = true;
                owner.Text = "";
                memberType.Text = "";
            }
        }

        private void OnSubmit(object sender, RoutedEventArgs e)
        {
            if(!Regex.IsMatch(licensePlate.Text, @"^[A-Z]{2}\s\d{4}\s[A-Z]{3}$"))
            {
                MessageBox.Show("License Plate is not valid!");
                return;
            }
            if (vehicleType.SelectedItem == null)
            {
                MessageBox.Show("Vehicle Type is required!");
                return;
            }
            if (inTime.SelectedDate == null)
            {
                MessageBox.Show("In time is required!");
                return;
            }
            if (outTime.SelectedDate == null)
            {
                MessageBox.Show("Out time is required!");
                return;
            }
            var parkingData = new ParkingDatum
            {
                LicensePlate = licensePlate.Text,
                EmployeeId = dashboard.employee.Id,
                HourlyRatesId = HRate.Id,
                DatetimeIn = inTime.SelectedDate ?? DateTime.Now,
                DatetimeOut = outTime.SelectedDate ?? DateTime.Now,
                AmountToPay = calculateAmount(),
            };
            if(currVehicle != null)
            {
                parkingData.VehicleId = currVehicle.Id;
            }
            DBC.ParkingData.Add(parkingData);
            DBC.SaveChanges();
            clearFields();
        }

        private void OnVehicleTypeChanged(object sender, SelectionChangedEventArgs e)
        {
            var vType = vehicleType.SelectedItem as VehicleType;
            HRate = vType.HourlyRates.Where(h => h.MembershipId == (currVehicle?.Member.MembershipId ?? 1)).First();
            hourlyRate.Text = string.Format("{0:Rp#,##0;(Rp#,##0);''}", HRate.Value);
            calculateAmount();
        }

        private void OnInTimeChanged(object sender, SelectionChangedEventArgs e)
        {
            if (outTime.SelectedDate != null && inTime.SelectedDate != null)
            {
                duration.Text = Convert.ToInt32((outTime.SelectedDate - inTime.SelectedDate)?.TotalHours).ToString();
                calculateAmount();
            }
        }

        private void OnOutTimeChanged(object sender, SelectionChangedEventArgs e)
        {
            if (outTime.SelectedDate != null && inTime.SelectedDate != null)
            {
                //if()
                duration.Text = Convert.ToInt32((outTime.SelectedDate - inTime.SelectedDate)?.TotalHours).ToString();
                calculateAmount();
            }
        }
        private decimal calculateAmount()
        {
            if(!int.TryParse(duration.Text, out int duration1)) return 0;
            decimal totalAmount = HRate.Value * duration1;
            amountToPay.Text = string.Format("{0:Rp#,##0;(Rp#,##0);''}", totalAmount);
            return totalAmount;
        }
        private void clearFields()
        {
            licensePlate.Text = "";
            owner.Text = "";
            memberType.Text = "";
            hourlyRate.Text = "";
            duration.Text = "";
            inTime.SelectedDate = DateTime.Now;
            inTimeDetail.Text = $"{DateTime.Now.Hour:00}:{DateTime.Now.Minute:00}";
            outTime.SelectedDate = DateTime.Now.AddHours(1);
            outTimeDetail.Text = $"{DateTime.Now.Hour + 1:00}:{DateTime.Now.Minute:00}";
        }
        protected override void OnClosed(EventArgs e)
        {
            dashboard.Show();
            base.OnClosed(e);
        }

        private void OnInTimeDetailLost(object sender, RoutedEventArgs e)
        {
            if(!Regex.IsMatch(inTimeDetail.Text, @"^\d{2}:\d{2}$"))
            {
                inTimeDetail.Text = "00:00";
                return;
            }
        }

        private void OnOutTimeDetailLost(object sender, RoutedEventArgs e)
        {
            if (!Regex.IsMatch(outTimeDetail.Text, @"^\d{2}:\d{2}$"))
            {
                outTimeDetail.Text = "01:00";
                return;
            }
        }

        private void OnInTimeDetailChanged(object sender, TextChangedEventArgs e)
        {
            SetDateNTime(inTime, inTimeDetail);
        }

        private void OnOutTimeDetailChanged(object sender, TextChangedEventArgs e)
        {
            SetDateNTime(outTime, outTimeDetail);
        }
        private void SetDateNTime(DatePicker datePicker, TextBox inputText)
        {
            if (Regex.IsMatch(inputText.Text, @"^\d{2}:\d{2}$"))
            {
                var array = inputText.Text.Split(':');
                var date = datePicker.SelectedDate?.Date;
                date = date?.AddHours(int.Parse(array[0]));
                date = date?.AddMinutes(int.Parse(array[1]));
                datePicker.SelectedDate = date;
            }
        }
    }
}
