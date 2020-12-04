using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HomeSpeaker.Lib;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace HomeSpeaker.Web.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger, Mp3Library mp3Library)
        {
            _logger = logger;
            Mp3Library = mp3Library;
        }

        public Mp3Library Mp3Library { get; }

        public void OnGet()
        {

        }
    }
}
