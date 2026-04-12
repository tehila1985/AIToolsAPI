using AutoMapper;
using Dto;
using Repository.Models;
namespace Api
{
    public class AutoMapper : Profile
    {
        public AutoMapper()
        {
            CreateMap<Product, DtoProduct_Id_Name_Category_Price_Desc_Image>();
            CreateMap<User, DtoUser_Gmail_Password>().ReverseMap();
            CreateMap<Style,DtoSyle_id_name>().ReverseMap();
            //CreateMap<PassWord, DtoPassword_Password_Strength>().ReverseMap();
            CreateMap<User, DtoUser_All>().ReverseMap();
            CreateMap<User, DtoUser_Name_Gmail_Role_Id>().ReverseMap();
            CreateMap<Category, DtoCategory_Name_Id>().ReverseMap();
            CreateMap<Category, DtocategoryAll>().ReverseMap();

            CreateMap<Style, DtoStyleAll>().ReverseMap();
            CreateMap<DtoProduct_Name_Description_Price_Stock_CategoryId_IsActive_StyleIds, Product>();
            //CreateMap<Product, DtoProduct_Id_Name_Category_Price_Desc_Image>().ForMember(dest => dest.Name,
            //                                              opts => opts.MapFrom(src => src.Category.Name));
            CreateMap<Product, DtoProduct_Id_Name_Category_Price_Desc_Image>()
                           .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name));
            CreateMap<Order, DtoOrder_Id_UserId_Date_Sum_OrderItems>()
                .ForMember(dest => dest.OrderItems, opt => opt.MapFrom(src => src.OrderItems)).ReverseMap();
            CreateMap<OrderItem, DtoOrderItem_Id_OrderId_ProductId_Quantity>()
                 .ForMember(dest => dest.ItemName, opt => opt.MapFrom(src => src.Product.Name));
            CreateMap<DtoOrderItem_Id_OrderId_ProductId_Quantity, OrderItem>()
                 .ForMember(dest => dest.Product, opt => opt.Ignore())
                 .ForMember(dest => dest.Order, opt => opt.Ignore());
        }
    }
}