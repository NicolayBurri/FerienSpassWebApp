   
using FerienspassWebApp.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
  
using global::FerienspassWebApp.Data;

namespace FerienspassWebApp.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/invoices")]
    public class InvoicesController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _env;

        public InvoicesController(ApplicationDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        [HttpGet("{id:int}/download")]
        public async Task<IActionResult> DownloadInvoice(int id)
        {
            var invoice = await _db.Invoices
                .FirstOrDefaultAsync(x => x.Id == id);

            if (invoice == null)
                return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var isAdmin = User.IsInRole("Admin");

            // Zugriff prüfen
            if (!isAdmin && invoice.ParentUserId != userId)
                return Forbid();

            var path = Path.Combine(
                _env.ContentRootPath,
                "PrivateFiles",
                "Rechnungen",
                $"rechnung_{id}.pdf"
            );

            if (!System.IO.File.Exists(path))
                return NotFound("PDF nicht gefunden");

            var bytes = await System.IO.File.ReadAllBytesAsync(path);

            return File(bytes, "application/pdf", $"rechnung_{id}.pdf");
        }
    }
}