using Entities.Concrete;
using FluentValidation;

namespace Business.ValidationRules.FluentValidation
{
    public class ModelValidator : AbstractValidator<Model>
    {
        public ModelValidator()
        {
            RuleFor(m => m.BrandId)
                .NotEmpty().WithMessage("Marka seçimi zorunludur")
                .GreaterThan(0).WithMessage("Geçerli bir marka seçmelisiniz");

            RuleFor(m => m.ModelName)
                .NotEmpty().WithMessage("Model adı zorunludur")
                .MinimumLength(2).WithMessage("Model adı en az 2 karakter olmalıdır")
                .MaximumLength(50).WithMessage("Model adı en fazla 50 karakter olabilir");
        }
    }
} 