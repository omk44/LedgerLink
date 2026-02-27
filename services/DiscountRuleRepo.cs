// Path: LedgerLink/Services/DiscountRuleRepo.cs
using LedgerLink.Data;
using LedgerLink.Interface;
using LedgerLink.Models;
using System;
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

        public IEnumerable<DiscountRule> GetAllDiscountRules(Guid shopId)
        {
            return _context.DiscountRules.Where(r => r.ShopId == shopId).ToList();
        }

        public DiscountRule? GetDiscountRuleById(int id, Guid shopId)
        {
            return _context.DiscountRules.FirstOrDefault(r => r.Id == id && r.ShopId == shopId);
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

        public DiscountRule? DeleteDiscountRule(int id, Guid shopId)
        {
            var rule = _context.DiscountRules.FirstOrDefault(r => r.Id == id && r.ShopId == shopId);
            if (rule != null)
            {
                _context.DiscountRules.Remove(rule);
                _context.SaveChanges();
            }
            return rule;
        }
    }
}