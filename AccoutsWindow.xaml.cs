using budget.contexts;
using budget.models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Windows;

namespace Budget
{
    public partial class AccoutsWindow : Window
    {
        private readonly OurDbContext _dbContext;

        public AccoutsWindow()
        {
            InitializeComponent();
            _dbContext = new OurDbContext();
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                MyGrid.ItemsSource = _dbContext.Accouts
                    .Include(a => a.Member)
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
                var addWindow = new AddEditAccoutWindow(_dbContext) { Owner = this };
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
            if (MyGrid.SelectedItem is not Accouts selected)
            {
                MessageBox.Show("Выберите запись для редактирования.", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var editWindow = new AddEditAccoutWindow(_dbContext, selected) { Owner = this };
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
            if (MyGrid.SelectedItem is not Accouts selected)
            {
                MessageBox.Show("Выберите запись для удаления.", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var linkedTransitions = _dbContext.Transitions
                .Where(t => t.IdAccout == selected.IdAccouts)
                .ToList();

            if (linkedTransitions.Count > 0)
            {
                var cascadeResult = MessageBox.Show(
                    $"Аккаунт \"{selected.NameBank}\" имеет {linkedTransitions.Count} связанных транзакций.\n\n" +
                    "Удалить аккаунт вместе со всеми связанными транзакциями?",
                    "Удаление связанных данных",
                    MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (cascadeResult != MessageBoxResult.Yes)
                    return;

                _dbContext.Transitions.RemoveRange(linkedTransitions);
            }
            else
            {
                var result = MessageBox.Show(
                    $"Вы уверены, что хотите удалить аккаунт: {selected.NameBank}?",
                    "Подтверждение удаления",
                    MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result != MessageBoxResult.Yes)
                    return;
            }

            try
            {
                var entity = _dbContext.Accouts.Find(selected.IdAccouts);
                if (entity is not null)
                {
                    _dbContext.Accouts.Remove(entity);
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
