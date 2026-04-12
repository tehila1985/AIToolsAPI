
using Dto;


namespace Services
{
    public interface IPasswordService
    {
        int getStrengthByPassword(string p);
    }
}