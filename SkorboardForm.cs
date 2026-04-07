using System;
using System.Collections.Generic;
using Avalonia.Layout;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Input;
using System.Linq;
using Avalonia;
using System.Collections.ObjectModel;

namespace MayinProje
{
    // SkorboardForm, oyunun skorlarını gösteren pencereyi temsil eder.
    public class SkorboardForm : Window
    {
        private Skorboard skorboard; // Skorları tutan Skorboard nesnesi
        private ListBox listBox; // Skorların listeleneceği kontrol
        private TextBlock messageTextBlock; // Mesajları göstermek için kullanılan kontrol

        private ObservableCollection<string> scoresCollection; // Skorları tutan koleksiyon

        private System.Diagnostics.Stopwatch stopwatch; // Süre ölçmek için kullanılan stopwatch
        public int FlaggedMines { get; set; } // İşaretlenmiş mayın sayısı

        // SkorboardForm sınıfının yapıcısı; skorboard nesnesini alır ve bileşenleri başlatır.
        public SkorboardForm(Skorboard skorboard)
        {
            this.skorboard = skorboard; // Skorboard nesnesini ata
            scoresCollection = new ObservableCollection<string>(); // Skor koleksiyonunu başlat
            stopwatch = new System.Diagnostics.Stopwatch(); // Stopwatch'i başlat
            InitializeComponent(); // Bileşenleri başlat
            ShowTopScores(); // En yüksek skorları göster
        }

        // En yüksek skorları listeleyen metot.
        private void ShowTopScores()
        {
            var scores = skorboard.GetTopScores(); // En yüksek skorları al

            if (scores == null || scores.Count == 0) // Eğer skor yoksa
            {
                messageTextBlock.Text = "Skor yok!"; // Mesaj göster
                listBox.IsVisible = false; // Listeyi gizle
                return; // Metodu bitir
            }

            scoresCollection.Clear(); // Mevcut skorları temizle
            foreach (var score in scores) // Her bir skoru döngü ile gez
            {
                scoresCollection.Add($"{score.PlayerName}: {score.Score}"); // Skoru koleksiyona ekle
                Console.WriteLine($"Skor eklendi: {score.PlayerName}: {score.Score}"); // Konsola yazdır
            }

            listBox.IsVisible = true; // Listeyi göster
        }

        // Bileşenlerin başlangıç ayarlarını yapan metot.
        private void InitializeComponent()
        {
            this.Title = "Skorboard"; // Pencere başlığı
            this.Width = 400; // Pencere genişliği
            this.Height = 300; // Pencere yüksekliği

            var stackPanel = new StackPanel // Dikey yerleşim için StackPanel oluştur
            {
                Orientation = Orientation.Vertical, // Dikey yerleşim
                Margin = new Thickness(10) // Kenar boşlukları
            };

            messageTextBlock = new TextBlock // Mesajları göstermek için TextBlock oluştur
            {
                HorizontalAlignment = HorizontalAlignment.Center, // Ortala
                VerticalAlignment = VerticalAlignment.Top, // Üstte konumlandır
                Margin = new Thickness(0, 0, 0, 10) // Alt boşluk
            };
            stackPanel.Children.Add(messageTextBlock); // StackPanel'e ekle

            listBox = new ListBox // Skorları listelemek için ListBox oluştur
            {
                HorizontalAlignment = HorizontalAlignment.Stretch, // Genişliği uzat
                VerticalAlignment = VerticalAlignment.Stretch, // Yüksekliği uzat
                Margin = new Thickness(0), // Kenar boşlukları
                ItemsSource = scoresCollection // Koleksiyonu listeye ata
            };
            stackPanel.Children.Add(listBox); // StackPanel'e ekle

            this.Content = stackPanel; // StackPanel'i pencerenin içeriği olarak ata
        }

        // Skor hesaplamak için kullanılan metot.
        private int CalculateScore()
        {
            int timeInSeconds = (int)stopwatch.Elapsed.TotalSeconds; // Geçen süreyi saniye cinsinden al

            // Eğer timeInSeconds sıfırsa, bunu minimum 1'e ayarlıyoruz
            if (timeInSeconds == 0)
            {
                timeInSeconds = 1; // Minimum süre
            }

            if (FlaggedMines == 0) // Eğer işaretlenmiş mayın yoksa
            {
                FlaggedMines = 1; // Minimum 1 olarak ayarla
            }

            // Skor hesapla
            int score = (FlaggedMines * 1000) / timeInSeconds;
            Console.WriteLine($"Flagged Mines: {FlaggedMines}, Time: {timeInSeconds}, Score: {score}"); // Konsola yazdır

            return score; // Hesaplanan skoru döndür
        }

        // Oyun bittiğinde çağrılan metot.
        public void OnGameOver(string playerName)
        {
            int score = CalculateScore(); // Skoru hesapla

            Console.WriteLine($"Oyun Bitti: {playerName}, Skor: {score}"); // Oyun bitti mesajı yazdır
            Console.WriteLine($"Flagged Mines: {FlaggedMines}, Time: {(int)stopwatch.Elapsed.TotalSeconds}, Score: {score}"); // Ek bilgiler

            skorboard.AddScore(playerName, score); // Skoru Skorboard'a ekle

            skorboard.ShowTopScores(); // En yüksek skorları göster

            ShowTopScores(); // Pencerede en yüksek skorları göster
            
            this.BringIntoView(); // Pencereyi ön plana getir
        }
    }
}
