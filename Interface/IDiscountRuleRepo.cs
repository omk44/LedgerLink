// Path: LedgerLink/Interface/IDiscountRuleRepo.cs
using LedgerLink.Models;
using System;
using System.Collections.Generic;

namespace LedgerLink.Interface
{
    public interface IDiscountRuleRepo
    {
        IEnumerable<DiscountRule> GetAllDiscountRules(Guid shopId);
        DiscountRule? GetDiscountRuleById(int id, Guid shopId);
        DiscountRule AddDiscountRule(DiscountRule discountRule);
        DiscountRule? UpdateDiscountRule(DiscountRule discountRule);
        DiscountRule? DeleteDiscountRule(int id, Guid shopId);
    }
}