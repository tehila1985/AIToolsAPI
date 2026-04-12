using Dto;


namespace Services
{
  public interface ICategoryService
  {
    Task<IEnumerable<DtoCategory_Name_Id>> GetCategories();
    Task<DtoCategory_Name_Id> AddNewCategory(DtocategoryAll newCategory);
    Task<DtoCategory_Name_Id> Delete(int id);
  }
}
