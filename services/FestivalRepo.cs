// Path: LedgerLink/Services/FestivalRepo.cs
using LedgerLink.Data;
using LedgerLink.Interface;
using LedgerLink.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LedgerLink.Services
{
    public class FestivalRepo : IFestivalRepo
    {
        private readonly AppDbContext _context;

        public FestivalRepo(AppDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Festival> GetAllFestivals(Guid shopId)
        {
            return _context.Festivals.Where(f => f.ShopId == shopId).ToList();
        }

        public Festival? GetFestivalById(int id, Guid shopId)
        {
            return _context.Festivals.FirstOrDefault(f => f.Id == id && f.ShopId == shopId);
        }

        public Festival AddFestival(Festival festival)
        {
            _context.Festivals.Add(festival);
            _context.SaveChanges();
            return festival;
        }

        public Festival? UpdateFestival(Festival festival)
        {
            var existingFestival = _context.Festivals.Find(festival.Id);
            if (existingFestival != null)
            {
                _context.Entry(existingFestival).CurrentValues.SetValues(festival);
                _context.SaveChanges();
            }
            return existingFestival;
        }

        public Festival? DeleteFestival(int id, Guid shopId)
        {
            var festival = _context.Festivals.FirstOrDefault(f => f.Id == id && f.ShopId == shopId);
            if (festival != null)
            {
                _context.Festivals.Remove(festival);
                _context.SaveChanges();
            }
            return festival;
        }
    }
}