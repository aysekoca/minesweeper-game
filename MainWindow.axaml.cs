using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Input;
using System;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Threading;

namespace MayinProje
{
    public partial class MainWindow : Window
    {
        private Oyun oyun;
        private int hamle_sayac = 0;// Hamle sayısını takip eder
        private int gridWidth;
        private int gridHeight;
        private int boyut; // Oyun alanının boyutu
        private int mayinSayisi; 
        private bool[,] openedCells; //  hangı hücrenın açılacağı
        private bool[,] flaggedCells; // Bayrak eklenen hücreleri takip eder
        private Skorboard skorboard; // Skorboard değişkeni burada tanımlandı
        string dosya_yolu = "skorlar.txt"; // Skorların kaydedileceği dosya


        public MainWindow()
        {
            InitializeComponent(); // Arayüz bileşenlerini yükler
            InitializeSkorboard(); // Skorboard'ı başlatır

            openedCells = new bool[gridWidth, gridHeight];// Açılan hücreleri başlat
            flaggedCells = new bool[gridWidth, gridHeight]; // Bayrak eklenen hücreleri başlat
            oyun = new Oyun(this, boyut, mayinSayisi); // MainWindow referansı ile oluşturuldu. // Yeni bir oyun başlat

        }

        private void InitializeSkorboard()
        {
            skorboard = new Skorboard(dosya_yolu); // Skorboard nesnesini oluştur

            ShowMessage("Skorboard oluşturuldu.");
        }
            // Başla butonuna tıklanınca çalışır
        private async void BtnBasla_OnClick(object? sender, RoutedEventArgs e) //Kullanıcıdan Veri Alır
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtPlayer.Text))
                {
                    await ShowMessage("Oyuncu adını giriniz");
                    return;
                }

                // Kullanıcıdan boyutu al
                if (string.IsNullOrWhiteSpace(txtBoyut.Text) || !int.TryParse(txtBoyut.Text, out int boyut) || boyut < 5)
                {
                    await ShowMessage("Oyun boyutu en az 5 olmalıdır.");
                    return;
                }

                if (MayinSayisi.Value == null)
                {
                    await ShowMessage("Mayın sayısı seçilmelidir.");
                    return;
                }

                int mayinSayisi = (int)MayinSayisi.Value;

                // Oyunu Başlat
                oyun = new Oyun(this, boyut,mayinSayisi);
                gridWidth = boyut;
                gridHeight = boyut;

                // // Hücre dizilerini başlat
                openedCells = new bool[gridWidth, gridHeight];
                flaggedCells = new bool[gridWidth, gridHeight];

                // Oyun alanını oluştur
                OyunGridOlustur(oyun);
                txtSonuc.Text = $"{txtPlayer.Text} için {boyut}x{boyut} boyutunda bir oyun alanı oluşturuldu.";
                gameGrid.IsVisible = true; // Make the grid visible
            }
            catch (FormatException)
            {
                await ShowMessage("Geçersiz boyut girişi. Lütfen sayısal bir değer girin.");
            }
        }

private void OyunGridOlustur(Oyun oyun)
{
    hamle_sayac = 0;
    txtMoveCounter.Text = "Hamle Sayısı: 0"; // Hamle sayısını ekrana yaz
    gameGrid.Children.Clear();  // Önceki oyun alanını temizle
    gameGrid.RowDefinitions.Clear();
    gameGrid.ColumnDefinitions.Clear();

    double _maxDisplaySize = 500; // Maksimum ekran boyutu
    double paddingFactor = 0.95; // Butonlar için kenar boşluğu
    int buttonCount = oyun.Boyut; // Oyun boyutunu belirle

    // Buton boyutunu dinamik olarak hesapla
    double buttonSize = Math.Min(_maxDisplaySize / buttonCount, 30); // Düğme boyutunu ayarla
    double fontSize = (buttonSize > 30) ? buttonSize * 0.6 : 12; // Minimum font boyutu 12px

    if (oyun.Boyut > 20)
    {
        buttonSize *= 0.8; // Boyutu %20 küçült
        fontSize = Math.Max(fontSize * 0.4, 12); // Font boyutunu da %20 azalt
    }

    // Oyun alanını merkeze yerleştir
    gameGrid.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center;
    gameGrid.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center;

    // Grid'i oluştur .  
    for (int i = 0; i < buttonCount; i++)
    {
        gameGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(buttonSize) });
        gameGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(buttonSize) });
    }
    // Her hücreye bir buton ekle
    for (int i = 0; i < buttonCount; i++)
    {
        for (int j = 0; j < buttonCount; j++)
        {
            int currentX = i;
            int currentY = j;

            Button btn = new Button
            {
                Content = "",
                Margin = new Thickness(1),
                Width = buttonSize,
                Height = buttonSize,
                FontSize = fontSize // Dinamik font boyutu
            };

            btn.Click += (s, e) => OyunHucresi_Click(currentX, currentY); // Sol tıklama için
            btn.PointerPressed += (s, e) =>
            {
                if (e.GetCurrentPoint(this).Properties.IsRightButtonPressed)
                {
                    ToggleFlag(currentX, currentY, btn); // Sağ tıklama için
                }
            };

            Grid.SetRow(btn, i);
            Grid.SetColumn(btn, j);
            gameGrid.Children.Add(btn);
        }
    }

    txtMoveCounter.IsVisible = true;  // Hamle sayısını görünür yap
}

        private void OyunHucresi_Click(int x, int y) //Kullanıcı bir hücreye tıkladığında çalışır.
        {   // Eğer oyun bitti ya da hücre bayraklıysa işlem yapma
            if (oyun.IsGameOver() || flaggedCells[x, y])
            {
                return;
            }
              // Hamle yap ve hamle sayısını artır
            if (oyun.HamleYap(x, y))
            {
                hamle_sayac++;
                txtMoveCounter.Text = $"Hamle Sayısı: {hamle_sayac}";

                var button = GetButtonAt(x, y);
                UpdateButtonContent(button, x, y);
               // Mayına basıldığında oyunu bitir
                if (oyun.oyun_alanı[x, y] == -1)
                {
                    RevealAllMines();
                    ShowMessage("Oyun Bitti! Mayını buldunuz.");
                    ButonGizle();
                    oyun.GameOver();
                    return;
                }

                // Hücre değeri 0 ise komşu hücreleri aç
                if (oyun.oyun_alanı[x, y] == 0)
                {
                    KomsuHücreler(x, y);
                }
                 // Eğer oyun tamamlandıysa
                if (oyun.IsGameOver())
                {
                    UpdateScoreboard(txtPlayer.Text,hamle_sayac); // Skor güncelle
                    RevealAllMines(); //RevealAllMines()
                    ShowMessage("Oyun Tamamlandı!");
                }
            }
        }

        private void KomsuHücreler(int x, int y)
        {   // Geçersiz hücrelerde işlem yapma
            if (x < 0 || x >= gridWidth || y < 0 || y >= gridHeight || openedCells[x, y])
            {
                return;
            }

            openedCells[x, y] = true; // Hücreyi açılmış olarak işaretle

            var button = GetButtonAt(x, y);
            if (button != null)
            {
                UpdateButtonContent(button, x, y);
            }
            // Hücre değeri 0 ise komşuları aç
            if (oyun.oyun_alanı[x, y] == 0)
            {
                KomsuHücreler(x + 1, y);
                KomsuHücreler(x - 1, y);
                KomsuHücreler(x, y + 1);
                KomsuHücreler(x, y - 1);
                KomsuHücreler(x + 1, y + 1);
                KomsuHücreler(x - 1, y - 1);
                KomsuHücreler(x + 1, y - 1);
                KomsuHücreler(x - 1, y + 1);
            }
        }

        private async void RevealAllMines()
        {   
             // Tüm mayınları açmak için UI güncellemesini sıraya al
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                for (int i = 0; i < oyun.Boyut; i++)
                {
                    for (int j = 0; j < oyun.Boyut; j++)
                    {
                        var button = GetButtonAt(i, j);
                        if (button != null)
                        {   // Mayın hücrelerini işaretle
                            if (oyun.oyun_alanı[i, j] == -1)
                            {
                                button.Content = "💣"; // Mayın simgesi
                            }
                            else if (openedCells[i, j])
                            {
                                UpdateButtonContent(button, i, j); // Açılan hücrelerin içeriğini güncelle
                            }
                        }
                    }
                }
            });
        }

        private void ToggleFlag(int x, int y, Button button)
        { // Eğer hücre açılmamışsa bayrak ekle veya kaldır
            if (!openedCells[x, y])
            {
                if (flaggedCells[x, y])
                {
                    button.Content = ""; // Bayrağı kaldır
                    flaggedCells[x, y] = false;
                    hamle_sayac--; // Hamle sayısını azalt
                }
                else
                {
                    button.Content = "🚩"; // Bayrak ekle
                    flaggedCells[x, y] = true;
                    hamle_sayac++; // Hamle sayısını artır
                }
                txtMoveCounter.Text = $"Hamle Sayısı: {hamle_sayac}"; // Hamle sayısını güncelle


            }
        }

        private Button? GetButtonAt(int x, int y)
        { 
            // Belirtilen hücredeki butonu bul ve döndür
            return gameGrid.Children
                .OfType<Button>()
                .FirstOrDefault(b => Grid.GetRow(b) == x && Grid.GetColumn(b) == y);
        }

        private void UpdateButtonContent(Button button, int x, int y)
        { 
            // Hücre içeriğini güncelle
            if (oyun.oyun_alanı[x, y] > 0)
            {
                button.Content = oyun.oyun_alanı[x, y].ToString(); // Hücre değeri varsa göster
            }
            else if (oyun.oyun_alanı[x, y] == 0)
            {
                button.Content = "0"; // Eğer hücre değeri 0 ise içeriği boş bırak
                KomsuHücreler(x, y); // 0 değerindeki hücre için komşuları aç
            }
            else if (oyun.oyun_alanı[x, y] == -1)
            {
                ;
                button.Content = "💣"; // Mayın simgesi
            }

            button.IsEnabled = false; // Hücreyi açıldı olarak işaretle
            openedCells[x, y] = true;
        }

        private void ButonGizle()
        {  // Tüm butonları devre dışı bırak
            foreach (var button in gameGrid.Children.OfType<Button>())
            {
                button.IsEnabled = false;
            }
        }

        private async Task ShowMessage(string message)
        {  // Mesaj göstermek için küçük bir dialog penceresi oluştur
            var dialog = new Window
            {
                Title = "Bilgi",
                Content = new TextBlock { Text = message },
                Width = 300,
                Height = 150
            };
            await dialog.ShowDialog(this); // Dialog penceresini göster
        }

        private void BtnSkor_OnClick(object? sender, RoutedEventArgs e)
        {  // Skorboard butonuna tıklanınca skorları göster
            if (skorboard == null) // Eğer skorboard null ise hata verir
            { 
                return;
            }
            var skorboardForm = new SkorboardForm(skorboard); // Skorboard formunu aç
            skorboardForm.Show(); // Skorboard  Formu göster
        }
        private void UpdateScoreboard(string playerName, int moveCount)
        { // Oyuncu adını ve hamle sayısını skorborda ekle
            if (skorboard != null)
            {
                skorboard.AddScore(playerName, moveCount); // Skoru güncelle
            }
        }


    }
}
