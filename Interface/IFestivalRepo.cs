// Path: LedgerLink/Interface/IFestivalRepo.cs
using LedgerLink.Models;
using System;
using System.Collections.Generic;

namespace LedgerLink.Interface
{
    public interface IFestivalRepo
    {
        IEnumerable<Festival> GetAllFestivals(Guid shopId);
        Festival? GetFestivalById(int id, Guid shopId);
        Festival AddFestival(Festival festival);
        Festival? UpdateFestival(Festival festival);
        Festival? DeleteFestival(int id, Guid shopId);
    }
}