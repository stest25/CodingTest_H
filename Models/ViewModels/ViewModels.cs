namespace VideoMediaApp.Models.ViewModels
{
    /// <summary>
    /// ViewModel for the main Index view
    /// </summary>
    public class IndexViewModel
    {
        public string ViewType { get; set; } = "catalogue";
        public CatalogueViewModel? Catalogue { get; set; }
    }

    /// <summary>
    /// ViewModel for catalogue view
    /// </summary>
    public class CatalogueViewModel
    {
        public List<VideoFile> Files { get; set; } = new();
        public int TotalItems { get; set; }
        public bool HasFiles => Files != null && Files.Any();
    }
}
