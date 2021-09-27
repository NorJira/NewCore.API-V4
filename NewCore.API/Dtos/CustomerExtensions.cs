using System;
using NewCore.API.Dtos;
using NewCore.Data.Models;

namespace NewCore.API.Dtos
{
    public static class CustomerExtenstions
    {
        public static CustomerDto AsDto(this Customer customer)
        {
            return new CustomerDto
            {
                CustomerId = customer.CustomerId,
                Name = customer.Name,
                Location = customer.Location,
                Email = customer.Email
            };
        }

        public static Customer AsModel(this CustomerDto customerDto)
        {
            return new Customer
            {
                CustomerId = customerDto.CustomerId,
                Name = customerDto.Name,
                Location = customerDto.Location,
                Email = customerDto.Email
            };
        }
    }
}
