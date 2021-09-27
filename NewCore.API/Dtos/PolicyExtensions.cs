using System;
using NewCore.API.Dtos;
using NewCore.Data.Models;

namespace NewCore.API.Dtos
{
    public static class PolicyExtenstions
    {
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

        public static Policy AsModel(this PolicyDto policyDto)
        {
            return new Policy
            {
                Id = policyDto.Id,
                PolId = policyDto.PolId,
                PolCommDt = policyDto.PolCommDt,
                PolExpryDate = policyDto.PolExpryDate,
                CustomerId = policyDto.CustomerId,
                PolStatus = policyDto.PolStatus,
                PolFaceAmt = policyDto.PolFaceAmt,
                PolPremAmt = policyDto.PolPremAmt
            };
        }
    }
}
