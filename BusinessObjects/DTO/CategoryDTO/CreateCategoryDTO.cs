namespace BusinessObjects.DTO.CategoryDTO
{
    public class CreateCategoryDTO
    {
        public string Name { get; set; }
        public string? ImageUrl { get; set; }
    }

    public class UpdateCategoryDTO : CreateCategoryDTO
    {
    }
}
