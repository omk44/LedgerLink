// Path: LedgerLink/Interface/IFestivalRepo.cs
using LedgerLink.Models;
using System.Collections.Generic;

namespace LedgerLink.Interface
{
    public interface IFestivalRepo
    {
        IEnumerable<Festival> GetAllFestivals();
        Festival? GetFestivalById(int id);
        Festival AddFestival(Festival festival);
        Festival? UpdateFestival(Festival festival);
        Festival? DeleteFestival(int id);
    }
}