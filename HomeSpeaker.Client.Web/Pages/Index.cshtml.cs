using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HomeSpeaker.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace HomeSpeaker.Web.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly HomeSpeakerClient homeSpeakerClient;

        public IndexModel(ILogger<IndexModel> logger,
            HomeSpeakerClient homeSpeakerClient)
        {
            this.homeSpeakerClient = homeSpeakerClient;
        }

        public void OnGet()
        {
            
        }
    }
}
