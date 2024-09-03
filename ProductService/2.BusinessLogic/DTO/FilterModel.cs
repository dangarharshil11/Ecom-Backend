namespace ProductService._2.BusinessLogic.DTO
{
    public class FilterModel
    {
        public string? SortByColumn { get; set; }
        public string? SortOrder { get; set; }
        public int? Page { get; set; }
        public int? PageSize { get; set; }
        public string? SearchText { get; set; }
        public string? SearchColumn { get; set; }
    }
}
