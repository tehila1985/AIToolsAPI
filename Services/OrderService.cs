using AutoMapper;
using Dto;
using Repository;
using Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class OrderService : IOrderService
    {
        IOrderRepository _r;
        IMapper _mapper;

        public OrderService(IOrderRepository i, IMapper mapperr)
        {
            _mapper = mapperr;
            _r = i;
        }
        public async Task<List<DtoOrder_Id_UserId_Date_Sum_OrderItems?>> GetOrdersUser(int id)
        {

           var o = await _r.GetOrdersUser(id);
           var r = _mapper.Map<List<Order>, List<DtoOrder_Id_UserId_Date_Sum_OrderItems>>(o);
           return r;
        }
        public async Task<DtoOrder_Id_UserId_Date_Sum_OrderItems?> GetOrderById(int id)
        {
            Order o = await _r.GetOrderById(id);
            return _mapper.Map<Order, DtoOrder_Id_UserId_Date_Sum_OrderItems>(o);

        }

        public async Task<DtoOrder_Id_UserId_Date_Sum_OrderItems> AddNewOrder(DtoOrder_Id_UserId_Date_Sum_OrderItems order)
        {
            var ooo = _mapper.Map<DtoOrder_Id_UserId_Date_Sum_OrderItems, Order>(order);
            Order o = await _r.AddNewOrder(ooo);
            var oo = _mapper.Map<Order, DtoOrder_Id_UserId_Date_Sum_OrderItems>(o);
            return oo;
        }
    }
}