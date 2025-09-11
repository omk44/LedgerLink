// Path: LedgerLink/Services/DiscountRuleRepo.cs
using LedgerLink.Data;
using LedgerLink.Interface;
using LedgerLink.Models;
using System.Collections.Generic;
using System.Linq;

namespace LedgerLink.Services
{
    public class DiscountRuleRepo : IDiscountRuleRepo
    {
        private readonly AppDbContext _context;

        public DiscountRuleRepo(AppDbContext context)
        {
            _context = context;
        }

        public IEnumerable<DiscountRule> GetAllDiscountRules()
        {
            return _context.DiscountRules.ToList();
        }

        public DiscountRule? GetDiscountRuleById(int id)
        {
            return _context.DiscountRules.Find(id);
        }

        public DiscountRule AddDiscountRule(DiscountRule discountRule)
        {
            _context.DiscountRules.Add(discountRule);
            _context.SaveChanges();
            return discountRule;
        }

        public DiscountRule? UpdateDiscountRule(DiscountRule discountRule)
        {
            var existingRule = _context.DiscountRules.Find(discountRule.Id);
            if (existingRule != null)
            {
                _context.Entry(existingRule).CurrentValues.SetValues(discountRule);
                _context.SaveChanges();
            }
            return existingRule;
        }

        public DiscountRule? DeleteDiscountRule(int id)
        {
            var rule = _context.DiscountRules.Find(id);
            if (rule != null)
            {
                _context.DiscountRules.Remove(rule);
                _context.SaveChanges();
            }
            return rule;
        }
    }
}