using AutoMapper;
using Dto;
using Repository;
using Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json; 
using AIToolsAPI.Services; 

namespace Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _r;
        private readonly IMapper _mapper;
        private readonly IKafkaProducerService _kafkaProducerService;

        public OrderService(IOrderRepository i, IMapper mapperr, IKafkaProducerService kafkaProducerService)
        {
            _mapper = mapperr;
            _r = i;
            _kafkaProducerService = kafkaProducerService;
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

            try
            {
                string jsonMessage = JsonSerializer.Serialize(oo);
                await _kafkaProducerService.SendNotificationAsync(jsonMessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Kafka Background Error] Failed to send order event: {ex.Message}");
            }

            return oo;
        }
    }
}