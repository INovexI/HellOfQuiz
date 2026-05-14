using Microsoft.EntityFrameworkCore;
using HellOfQuiz.Models;

namespace HellOfQuiz.Data
{
    /// <summary>
    /// Uygulamanın veritabanı bağlamı. Entity Framework Core ile MySQL arasında köprü görevi görür.
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // Veritabanı tabloları
        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Question> Questions { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Kullanıcı email'i benzersiz olmalı
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Kullanıcı adı benzersiz olmalı
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            // Kategoriler için başlangıç verisi (Seed Data)
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Genel Kültür", Icon = "🌍", Color = "#6C63FF" },
                new Category { Id = 2, Name = "Tarih", Icon = "🏛️", Color = "#FF6584" },
                new Category { Id = 3, Name = "Bilim & Teknoloji", Icon = "🔬", Color = "#43D9C8" },
                new Category { Id = 4, Name = "Spor", Icon = "⚽", Color = "#FF8C42" },
                new Category { Id = 5, Name = "Coğrafya", Icon = "🗺️", Color = "#2ECC71" },
                new Category { Id = 6, Name = "Yazılım & Programlama", Icon = "💻", Color = "#3498DB" }
            );

            // Admin kullanıcısı için başlangıç verisi
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Username = "admin",
                    Email = "admin@HellOfQuiz.com",
                    // Şifre: admin123 (BCrypt ile hashlenmiş)
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                    IsAdmin = true,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                }
            );

            // Örnek sorular için başlangıç verisi
            modelBuilder.Entity<Question>().HasData(
                // Genel Kültür Soruları
                new Question { Id = 1, CategoryId = 1, Text = "Türkiye'nin başkenti neresidir?", OptionA = "İstanbul", OptionB = "Ankara", OptionC = "İzmir", OptionD = "Bursa", CorrectAnswer = "B", Difficulty = 1 },
                new Question { Id = 2, CategoryId = 1, Text = "Dünya'da en çok konuşulan dil hangisidir?", OptionA = "İngilizce", OptionB = "Türkçe", OptionC = "Çince (Mandarin)", OptionD = "İspanyolca", CorrectAnswer = "C", Difficulty = 2 },
                new Question { Id = 3, CategoryId = 1, Text = "Bir yılda kaç hafta vardır?", OptionA = "48", OptionB = "50", OptionC = "52", OptionD = "54", CorrectAnswer = "C", Difficulty = 1 },
                new Question { Id = 4, CategoryId = 1, Text = "Hangi gezegen Güneş Sistemi'nin en büyüğüdür?", OptionA = "Satürn", OptionB = "Jüpiter", OptionC = "Neptün", OptionD = "Uranüs", CorrectAnswer = "B", Difficulty = 1 },
                new Question { Id = 5, CategoryId = 1, Text = "Leonardo da Vinci'nin ünlü tablosu hangisidir?", OptionA = "Guernica", OptionB = "Yıldızlı Gece", OptionC = "Mona Lisa", OptionD = "Son Akşam Yemeği", CorrectAnswer = "C", Difficulty = 1 },
                new Question { Id = 6, CategoryId = 1, Text = "Su kaç derecede kaynar? (Normal basınçta)", OptionA = "90°C", OptionB = "95°C", OptionC = "100°C", OptionD = "110°C", CorrectAnswer = "C", Difficulty = 1 },
                new Question { Id = 7, CategoryId = 1, Text = "Dünya'nın uydusu nedir?", OptionA = "Güneş", OptionB = "Ay", OptionC = "Venüs", OptionD = "Mars", CorrectAnswer = "B", Difficulty = 1 },
                new Question { Id = 8, CategoryId = 1, Text = "Hangi hayvan 'Ormanın Kralı' olarak anılır?", OptionA = "Kaplan", OptionB = "Fil", OptionC = "Aslan", OptionD = "Kurt", CorrectAnswer = "C", Difficulty = 1 },
                new Question { Id = 9, CategoryId = 1, Text = "Olimpiyat Oyunları kaç yılda bir düzenlenir?", OptionA = "2", OptionB = "3", OptionC = "4", OptionD = "5", CorrectAnswer = "C", Difficulty = 1 },
                new Question { Id = 10, CategoryId = 1, Text = "Hangi element kimyasal sembolü 'O' ile gösterilir?", OptionA = "Altın", OptionB = "Oksijen", OptionC = "Osmiyum", OptionD = "Ozon", CorrectAnswer = "B", Difficulty = 1 },

                // Tarih Soruları
                new Question { Id = 11, CategoryId = 2, Text = "Türkiye Cumhuriyeti hangi yılda kurulmuştur?", OptionA = "1919", OptionB = "1920", OptionC = "1923", OptionD = "1926", CorrectAnswer = "C", Difficulty = 1 },
                new Question { Id = 12, CategoryId = 2, Text = "Kurtuluş Savaşı'nın başladığı tarih hangisidir?", OptionA = "19 Mayıs 1919", OptionB = "30 Ağustos 1922", OptionC = "23 Nisan 1920", OptionD = "29 Ekim 1923", CorrectAnswer = "A", Difficulty = 2 },
                new Question { Id = 13, CategoryId = 2, Text = "İstanbul'un fethini kim gerçekleştirmiştir?", OptionA = "I. Murat", OptionB = "II. Bayezid", OptionC = "Yavuz Sultan Selim", OptionD = "Fatih Sultan Mehmet", CorrectAnswer = "D", Difficulty = 1 },
                new Question { Id = 14, CategoryId = 2, Text = "İstanbul hangi yılda fethedilmiştir?", OptionA = "1451", OptionB = "1453", OptionC = "1455", OptionD = "1460", CorrectAnswer = "B", Difficulty = 2 },
                new Question { Id = 15, CategoryId = 2, Text = "Birinci Dünya Savaşı hangi yılda başlamıştır?", OptionA = "1910", OptionB = "1912", OptionC = "1914", OptionD = "1916", CorrectAnswer = "C", Difficulty = 1 },
                new Question { Id = 16, CategoryId = 2, Text = "Osmanlı İmparatorluğu'nun kurucusu kimdir?", OptionA = "Orhan Gazi", OptionB = "Osman Gazi", OptionC = "Ertuğrul Gazi", OptionD = "Alp Arslan", CorrectAnswer = "B", Difficulty = 2 },
                new Question { Id = 17, CategoryId = 2, Text = "Fransız İhtilali hangi yılda gerçekleşmiştir?", OptionA = "1776", OptionB = "1789", OptionC = "1799", OptionD = "1804", CorrectAnswer = "B", Difficulty = 2 },
                new Question { Id = 18, CategoryId = 2, Text = "Malazgirt Savaşı hangi yılda yapılmıştır?", OptionA = "1054", OptionB = "1071", OptionC = "1096", OptionD = "1176", CorrectAnswer = "B", Difficulty = 3 },
                new Question { Id = 19, CategoryId = 2, Text = "Atatürk'ün soyadı hangi yılda kabul edilmiştir?", OptionA = "1932", OptionB = "1933", OptionC = "1934", OptionD = "1935", CorrectAnswer = "C", Difficulty = 2 },
                new Question { Id = 20, CategoryId = 2, Text = "Kurtuluş Savaşı'nda TBMM hangi şehirde açılmıştır?", OptionA = "İstanbul", OptionB = "Ankara", OptionC = "Sivas", OptionD = "Erzurum", CorrectAnswer = "B", Difficulty = 1 },

                // Bilim & Teknoloji Soruları
                new Question { Id = 21, CategoryId = 3, Text = "Işığın boşluktaki hızı yaklaşık ne kadardır?", OptionA = "200.000 km/s", OptionB = "300.000 km/s", OptionC = "400.000 km/s", OptionD = "500.000 km/s", CorrectAnswer = "B", Difficulty = 2 },
                new Question { Id = 22, CategoryId = 3, Text = "DNA'nın açılımı nedir?", OptionA = "Deoksiribonükleik Asit", OptionB = "Dinamik Nükleik Asit", OptionC = "Denatüre Nükleotid Analizi", OptionD = "Difüzyon Nükleik Aminoasit", CorrectAnswer = "A", Difficulty = 2 },
                new Question { Id = 23, CategoryId = 3, Text = "Hangi bilim insanı yerçekimini keşfetmiştir?", OptionA = "Albert Einstein", OptionB = "Galileo Galilei", OptionC = "Isaac Newton", OptionD = "Nikola Tesla", CorrectAnswer = "C", Difficulty = 1 },
                new Question { Id = 24, CategoryId = 3, Text = "İnsan vücudunda kaç kemik bulunur?", OptionA = "186", OptionB = "196", OptionC = "206", OptionD = "216", CorrectAnswer = "C", Difficulty = 2 },
                new Question { Id = 25, CategoryId = 3, Text = "Periyodik tablodaki en hafif element hangisidir?", OptionA = "Helyum", OptionB = "Hidrojen", OptionC = "Lityum", OptionD = "Karbon", CorrectAnswer = "B", Difficulty = 2 },
                new Question { Id = 26, CategoryId = 3, Text = "İlk bilgisayar virüsü hangi yılda ortaya çıkmıştır?", OptionA = "1971", OptionB = "1981", OptionC = "1986", OptionD = "1990", CorrectAnswer = "A", Difficulty = 3 },
                new Question { Id = 27, CategoryId = 3, Text = "Hangi şirket 'Android' işletim sistemini geliştirmiştir?", OptionA = "Apple", OptionB = "Microsoft", OptionC = "Google", OptionD = "Samsung", CorrectAnswer = "C", Difficulty = 1 },
                new Question { Id = 28, CategoryId = 3, Text = "1 gigabayt kaç megabayta eşittir?", OptionA = "512 MB", OptionB = "1000 MB", OptionC = "1024 MB", OptionD = "2048 MB", CorrectAnswer = "C", Difficulty = 2 },
                new Question { Id = 29, CategoryId = 3, Text = "İnsan beyninin ortalama ağırlığı ne kadardır?", OptionA = "1 kg", OptionB = "1.4 kg", OptionC = "1.8 kg", OptionD = "2.2 kg", CorrectAnswer = "B", Difficulty = 2 },
                new Question { Id = 30, CategoryId = 3, Text = "Evren hangi olayla başlamıştır?", OptionA = "Kara Delik Patlaması", OptionB = "Büyük Patlama (Big Bang)", OptionC = "Süpernova Patlaması", OptionD = "Galaksi Çarpışması", CorrectAnswer = "B", Difficulty = 1 },

                // Spor Soruları
                new Question { Id = 31, CategoryId = 4, Text = "Futbolda bir maçta kaç oyuncu sahaya çıkar? (Her takımdan)", OptionA = "10", OptionB = "11", OptionC = "12", OptionD = "9", CorrectAnswer = "B", Difficulty = 1 },
                new Question { Id = 32, CategoryId = 4, Text = "Türkiye'nin en fazla Süper Lig şampiyonluğu olan takımı hangisidir?", OptionA = "Fenerbahçe", OptionB = "Beşiktaş", OptionC = "Trabzonspor", OptionD = "Galatasaray", CorrectAnswer = "D", Difficulty = 2 },
                new Question { Id = 33, CategoryId = 4, Text = "Basketbolda normal bir maç kaç dakika sürer?", OptionA = "30 dakika", OptionB = "36 dakika", OptionC = "40 dakika", OptionD = "48 dakika", CorrectAnswer = "C", Difficulty = 2 },
                new Question { Id = 34, CategoryId = 4, Text = "Tenis'te 'love' ne anlama gelir?", OptionA = "1 Puan", OptionB = "Sıfır Puan", OptionC = "Eşitlik", OptionD = "Üstün Sayı", CorrectAnswer = "B", Difficulty = 2 },
                new Question { Id = 35, CategoryId = 4, Text = "Yüzme sporunda en uzun mesafe yarışması kaç metredir?", OptionA = "400m", OptionB = "800m", OptionC = "1500m", OptionD = "2000m", CorrectAnswer = "C", Difficulty = 3 },
                new Question { Id = 36, CategoryId = 4, Text = "Formula 1'de her tur başında verilen bayrak hangi renktir?", OptionA = "Yeşil", OptionB = "Kırmızı", OptionC = "Beyaz", OptionD = "Mavi", CorrectAnswer = "A", Difficulty = 2 },
                new Question { Id = 37, CategoryId = 4, Text = "Voleybolda her takımda sahada kaç oyuncu yer alır?", OptionA = "5", OptionB = "6", OptionC = "7", OptionD = "8", CorrectAnswer = "B", Difficulty = 1 },
                new Question { Id = 38, CategoryId = 4, Text = "Futbol topunun ağırlığı kaç gram olmalıdır?", OptionA = "360-370 gram", OptionB = "410-450 gram", OptionC = "410-450 gram", OptionD = "500-520 gram", CorrectAnswer = "B", Difficulty = 3 },
                new Question { Id = 39, CategoryId = 4, Text = "2002 Dünya Kupası'nda Türkiye kaçıncı olmuştur?", OptionA = "İkinci", OptionB = "Üçüncü", OptionC = "Dördüncü", OptionD = "Beşinci", CorrectAnswer = "B", Difficulty = 2 },
                new Question { Id = 40, CategoryId = 4, Text = "Boks maçlarında ringle çevresi hangi ölçüde olmalıdır?", OptionA = "4.9m - 6.1m", OptionB = "5.5m - 7.3m", OptionC = "5.5m - 8.0m", OptionD = "6.0m - 9.0m", CorrectAnswer = "C", Difficulty = 3 },

                // Coğrafya Soruları
                new Question { Id = 41, CategoryId = 5, Text = "Dünyanın en uzun nehri hangisidir?", OptionA = "Amazon", OptionB = "Nil", OptionC = "Yangtze", OptionD = "Mississippi", CorrectAnswer = "B", Difficulty = 1 },
                new Question { Id = 42, CategoryId = 5, Text = "Dünya'nın en yüksek dağı hangisidir?", OptionA = "K2", OptionB = "Kangchenjunga", OptionC = "Everest", OptionD = "Annapurna", CorrectAnswer = "C", Difficulty = 1 },
                new Question { Id = 43, CategoryId = 5, Text = "Türkiye'nin en büyük gölü hangisidir?", OptionA = "Beyşehir Gölü", OptionB = "Tuz Gölü", OptionC = "Eğirdir Gölü", OptionD = "Van Gölü", CorrectAnswer = "D", Difficulty = 2 },
                new Question { Id = 44, CategoryId = 5, Text = "Avustralya hangi kıtada yer alır?", OptionA = "Asya", OptionB = "Okyanusya", OptionC = "Afrika", OptionD = "Güney Amerika", CorrectAnswer = "B", Difficulty = 1 },
                new Question { Id = 45, CategoryId = 5, Text = "Türkiye'nin en büyük ilçesi hangisidir?", OptionA = "Konya Merkez", OptionB = "Erzurum Merkez", OptionC = "Karapınar", OptionD = "Beyşehir", CorrectAnswer = "C", Difficulty = 3 },
                new Question { Id = 46, CategoryId = 5, Text = "Dünya'nın en küçük ülkesi hangisidir?", OptionA = "Monaco", OptionB = "San Marino", OptionC = "Liechtenstein", OptionD = "Vatikan", CorrectAnswer = "D", Difficulty = 2 },
                new Question { Id = 47, CategoryId = 5, Text = "Nil nehri hangi ülkelerde akar?", OptionA = "Sadece Mısır", OptionB = "Mısır ve Sudan", OptionC = "Birden fazla Afrika ülkesi", OptionD = "Mısır ve Libya", CorrectAnswer = "C", Difficulty = 2 },
                new Question { Id = 48, CategoryId = 5, Text = "Japonya'nın başkenti neresidir?", OptionA = "Osaka", OptionB = "Kyoto", OptionC = "Tokyo", OptionD = "Hiroshima", CorrectAnswer = "C", Difficulty = 1 },
                new Question { Id = 49, CategoryId = 5, Text = "Boğaziçi Köprüsü hangi iki kıtayı birbirine bağlar?", OptionA = "Avrupa-Afrika", OptionB = "Asya-Afrika", OptionC = "Avrupa-Asya", OptionD = "Avrupa-Amerika", CorrectAnswer = "C", Difficulty = 1 },
                new Question { Id = 50, CategoryId = 5, Text = "Sahara Çölü hangi kıtada yer alır?", OptionA = "Asya", OptionB = "Güney Amerika", OptionC = "Afrika", OptionD = "Avustralya", CorrectAnswer = "C", Difficulty = 1 },

                // Yazılım & Programlama Soruları
                new Question { Id = 51, CategoryId = 6, Text = "HTML'nin açılımı nedir?", OptionA = "Hyper Text Making Language", OptionB = "Hyper Text Markup Language", OptionC = "High Transfer Markup Language", OptionD = "Hyper Transfer Making Language", CorrectAnswer = "B", Difficulty = 1 },
                new Question { Id = 52, CategoryId = 6, Text = "C# programlama dili hangi şirket tarafından geliştirilmiştir?", OptionA = "Apple", OptionB = "Google", OptionC = "Microsoft", OptionD = "Oracle", CorrectAnswer = "C", Difficulty = 1 },
                new Question { Id = 53, CategoryId = 6, Text = "SQL'de veri silmek için hangi komut kullanılır?", OptionA = "REMOVE", OptionB = "ERASE", OptionC = "DELETE", OptionD = "DROP", CorrectAnswer = "C", Difficulty = 1 },
                new Question { Id = 54, CategoryId = 6, Text = "MVC mimarisinde 'V' neyi temsil eder?", OptionA = "Variable", OptionB = "View", OptionC = "Value", OptionD = "Version", CorrectAnswer = "B", Difficulty = 1 },
                new Question { Id = 55, CategoryId = 6, Text = "Python programlama dilini kim yaratmıştır?", OptionA = "Dennis Ritchie", OptionB = "Bjarne Stroustrup", OptionC = "James Gosling", OptionD = "Guido van Rossum", CorrectAnswer = "D", Difficulty = 2 },
                new Question { Id = 56, CategoryId = 6, Text = "Git'te değişiklikleri kaydetmek için hangi komut kullanılır?", OptionA = "git save", OptionB = "git commit", OptionC = "git push", OptionD = "git store", CorrectAnswer = "B", Difficulty = 1 },
                new Question { Id = 57, CategoryId = 6, Text = "JSON'un açılımı nedir?", OptionA = "JavaScript Object Notation", OptionB = "Java Standard Object Network", OptionC = "JavaScript Online Notation", OptionD = "Java Script Object Node", CorrectAnswer = "A", Difficulty = 1 },
                new Question { Id = 58, CategoryId = 6, Text = "Hangi veri yapısı LIFO (Son Giren İlk Çıkar) prensibine dayanır?", OptionA = "Queue (Kuyruk)", OptionB = "Array (Dizi)", OptionC = "Stack (Yığın)", OptionD = "Tree (Ağaç)", CorrectAnswer = "C", Difficulty = 2 },
                new Question { Id = 59, CategoryId = 6, Text = "HTTP 404 hata kodu ne anlama gelir?", OptionA = "Sunucu Hatası", OptionB = "Erişim Reddedildi", OptionC = "Sayfa Bulunamadı", OptionD = "Bağlantı Zaman Aşımı", CorrectAnswer = "C", Difficulty = 1 },
                new Question { Id = 60, CategoryId = 6, Text = "Entity Framework'te veritabanı değişikliklerini uygulamak için hangi komut kullanılır?", OptionA = "dotnet ef db apply", OptionB = "dotnet ef database update", OptionC = "dotnet ef migrate apply", OptionD = "dotnet ef sync", CorrectAnswer = "B", Difficulty = 2 }
            );
        }
    }
}
