using budget.contexts;
using budget.models;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Budget
{
    public partial class AddEditKategoryWindow : Window
    {
        private readonly OurDbContext _dbContext;
        private readonly Kategories? _editingKategory;
        private readonly bool _isEditMode;

        public AddEditKategoryWindow(OurDbContext dbContext, Kategories? kategoryToEdit = null)
        {
            InitializeComponent();
            _dbContext = dbContext;
            _editingKategory = kategoryToEdit;
            _isEditMode = kategoryToEdit != null;

            Title = _isEditMode ? "Редактировать категорию" : "Добавить категорию";

            if (_isEditMode && _editingKategory != null)
            {
                NameKategoryTextBox.Text = _editingKategory.NameKategory ?? "";
                TypeComboBox.SelectedIndex = _editingKategory.IsIncome ? 1 : 0;
            }
        }

        private void SaveClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameKategoryTextBox.Text))
            {
                MessageBox.Show("Введите название категории.", "Ошибка ввода",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (TypeComboBox.SelectedItem is not ComboBoxItem selectedItem)
            {
                MessageBox.Show("Выберите тип категории.", "Ошибка ввода",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                bool isIncome = selectedItem.Tag?.ToString() == "True";

                if (_isEditMode && _editingKategory != null)
                {
                    var entity = _dbContext.Kategories.Find(_editingKategory.IdKategory);
                    if (entity != null)
                    {
                        entity.NameKategory = NameKategoryTextBox.Text.Trim();
                        entity.IsIncome = isIncome;
                    }
                }
                else
                {
                    _dbContext.Kategories.Add(new Kategories
                    {
                        NameKategory = NameKategoryTextBox.Text.Trim(),
                        IsIncome = isIncome
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
