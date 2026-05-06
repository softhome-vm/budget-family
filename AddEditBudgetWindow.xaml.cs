using budget.contexts;
using budget.models;
using System;
using System.Linq;
using System.Windows;

namespace Budget
{
    public partial class AddEditBudgetWindow : Window
    {
        private readonly OurDbContext _dbContext;
        private readonly Budgets? _editingBudget;
        private readonly bool _isEditMode;

        public AddEditBudgetWindow(OurDbContext dbContext, Budgets? budgetToEdit = null)
        {
            InitializeComponent();
            _dbContext = dbContext;
            _editingBudget = budgetToEdit;
            _isEditMode = budgetToEdit != null;

            Title = _isEditMode ? "Редактировать бюджет" : "Добавить бюджет";

            LoadCategories();

            DatePicker.SelectedDate = DateTime.Today;

            if (_isEditMode && _editingBudget != null)
            {
                LimitTextBox.Text = _editingBudget.Limit.ToString();
                DatePicker.SelectedDate = _editingBudget.Date;
                CategoryComboBox.SelectedValue = _editingBudget.IdKategory;
            }
        }

        private void LoadCategories()
        {
            try
            {
                CategoryComboBox.ItemsSource = _dbContext.Kategories
                    .Where(k => !k.IsIncome)
                    .ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки категорий:\n{ex.InnerException?.Message ?? ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveClick(object sender, RoutedEventArgs e)
        {
            if (CategoryComboBox.SelectedItem is not Kategories selectedCategory)
            {
                MessageBox.Show("Выберите категорию.", "Ошибка ввода",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (DatePicker.SelectedDate is null)
            {
                MessageBox.Show("Выберите дату.", "Ошибка ввода",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(LimitTextBox.Text, out decimal limit))
            {
                MessageBox.Show("Лимит должен быть числом.", "Ошибка ввода",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                if (_isEditMode && _editingBudget != null)
                {
                    var entity = _dbContext.Budgets.Find(_editingBudget.IdBudget);
                    if (entity != null)
                    {
                        entity.IdKategory = selectedCategory.IdKategory;
                        entity.Date = DatePicker.SelectedDate.Value;
                        entity.Limit = limit;
                    }
                }
                else
                {
                    _dbContext.Budgets.Add(new Budgets
                    {
                        IdKategory = selectedCategory.IdKategory,
                        Date = DatePicker.SelectedDate.Value,
                        Limit = limit
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
