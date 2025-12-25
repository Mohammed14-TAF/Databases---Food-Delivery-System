YEMEKSEPETI VERITABANI VE WEB PROJESI
===================================

1. PROJE TANIMI
---------------
Bu proje, ASP.NET Core MVC ve SQL Server kullanılarak geliştirilmiş
bir online yemek siparişi sistemidir.

Sistem; kullanıcıların restoranları görüntülemesini, yemekleri sepete
eklemesini, sipariş vermesini ve sipariş geçmişini görmesini sağlar.
Admin paneli üzerinden restoran, menü, kullanıcı ve sipariş yönetimi
yapılabilmektedir.

---

2. KULLANILAN TEKNOLOJILER
--------------------------
- ASP.NET Core MVC
- C#
- SQL Server
- SQL Server Management Studio (SSMS)
- ADO.NET
- HTML / CSS / Bootstrap

---

3. DIL DESTEGI
--------------
Proje arayüzü varsayılan olarak Inglizce dilindedir.
Ileride başka diller eklenebilir.

--------------

4. VERITABANI KURULUMU (ZORUNLU)
--------------------------------

ADIM 1: GEREKLI YAZILIMLAR
- SQL Server
- SQL Server Management Studio (SSMS)
- Visual Studio 2022 veya üzeri

ADIM 2: VERITABANI OLUSTURMA
SSMS üzerinden aşağıdaki komutu çalıştırın:

CREATE DATABASE YemekSepeti;

ADIM 3: SQL SCRIPT CALISTIRMA
Proje klasörü içinde bulunan:

YemekSepeti_Database.sql

dosyasını SSMS'te açın ve EXECUTE edin.

Bu script şunları içerir:
- Tablolar
- Primary Key ve Foreign Key'ler
- Triggers
- Stored Procedures
- Views
- Functions
- Örnek veriler

---

5. UYGULAMA KURULUMU
---------------------

1. Projeyi Visual Studio ile açın
2. appsettings.json dosyasını açın
3. Aşağıdaki Connection String'i kendi bilgisayarınıza göre düzenleyin:

"ConnectionStrings": {
  "DefaultConnection": "Server=.;Database=YemekSepeti;Trusted_Connection=True;TrustServerCertificate=True;"
}

NOT:
Eğer SQL Server ismi farklıysa (örneğin DESKTOP-XXXX),
Server kısmını değiştiriniz.

4. Projeyi çalıştırın (Run)

---

6. ORNEK GIRIS BILGILERI
------------------------

ADMIN:
Email    : admin@test.com
Password : 123456

KULLANICI:
Email    : user@test.com
Password : 123456

---

7. SISTEM OZELLIKLERI
----------------------

KULLANICI:
- Kayıt olma ve giriş yapma
- Restoran ve menüleri görüntüleme
- Sepete ekleme
- Sipariş verme
- Sipariş geçmişi görüntüleme

ADMIN:
- Restoran yönetimi
- Menü yönetimi
- Kullanıcı yönetimi
- Siparişleri görüntüleme
- Sipariş durumunu değiştirme

---

8. VERITABANI NESNELERI
------------------------

- 5 Tablo
- 4 Trigger
- 5 Stored Procedure
- 1 View (vw_SiparisOzeti)
- 3 Function (UDF)
- ER Diagram (Foreign Key'ler ile otomatik oluşturulmuştur)

---

9. TASINABILIRLIK
------------------
Proje, SQL script ve yapılandırılabilir connection string sayesinde
farklı bilgisayarlarda sorunsuz çalışacak şekilde tasarlanmıştır.

---

10. ACIKLAMA
-------------
Bu proje bir akademik veritabanı ve web programlama ödevi kapsamında
geliştirilmiştir.
