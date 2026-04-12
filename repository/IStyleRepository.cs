using Microsoft.EntityFrameworkCore;
using Repository.Models;

namespace Repository
{
    public interface IStyleRepository
    {
        Task<List<Style>> GetStyles();
        Task<Style> AddNewStyle(Style style);
        Task<Style> Delete(int id);
       

    }
}