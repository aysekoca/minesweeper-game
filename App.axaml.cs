using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using MayinProje;

namespace MayınProje
{ 
    public partial class App : Application // App sınıfı uygulamanın başlangıç ayarlarını yapar.
    {
        public override void Initialize() // Uygulama başladığında bu fonksiyon çalışır ve arayüzleri yükler.
        {
            AvaloniaXamlLoader.Load(this);  // Arayüzü (XAML) yükler.
        }
        string filePath = "Desktop/mayin.txt"; // Skorların kaydedileceği dosyanın yolu.


        public override void OnFrameworkInitializationCompleted() // Uygulama çalışmaya hazır olduğunda bu fonksiyon çalışır.
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                
                desktop.MainWindow = new MainWindow();

               // Skor tablosu için gerekli nesneleri oluştur.
                var skorboard = new Skorboard(filePath);
                var scoreboardWindow = new SkorboardForm(skorboard);

                
                scoreboardWindow.Show();
            }
            
            //uygulama başlatılabilir.
            base.OnFrameworkInitializationCompleted();
        }
    }
}
