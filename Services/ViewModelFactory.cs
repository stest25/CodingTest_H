using VideoMediaApp.Models;
using VideoMediaApp.Models.ViewModels;

namespace VideoMediaApp.Services
{
    /// <summary>
    /// Service for creating ViewModels from domain data
    /// </summary>
    public interface IViewModelFactory
    {
        IndexViewModel CreateCatalogueViewModel(List<VideoFile> files);
        IndexViewModel CreateUploadViewModel();
    }

    public class ViewModelFactory : IViewModelFactory
    {
        public IndexViewModel CreateCatalogueViewModel(List<VideoFile> files)
        {
            return new IndexViewModel
            {
                ViewType = "catalogue",
                Catalogue = new CatalogueViewModel
                {
                    Files = files,
                    TotalItems = files.Count
                }
            };
        }

        public IndexViewModel CreateUploadViewModel()
        {
            return new IndexViewModel
            {
                ViewType = "upload"
            };
        }
    }
}
