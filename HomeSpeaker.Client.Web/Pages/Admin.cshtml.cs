using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static HomeSpeaker.Server.gRPC.HomeSpeaker;

namespace HomeSpeaker.Client.Web.Pages
{
    public class AdminModel : PageModel
    {
        private readonly HomeSpeakerClient client;

        public AdminModel(HomeSpeakerClient client)
        {
            this.client = client;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostClearLibraryAsync()
        {
            await client.ResetLibraryAsync(new Empty());
            return RedirectToPage();
        }
    }
}
