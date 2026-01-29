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
    public partial class MasterMember : Window
    {
        MandhegParkingSystemContext DBC;
        Dashboard dashboard;
        bool _inputting = false;
        bool _editing = false;
        public MasterMember(MandhegParkingSystemContext context, Dashboard dashb)
        {
            DBC = context;
            dashboard = dashb;
            InitializeComponent();
            RefreshData();
            membership.ItemsSource = DBC.Memberships.ToList();
            updateFields(false);
        }

        private void RefreshData()
        {
            table1.ItemsSource = DBC.Members.Include(e => e.Membership).ToList();
        }

        private Member getSelectedRow()
        {
            return table1.SelectedItem as Member;
        }

        private void OnCancel(object sender, RoutedEventArgs e)
        {
            updateFields(false, true);
            _inputting = false;
            _editing = false;
        }

        private void OnSave(object sender, RoutedEventArgs _)
        {
            if(name.Text.Length == 0)
            {
                MessageBox.Show("Name is required!");
                return;
            }
            if (!email.Text.Contains('@'))
            {
                MessageBox.Show("Email is not valid!");
                return;
            }
            if (addr.Text.Length == 0)
            {
                MessageBox.Show("Address is required!");
                return;
            }
            if (!Regex.IsMatch(phoneNum.Text, @"^\d{1,2}-\d{3}-\d{3}-\d{4}$"))
            {
                MessageBox.Show("Phone Number is not valid!");
                return;
            }
            if(membership.SelectedItem == null)
            {
                MessageBox.Show("Membership is not valid!");
                return;
            }
            if (dateOfBirth.SelectedDate == null)
            {
                MessageBox.Show("Date of Birth is not valid!");
                return;
            }
            if(!male.IsChecked.GetValueOrDefault(false) && !female.IsChecked.GetValueOrDefault(false))
            {
                MessageBox.Show("Gender is not valid!");
                return;
            }
            if(_editing)
            {
                var e = DBC.Members.FirstOrDefault(e => e.Id == getSelectedRow().Id);
                if (e != null)
                {
                    e.Name = name.Text;
                    e.Email = email.Text;
                    e.PhoneNumber = phoneNum.Text;
                    e.Address = addr.Text;
                    e.MembershipId = (int)membership.SelectedValue;
                    e.DateOfBirth = DateOnly.FromDateTime(dateOfBirth.SelectedDate ?? DateTime.Now);
                    e.Gender = male.IsChecked.GetValueOrDefault(false) ? "Male" : "Female";
                    e.LastUpdatedAt = DateTime.Now;
                }
            } else
            {
                DBC.Members.Add(new Member
                {
                    Name = name.Text,
                    PhoneNumber = phoneNum.Text,
                    Email = email.Text,
                    Address = addr.Text,
                    MembershipId = (int)membership.SelectedValue,
                    DateOfBirth = DateOnly.FromDateTime(dateOfBirth.SelectedDate ?? DateTime.Now),
                    Gender = male.IsChecked.GetValueOrDefault(false) ? "Male" : "Female",
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
            _editing = true;
            _inputting = true;
            updateFields(true, false);
        }

        private void OnDelete(object sender, RoutedEventArgs _)
        {
            MessageBoxResult res = MessageBox.Show($"Are you sure want delete {getSelectedRow().Name}", "Confirmation", MessageBoxButton.YesNo);
            if(res == MessageBoxResult.Yes)
            {
                var e = DBC.Members.Find(getSelectedRow().Id);
                DBC.Members.Remove(e);
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
                name.Text = e.Name;
                email.Text = e.Email;
                membership.SelectedValue = e.MembershipId;
                addr.Text = e.Address;
                phoneNum.Text = e.PhoneNumber;
                dateOfBirth.SelectedDate = e.DateOfBirth.ToDateTime(TimeOnly.MinValue);
                male.IsChecked = e.Gender == "Male";
                female.IsChecked = !male.IsChecked;
                updateBtn.IsEnabled = true;
                deleteBtn.IsEnabled = true;
            }
        }
        private void updateFields(bool enabled = true, bool clearFields = false)
        {
            if(clearFields)
            {
                name.Text = ""; email.Text = ""; membership.SelectedIndex = -1;
                addr.Text = ""; phoneNum.Text = ""; dateOfBirth.SelectedDate = new DateTime(2000, 1, 1);
                male.IsChecked = false; female.IsChecked = false;
            }
            name.IsReadOnly = !enabled;
            email.IsReadOnly = !enabled;
            addr.IsReadOnly = !enabled;
            phoneNum.IsReadOnly = !enabled;
            membership.IsEnabled = enabled;
            dateOfBirth.IsEnabled = enabled;
            male.IsEnabled = enabled;
            female.IsEnabled = enabled;
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
    }
}
