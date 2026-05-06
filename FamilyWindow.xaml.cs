using budget.contexts;
using budget.models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Windows;

namespace Budget
{
    public partial class FamilyWindow : Window
    {
        private readonly OurDbContext _dbContext;

        public FamilyWindow()
        {
            InitializeComponent();
            _dbContext = new OurDbContext();
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                MyGrid.ItemsSource = _dbContext.Family.ToList();
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
                var addWindow = new AddEditFamilyWindow(_dbContext)
                {
                    Owner = this
                };
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
            if (MyGrid.SelectedItem is not Family selected)
            {
                MessageBox.Show("Выберите запись для редактирования.", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var editWindow = new AddEditFamilyWindow(_dbContext, selected)
                {
                    Owner = this
                };
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
            if (MyGrid.SelectedItem is not Family selected)
            {
                MessageBox.Show("Выберите запись для удаления.", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var linkedAccounts = _dbContext.Accouts
                .Where(a => a.IdMember == selected.IdMember)
                .ToList();

            var linkedTransitions = _dbContext.Transitions
                .Where(t => linkedAccounts.Select(a => a.IdAccouts).Contains(t.IdAccout))
                .ToList();

            int totalCount = linkedAccounts.Count + linkedTransitions.Count;

            if (totalCount > 0)
            {
                var cascadeResult = MessageBox.Show(
                    $"Член семьи \"{selected.LastName} {selected.FirstName}\" имеет:\n" +
                    $"- {linkedAccounts.Count} аккаунтов\n" +
                    $"- {linkedTransitions.Count} транзакций\n\n" +
                    "Удалить всё связанное вместе с членом семьи?",
                    "Удаление связанных данных",
                    MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (cascadeResult != MessageBoxResult.Yes)
                    return;

                _dbContext.Transitions.RemoveRange(linkedTransitions);
                _dbContext.Accouts.RemoveRange(linkedAccounts);
            }
            else
            {
                var result = MessageBox.Show(
                    $"Вы уверены, что хотите удалить члена семьи: {selected.LastName} {selected.FirstName}?",
                    "Подтверждение удаления",
                    MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result != MessageBoxResult.Yes)
                    return;
            }

            try
            {
                var entity = _dbContext.Family.Find(selected.IdMember);
                if (entity is not null)
                {
                    _dbContext.Family.Remove(entity);
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
