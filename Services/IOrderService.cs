using Dto;

namespace Services
{
    public interface IOrderService
    {
        Task<List<DtoOrder_Id_UserId_Date_Sum_OrderItems?>> GetOrdersUser(int id);
        Task<DtoOrder_Id_UserId_Date_Sum_OrderItems> AddNewOrder(DtoOrder_Id_UserId_Date_Sum_OrderItems order);
        Task<DtoOrder_Id_UserId_Date_Sum_OrderItems?> GetOrderById(int id);
    }
}