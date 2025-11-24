using Microsoft.AspNetCore.Mvc;
using VideoMediaApp.Models.ViewModels;
using VideoMediaApp.Services;

namespace VideoMediaApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IMediaFileService _mediaFileService;
        private readonly IVideoFileMapper _videoFileMapper;
        private readonly IViewModelFactory _viewModelFactory;

        public HomeController(
            IMediaFileService mediaFileService,
            IVideoFileMapper videoFileMapper,
            IViewModelFactory viewModelFactory)
        {
            _mediaFileService = mediaFileService;
            _videoFileMapper = videoFileMapper;
            _viewModelFactory = viewModelFactory;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var viewModel = await CreateCatalogueViewModelAsync();
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Index(string actionType)
        {
            if (actionType == "Catalogue")
            {
                return await ShowCatalogue();
            }

            if (actionType == "Upload")
            {
                return ShowUpload();
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task<IActionResult> ShowCatalogue()
        {
            var viewModel = await CreateCatalogueViewModelAsync();
            return View("Index", viewModel);
        }

        private IActionResult ShowUpload()
        {
            var viewModel = _viewModelFactory.CreateUploadViewModel();
            return View("Index", viewModel);
        }

        /// <summary>
        /// Creates the catalogue ViewModel 
        /// </summary>
        private async Task<IndexViewModel> CreateCatalogueViewModelAsync()
        {
            var allFilesData = await _mediaFileService.GetAllMediaFilesAsync();
            var allFiles = _videoFileMapper.MapToVideoFiles(allFilesData);
            return _viewModelFactory.CreateCatalogueViewModel(allFiles);
        }
    }
}