using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Avalonia.Platform;

namespace MayinProje
{
    public class Oyun(MainWindow mainWindow)
    {// Oyun alanının boyutunu tutuyor
        public int Boyut { get; private set; }
        public int[,] oyun_alanı; // Oyun alanı 2D bir dizi olarak tanımlandı, mayınlar ve sayılar burada saklanacak
        private Random random = new Random(); //rastgele mayın yerleştırme
        private Stopwatch stopwatch; // Oyunun süresini ölçme
        private Skorboard scoreboard;  // Skorları kaydetmek

        private MainWindow mainWindow; // MainWindow referansı ana pencereye erişmek için

  // Bayrakla işaretlenen mayınların sayısını tutuyor
        public int FlaggedMines { get; private set; }

    // Yapıcı metot: Oyunun boyutunu ve mayın sayısını alıyor
     public Oyun(MainWindow mainWindow,int boyut, int mayinSayisi):this(mainWindow)
        {
            this.mainWindow = mainWindow; // Ana pencereyi kaydediyoruz
            Boyut = boyut;
            oyun_alanı = new int[boyut, boyut]; // Oyun alanını oluşturuyoruz
            MayınKonum(mayinSayisi); // Mayınları yerleştiriyoruz
            SayiHesapla(); // Komşu mayın sayısı
            FlaggedMines = 0; // Başlangıçta bayraklı mayın yok
            stopwatch = new Stopwatch(); // Kronometre başlatılıyor
            stopwatch.Start(); 
            string filePath = "yeni.txt";
            scoreboard = new Skorboard(filePath); // Skorların kaydedileceği dosya yolu
        }
         // Mayınları rastgele konumlara yerleştiren metot
        private void MayınKonum(int mayinSayisi)
        {
            for (int i = 0; i < mayinSayisi; i++)
            {
                int x, y;
                do
                {
                    x = random.Next(Boyut);
                    y = random.Next(Boyut);
                } while (oyun_alanı[x, y] == -1); // Eğer bu koordinatta mayın varsa yeniden seç

                oyun_alanı[x, y] = -1; // Mayını yerleştir
            }
        }
        // Her hücre için komşu mayın sayısını hesaplayan metot
        private void SayiHesapla()
        {
            for (int x = 0; x < Boyut; x++)
            {
                for (int y = 0; y < Boyut; y++)
                {
                    if (oyun_alanı[x, y] == -1) // Eğer hücrede mayın varsa devam et
                    {
                        continue; 
                    }

                    oyun_alanı[x, y] = KomsuMayinSayac(x, y); // Komşu mayın sayısını hesapla
                }
            }
        }

        private int KomsuMayinSayac(int x, int y)//Hücrenin komşularını kontrol eder ve mayın sayısını döner.
        {
            int count = 0;
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (IsInBorder(x + i, y + j) && oyun_alanı[x + i, y + j] == -1) // Komşu hücre sınır içinde ve mayınsa
                    {
                        count++;
                    }
                }
            }

            return count; // Mayın sayısını geri döndür
        }
        // Hücrenin oyun alanı sınırları içinde olup olmadığını kontrol eden metot
        private bool IsInBorder(int x, int y)
        {
            return x >= 0 && x < Boyut && y >= 0 && y < Boyut;
        }
         // Oyuncu bir hamle yaptığında çalışan metot
        public bool HamleYap(int x, int y)
        {
            GameOver();
            SonOyun();  // Oyun sonunda skorları hesapla 
            
            return true; 
        }
        // Oyun sona erdiğinde yapılacak işlemler
        public void SonOyun()
        {
            stopwatch.Stop();
            int finalScore = CalculateScore(); // Skoru hesapla

            string playerName = mainWindow.txtPlayer.Text; // Oyuncu adını al
    
            if (string.IsNullOrWhiteSpace(playerName)) // Oyuncu adı boşsa geçersiz ad koy
            {
                playerName = "Geçersiz"; 
            }
    
            scoreboard.AddScore(playerName, finalScore); // Skoru skorboarda ekle

            
            SkorboardForm form = new SkorboardForm(scoreboard);
            form.Show(); // Formu göster
            scoreboard.AddScore(playerName, finalScore);
            
        }
         // Oyun bitmiş mi diye kontrol eden metot
        public bool IsGameOver()
        {
            
            return false;
        }
      // Oyun bittiğinde yapılacak işlemler
        public void GameOver()
        {
            stopwatch.Stop(); 

        }

      // Skoru hesaplayan metot
        private int CalculateScore()
        {
           
            int timeInSeconds = (int)stopwatch.Elapsed.TotalSeconds; // Geçen süreyi al

            if (timeInSeconds == 0) // Süre 0 ise 1 saniye yap
            {
                timeInSeconds = 1;
            }

            if (FlaggedMines == 0) // Bayraklı mayın yoksa 1 yap
            {
                FlaggedMines++;
            }

            int score = (FlaggedMines * 1000) / timeInSeconds; // Skoru hesapla
            Console.WriteLine($"Flagged Mines: {FlaggedMines}, Time: {timeInSeconds}, Score: {score}");

            return score; // Skoru geri döndür
            
        }   
        
    }
}
