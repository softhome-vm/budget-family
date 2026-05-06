using budget.contexts;
using budget.models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Windows;

namespace Budget
{
    public partial class AddEditTransitionWindow : Window
    {
        private readonly OurDbContext _dbContext;
        private readonly Transitions? _editingTransition;
        private readonly bool _isEditMode;

        public AddEditTransitionWindow(OurDbContext dbContext, Transitions? transitionToEdit = null)
        {
            InitializeComponent();
            _dbContext = dbContext;
            _editingTransition = transitionToEdit;
            _isEditMode = transitionToEdit != null;

            Title = _isEditMode ? "Редактировать транзакцию" : "Добавить транзакцию";

            LoadComboBoxes();

            DatePicker.SelectedDate = DateTime.Today;

            if (_isEditMode && _editingTransition != null)
            {
                AmountTextBox.Text = _editingTransition.Amount.ToString();
                DatePicker.SelectedDate = _editingTransition.Date;
                DescriptionTextBox.Text = _editingTransition.Description ?? "";
                AccountComboBox.SelectedValue = _editingTransition.IdAccout;
                CategoryComboBox.SelectedValue = _editingTransition.IdKategories;
            }
        }

        private void LoadComboBoxes()
        {
            try
            {
                AccountComboBox.ItemsSource = _dbContext.Accouts
                    .Include(a => a.Member)
                    .ToList();

                CategoryComboBox.ItemsSource = _dbContext.Kategories.ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных:\n{ex.InnerException?.Message ?? ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveClick(object sender, RoutedEventArgs e)
        {
            if (AccountComboBox.SelectedItem is not Accouts selectedAccount)
            {
                MessageBox.Show("Выберите аккаунт.", "Ошибка ввода",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (CategoryComboBox.SelectedItem is not Kategories selectedCategory)
            {
                MessageBox.Show("Выберите категорию.", "Ошибка ввода",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(AmountTextBox.Text, out decimal amount))
            {
                MessageBox.Show("Сумма должна быть числом.", "Ошибка ввода",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (DatePicker.SelectedDate is null)
            {
                MessageBox.Show("Выберите дату.", "Ошибка ввода",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                if (_isEditMode && _editingTransition != null)
                {
                    var entity = _dbContext.Transitions.Find(_editingTransition.IdTransitions);
                    if (entity != null)
                    {
                        entity.IdAccout = selectedAccount.IdAccouts;
                        entity.IdKategories = selectedCategory.IdKategory;
                        entity.Amount = amount;
                        entity.Date = DatePicker.SelectedDate.Value;
                        entity.Description = string.IsNullOrWhiteSpace(DescriptionTextBox.Text) ? null : DescriptionTextBox.Text.Trim();
                    }
                }
                else
                {
                    _dbContext.Transitions.Add(new Transitions
                    {
                        IdAccout = selectedAccount.IdAccouts,
                        IdKategories = selectedCategory.IdKategory,
                        Amount = amount,
                        Date = DatePicker.SelectedDate.Value,
                        Description = string.IsNullOrWhiteSpace(DescriptionTextBox.Text) ? null : DescriptionTextBox.Text.Trim()
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
