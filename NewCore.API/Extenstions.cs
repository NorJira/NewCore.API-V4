using System;
using NewCore.API.Dtos;
using NewCore.Data.Models;

namespace NewCore.API
{
    public static class Extenstions
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

        public static PolicyDto AsDto(this Policy policy)
        {
            return new PolicyDto
            {
                Id = policy.Id,
                PolId = policy.PolId,
                PolCommDt = policy.PolCommDt,
                PolExpryDate = policy.PolExpryDate,
                CustomerId = policy.CustomerId,
                PolStatus = policy.PolStatus,
                PolFaceAmt = policy.PolFaceAmt,
                PolPremAmt = policy.PolPremAmt
            };
        }
    }
}
