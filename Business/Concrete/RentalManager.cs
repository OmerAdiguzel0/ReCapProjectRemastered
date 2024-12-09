using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business.Abstract;
using Business.Constants;
using Business.ValidationRules.FluentValidation;
using Core.Aspects.Autofac.Validation;
using Core.CrossCuttingConcerns.Validation.FluentValidation;
using Core.Utilities.Results.Abstract;
using Core.Utilities.Results.Concrete;
using DataAccess.Abstract;
using Entities.Concrete;

namespace Business.Concrete
{
    public class RentalManager:IRentalService
    {
        private IRentalDal _rentalDal;
        private ICustomerDal _customerDal;

        public RentalManager(IRentalDal rentalDal, ICustomerDal customerDal)
        {
            _rentalDal = rentalDal;
            _customerDal = customerDal;
        }

        public IDataResult<List<Rental>> GetAll()
        {
            return new SuccessDataResult<List<Rental>>(_rentalDal.GetAll());
        }

        [ValidationAspect(typeof(RentalValidator))]
        public IResult Add(Rental rental)
        {
            try
            {
                // Araç müsaitlik kontrolü
                var carAvailable = _rentalDal.GetAll(r => r.CarId == rental.CarId && 
                    r.ReturnDate >= rental.RentDate && 
                    r.RentDate <= rental.ReturnDate).Any();

                if (carAvailable)
                {
                    return new ErrorResult("Bu araç seçilen tarihler için müsait değil");
                }

                // Kullanıcının customer kaydını kontrol et veya oluştur
                var customer = _customerDal.Get(c => c.UserId == rental.CustomerId);
                if (customer == null)
                {
                    // Yeni customer kaydı oluştur
                    customer = new Customer
                    {
                        UserId = rental.CustomerId,
                        CompanyName = "Bireysel" // veya kullanıcıdan alınan başka bir değer
                    };
                    _customerDal.Add(customer);
                }

                // Rental kaydını customer ID ile oluştur
                rental.CustomerId = customer.CustomerId; // Id yerine CustomerId kullanıyoruz
                _rentalDal.Add(rental);
                
                return new SuccessResult("Kiralama işlemi başarıyla tamamlandı");
            }
            catch (Exception ex)
            {
                return new ErrorResult($"Kiralama işlemi sırasında bir hata oluştu: {ex.Message}");
            }
        }

        public IResult Delete(Rental rental)
        {
            _rentalDal.Delete(rental);
            return new SuccessResult();
        }

        public IResult Update(Rental rental)
        {
            _rentalDal.Update(rental);
            return new SuccessResult();
        }
    }
}
