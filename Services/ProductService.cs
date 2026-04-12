using AutoMapper;
using Dto;
using Microsoft.AspNetCore.Mvc;
using Repository;
using Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
  public class ProductService : IProductService
  {
    IProductRepository _r;
    IMapper _mapper;
        public ProductService(IProductRepository i, IMapper mapper)
            {
                  _r = i;
                 _mapper= mapper;
             }

        public async Task<Dto_result_product> GetProducts(
     [FromQuery] int position,
     [FromQuery] int skip,
     [FromQuery] string? desc,
     [FromQuery] int? minPrice,
     [FromQuery] int? maxPrice,
     [FromQuery] int?[] categoryIds,
     [FromQuery] int?[] styleIds)
        {
            var u = await _r.getProducts(position, skip, desc, minPrice, maxPrice, categoryIds, styleIds);
            var r = _mapper.Map<List<Product>, List<DtoProduct_Id_Name_Category_Price_Desc_Image>>(u.Items);
            var n = new Dto_result_product(
                r,
                u.TotalCount
            );
            return n;
        }
        public async Task<DtoProduct_Id_Name_Category_Price_Desc_Image> AddNewProduct(DtoProduct_Name_Description_Price_Stock_CategoryId_IsActive_StyleIds productDto)
        {
            var productEntity = _mapper.Map<Product>(productDto);
            productEntity.ProductStyles = productDto.ProductStyles.Select(id => new ProductStyle
            {
                StyleId = id.StyleId
            }).ToList();

            var savedProduct = await _r.AddNewProduct(productEntity);
            return _mapper.Map<DtoProduct_Id_Name_Category_Price_Desc_Image>(savedProduct);
        }
        public async Task<DtoProduct_Id_Name_Category_Price_Desc_Image> GetById(
        int id)
        {
            Product p = await _r.GetById(id);
            return _mapper.Map<Product, DtoProduct_Id_Name_Category_Price_Desc_Image>(p);

        }
        public async Task<DtoProduct_Id_Name_Category_Price_Desc_Image>  Delete(int id)
        {
            var savedProduct = await _r.Delete(id);
            return _mapper.Map<DtoProduct_Id_Name_Category_Price_Desc_Image>(savedProduct);

        }
    }
}
