using Avalonia;
using System;

namespace MayınProje;

class Program
{
    
    [STAThread]
    public static void Main(string[] args) => BuildAvaloniaApp()
        .StartWithClassicDesktopLifetime(args); // Uygulama başlatılıyor.

     // Avalonia yapılandırması
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
        
}
