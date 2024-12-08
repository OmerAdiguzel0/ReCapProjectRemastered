using Core.Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Business.Constants
{
    public static class Messages
    {
        public static string CarAdded = "Araç Eklendi!";
        public static string CarDeleted = "Araç Silindi!";
        public static string CarUpdated = "Araç Güncellendi!";
        public static string MaintenanceTime = "Bakım Zamanı!";
        public static string CarsListed = "Araçler Listelendi!";
        public static string CustomerAdded = "Müşteri eklendi!";
        public static string CarAlreadyRented = "Araç Kiralama İşlemi Başarısız. Seçili Araç Kirada!";
        public static string RentalSuccessful = "Araç Kiralama İşlemi Başarılı!";
        public static string CouldNotCarAdded = "Araç Eklenemedi!";
        public static string CarCountOfBrandError="Bir Markadan En Fazla 10 Araç Eklenebilir!";
        public static string BrandNameAlreadyExists="Marka İsmi Veritabanına Zaten Kayıtlı!";
        public static string BrandAdded = "Marka Başarıyla Eklendi!";
        public static string BrandDeleted = "Marka Başarıyla Silindi!";
        public static string BrandUpdated = "Marka Başarıyla Güncellendi!";
        public static string BrandNotFound = "Marka Bulunamadı!";
        public static string ColorLimitExceded="Renk Limiti Aşıldı!";
        public static string ColorNameAlreadyExists = "Renk İsmi Veritabanına Zaten Kayıtlı!";
        public static string ColorAdded = "Renk Başarıyla Eklendi!";
        public static string ColorDeleted = "Renk Başarıyla Silindi!";
        public static string ColorUpdated = "Renk Başarıyla Güncellendi!";
        public static string ColorNotFound = "Renk Bulunamadı!";
        public static string ImageUploaded="Resim Yüklendi!";
        public static string DeletedImage="Resim Silindi!";
        public static string UpdatedImage="Resim Güncellendi!";
        public static string CarImageCountOfCarError="Bir Arabanın En Fazla 5 Resmi Olabilir";
        public static string AuthorizationDenied="Yetkiniz Yok!";
        public static string UserRegistered="Kayıt Oldu";
        public static string UserNotFound="Kullanıcı Bulunamadı";
        public static string PasswordError="Şifre Hatalı";
        public static string SuccessfulLogin="Başarılı Giriş";
        public static string UserAlreadyExists="Kullanıcı Mevcut";
        public static string AccessTokenCreated="Token Oluşturuldu";
        public static string DeletedUser="Kullanıcı Silindi";
        public static string UserList= "Kullanıcı Listelendi";
        public static string UpdatedUser= "Kullanıcı Güncellendi";
        public static string CarAddedWithDetails = "Araba başarıyla eklendi. ID: {0}";
        public static string CarNotFound = "Araç bulunamadı";
    }
}
