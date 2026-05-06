using budget.contexts;
using budget.models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Windows;

namespace Budget
{
    public partial class BudgetWindow : Window
    {
        private readonly OurDbContext _dbContext;

        public BudgetWindow()
        {
            InitializeComponent();
            _dbContext = new OurDbContext();
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                MyGrid.ItemsSource = _dbContext.Budgets
                    .Include(b => b.Kategory)
                    .OrderBy(b => b.Date)
                    .ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных:\n{ex.InnerException?.Message ?? ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var addWindow = new AddEditBudgetWindow(_dbContext) { Owner = this };
                if (addWindow.ShowDialog() == true)
                {
                    LoadData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка открытия окна:\n{ex.InnerException?.Message ?? ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EditClick(object sender, RoutedEventArgs e)
        {
            if (MyGrid.SelectedItem is not Budgets selected)
            {
                MessageBox.Show("Выберите запись для редактирования.", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var editWindow = new AddEditBudgetWindow(_dbContext, selected) { Owner = this };
                if (editWindow.ShowDialog() == true)
                {
                    LoadData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка открытия окна:\n{ex.InnerException?.Message ?? ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteClick(object sender, RoutedEventArgs e)
        {
            if (MyGrid.SelectedItem is not Budgets selected)
            {
                MessageBox.Show("Выберите запись для удаления.", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show(
                $"Вы уверены, что хотите удалить бюджет от {selected.Date:dd.MM.yyyy}?",
                "Подтверждение удаления",
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
                return;

            try
            {
                var entity = _dbContext.Budgets.Find(selected.IdBudget);
                if (entity is not null)
                {
                    _dbContext.Budgets.Remove(entity);
                    _dbContext.SaveChanges();
                    LoadData();
                    MessageBox.Show("Запись удалена.", "Информация",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Запись не найдена в базе данных. Возможно, она уже удалена.",
                        "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                    LoadData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка удаления:\n{ex.InnerException?.Message ?? ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RefreshClick(object sender, RoutedEventArgs e)
        {
            LoadData();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            _dbContext?.Dispose();
        }
    }
}
