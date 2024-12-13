# ReCap Project - Backend

## 🚀 Proje Açıklaması
Bu proje, araç kiralama sisteminin backend tarafını oluşturan, .NET 6.0 ile geliştirilmiş, kurumsal mimari yapısında bir web API projesidir. Proje, **SOLID prensiplerine** uygun olarak geliştirilmiş ve **çok katmanlı mimari** yapısı kullanılmıştır.

---

## 📦 Katmanlar

### **Entities Layer**
- Veritabanı nesneleri
- DTO (Data Transfer Objects)
- Concrete ve Abstract klasörleri

### **Data Access Layer**
- Veritabanı işlemleri
- Entity Framework Core implementasyonu
- Repository Pattern uygulaması
- Abstract (Interfaces) ve Concrete implementasyonlar

### **Business Layer**
- İş kuralları
- Validasyon işlemleri
- Dependency Injection konfigürasyonları
- Servis implementasyonları

### **Core Layer**
- Tüm projelerde kullanılabilecek yapılar
- Cross Cutting Concerns
- Extensions
- Utilities
- Custom Exception Handlers

### **WebAPI Layer**
- RESTful API endpoints
- JWT Authentication
- Swagger implementasyonu
- CORS politikaları

---

## 🛠 Teknolojiler ve Frameworks
- **.NET 6.0**
- **Entity Framework Core 6.0**
- **Autofac (IoC Container)**
- **FluentValidation**
- **JWT Bearer Authentication**
- **AutoMapper**
- **Serilog**
- **Microsoft SQL Server**

---

## ⚙️ Kurulum

1. Projeyi klonlayın:
   ```bash
   git clone https://github.com/OmerAdiguzel0/ReCapProjectRemastered.git
   ```

2. Veritabanı bağlantı ayarlarını yapılandırın:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=RentACarDb;Trusted_Connection=True;"
     }
   }
   ```

3. Migration'ları uygulayın:
   ```bash
   dotnet ef database update
   ```

4. Projeyi çalıştırın:
   ```bash
   dotnet run
   ```

---

## 📌 API Endpoints

### **Auth Controller**
- `POST /api/auth/login`
- `POST /api/auth/register`

### **Cars Controller**
- `GET /api/cars/getall`
- `GET /api/cars/getbyid/{id}`
- `POST /api/cars/add`
- `PUT /api/cars/update`
- `DELETE /api/cars/delete`

### **Brands Controller**
- `GET /api/brands/getall`
- `POST /api/brands/add`
- `PUT /api/brands/update`
- `DELETE /api/brands/delete`

### **Users Controller**
- `GET /api/users/getall`
- `GET /api/users/getbyid/{id}`
- `PUT /api/users/update`
- `POST /api/users/updateprofileimage`

---

## 🔒 Güvenlik Özellikleri
- **JWT tabanlı kimlik doğrulama**
- **Role bazlı yetkilendirme**
- **Password hashing**
- **Secure token handling**
- **Request/Response logging**

---

## 🏗 Mimari Yapı

### **Dependency Injection**
```csharp
public class AutofacBusinessModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<CarManager>().As<ICarService>();
        builder.RegisterType<EfCarDal>().As<ICarDal>();
        // ... diğer servisler
    }
}
```

### **Validation**
```csharp
public class CarValidator : AbstractValidator<Car>
{
    public CarValidator()
    {
        RuleFor(c => c.BrandId).NotEmpty();
        RuleFor(c => c.DailyPrice).GreaterThan(0);
        // ... diğer kurallar
    }
}
```

---

## 📊 Veritabanı Şeması

### **Ana Tablolar**
- **Cars**
- **Brands**
- **Colors**
- **Users**
- **UserOperationClaims**
- **OperationClaims**
- **CarImages**
- **Rentals**

---

## 🔍 Hata Yönetimi
- **Global exception handling**
- **Custom exception types**
- **Detaylı hata loglaması**
- **Client-friendly error messages**

---

## 🔄 Cross-Cutting Concerns
- **Caching**
- **Logging**
- **Transaction management**
- **Validation**
- **Performance monitoring**

---

## 📝 Logging
- **Serilog implementasyonu ile:**
  - File logging
  - Console logging
  - Database logging (opsiyonel)

---

## 🚀 Performans
- **Asenkron operasyonlar**
- **Entity Framework optimizasyonları**
- **Caching mekanizmaları**
- **Minimal API kullanımı**

---

## 📄 Lisans
Bu proje **MIT lisansı** altında lisanslanmıştır.

---

Eğer daha fazla bilgiye ihtiyaç duyarsanız veya katkıda bulunmak isterseniz, lütfen iletişime geçin! 😊

