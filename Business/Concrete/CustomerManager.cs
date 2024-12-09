using System;
using System.Collections.Generic;
using Business.Abstract;
using Business.ValidationRules.FluentValidation;
using Core.Aspects.Autofac.Validation;
using Core.Utilities.Results.Abstract;
using Core.Utilities.Results.Concrete;
using DataAccess.Abstract;
using Entities.Concrete;

namespace Business.Concrete
{
    public class CustomerManager : ICustomerService
    {
        private ICustomerDal _customerDal;

        public CustomerManager(ICustomerDal customerDal)
        {
            _customerDal = customerDal;
        }

        public IDataResult<List<Customer>> GetAll()
        {
            return new SuccessDataResult<List<Customer>>(_customerDal.GetAll());
        }

        public IDataResult<Customer> GetById(int customerId)
        {
            return new SuccessDataResult<Customer>(_customerDal.Get(c => c.CustomerId == customerId));
        }

        public IDataResult<Customer> GetByUserId(int userId)
        {
            var customer = _customerDal.Get(c => c.UserId == userId);
            return new SuccessDataResult<Customer>(customer);
        }

        [ValidationAspect(typeof(CustomerValidator))]
        public IResult Add(Customer customer)
        {
            try
            {
                _customerDal.Add(customer);
                return new SuccessResult("Müşteri başarıyla eklendi");
            }
            catch (Exception ex)
            {
                return new ErrorResult($"Müşteri eklenirken hata oluştu: {ex.Message}");
            }
        }

        public IResult Delete(Customer customer)
        {
            try
            {
                _customerDal.Delete(customer);
                return new SuccessResult("Müşteri başarıyla silindi");
            }
            catch (Exception ex)
            {
                return new ErrorResult($"Müşteri silinirken hata oluştu: {ex.Message}");
            }
        }

        public IResult Update(Customer customer)
        {
            try
            {
                _customerDal.Update(customer);
                return new SuccessResult("Müşteri başarıyla güncellendi");
            }
            catch (Exception ex)
            {
                return new ErrorResult($"Müşteri güncellenirken hata oluştu: {ex.Message}");
            }
        }
    }
}
