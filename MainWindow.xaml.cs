using System.Windows;

namespace Budget
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Accounts(object sender, RoutedEventArgs e)
        {
            AccoutsWindow accouts = new();
            accouts.Show();
        }

        private void BudgetsClick(object sender, RoutedEventArgs e)
        {
            BudgetWindow budgetWindow = new();
            budgetWindow.Show();
        }

        private void Families(object sender, RoutedEventArgs e)
        {
            FamilyWindow family = new();
            family.Show();
        }

        private void KategoriesClick(object sender, RoutedEventArgs e)
        {
            KategoriesWindow kategories = new();
            kategories.Show();
        }

        private void TransitionsClick(object sender, RoutedEventArgs e)
        {
            TransitionsWindow transitions = new();
            transitions.Show();
        }
    }
}
