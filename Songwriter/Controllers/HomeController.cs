using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Redzen.Random;
using Songwriter.Configuration;
using Songwriter.Models;
using Songwriter.Services;
using X.PagedList;

namespace Songwriter.Controllers {
    public class HomeController(
        IMidiGenerationService midiService, 
        ISongGenerationService soundService, 
        IIconGenerationService imageSerice, 
        IOptions<AppSettings> settings) : Controller {

        [HttpGet]
        public IActionResult Index(int? page, ulong? seed, string? lang, double? likes) {
            page ??= settings.Value.Defaults.Page;
            seed ??= settings.Value.Defaults.Seed;
            lang ??= settings.Value.Defaults.Language;
            likes ??= settings.Value.Defaults.AvgLikes;
            SetViewBagParameters(page.Value, seed.Value, lang, likes.Value);
            var songs = soundService.GenerateSongs(seed.Value, lang, likes.Value, page.Value);
            return View(new StaticPagedList<Song>(songs, page.Value, settings.Value.Paging.PageSize, settings.Value.Paging.TotalSongs));
        }

        [HttpGet]
        public IActionResult LoadGallery(int page, ulong seed, string lang, double likes) {
            var songs = soundService.GenerateSongs(seed, lang, likes, page);
            return PartialView("_SongsGalleryItems", new StaticPagedList<Song>(songs, page, settings.Value.Paging.PageSize, settings.Value.Paging.TotalSongs));
        }

        [HttpGet]
        public IActionResult GenerateSeed() {
            var seed = new Xoshiro256StarStarRandom().NextULong();
            return Json(new { seed });
        }

        [HttpGet]
        public ActionResult GenerateAudio(int id, ulong seed) {
            var midiData = midiService.GenerateComposition(id, seed);
            return File(midiData, "audio/midi", $"song_seed_{id}.mid");
        }

        [HttpGet]
        public async Task<IActionResult> CoverIcon(Song song) {
            var icon = await imageSerice.CoverIconAsync(song);
            return Content(icon, "image/svg+xml");
        }

        [HttpGet]
        public IActionResult Privacy() {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private void SetViewBagParameters(int page, ulong seed, string lang, double likes) {
            ViewBag.CurrentPage = page;
            ViewBag.CurrentSeed = seed;
            ViewBag.CurrentLang = lang;
            ViewBag.AvgLikes = likes;
        }
    }
}
