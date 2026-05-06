using budget.contexts;
using budget.models;
using System;
using System.Windows;

namespace Budget
{
    public partial class AddEditFamilyWindow : Window
    {
        private readonly OurDbContext _dbContext;
        private readonly Family? _editingFamily;
        private readonly bool _isEditMode;

        public AddEditFamilyWindow(OurDbContext dbContext, Family? familyToEdit = null)
        {
            InitializeComponent();
            _dbContext = dbContext;
            _editingFamily = familyToEdit;
            _isEditMode = familyToEdit != null;

            Title = _isEditMode ? "Редактировать члена семьи" : "Добавить члена семьи";

            if (_isEditMode && _editingFamily != null)
            {
                LastNameTextBox.Text = _editingFamily.LastName;
                FirstNameTextBox.Text = _editingFamily.FirstName;
                MiddleNameTextBox.Text = _editingFamily.MiddleName ?? "";
                PhoneTextBox.Text = _editingFamily.NumberPhone ?? "";
                EmailTextBox.Text = _editingFamily.Email ?? "";
            }
        }

        private void SaveClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(LastNameTextBox.Text))
            {
                MessageBox.Show("Введите фамилию.", "Ошибка ввода",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(FirstNameTextBox.Text))
            {
                MessageBox.Show("Введите имя.", "Ошибка ввода",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                if (_isEditMode && _editingFamily != null)
                {
                    var entity = _dbContext.Family.Find(_editingFamily.IdMember);
                    if (entity != null)
                    {
                        entity.LastName = LastNameTextBox.Text.Trim();
                        entity.FirstName = FirstNameTextBox.Text.Trim();
                        entity.MiddleName = string.IsNullOrWhiteSpace(MiddleNameTextBox.Text) ? null : MiddleNameTextBox.Text.Trim();
                        entity.NumberPhone = string.IsNullOrWhiteSpace(PhoneTextBox.Text) ? null : PhoneTextBox.Text.Trim();
                        entity.Email = string.IsNullOrWhiteSpace(EmailTextBox.Text) ? null : EmailTextBox.Text.Trim();
                    }
                }
                else
                {
                    _dbContext.Family.Add(new Family
                    {
                        LastName = LastNameTextBox.Text.Trim(),
                        FirstName = FirstNameTextBox.Text.Trim(),
                        MiddleName = string.IsNullOrWhiteSpace(MiddleNameTextBox.Text) ? null : MiddleNameTextBox.Text.Trim(),
                        NumberPhone = string.IsNullOrWhiteSpace(PhoneTextBox.Text) ? null : PhoneTextBox.Text.Trim(),
                        Email = string.IsNullOrWhiteSpace(EmailTextBox.Text) ? null : EmailTextBox.Text.Trim()
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
