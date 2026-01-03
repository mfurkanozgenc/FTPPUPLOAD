using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FTPPUPLOAD
{
    public partial class Form1 : Form
    {
        // FTP öğesi (dosya veya klasör)
        private class FtpItem
        {
            public string Path { get; set; }
            public bool IsDirectory { get; set; }

            public override string ToString()
            {
                return IsDirectory ? $"[KLASÖR] {Path}" : Path;
            }
        }

        // Kayıtlı sunucu bilgisi
        private class SavedServer
        {
            public string Name { get; set; }
            public string Host { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
            public int Port { get; set; }

            public override string ToString()
            {
                return Name;
            }
        }

        public Form1()
        {
            InitializeComponent();
            this.Load += Form1_Load;
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                txtFolderPath.Text = folderBrowserDialog.SelectedPath;
                AddLog($"Klasör seçildi: {txtFolderPath.Text}");
            }
        }

        private async void btnUpload_Click(object sender, EventArgs e)
        {
            // Doğrulamalar
            if (string.IsNullOrWhiteSpace(txtFtpHost.Text) || txtFtpHost.Text == "ftp://")
            {
                MessageBox.Show("Lütfen FTP sunucu adresini giriniz!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                MessageBox.Show("Lütfen kullanıcı adını giriniz!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Lütfen şifre giriniz!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtFolderPath.Text) || !Directory.Exists(txtFolderPath.Text))
            {
                MessageBox.Show("Lütfen geçerli bir klasör seçiniz!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Kontrolları devre dışı bırak
            SetControlsEnabled(false);
            lstLog.Items.Clear();
            progressBar.Value = 0;

            try
            {
                // Direkt yükleme
                await UploadFolderToFtp();
                MessageBox.Show("Dosyalar başarıyla yüklendi!", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                AddLog($"HATA: {ex.Message}");
            }
            finally
            {
                SetControlsEnabled(true);
                lblStatus.Text = "Hazır...";
                progressBar.Value = 0;
            }
        }

        private async Task UploadFolderToFtp()
        {
            string ftpHost = txtFtpHost.Text.Trim();
            if (!ftpHost.StartsWith("ftp://"))
            {
                ftpHost = "ftp://" + ftpHost;
            }

            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;
            int port = 21;
            int.TryParse(txtPort.Text, out port);

            string localPath = txtFolderPath.Text;

            // Hariç tutulacak klasörler (PHP projeleri için)
            string[] excludedFolders = new string[]
            {
                ".git", "node_modules", "vendor", "cache", ".idea",
                ".vscode", "temp", "tmp", "__pycache__", ".vs",
                "storage/logs", "storage/framework/cache", "storage/framework/sessions"
            };

            // Hariç tutulacak dosyalar
            string[] excludedFiles = new string[]
            {
                ".DS_Store", "Thumbs.db", "desktop.ini", ".gitignore",
                ".gitattributes", ".editorconfig", ".env", ".env.example",
                "composer.lock", "package-lock.json", "yarn.lock"
            };

            AddLog("Dosyalar taranıyor...");

            // Tüm dosyaları al ve filtrele
            string[] allFiles = Directory.GetFiles(localPath, "*.*", SearchOption.AllDirectories)
                .Where(file =>
                {
                    string relativePath = file.Substring(localPath.Length).TrimStart('\\', '/');
                    string[] pathParts = relativePath.Split('\\', '/');

                    // Hariç tutulan klasörleri kontrol et
                    foreach (string excludedFolder in excludedFolders)
                    {
                        if (pathParts.Any(part => part.Equals(excludedFolder, StringComparison.OrdinalIgnoreCase)))
                        {
                            return false;
                        }
                    }

                    // Hariç tutulan dosyaları kontrol et
                    string fileName = Path.GetFileName(file);
                    if (excludedFiles.Any(ef => fileName.Equals(ef, StringComparison.OrdinalIgnoreCase)))
                    {
                        return false;
                    }

                    return true;
                })
                .ToArray();

            if (allFiles.Length == 0)
            {
                AddLog("Klasörde yüklenecek dosya bulunamadı!");
                MessageBox.Show("Yüklenecek dosya bulunamadı!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            progressBar.Maximum = allFiles.Length;
            AddLog($"─────────────────────────────────");
            AddLog($"Hariç tutulan klasörler: {string.Join(", ", excludedFolders.Take(5))}...");
            AddLog($"Toplam {allFiles.Length} dosya yüklenecek.");
            AddLog($"FTP Sunucu: {ftpHost}");
            AddLog($"─────────────────────────────────");

            int successCount = 0;
            int failCount = 0;

            foreach (string filePath in allFiles)
            {
                try
                {
                    // Yerel dosyanın klasör yapısını koru
                    string relativePath = filePath.Substring(localPath.Length).TrimStart('\\', '/');
                    string ftpFilePath = ftpHost.TrimEnd('/') + "/" + relativePath.Replace('\\', '/');

                    lblStatus.Text = $"Yükleniyor: {relativePath}...";
                    Application.DoEvents();

                    // Önce FTP'de klasör yapısını oluştur (URL-safe şekilde)
                    string ftpDirPath = GetFtpDirectoryPath(ftpFilePath);
                    if (!string.IsNullOrEmpty(ftpDirPath))
                    {
                        await CreateFtpDirectory(ftpDirPath, username, password);
                    }

                    // Dosyayı yükle
                    bool uploaded = await UploadFileToFtp(filePath, ftpFilePath, username, password);

                    if (uploaded)
                    {
                        successCount++;
                        AddLog($"✓ Yüklendi: {relativePath}");
                    }
                    else
                    {
                        failCount++;
                        AddLog($"✗ Yüklenemedi: {relativePath}");
                    }

                    progressBar.Value++;
                }
                catch (Exception ex)
                {
                    failCount++;
                    AddLog($"✗ Hata ({Path.GetFileName(filePath)}): {ex.Message}");
                    progressBar.Value++;
                }
            }

            AddLog("─────────────────────────────────");
            AddLog($"İşlem Tamamlandı! Başarılı: {successCount}, Başarısız: {failCount}");
        }

        // ZIP ile hızlı yükleme
        private async Task UploadAsZip()
        {
            string ftpHost = txtFtpHost.Text.Trim();
            if (!ftpHost.StartsWith("ftp://"))
            {
                ftpHost = "ftp://" + ftpHost;
            }

            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;
            string localPath = txtFolderPath.Text;

            // Hariç tutulacak klasörler
            string[] excludedFolders = new string[]
            {
                ".git", "node_modules", "vendor", "cache", ".idea",
                ".vscode", "temp", "tmp", "__pycache__", ".vs",
                "storage/logs", "storage/framework/cache", "storage/framework/sessions"
            };

            string zipFileName = $"upload_{DateTime.Now:yyyyMMdd_HHmmss}.zip";
            string tempZipPath = Path.Combine(Path.GetTempPath(), zipFileName);

            try
            {
                // 1. ZIP dosyası oluştur
                AddLog("─────────────────────────────────");
                AddLog($"📦 ZIP dosyası oluşturuluyor...");
                lblStatus.Text = "ZIP dosyası oluşturuluyor...";
                progressBar.Style = ProgressBarStyle.Marquee;

                await Task.Run(() =>
                {
                    using (ZipArchive zip = ZipFile.Open(tempZipPath, ZipArchiveMode.Create))
                    {
                        var files = Directory.GetFiles(localPath, "*.*", SearchOption.AllDirectories)
                            .Where(file =>
                            {
                                string relativePath = file.Substring(localPath.Length).TrimStart('\\', '/');
                                string[] pathParts = relativePath.Split('\\', '/');

                                // Hariç tutulan klasörleri kontrol et
                                foreach (string excludedFolder in excludedFolders)
                                {
                                    if (pathParts.Any(part => part.Equals(excludedFolder, StringComparison.OrdinalIgnoreCase)))
                                        return false;
                                }
                                return true;
                            });

                        foreach (string file in files)
                        {
                            string relativePath = file.Substring(localPath.Length).TrimStart('\\', '/');
                            zip.CreateEntryFromFile(file, relativePath, CompressionLevel.Fastest);
                        }
                    }
                });

                FileInfo zipInfo = new FileInfo(tempZipPath);
                AddLog($"✓ ZIP oluşturuldu: {zipFileName} ({zipInfo.Length / 1024 / 1024:F2} MB)");

                // 2. ZIP'i FTP'ye yükle
                AddLog($"⬆ ZIP dosyası FTP'ye yükleniyor...");
                lblStatus.Text = "ZIP FTP'ye yükleniyor...";

                string ftpZipPath = ftpHost.TrimEnd('/') + "/" + zipFileName;
                bool zipUploaded = await UploadFileToFtp(tempZipPath, ftpZipPath, username, password);

                if (!zipUploaded)
                {
                    throw new Exception("ZIP dosyası yüklenemedi!");
                }

                AddLog($"✓ ZIP FTP'ye yüklendi!");

                // 3. PHP unzip scripti oluştur
                AddLog($"📝 PHP unzip scripti oluşturuluyor...");
                string phpScript = CreateUnzipPhpScript(zipFileName);
                string phpScriptPath = Path.Combine(Path.GetTempPath(), "unzip.php");
                File.WriteAllText(phpScriptPath, phpScript);

                // 4. PHP scriptini yükle
                string ftpPhpPath = ftpHost.TrimEnd('/') + "/unzip.php";
                bool phpUploaded = await UploadFileToFtp(phpScriptPath, ftpPhpPath, username, password);

                string hostForUrl = ftpHost.Replace("ftp://", "http://");
                string unzipUrl = hostForUrl.TrimEnd('/') + "/unzip_auto.php";
                bool autoUnzipSuccess = false;

                if (!phpUploaded)
                {
                    AddLog($"⚠ PHP scripti yüklenemedi - manuel açmanız gerekecek");
                }
                else
                {
                    AddLog($"✓ PHP unzip scripti yüklendi!");

                    // 5. Otomatik unzip'i tetikle
                    AddLog($"🔓 ZIP otomatik açılıyor...");
                    lblStatus.Text = "ZIP dosyası açılıyor...";

                    try
                    {
                        autoUnzipSuccess = await TriggerAutoUnzip(unzipUrl, zipFileName);

                        if (autoUnzipSuccess)
                        {
                            AddLog($"✅ ZIP başarıyla açıldı!");

                            // Temizlik scriptini çalıştır
                            AddLog($"🗑️ Geçici dosyalar temizleniyor...");
                            await CleanupZipFiles(ftpHost, zipFileName, username, password);
                        }
                        else
                        {
                            AddLog($"⚠ Otomatik açma başarısız - manuel deneyiniz");
                        }
                    }
                    catch (Exception ex)
                    {
                        AddLog($"⚠ Otomatik açma hatası: {ex.Message}");
                    }
                }

                progressBar.Style = ProgressBarStyle.Blocks;
                progressBar.Value = 100;

                // 6. Kullanıcıya sonucu göster
                string message;
                if (autoUnzipSuccess)
                {
                    message = $"✅ İŞLEM TAMAMEN TAMAMLANDI!\n\n" +
                              $"📦 ZIP oluşturuldu: {zipFileName}\n" +
                              $"⬆️ FTP'ye yüklendi\n" +
                              $"🔓 Otomatik açıldı\n" +
                              $"🗑️ Geçici dosyalar silindi\n\n" +
                              $"Tüm dosyalarınız sunucuda hazır!";
                }
                else if (phpUploaded)
                {
                    message = $"✅ ZIP yüklendi ama otomatik açılamadı.\n\n" +
                              $"📦 Dosya: {zipFileName}\n\n" +
                              $"MANUEL AÇMAK İÇİN 2 YÖNTEM:\n\n" +
                              $"YÖNTEM 1 - Tarayıcıdan:\n" +
                              $"{unzipUrl.Replace("_auto", "")}\n" +
                              $"Bu URL'i açıp 'Unzip' butonuna tıklayın.\n\n" +
                              $"YÖNTEM 2 - cPanel:\n" +
                              $"File Manager'da {zipFileName} dosyasına\n" +
                              $"sağ tıklayıp 'Extract' seçin.";

                    // Clipboard'a kopyala
                    try
                    {
                        Clipboard.SetText(unzipUrl.Replace("_auto", ""));
                        AddLog($"📋 URL panoya kopyalandı!");
                    }
                    catch { }
                }
                else
                {
                    message = $"✅ ZIP dosyası yüklendi ama PHP scripti yüklenemedi.\n\n" +
                              $"📦 Dosya: {zipFileName}\n\n" +
                              $"MANUEL AÇMAK İÇİN:\n" +
                              $"1. cPanel File Manager'a girin\n" +
                              $"2. {zipFileName} dosyasına sağ tıklayın\n" +
                              $"3. 'Extract' veya 'Açıkla' seçeneğini seçin";
                }

                AddLog("─────────────────────────────────");
                AddLog($"✅ İşlem tamamlandı!");

                MessageBox.Show(message, autoUnzipSuccess ? "Başarılı! 🎉" : "Yükleme Tamamlandı",
                    MessageBoxButtons.OK, autoUnzipSuccess ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
            }
            finally
            {
                // Geçici dosyaları temizle
                try
                {
                    if (File.Exists(tempZipPath))
                        File.Delete(tempZipPath);
                    if (File.Exists(Path.Combine(Path.GetTempPath(), "unzip.php")))
                        File.Delete(Path.Combine(Path.GetTempPath(), "unzip.php"));
                }
                catch { }

                progressBar.Style = ProgressBarStyle.Blocks;
            }
        }

        // PHP unzip scripti oluştur
        private string CreateUnzipPhpScript(string zipFileName)
        {
            return $@"<?php
// Auto-generated unzip script
$zipFile = '{zipFileName}';
$extractTo = './';

// Otomatik mod (auto=1 parametresi ile)
$autoMode = isset($_GET['auto']) && $_GET['auto'] == '1';

if (!file_exists($zipFile)) {{
    if ($autoMode) {{
        echo 'ERROR: ZIP file not found';
        exit;
    }}
    die('❌ ZIP dosyası bulunamadı: ' . $zipFile);
}}

if ($autoMode) {{
    // Otomatik açma modu
    $zip = new ZipArchive;
    if ($zip->open($zipFile) === TRUE) {{
        $numFiles = $zip->numFiles;
        $zip->extractTo($extractTo);
        $zip->close();
        echo 'SUCCESS: Extracted ' . $numFiles . ' files';
    }} else {{
        echo 'ERROR: Could not open ZIP file';
    }}
    exit;
}}

// Manuel mod - kullanıcı arayüzü
echo '<h2>📦 ZIP Dosyası Açılıyor...</h2>';
echo '<p><strong>Dosya:</strong> ' . $zipFile . '</p>';
echo '<p><strong>Boyut:</strong> ' . round(filesize($zipFile) / 1024 / 1024, 2) . ' MB</p>';
echo '<hr>';

if (isset($_POST['unzip'])) {{
    $zip = new ZipArchive;
    if ($zip->open($zipFile) === TRUE) {{
        echo '<p>✓ ZIP açılıyor...</p>';
        flush();

        $zip->extractTo($extractTo);
        $numFiles = $zip->numFiles;
        $zip->close();

        echo '<p style=""color: green; font-weight: bold;"">✅ Tüm dosyalar başarıyla açıldı!</p>';
        echo '<p>📁 Toplam ' . $numFiles . ' dosya açıldı.</p>';
        echo '<hr>';
        echo '<p><strong>ŞİMDİ YAPABİLİRSİNİZ:</strong></p>';
        echo '<ul>';
        echo '<li>✓ Bu dosyayı silebilirsiniz: <code>unzip.php</code></li>';
        echo '<li>✓ ZIP dosyasını silebilirsiniz: <code>' . $zipFile . '</code></li>';
        echo '</ul>';
        echo '<p style=""color: orange;"">⚠ GÜVENLİK: Bu dosyaları silmeyi unutmayın!</p>';
    }} else {{
        echo '<p style=""color: red;"">❌ ZIP açılamadı! Manuel olarak cPanel\'den açmayı deneyin.</p>';
    }}
}} else {{
    echo '<form method=""post"">';
    echo '<button type=""submit"" name=""unzip"" style=""padding: 15px 30px; font-size: 18px; background: #4CAF50; color: white; border: none; cursor: pointer; border-radius: 5px;"">🚀 DOSYALARI AÇ (UNZIP)</button>';
    echo '</form>';
    echo '<p style=""color: gray; margin-top: 20px;"">Not: Bu işlem birkaç saniye sürebilir.</p>';
}}
?>";
        }

        // Otomatik unzip'i tetikle
        private async Task<bool> TriggerAutoUnzip(string unzipUrl, string zipFileName)
        {
            try
            {
                // URL'i düzelt (unzip_auto.php -> unzip.php?auto=1)
                unzipUrl = unzipUrl.Replace("unzip_auto.php", "unzip.php?auto=1");

                using (System.Net.Http.HttpClient client = new System.Net.Http.HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(60); // 60 saniye timeout

                    var response = await client.GetAsync(unzipUrl);
                    string result = await response.Content.ReadAsStringAsync();

                    // Başarı kontrolü
                    if (result.Contains("SUCCESS"))
                    {
                        // Dosya sayısını parse et
                        if (result.Contains("Extracted"))
                        {
                            AddLog($"  → {result.Replace("SUCCESS: ", "")}");
                        }
                        return true;
                    }
                    else if (result.Contains("ERROR"))
                    {
                        AddLog($"  → Hata: {result.Replace("ERROR: ", "")}");
                        return false;
                    }
                    else
                    {
                        AddLog($"  → Beklenmeyen yanıt: {result.Substring(0, Math.Min(100, result.Length))}");
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                AddLog($"  → HTTP hatası: {ex.Message}");
                return false;
            }
        }

        // ZIP ve unzip.php dosyalarını temizle
        private async Task CleanupZipFiles(string ftpHost, string zipFileName, string username, string password)
        {
            try
            {
                // ZIP dosyasını sil
                string ftpZipPath = ftpHost.TrimEnd('/') + "/" + zipFileName;
                FtpWebRequest zipDelRequest = (FtpWebRequest)WebRequest.Create(ftpZipPath);
                zipDelRequest.Method = WebRequestMethods.Ftp.DeleteFile;
                zipDelRequest.Credentials = new NetworkCredential(username, password);

                try
                {
                    using (var response = await zipDelRequest.GetResponseAsync())
                    {
                        AddLog($"  ✓ ZIP dosyası silindi: {zipFileName}");
                    }
                }
                catch
                {
                    AddLog($"  ⚠ ZIP dosyası silinemedi (manuel silin)");
                }

                // unzip.php'yi sil
                string ftpPhpPath = ftpHost.TrimEnd('/') + "/unzip.php";
                FtpWebRequest phpDelRequest = (FtpWebRequest)WebRequest.Create(ftpPhpPath);
                phpDelRequest.Method = WebRequestMethods.Ftp.DeleteFile;
                phpDelRequest.Credentials = new NetworkCredential(username, password);

                try
                {
                    using (var response = await phpDelRequest.GetResponseAsync())
                    {
                        AddLog($"  ✓ unzip.php silindi");
                    }
                }
                catch
                {
                    AddLog($"  ⚠ unzip.php silinemedi (manuel silin)");
                }
            }
            catch (Exception ex)
            {
                AddLog($"  ⚠ Temizlik hatası: {ex.Message}");
            }
        }

        private async Task<bool> UploadFileToFtp(string localFilePath, string ftpFilePath, string username, string password)
        {
            try
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpFilePath);
                request.Method = WebRequestMethods.Ftp.UploadFile;
                request.Credentials = new NetworkCredential(username, password);
                request.UseBinary = true;
                request.KeepAlive = false;

                // Dosyayı oku ve yükle
                byte[] fileContents = await Task.Run(() => File.ReadAllBytes(localFilePath));
                request.ContentLength = fileContents.Length;

                using (Stream requestStream = await request.GetRequestStreamAsync())
                {
                    await requestStream.WriteAsync(fileContents, 0, fileContents.Length);
                }

                using (FtpWebResponse response = (FtpWebResponse)await request.GetResponseAsync())
                {
                    return response.StatusCode == FtpStatusCode.ClosingData ||
                           response.StatusCode == FtpStatusCode.FileActionOK;
                }
            }
            catch
            {
                return false;
            }
        }

        // FTP URL'den klasör yolunu çıkar (dosya adı olmadan)
        private string GetFtpDirectoryPath(string ftpFilePath)
        {
            try
            {
                // Son slash'ten önceki kısmı al (dosya adını çıkar)
                int lastSlash = ftpFilePath.LastIndexOf('/');
                if (lastSlash > 0)
                {
                    string dirPath = ftpFilePath.Substring(0, lastSlash);

                    // ftp://example.com gibi base URL'leri hariç tut
                    if (dirPath.EndsWith("://") || !dirPath.Contains("://"))
                        return null;

                    // En az bir klasör içermeli (sadece ftp://host olmamalı)
                    string[] parts = dirPath.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length <= 2) // ["ftp:", "example.com"]
                        return null;

                    return dirPath;
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        private async Task CreateFtpDirectory(string dirPath, string username, string password)
        {
            try
            {
                if (string.IsNullOrEmpty(dirPath) || dirPath.EndsWith("://"))
                    return;

                // Base URL'i hariç tut (ftp://host kısmı)
                string[] parts = dirPath.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length <= 2) // ["ftp:", "example.com"]
                    return;

                // Önce üst klasörü oluştur (recursive)
                int lastSlash = dirPath.LastIndexOf('/');
                if (lastSlash > 0)
                {
                    string parentDir = dirPath.Substring(0, lastSlash);
                    string[] parentParts = parentDir.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parentParts.Length > 2) // Parent da klasör içeriyorsa
                    {
                        await CreateFtpDirectory(parentDir, username, password);
                    }
                }

                // Klasörün var olup olmadığını kontrol et
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(dirPath);
                request.Method = WebRequestMethods.Ftp.ListDirectory;
                request.Credentials = new NetworkCredential(username, password);
                request.KeepAlive = false;
                request.Timeout = 5000; // 5 saniye timeout

                try
                {
                    using (FtpWebResponse response = (FtpWebResponse)await request.GetResponseAsync())
                    {
                        // Klasör zaten var
                        return;
                    }
                }
                catch (WebException)
                {
                    // Klasör yok, oluştur
                    try
                    {
                        FtpWebRequest createRequest = (FtpWebRequest)WebRequest.Create(dirPath);
                        createRequest.Method = WebRequestMethods.Ftp.MakeDirectory;
                        createRequest.Credentials = new NetworkCredential(username, password);
                        createRequest.KeepAlive = false;

                        using (FtpWebResponse response = (FtpWebResponse)await createRequest.GetResponseAsync())
                        {
                            // Klasör oluşturuldu
                            string folderName = dirPath.Substring(dirPath.LastIndexOf('/') + 1);
                            AddLog($"  → Klasör oluşturuldu: {folderName}");
                        }
                    }
                    catch (Exception ex)
                    {
                        // Klasör oluşturulamadı - log tut ama devam et
                        AddLog($"  ⚠ Klasör oluşturulamadı: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                // Genel hata - log tut ama devam et
                AddLog($"  ⚠ CreateFtpDirectory hatası: {ex.Message}");
            }
        }

        private void AddLog(string message)
        {
            lstLog.Items.Add($"[{DateTime.Now:HH:mm:ss}] {message}");
            lstLog.TopIndex = lstLog.Items.Count - 1;
            Application.DoEvents();
        }

        private void SetControlsEnabled(bool enabled)
        {
            txtFtpHost.Enabled = enabled;
            txtUsername.Enabled = enabled;
            txtPassword.Enabled = enabled;
            txtPort.Enabled = enabled;
            txtFolderPath.Enabled = enabled;
            btnBrowse.Enabled = enabled;
            btnUpload.Enabled = enabled;
            btnListFiles.Enabled = enabled;
            btnDeleteSelected.Enabled = enabled;
            btnDeleteAll.Enabled = enabled;
            btnRename.Enabled = enabled;
            btnForceDeleteCorrupted.Enabled = enabled;
        }

        // FTP dosyalarını listele
        private async void btnListFiles_Click(object sender, EventArgs e)
        {
            // Doğrulamalar
            if (string.IsNullOrWhiteSpace(txtFtpHost.Text) || txtFtpHost.Text == "ftp://")
            {
                MessageBox.Show("Lütfen FTP sunucu adresini giriniz!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                MessageBox.Show("Lütfen kullanıcı adını giriniz!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Lütfen şifre giriniz!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SetControlsEnabled(false);
            lstFtpFiles.Items.Clear();
            lblStatus.Text = "FTP dosyaları listeleniyor...";

            try
            {
                string ftpHost = txtFtpHost.Text.Trim();
                if (!ftpHost.StartsWith("ftp://"))
                {
                    ftpHost = "ftp://" + ftpHost;
                }

                string username = txtUsername.Text.Trim();
                string password = txtPassword.Text;

                AddLog("FTP sunucusuna bağlanılıyor...");
                var items = await ListFtpItemsRecursive(ftpHost, username, password);

                if (items.Count == 0)
                {
                    AddLog("FTP sunucusunda dosya/klasör bulunamadı.");
                    MessageBox.Show("FTP sunucusunda dosya/klasör bulunamadı.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    int fileCount = items.Count(i => !i.IsDirectory);
                    int folderCount = items.Count(i => i.IsDirectory);

                    foreach (var item in items)
                    {
                        lstFtpFiles.Items.Add(item); // ToString() otomatik çağrılır
                    }

                    AddLog($"Toplam {fileCount} dosya, {folderCount} klasör listelendi.");
                    MessageBox.Show($"{fileCount} dosya, {folderCount} klasör bulundu.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                AddLog($"HATA: {ex.Message}");
            }
            finally
            {
                SetControlsEnabled(true);
                lblStatus.Text = "Hazır...";
            }
        }

        private async Task<List<FtpItem>> ListFtpItemsRecursive(string ftpPath, string username, string password)
        {
            List<FtpItem> allItems = new List<FtpItem>();

            try
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpPath);
                request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                request.Credentials = new NetworkCredential(username, password);

                using (FtpWebResponse response = (FtpWebResponse)await request.GetResponseAsync())
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    string line;
                    while ((line = await reader.ReadLineAsync()) != null)
                    {
                        if (string.IsNullOrWhiteSpace(line))
                            continue;

                        // Unix/Linux formatı için basit parsing
                        string[] tokens = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        if (tokens.Length < 9)
                            continue;

                        string name = string.Join(" ", tokens.Skip(8));
                        if (name == "." || name == "..")
                            continue;

                        string fullPath = ftpPath.TrimEnd('/') + "/" + name;

                        // Dizin mi dosya mı kontrol et
                        if (line.StartsWith("d"))
                        {
                            // Dizin - listeye ekle
                            allItems.Add(new FtpItem { Path = fullPath, IsDirectory = true });

                            // Dizin - recursive olarak içini listele
                            var subItems = await ListFtpItemsRecursive(fullPath, username, password);
                            allItems.AddRange(subItems);
                        }
                        else
                        {
                            // Dosya - listeye ekle
                            allItems.Add(new FtpItem { Path = fullPath, IsDirectory = false });
                        }
                    }
                }
            }
            catch
            {
                // Hata durumunda devam et
            }

            return allItems;
        }

        // Seçili dosyaları/klasörleri sil
        private async void btnDeleteSelected_Click(object sender, EventArgs e)
        {
            if (lstFtpFiles.CheckedItems.Count == 0)
            {
                MessageBox.Show("Lütfen silinecek öğeleri seçiniz!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var result = MessageBox.Show(
                $"{lstFtpFiles.CheckedItems.Count} öğe silinecek. Emin misiniz?",
                "Onay",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result != DialogResult.Yes)
                return;

            SetControlsEnabled(false);
            lblStatus.Text = "Öğeler siliniyor...";

            try
            {
                string username = txtUsername.Text.Trim();
                string password = txtPassword.Text;

                int successCount = 0;
                int failCount = 0;

                // Klasörleri en sona al (önce dosyaları sil)
                var checkedItems = lstFtpFiles.CheckedItems.Cast<FtpItem>()
                    .OrderBy(i => i.IsDirectory)
                    .ThenByDescending(i => i.Path.Length)
                    .ToList();

                foreach (FtpItem item in checkedItems)
                {
                    bool deleted = await DeleteFtpItem(item, username, password);
                    if (deleted)
                    {
                        successCount++;
                        string itemType = item.IsDirectory ? "KLASÖR" : "DOSYA";
                        AddLog($"✓ Silindi ({itemType}): {item.Path}");
                    }
                    else
                    {
                        failCount++;
                        string itemType = item.IsDirectory ? "KLASÖR" : "DOSYA";
                        AddLog($"✗ Silinemedi ({itemType}): {item.Path}");
                    }
                }

                AddLog($"İşlem Tamamlandı! Silinen: {successCount}, Başarısız: {failCount}");
                MessageBox.Show($"Silinen: {successCount}\nBaşarısız: {failCount}", "İşlem Tamamlandı", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Listeyi yenile
                btnListFiles_Click(sender, e);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                AddLog($"HATA: {ex.Message}");
            }
            finally
            {
                SetControlsEnabled(true);
                lblStatus.Text = "Hazır...";
            }
        }

        // Tüm dosyaları/klasörleri sil
        private async void btnDeleteAll_Click(object sender, EventArgs e)
        {
            if (lstFtpFiles.Items.Count == 0)
            {
                MessageBox.Show("Silinecek öğe bulunamadı!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var result1 = MessageBox.Show(
                $"DİKKAT! FTP sunucusundaki TÜM ÖĞELER ({lstFtpFiles.Items.Count} adet) silinecek!\n\nBu işlem geri alınamaz. Emin misiniz?",
                "UYARI",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result1 != DialogResult.Yes)
                return;

            var result2 = MessageBox.Show(
                "Son kez soruyorum: TÜM ÖĞELERİ (DOSYA VE KLASÖR) SİLMEK İSTEDİĞİNİZDEN EMİN MİSİNİZ?",
                "ONAY",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Stop);

            if (result2 != DialogResult.Yes)
                return;

            SetControlsEnabled(false);
            lblStatus.Text = "Tüm öğeler siliniyor...";

            try
            {
                string username = txtUsername.Text.Trim();
                string password = txtPassword.Text;

                int successCount = 0;
                int failCount = 0;

                // Önce dosyaları, sonra klasörleri sil (en derin klasörden başla)
                List<FtpItem> itemsToDelete = lstFtpFiles.Items.Cast<FtpItem>()
                    .OrderBy(i => i.IsDirectory)
                    .ThenByDescending(i => i.Path.Length)
                    .ToList();

                foreach (FtpItem item in itemsToDelete)
                {
                    bool deleted = await DeleteFtpItem(item, username, password);
                    if (deleted)
                    {
                        successCount++;
                        string itemType = item.IsDirectory ? "KLASÖR" : "DOSYA";
                        AddLog($"✓ Silindi ({itemType}): {item.Path}");
                    }
                    else
                    {
                        failCount++;
                        string itemType = item.IsDirectory ? "KLASÖR" : "DOSYA";
                        AddLog($"✗ Silinemedi ({itemType}): {item.Path}");
                    }
                }

                AddLog($"İşlem Tamamlandı! Silinen: {successCount}, Başarısız: {failCount}");
                MessageBox.Show($"Silinen: {successCount}\nBaşarısız: {failCount}", "İşlem Tamamlandı", MessageBoxButtons.OK, MessageBoxIcon.Information);

                lstFtpFiles.Items.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                AddLog($"HATA: {ex.Message}");
            }
            finally
            {
                SetControlsEnabled(true);
                lblStatus.Text = "Hazır...";
            }
        }

        private async Task<bool> DeleteFileFromFtp(string ftpFilePath, string username, string password)
        {
            try
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpFilePath);
                request.Method = WebRequestMethods.Ftp.DeleteFile;
                request.Credentials = new NetworkCredential(username, password);

                using (FtpWebResponse response = (FtpWebResponse)await request.GetResponseAsync())
                {
                    return response.StatusCode == FtpStatusCode.FileActionOK;
                }
            }
            catch
            {
                return false;
            }
        }

        // FTP öğesi sil (dosya veya klasör)
        private async Task<bool> DeleteFtpItem(FtpItem item, string username, string password)
        {
            try
            {
                if (item.IsDirectory)
                {
                    // Klasörse - önce içini boşalt
                    try
                    {
                        var subItems = await ListFtpItemsRecursive(item.Path, username, password);

                        // Önce dosyaları sil, sonra klasörleri (en derindan başa doğru)
                        foreach (var subItem in subItems.OrderBy(i => i.IsDirectory).ThenByDescending(i => i.Path.Length))
                        {
                            await DeleteFtpItem(subItem, username, password);
                        }
                    }
                    catch
                    {
                        // İçini boşaltamadıysa devam et
                    }

                    // Klasörü sil
                    FtpWebRequest request = (FtpWebRequest)WebRequest.Create(item.Path);
                    request.Method = WebRequestMethods.Ftp.RemoveDirectory;
                    request.Credentials = new NetworkCredential(username, password);

                    using (FtpWebResponse response = (FtpWebResponse)await request.GetResponseAsync())
                    {
                        return response.StatusCode == FtpStatusCode.FileActionOK;
                    }
                }
                else
                {
                    // Dosyaysa - direkt sil
                    FtpWebRequest request = (FtpWebRequest)WebRequest.Create(item.Path);
                    request.Method = WebRequestMethods.Ftp.DeleteFile;
                    request.Credentials = new NetworkCredential(username, password);

                    using (FtpWebResponse response = (FtpWebResponse)await request.GetResponseAsync())
                    {
                        return response.StatusCode == FtpStatusCode.FileActionOK;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        #region Kayıtlı Sunucular

        private readonly string serversFilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "FtpUploadApp",
            "servers.json"
        );

        // Sunucuları yükle
        private List<SavedServer> LoadServers()
        {
            try
            {
                if (!File.Exists(serversFilePath))
                    return new List<SavedServer>();

                var servers = new List<SavedServer>();
                string[] lines = File.ReadAllLines(serversFilePath);

                SavedServer currentServer = null;
                foreach (string line in lines)
                {
                    string trimmed = line.Trim();
                    if (trimmed == "{")
                    {
                        currentServer = new SavedServer();
                    }
                    else if (trimmed == "}" && currentServer != null)
                    {
                        servers.Add(currentServer);
                        currentServer = null;
                    }
                    else if (currentServer != null && trimmed.Contains(":"))
                    {
                        string[] parts = trimmed.Split(new[] { ':' }, 2);
                        string key = parts[0].Trim().Trim('"', ',');
                        string value = parts[1].Trim().Trim('"', ',');

                        if (key == "Name") currentServer.Name = value;
                        else if (key == "Host") currentServer.Host = value;
                        else if (key == "Username") currentServer.Username = value;
                        else if (key == "Password") currentServer.Password = value;
                        else if (key == "Port")
                        {
                            int port;
                            if (int.TryParse(value, out port))
                                currentServer.Port = port;
                        }
                    }
                }

                return servers;
            }
            catch
            {
                return new List<SavedServer>();
            }
        }

        // Sunucuları kaydet
        private void SaveServers(List<SavedServer> servers)
        {
            try
            {
                string directory = Path.GetDirectoryName(serversFilePath);
                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);

                var sb = new StringBuilder();
                sb.AppendLine("[");
                for (int i = 0; i < servers.Count; i++)
                {
                    var server = servers[i];
                    sb.AppendLine("  {");
                    sb.AppendLine($"    \"Name\": \"{server.Name}\",");
                    sb.AppendLine($"    \"Host\": \"{server.Host}\",");
                    sb.AppendLine($"    \"Username\": \"{server.Username}\",");
                    sb.AppendLine($"    \"Password\": \"{server.Password}\",");
                    sb.AppendLine($"    \"Port\": {server.Port}");
                    sb.AppendLine(i < servers.Count - 1 ? "  }," : "  }");
                }
                sb.AppendLine("]");

                File.WriteAllText(serversFilePath, sb.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Sunucular kaydedilemedi: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Bağlantı testi
        private async Task<bool> TestFtpConnection(string host, string username, string password, int port)
        {
            try
            {
                if (!host.StartsWith("ftp://"))
                    host = "ftp://" + host;

                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(host);
                request.Method = WebRequestMethods.Ftp.ListDirectory;
                request.Credentials = new NetworkCredential(username, password);
                request.Timeout = 10000; // 10 saniye

                using (FtpWebResponse response = (FtpWebResponse)await request.GetResponseAsync())
                {
                    return response.StatusCode == FtpStatusCode.OpeningData ||
                           response.StatusCode == FtpStatusCode.DataAlreadyOpen;
                }
            }
            catch
            {
                return false;
            }
        }

        // ComboBox'ı güncelle
        private void RefreshServerList()
        {
            var servers = LoadServers();
            cmbSavedServers.Items.Clear();
            foreach (var server in servers)
            {
                cmbSavedServers.Items.Add(server);
            }

            if (cmbSavedServers.Items.Count > 0)
                cmbSavedServers.SelectedIndex = 0;
        }

        // Form yüklendiğinde
        private void Form1_Load(object sender, EventArgs e)
        {
            RefreshServerList();
        }

        // Test et ve kaydet
        private async void btnTestAndSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtServerName.Text))
            {
                MessageBox.Show("Lütfen sunucu adı giriniz!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtFtpHost.Text) || txtFtpHost.Text == "ftp://")
            {
                MessageBox.Show("Lütfen FTP sunucu adresini giriniz!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                MessageBox.Show("Lütfen kullanıcı adını giriniz!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Lütfen şifre giriniz!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SetControlsEnabled(false);
            lblStatus.Text = "Bağlantı test ediliyor...";

            try
            {
                int port = 21;
                int.TryParse(txtPort.Text, out port);

                bool success = await TestFtpConnection(txtFtpHost.Text, txtUsername.Text, txtPassword.Text, port);

                if (success)
                {
                    // Sunucuyu kaydet
                    var servers = LoadServers();

                    // Aynı isimde varsa güncelle
                    var existing = servers.FirstOrDefault(s => s.Name == txtServerName.Text);
                    if (existing != null)
                        servers.Remove(existing);

                    servers.Add(new SavedServer
                    {
                        Name = txtServerName.Text.Trim(),
                        Host = txtFtpHost.Text.Trim(),
                        Username = txtUsername.Text.Trim(),
                        Password = txtPassword.Text,
                        Port = port
                    });

                    SaveServers(servers);
                    RefreshServerList();

                    AddLog($"✓ Sunucu '{txtServerName.Text}' başarıyla kaydedildi!");
                    MessageBox.Show("Bağlantı başarılı! Sunucu kaydedildi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    txtServerName.Clear();
                }
                else
                {
                    AddLog($"✗ Bağlantı başarısız!");
                    MessageBox.Show("FTP bağlantısı kurulamadı! Lütfen bilgileri kontrol edin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SetControlsEnabled(true);
                lblStatus.Text = "Hazır...";
            }
        }

        // Sunucu yükle
        private void btnLoadServer_Click(object sender, EventArgs e)
        {
            if (cmbSavedServers.SelectedItem is SavedServer server)
            {
                txtFtpHost.Text = server.Host;
                txtUsername.Text = server.Username;
                txtPassword.Text = server.Password;
                txtPort.Text = server.Port.ToString();

                AddLog($"✓ Sunucu '{server.Name}' yüklendi.");
                MessageBox.Show($"Sunucu '{server.Name}' bilgileri yüklendi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Lütfen bir sunucu seçiniz!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // Sunucu sil
        private void btnDeleteServer_Click(object sender, EventArgs e)
        {
            if (cmbSavedServers.SelectedItem is SavedServer server)
            {
                var result = MessageBox.Show(
                    $"'{server.Name}' sunucusunu silmek istediğinizden emin misiniz?",
                    "Onay",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    var servers = LoadServers();
                    servers.RemoveAll(s => s.Name == server.Name);
                    SaveServers(servers);
                    RefreshServerList();

                    AddLog($"✓ Sunucu '{server.Name}' silindi.");
                    MessageBox.Show($"Sunucu '{server.Name}' silindi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                MessageBox.Show("Lütfen bir sunucu seçiniz!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        #endregion

        // Seçili öğeyi yeniden adlandır
        private async void btnRename_Click(object sender, EventArgs e)
        {
            // Sadece bir öğe seçilmiş olmalı
            if (lstFtpFiles.SelectedItem == null)
            {
                MessageBox.Show("Lütfen yeniden adlandırılacak bir öğe seçiniz!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            FtpItem selectedItem = (FtpItem)lstFtpFiles.SelectedItem;

            // Mevcut adı göster
            string currentName = selectedItem.Path.Substring(selectedItem.Path.LastIndexOf('/') + 1);

            // Yeni ad iste
            string newName = Microsoft.VisualBasic.Interaction.InputBox(
                $"Yeni ad girin:\n\nMevcut ad: {currentName}\n\n" +
                (selectedItem.IsDirectory ? "(Klasör yeniden adlandırılacak)" : "(Dosya yeniden adlandırılacak)"),
                "Yeniden Adlandır",
                currentName);

            if (string.IsNullOrWhiteSpace(newName) || newName == currentName)
            {
                return; // Kullanıcı iptal etti veya aynı adı girdi
            }

            // Geçersiz karakterleri kontrol et
            char[] invalidChars = new char[] { '/', '\\', ':', '*', '?', '"', '<', '>', '|' };
            if (newName.IndexOfAny(invalidChars) >= 0)
            {
                MessageBox.Show("Dosya/klasör adı geçersiz karakterler içeremez:\n/ \\ : * ? \" < > |", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            SetControlsEnabled(false);
            lblStatus.Text = "Yeniden adlandırılıyor...";

            try
            {
                string username = txtUsername.Text.Trim();
                string password = txtPassword.Text;

                bool renamed = await RenameFtpItem(selectedItem, newName, username, password);

                if (renamed)
                {
                    AddLog($"✓ Yeniden adlandırıldı: {currentName} → {newName}");
                    MessageBox.Show($"'{currentName}' başarıyla '{newName}' olarak yeniden adlandırıldı.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Listeyi yenile
                    btnListFiles_Click(sender, e);
                }
                else
                {
                    AddLog($"✗ Yeniden adlandırılamadı: {currentName}");
                    MessageBox.Show("Yeniden adlandırma işlemi başarısız oldu!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                AddLog($"HATA: {ex.Message}");
            }
            finally
            {
                SetControlsEnabled(true);
                lblStatus.Text = "Hazır...";
            }
        }

        // FTP öğesini yeniden adlandır
        private async Task<bool> RenameFtpItem(FtpItem item, string newName, string username, string password)
        {
            try
            {
                // Eski ve yeni tam yol
                string oldPath = item.Path;
                string parentPath = oldPath.Substring(0, oldPath.LastIndexOf('/'));
                string newPath = parentPath + "/" + newName;

                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(oldPath);
                request.Method = WebRequestMethods.Ftp.Rename;
                request.Credentials = new NetworkCredential(username, password);
                request.RenameTo = newName; // Sadece yeni adı ver, tam yol değil

                using (FtpWebResponse response = (FtpWebResponse)await request.GetResponseAsync())
                {
                    return response.StatusCode == FtpStatusCode.FileActionOK;
                }
            }
            catch
            {
                return false;
            }
        }

        // Bozuk klasör için zorla silme (ham FTP komutu ile)
        private async void btnForceDeleteCorrupted_Click(object sender, EventArgs e)
        {
            if (lstFtpFiles.SelectedItem == null)
            {
                MessageBox.Show("Lütfen silinecek klasörü/dosyayı seçiniz!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            FtpItem selectedItem = (FtpItem)lstFtpFiles.SelectedItem;

            var result = MessageBox.Show(
                $"SEÇİLİ ÖĞEYE ZORLA SİLME KOMUTU GÖNDERİLECEK:\n\n{selectedItem.Path}\n\nEmin misiniz?",
                "ZORLA SİL",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result != DialogResult.Yes)
                return;

            SetControlsEnabled(false);
            lblStatus.Text = "Zorla siliniyor...";

            try
            {
                string username = txtUsername.Text.Trim();
                string password = txtPassword.Text;

                // Yöntem 1: Normal silme
                AddLog("Yöntem 1: Normal silme deneniyor...");
                bool deleted = await DeleteFtpItem(selectedItem, username, password);

                if (!deleted && selectedItem.IsDirectory)
                {
                    // Yöntem 2: İçeriği manuel sil, sonra klasörü sil
                    AddLog("Yöntem 2: İçerik manuel siliniyor...");
                    try
                    {
                        await ForceDeleteDirectory(selectedItem.Path, username, password);
                        deleted = true;
                    }
                    catch { }
                }

                if (!deleted)
                {
                    // Yöntem 3: Ham FTP komutu gönder
                    AddLog("Yöntem 3: Ham FTP komutu deneniyor...");
                    deleted = await SendRawFtpCommand(selectedItem, username, password);
                }

                if (deleted)
                {
                    AddLog($"✓ BAŞARILI: Öğe silindi!");
                    MessageBox.Show("Klasör/dosya başarıyla silindi!", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    btnListFiles_Click(sender, e);
                }
                else
                {
                    AddLog($"✗ TÜM YÖNTEMLER BAŞARISIZ");
                    MessageBox.Show(
                        "Klasör/dosya silinemedi.\n\nAlternatif Çözümler:\n" +
                        "1. cPanel File Manager'dan silin\n" +
                        "2. SSH/Terminal erişimi varsa komut satırından: rm -rf \"klasör_adı\"\n" +
                        "3. Farklı bir FTP istemcisi deneyin (FileZilla)\n" +
                        "4. Hosting sağlayıcınızla iletişime geçin",
                        "Silme Başarısız",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                AddLog($"HATA: {ex.Message}");
            }
            finally
            {
                SetControlsEnabled(true);
                lblStatus.Text = "Hazır...";
            }
        }

        // Klasörü zorla sil (tüm içeriği manuel temizle)
        private async Task ForceDeleteDirectory(string dirPath, string username, string password)
        {
            try
            {
                // Klasör içeriğini ham listele
                FtpWebRequest listRequest = (FtpWebRequest)WebRequest.Create(dirPath);
                listRequest.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                listRequest.Credentials = new NetworkCredential(username, password);

                using (FtpWebResponse response = (FtpWebResponse)await listRequest.GetResponseAsync())
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    string line;
                    List<string> itemsToDelete = new List<string>();

                    while ((line = await reader.ReadLineAsync()) != null)
                    {
                        if (string.IsNullOrWhiteSpace(line)) continue;

                        string[] tokens = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        if (tokens.Length < 9) continue;

                        string name = string.Join(" ", tokens.Skip(8));
                        if (name == "." || name == "..") continue;

                        bool isDir = line.StartsWith("d");
                        string fullPath = dirPath.TrimEnd('/') + "/" + name;

                        if (isDir)
                        {
                            // Alt klasörü recursive sil
                            await ForceDeleteDirectory(fullPath, username, password);
                        }
                        else
                        {
                            // Dosyayı sil
                            try
                            {
                                FtpWebRequest delRequest = (FtpWebRequest)WebRequest.Create(fullPath);
                                delRequest.Method = WebRequestMethods.Ftp.DeleteFile;
                                delRequest.Credentials = new NetworkCredential(username, password);
                                using (var delResponse = await delRequest.GetResponseAsync()) { }
                                AddLog($"  ✓ Silindi: {name}");
                            }
                            catch (Exception ex)
                            {
                                AddLog($"  ✗ Silinemedi: {name} - {ex.Message}");
                            }
                        }
                    }
                }

                // Şimdi boş klasörü sil
                FtpWebRequest rmdirRequest = (FtpWebRequest)WebRequest.Create(dirPath);
                rmdirRequest.Method = WebRequestMethods.Ftp.RemoveDirectory;
                rmdirRequest.Credentials = new NetworkCredential(username, password);
                using (var rmdirResponse = await rmdirRequest.GetResponseAsync()) { }
            }
            catch (Exception ex)
            {
                throw new Exception($"Klasör silinemedi: {ex.Message}");
            }
        }

        // Ham FTP komutu gönder (son çare)
        private async Task<bool> SendRawFtpCommand(FtpItem item, string username, string password)
        {
            try
            {
                // URL encode ile dene
                string encodedPath = item.Path;

                // Klasör adının son kısmını URL encode et
                int lastSlash = item.Path.LastIndexOf('/');
                if (lastSlash >= 0)
                {
                    string pathBase = item.Path.Substring(0, lastSlash + 1);
                    string folderName = item.Path.Substring(lastSlash + 1);
                    string encodedName = Uri.EscapeDataString(folderName);
                    encodedPath = pathBase + encodedName;
                }

                AddLog($"  URL Encoded path: {encodedPath}");

                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(encodedPath);
                request.Method = item.IsDirectory ? WebRequestMethods.Ftp.RemoveDirectory : WebRequestMethods.Ftp.DeleteFile;
                request.Credentials = new NetworkCredential(username, password);
                request.KeepAlive = false;

                using (FtpWebResponse response = (FtpWebResponse)await request.GetResponseAsync())
                {
                    return response.StatusCode == FtpStatusCode.FileActionOK;
                }
            }
            catch
            {
                return false;
            }
        }

        private void txtFtpHost_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
