// Path: LedgerLink/Services/FestivalRepo.cs
using LedgerLink.Data;
using LedgerLink.Interface;
using LedgerLink.Models;
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

        public IEnumerable<Festival> GetAllFestivals()
        {
            return _context.Festivals.ToList();
        }

        public Festival? GetFestivalById(int id)
        {
            return _context.Festivals.Find(id);
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

        public Festival? DeleteFestival(int id)
        {
            var festival = _context.Festivals.Find(id);
            if (festival != null)
            {
                _context.Festivals.Remove(festival);
                _context.SaveChanges();
            }
            return festival;
        }
    }
}