using budget.contexts;
using budget.models;
using System;
using System.Linq;
using System.Windows;

namespace Budget
{
    public partial class AddEditAccoutWindow : Window
    {
        private readonly OurDbContext _dbContext;
        private readonly Accouts? _editingAccout;
        private readonly bool _isEditMode;

        public AddEditAccoutWindow(OurDbContext dbContext, Accouts? accoutToEdit = null)
        {
            InitializeComponent();
            _dbContext = dbContext;
            _editingAccout = accoutToEdit;
            _isEditMode = accoutToEdit != null;

            Title = _isEditMode ? "Редактировать аккаунт" : "Добавить аккаунт";

            LoadMembers();

            if (_isEditMode && _editingAccout != null)
            {
                NameBankTextBox.Text = _editingAccout.NameBank ?? "";
                BalanceTextBox.Text = _editingAccout.Balance.ToString();
                MemberComboBox.SelectedValue = _editingAccout.IdMember;
            }
        }

        private void LoadMembers()
        {
            try
            {
                MemberComboBox.ItemsSource = _dbContext.Family.ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки членов семьи:\n{ex.InnerException?.Message ?? ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveClick(object sender, RoutedEventArgs e)
        {
            if (MemberComboBox.SelectedItem is not Family selectedMember)
            {
                MessageBox.Show("Выберите члена семьи.", "Ошибка ввода",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(BalanceTextBox.Text, out decimal balance))
            {
                MessageBox.Show("Баланс должен быть числом.", "Ошибка ввода",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                if (_isEditMode && _editingAccout != null)
                {
                    var entity = _dbContext.Accouts.Find(_editingAccout.IdAccouts);
                    if (entity != null)
                    {
                        entity.IdMember = selectedMember.IdMember;
                        entity.NameBank = string.IsNullOrWhiteSpace(NameBankTextBox.Text) ? null : NameBankTextBox.Text.Trim();
                        entity.Balance = balance;
                    }
                }
                else
                {
                    _dbContext.Accouts.Add(new Accouts
                    {
                        IdMember = selectedMember.IdMember,
                        NameBank = string.IsNullOrWhiteSpace(NameBankTextBox.Text) ? null : NameBankTextBox.Text.Trim(),
                        Balance = balance
                    });
                }

                _dbContext.SaveChanges();
                DialogResult = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения:\n{ex.InnerException?.Message ?? ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
