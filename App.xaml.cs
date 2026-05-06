using System.Windows;

namespace Budget
{
    public partial class App : Application
    {
        public App()
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        }
    }
}
