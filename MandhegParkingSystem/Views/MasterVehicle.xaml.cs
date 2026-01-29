using System;
using System.Collections.Generic;
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
    public partial class MasterVehicle : Window
    {
        MandhegParkingSystemContext DBC;
        Dashboard dashboard;
        bool _inputting = false;
        bool _editing = false;
        int selectedMemberId = -1;
        public MasterVehicle(MandhegParkingSystemContext context, Dashboard dashb)
        {
            DBC = context;
            dashboard = dashb;
            InitializeComponent();
            RefreshData();
            vehicleType.ItemsSource = DBC.VehicleTypes.ToList();
            updateFields(false);
        }

        private void RefreshData(string src = "")
        {
            var includedQuery = DBC.Vehicles.Include(e => e.Member).Include(e => e.VehicleType);
            if (src.Length == 0 || string.IsNullOrWhiteSpace(src))
            {
                table1.ItemsSource = includedQuery.ToList();
            } else
            {
                if(searchBy.SelectedIndex == 0)
                {
                    table1.ItemsSource = includedQuery.Where(e => EF.Functions.Like(e.Member.Name, $"%{src}%")).ToList();
                } else
                {
                    table1.ItemsSource = includedQuery.Where(e => EF.Functions.Like(e.LicensePlate, $"%{src}%")).ToList();
                }
            }
        }

        private Vehicle getSelectedRow()
        {
            return table1.SelectedItem as Vehicle;
        }

        private void OnCancel(object sender, RoutedEventArgs e)
        {
            updateFields(false, true);
            _inputting = false;
            _editing = false;
        }
        // 
        private void OnSave(object sender, RoutedEventArgs _)
        {
            if(!Regex.IsMatch(licensePlate.Text, @"^[A-Z]{2}\s\d{4}\s[A-Z]{3}$"))
            {
                MessageBox.Show("License Plate is not valid!");
                return;
            }
            if (!Regex.IsMatch(owner.Text, @"^\d+\s-\s.*$"))
            {
                MessageBox.Show("Owner field is not valid!");
                return;
            }
            if(vehicleType.SelectedItem == null)
            {
                MessageBox.Show("Vehicle Type is not valid!");
                return;
            }
            if(_editing)
            {
                var e = DBC.Vehicles.FirstOrDefault(e => e.Id == getSelectedRow().Id);
                if (e != null)
                {
                    e.LicensePlate = licensePlate.Text;
                    e.VehicleTypeId = (int)vehicleType.SelectedValue;
                    e.MemberId = selectedMemberId;
                    e.Notes = note.Text;
                    e.LastUpdatedAt = DateTime.Now;
                }
            } else
            {
                DBC.Vehicles.Add(new Vehicle
                {
                    LicensePlate = licensePlate.Text,
                    VehicleTypeId = (int)vehicleType.SelectedValue,
                    MemberId = selectedMemberId,
                    Notes = note.Text,
                    LastUpdatedAt = DateTime.Now,
                });
            }
            DBC.SaveChanges();
            RefreshData();
            updateFields(false, true);
            _editing = false;
            _inputting = false;
        }

        private void OnInsert(object sender, RoutedEventArgs e)
        {
            _inputting = true;
            _editing = false;
            updateFields(true, true);
        }

        private void OnUpdate(object sender, RoutedEventArgs e)
        {
            if (getSelectedRow() == null) return;
            _editing = true;
            _inputting = true;
            selectedMemberId = getSelectedRow().MemberId;
            updateFields(true, false);
        }

        private void OnDelete(object sender, RoutedEventArgs _)
        {
            if (getSelectedRow() == null) return;
            MessageBoxResult res = MessageBox.Show($"Are you sure want to delete {getSelectedRow().LicensePlate}", "Confirmation", MessageBoxButton.YesNo);
            if(res == MessageBoxResult.Yes)
            {
                var e = DBC.Vehicles.Find(getSelectedRow().Id);
                DBC.Vehicles.Remove(e);
                DBC.SaveChanges();
                RefreshData();
                updateFields(false, true);
            }
        }

        private void rowSelected(object sender, SelectedCellsChangedEventArgs _)
        {
            if(!_inputting)
            {
                var e = getSelectedRow();
                if (e == null) return;
                var updatedAt = e.LastUpdatedAt.GetValueOrDefault(DateTime.Now).ToString("yyyy-MM-dd, HH:mm:ss");
                lastUpdated.Text = $"This record is last modified at {updatedAt}";
                licensePlate.Text = e.LicensePlate;
                vehicleType.SelectedValue = e.VehicleTypeId;
                note.Text = e.Notes;
                owner.Text = $"{e.MemberId} - {e.Member.Name}";
                updateBtn.IsEnabled = true;
                deleteBtn.IsEnabled = true;
            }
        }
        private void updateFields(bool enabled = true, bool clearFields = false)
        {
            if(clearFields)
            {
                licensePlate.Text = ""; vehicleType.SelectedIndex = -1;
                note.Text = ""; owner.Text = "";
            }
            licensePlate.IsReadOnly = !enabled;
            note.IsReadOnly = !enabled;
            owner.IsReadOnly = !enabled;
            vehicleType.IsEnabled = enabled;
            // buttons
            saveBtn.IsEnabled = enabled;
            cancelBtn.IsEnabled = enabled;
            updateBtn.IsEnabled = !enabled && getSelectedRow() != null;
            deleteBtn.IsEnabled = !enabled && getSelectedRow() != null;
            insertBtn.IsEnabled = !enabled;
        }
        protected override void OnClosed(EventArgs e)
        {
            dashboard.Show();
            base.OnClosed(e);
        }

        private void OnTrySearch(object sender, TextChangedEventArgs e)
        {
            RefreshData(searchInp.Text);
        }

        private void OnMemberSearch(object sender, TextChangedEventArgs e)
        {
            var src = owner.Text;
            if(src.Length == 0 || string.IsNullOrWhiteSpace(src))
            {
                if (list1.IsVisible) hideList();
                return;
            }
            var members = DBC.Members.Where(e => EF.Functions.Like(e.Name, $"%{src}%")).ToList();
            showList(members);
        }
        private void showList(List<Member> members)
        {
            if (members.Any())
            {
                list1.ItemsSource = members;
                list1.Visibility = Visibility.Visible;
                list1.SelectedIndex = 0;
            } else
            {
                hideList();
            }
        }
        private void hideList()
        {
            list1.Visibility = Visibility.Collapsed;
        }
        private void ApplySuggestion()
        {
            if (list1.SelectedItem == null) return;
            Member mem = list1.SelectedItem as Member;
            owner.Text = $"{mem.Id} - {mem.Name}";
            selectedMemberId = mem.Id;
            hideList();
        }

        private void OnList1KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    hideList();
                    e.Handled = true;
                    break;
                case Key.Enter:
                case Key.Tab:
                    ApplySuggestion();
                    e.Handled = true;
                    break;
            }
        }

        private void OnList1DoubleClick(object sender, MouseButtonEventArgs e)
        {
            ApplySuggestion();
        }

        private void OnOwnerKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key) {
                case Key.Enter:
                case Key.Tab:
                    ApplySuggestion();
                    e.Handled = true;
                    break;
                case Key.Down:
                    list1.Focus();
                    list1.SelectedIndex = 0;
                    e.Handled = true;
                    break;
            }
        }

        private void OnOwnerLostFocus(object sender, RoutedEventArgs e)
        {
            if (selectedMemberId == -1)
            {
                owner.Text = "";
                return;
            }
            if (list1.SelectedItem == null)
            {
                owner.Text = "";
                return;
            }
            if ((list1.SelectedItem as Member).Id != selectedMemberId)
            {
                owner.Text = "";
                return;
            }
        }
    }
}
