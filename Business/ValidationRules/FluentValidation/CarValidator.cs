using Entities.Concrete;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.ValidationRules.FluentValidation
{
    public class CarValidator : AbstractValidator<Car>
    {
        public CarValidator()
        {
            RuleFor(c => c.BrandId)
                .NotEmpty().WithMessage("Marka seçimi zorunludur")
                .GreaterThan(0).WithMessage("Geçerli bir marka seçmelisiniz");

            RuleFor(c => c.ColorId)
                .NotEmpty().WithMessage("Renk seçimi zorunludur")
                .GreaterThan(0).WithMessage("Geçerli bir renk seçmelisiniz");

            RuleFor(c => c.ModelYear)
                .NotEmpty().WithMessage("Model yılı zorunludur")
                .GreaterThan(0).WithMessage("Geçerli bir model yılı giriniz");

            RuleFor(c => c.DailyPrice)
                .NotEmpty().WithMessage("Günlük fiyat zorunludur")
                .GreaterThan(0).WithMessage("Günlük fiyat 0'dan büyük olmalıdır");

            RuleFor(c => c.Description)
                .NotEmpty().WithMessage("Açıklama zorunludur")
                .MinimumLength(2).WithMessage("Açıklama en az 2 karakter olmalıdır");

            RuleFor(c => c.MinFindeksScore)
                .InclusiveBetween(0, 1500).WithMessage("Findeks puanı 0-1500 arasında olmalıdır");
        }
    }
}
