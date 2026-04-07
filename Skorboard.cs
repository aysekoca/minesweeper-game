using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class Skorboard // Skorboard sınıfı, oyuncuların skorlarını tutmak ve yönetmek için kullanılır.
{
    private Dictionary<string, int> scores; //Oyuncu isimleri ve skorlarını tutar
    private string filePath;  // Skorların kaydedileceği dosya yolu
     // Skorboard sınıfının yapıcısı; dosya yolunu alır ve mevcut skorları yükler.
    public Skorboard(string filePath)
    {
        scores = new Dictionary<string, int>(); // Skorları tutan sözlük oluşturuluyor
        this.filePath = filePath;
        LoadScoresFromFile(); // Dosyadan mevcut skorlar yükleniyor
    }
     // Yeni bir skoru eklemek için 
    public void AddScore(string playerName, int score)
    {   // Eğer oyuncu daha önce kaydedilmişse, mevcut skoru güncelle
        if (scores.ContainsKey(playerName))
        {
            scores[playerName] = Math.Max(scores[playerName], score); // En yüksek skoru sakla
        }
        else
        {
            scores[playerName] = score; // Yeni oyuncu için skoru ekle
        }

        SaveScoresToFile(); // Skorları dosyaya kaydet
        Console.WriteLine($"Skor eklendi: {playerName}: {score}"); // Eklenen skoru göster
    }
    // En yüksek 10 skoru döndürmek için kullanılan metot.
    public List<PlayerScore> GetTopScores()
    {
        return scores
            .OrderByDescending(s => s.Value) // Skorları azalan düzende sırala
            .Take(10)
            .Select(s => new PlayerScore(s.Key, s.Value)) // PlayerScore nesnelerine dönüştür
            .ToList(); // Listeye çevir
    }

    public void ShowTopScores()
    {
        Console.WriteLine("En İyi 10 Skor:");
        foreach (var score in GetTopScores())
        {
            Console.WriteLine($"{score.PlayerName}: {score.Score}");
        }
    }

    // Skorları dosyaya kaydetmek için
    private void SaveScoresToFile()
    {
        
            string filePath = "yeni.txt"; // veya "Desktop/mayin.txt" 

            // Dosya mevcut mu kontrol et
            if (!File.Exists(filePath))
            {
                // Dosya yoksa oluştur
                File.Create(filePath).Dispose(); // Dispose, dosyayı kapatır
            }

            // Skorları dosyaya yaz
            using (StreamWriter sw = new StreamWriter(filePath, true)) // Append mode
            {
                foreach (var score in scores) // scores, oyuncu ve puanlarını içeren bir koleksiyon
                {
                    sw.WriteLine($"{score.Key}: {score.Value}");
                }
            }
        
    }
    // Dosyadan skorları yüklemek için kullanılır.
    private void LoadScoresFromFile()
    {
        if (File.Exists(filePath))
        {
            using (StreamReader reader = new StreamReader(filePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    var parts = line.Split(','); // Satırı virgüle göre ayır
                    if (parts.Length == 2 && int.TryParse(parts[1], out int score))
                    {
                        AddScore(parts[0], score);  // Skoru ekle
                    }
                }
            }
        }
        else
        {
            Console.WriteLine($"Dosya mevcut değil: {filePath}"); // Dosya yoksa mesaj göster
        }
    }
}
// PlayerScore sınıfı, bir oyuncunun ismi ve skorunu tutar.
public class PlayerScore
{
    public string PlayerName { get; }
    public int Score { get; }
  // PlayerScore sınıfının yapıcısı isim ve skoru al
    public PlayerScore(string playerName, int score)
    {
        PlayerName = playerName; // Oyuncunun ismini ata
        Score = score; // Oyuncunun skorunu ata
    }
}