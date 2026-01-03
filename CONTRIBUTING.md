# Katkıda Bulunma Rehberi

FTP Dosya Yükleme Otomasyonu projesine katkıda bulunmak istediğiniz için teşekkürler! 🎉

## 🚀 Nasıl Katkıda Bulunurum?

### 1. Fork & Clone

```bash
# Projeyi fork edin (GitHub üzerinden)
# Sonra klonlayın:
git clone https://github.com/KULLANICI_ADINIZ/ftp-upload-tool.git
cd ftp-upload-tool
```

### 2. Branch Oluşturun

```bash
git checkout -b feature/amazing-feature
# veya
git checkout -b fix/bug-fix
```

### 3. Değişikliklerinizi Yapın

- Kod standartlarına uyun
- Yorumları Türkçe veya İngilizce yazın
- Anlamlı değişken/method isimleri kullanın

### 4. Test Edin

- Değişikliklerinizi test edin
- Mevcut özelliklerin çalıştığından emin olun
- Hata durumlarını kontrol edin

### 5. Commit Edin

```bash
git add .
git commit -m "feat: Yeni özellik eklendi"
# veya
git commit -m "fix: Bug düzeltildi"
```

#### Commit Mesaj Formatı

- `feat:` - Yeni özellik
- `fix:` - Bug düzeltme
- `docs:` - Dokümantasyon
- `style:` - Kod formatı
- `refactor:` - Kod iyileştirme
- `test:` - Test ekleme
- `chore:` - Diğer değişiklikler

### 6. Push Edin

```bash
git push origin feature/amazing-feature
```

### 7. Pull Request Açın

- GitHub'da Pull Request açın
- Değişikliklerinizi açıklayın
- İlgili issue'ları bağlayın

## 📋 Kod Standartları

### C# Stil Rehberi

```csharp
// ✅ İyi
private async Task UploadFileAsync(string filePath)
{
    if (string.IsNullOrEmpty(filePath))
    {
        throw new ArgumentException("Dosya yolu boş olamaz");
    }
    
    // Kod...
}

// ❌ Kötü
private async Task upload(string fp)
{
    if(fp=="") return;
    // Kod...
}
```

### İsimlendirme

- **Class**: PascalCase → `FtpManager`
- **Method**: PascalCase → `UploadFile()`
- **Variable**: camelCase → `ftpHost`
- **Constant**: UPPER_CASE → `MAX_RETRY_COUNT`
- **Private field**: camelCase → `_username`

### Dosya Organizasyonu

```
FTPPUPLOAD/
├── Core/              # Temel mantık
├── UI/                # Kullanıcı arayüzü
├── Utils/             # Yardımcı sınıflar
└── Models/            # Veri modelleri
```

## 🐛 Bug Bildirimi

Bug bulduysanız:

1. Issue açın
2. Bug'ı detaylı açıklayın
3. Adımları ekleyin (nasıl tekrar edilir)
4. Ekran görüntüsü ekleyin (varsa)
5. Hata mesajını paylaşın

### Bug Rapor Şablonu

```markdown
## Bug Açıklaması
[Bug'ı kısaca açıklayın]

## Adımlar
1. X yapın
2. Y tıklayın
3. Hata görün

## Beklenen Davranış
[Ne olmasını bekliyordunuz]

## Gerçek Davranış
[Ne oldu]

## Ekran Görüntüsü
[Varsa ekleyin]

## Ortam
- OS: Windows 10
- .NET: 4.8
- Versiyon: 1.0
```

## 💡 Özellik İsteği

Yeni özellik önerisi için:

1. Issue açın
2. Özelliği detaylı açıklayın
3. Kullanım senaryosu ekleyin
4. Mockup/tasarım ekleyin (varsa)

## ✅ Pull Request Kontrol Listesi

PR açmadan önce:

- [ ] Kod derleniyor mu?
- [ ] Testler geçiyor mu?
- [ ] Mevcut özellikler çalışıyor mu?
- [ ] README güncel mi?
- [ ] Yorumlar eklendi mi?
- [ ] Commit mesajları anlamlı mı?

## 📞 İletişim

Sorularınız için:

- Issue açın
- Email: [email@example.com](mailto:email@example.com)
- Discussions: GitHub Discussions kullanın

## 🙏 Teşekkürler!

Katkılarınız için teşekkür ederiz! Her katkı, projeyi daha iyi hale getirir.

---

Mutlu kodlamalar! 🚀
