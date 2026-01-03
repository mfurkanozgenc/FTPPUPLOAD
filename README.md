# 🚀 FTP Dosya Yükleme Otomasyonu

Modern ve kullanıcı dostu FTP dosya yükleme aracı. Klasörlerinizi tek tıkla FTP sunucusuna yükleyin, sunucudaki dosyaları yönetin ve kayıtlı sunucu bilgilerinizi saklayın.

## ✨ Özellikler

### 📤 Dosya Yükleme
- **Otomatik Klasör Yapısı**: Yerel klasör yapısını koruyarak tüm dosyaları FTP'ye yükler
- **Akıllı Filtreleme**: `.git`, `node_modules`, `vendor` gibi gereksiz klasörleri otomatik atlar
- **İlerleme Takibi**: Her dosya için detaylı ilerleme çubuğu ve log kayıtları
- **Hata Yönetimi**: Başarısız yüklemeleri raporlar ve devam eder

### 🗂️ FTP Yönetimi
- **Dosya Listeleme**: Sunucudaki tüm dosya ve klasörleri recursive olarak listeler
- **Toplu Silme**: Seçili dosyaları veya tümünü tek seferde siler
- **Yeniden Adlandırma**: Dosya ve klasörleri yeniden adlandırın
- **Zorla Silme**: Bozuk karakterli klasörler için özel silme modu

### 💾 Sunucu Yönetimi
- **Kayıtlı Sunucular**: FTP bilgilerinizi kaydedin ve hızlıca yükleyin
- **Bağlantı Testi**: Kaydetmeden önce bağlantıyı test edin
- **Güvenli Depolama**: Sunucu bilgileri AppData klasöründe JSON formatında saklanır

## 🖥️ Ekran Görüntüleri

![Ana Ekran](screenshots/main.png)
*Ana ekran - FTP bilgileri ve yükleme arayüzü*

![FTP Yönetimi](screenshots/ftp-manager.png)
*FTP yönetimi - Dosya listeleme ve silme*

> 📸 Ekran görüntülerini `screenshots/` klasörüne ekleyin

## 🔧 Kurulum

### Gereksinimler
- Windows 7 veya üzeri
- .NET Framework 4.8
- Visual Studio 2019+ (geliştirme için)

### Projeyi Çalıştırma

1. **Depoyu klonlayın:**
   ```bash
   git clone https://github.com/kullaniciadi/ftp-upload-tool.git
   cd ftp-upload-tool
   ```

2. **Projeyi açın:**
   - Visual Studio ile `FTPPUPLOAD.sln` dosyasını açın

3. **Projeyi derleyin:**
   - Solution Explorer'da projeye sağ tıklayın
   - "Build" seçeneğini seçin
   - Veya `Ctrl + Shift + B` kısayolunu kullanın

4. **Çalıştırın:**
   - `F5` tuşuna basın veya "Start" butonuna tıklayın

### Executable Oluşturma

1. **Release modunda derleyin:**
   - Üstteki araç çubuğundan `Debug` → `Release` seçin
   - `Ctrl + Shift + B` ile derleyin

2. **Uygulamayı bulun:**
   - Derlenmiş `.exe` dosyası: `bin/Release/FTPPUPLOAD.exe`

## 📖 Kullanım

### 1️⃣ FTP Bağlantısı Kurma

1. **FTP Bilgilerini Girin:**
   - FTP Sunucu: `ftp://yoursite.com`
   - Kullanıcı Adı: FTP kullanıcı adınız
   - Şifre: FTP şifreniz
   - Port: `21` (varsayılan)

2. **Sunucu Kaydetme (Opsiyonel):**
   - Sunucu Adı: Tanımlayıcı bir ad girin
   - "Kaydet" butonuna tıklayın
   - Gelecekte "Kayıtlı Sunucular" listesinden yükleyebilirsiniz

### 2️⃣ Dosya Yükleme

1. **Klasör Seçin:**
   - "Gözat..." butonuna tıklayın
   - Yüklemek istediğiniz klasörü seçin

2. **Yükleyin:**
   - "FTP'ye Yükle" butonuna tıklayın
   - İlerleme çubuğunu ve log kayıtlarını izleyin

### 3️⃣ Sunucu Yönetimi

1. **Dosyaları Listele:**
   - "FTP'deki Dosyaları Listele" butonuna tıklayın
   - Tüm dosya ve klasörler görüntülenir

2. **Dosya Silme:**
   - **Seçili Silme**: Dosyaları işaretleyip "Seçilenleri Sil"
   - **Toplu Silme**: "Tümünü Sil" (onay gerektirir)

3. **Yeniden Adlandırma:**
   - Listeden bir dosya seçin (tek tıkla)
   - "Seçileni Yeniden Adlandır" butonuna tıklayın
   - Yeni adı girin

4. **Bozuk Klasör Silme:**
   - Klasörü seçin
   - "⚠ ZORLA SİL" butonuna tıklayın
   - 3 farklı yöntem denenir

## 🏗️ Teknik Detaylar

### Kullanılan Teknolojiler
- **Platform**: .NET Framework 4.8
- **UI**: Windows Forms
- **FTP İletişimi**: `FtpWebRequest` / `FtpWebResponse`
- **Async/Await**: Asenkron dosya yükleme
- **JSON**: Manuel serialization/deserialization

### Proje Yapısı
```
FTPPUPLOAD/
├── FTPPUPLOAD/
│   ├── Form1.cs              # Ana form mantığı
│   ├── Form1.Designer.cs     # UI tasarımı
│   ├── Program.cs            # Giriş noktası
│   └── Properties/           # Proje özellikleri
├── FTPPUPLOAD.sln           # Solution dosyası
└── README.md                # Bu dosya
```

### Önemli Sınıflar ve Metodlar

#### `FtpItem`
```csharp
private class FtpItem
{
    public string Path { get; set; }
    public bool IsDirectory { get; set; }
}
```
FTP'deki dosya ve klasörleri temsil eder.

#### `SavedServer`
```csharp
private class SavedServer
{
    public string Name { get; set; }
    public string Host { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public int Port { get; set; }
}
```
Kayıtlı sunucu bilgilerini saklar.

#### Ana Metodlar
- `UploadFolderToFtp()`: Klasör yükleme
- `ListFtpItemsRecursive()`: Recursive dosya listeleme
- `DeleteFtpItem()`: Dosya/klasör silme
- `CreateFtpDirectory()`: Klasör yapısı oluşturma
- `RenameFtpItem()`: Yeniden adlandırma

## 🔒 Güvenlik

- ⚠️ **Şifre Depolama**: Şifreler düz metin olarak `AppData` klasöründe saklanır
- 🔐 **Gelecek İyileştirme**: Windows DPAPI ile şifreleme eklenebilir
- 📁 **Dosya Konumu**: `%AppData%\FtpUploadApp\servers.json`

## 📋 Filtrelenen Klasörler

Aşağıdaki klasörler otomatik olarak yüklemeye dahil edilmez:

```
.git
node_modules
vendor
cache
.idea
.vscode
temp
tmp
__pycache__
.vs
storage/logs
storage/framework/cache
storage/framework/sessions
```

## 🐛 Bilinen Sorunlar

- Çok büyük dosyalar (>100MB) timeout verebilir
- Bazı FTP sunucuları passive mode gerektirebilir
- Türkçe karakterli dosya adları bazı sunucularda sorun çıkarabilir

## 🚀 Gelecek Özellikler

- [ ] SFTP desteği
- [ ] Çoklu dosya seçimi
- [ ] Drag & drop yükleme
- [ ] İndirme özelliği
- [ ] Tema desteği (dark mode)
- [ ] Otomatik yedekleme
- [ ] İlerleme için detaylı istatistikler

## 🤝 Katkıda Bulunma

Katkılarınızı bekliyoruz! Lütfen:

1. Projeyi fork edin
2. Feature branch oluşturun (`git checkout -b feature/AmazingFeature`)
3. Değişikliklerinizi commit edin (`git commit -m 'Add some AmazingFeature'`)
4. Branch'inizi push edin (`git push origin feature/AmazingFeature`)
5. Pull Request açın

## 📝 Lisans

Bu proje MIT lisansı altında lisanslanmıştır. Detaylar için [LICENSE](LICENSE) dosyasına bakın.

## 👨‍💻 Geliştirici

**Proje Geliştiricisi** - [GitHub Profiliniz](https://github.com/kullaniciadi)

## 🙏 Teşekkürler

- .NET Framework ekibine
- Tüm katkıda bulunanlara
- Bu projeyi kullanan herkese

---

⭐ Beğendiyseniz yıldız vermeyi unutmayın!

📧 Sorularınız için: [email@example.com](mailto:email@example.com)
