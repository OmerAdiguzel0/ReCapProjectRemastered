using Entities.DTOs;
using FluentValidation;

namespace Business.ValidationRules.FluentValidation
{
    public class UserForRegisterDtoValidator : AbstractValidator<UserForRegisterDto>
    {
        public UserForRegisterDtoValidator()
        {
            RuleFor(u => u.FirstName).NotEmpty().WithMessage("Ad alanı boş olamaz");
            RuleFor(u => u.FirstName).MinimumLength(2).WithMessage("Ad en az 2 karakter olmalıdır");
            
            RuleFor(u => u.LastName).NotEmpty().WithMessage("Soyad alanı boş olamaz");
            RuleFor(u => u.LastName).MinimumLength(2).WithMessage("Soyad en az 2 karakter olmalıdır");
            
            RuleFor(u => u.Email).NotEmpty().WithMessage("Email alanı boş olamaz");
            RuleFor(u => u.Email).EmailAddress().WithMessage("Geçerli bir email adresi giriniz");
            
            RuleFor(u => u.Password).NotEmpty().WithMessage("Şifre alanı boş olamaz");
            RuleFor(u => u.Password).MinimumLength(6).WithMessage("Şifre en az 6 karakter olmalıdır");
        }
    }
}
