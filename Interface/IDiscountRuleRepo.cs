// Path: LedgerLink/Interface/IDiscountRuleRepo.cs
using LedgerLink.Models;
using System.Collections.Generic;

namespace LedgerLink.Interface
{
    public interface IDiscountRuleRepo
    {
        IEnumerable<DiscountRule> GetAllDiscountRules();
        DiscountRule? GetDiscountRuleById(int id);
        DiscountRule AddDiscountRule(DiscountRule discountRule);
        DiscountRule? UpdateDiscountRule(DiscountRule discountRule);
        DiscountRule? DeleteDiscountRule(int id);
    }
}