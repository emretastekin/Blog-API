using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlogAPI.Data;
using BlogAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace BlogAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorsController : ControllerBase
    {
        private readonly ApplicationContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AuthorsController(ApplicationContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // GET: api/Authors
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Author>>> GetAuthors()
        {
          if (_context.Authors == null)
          {
              return NotFound();
          }
            return await _context.Authors.ToListAsync();
        }

        // GET: api/Authors/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Author>> GetAuthor(string id)
        {
          if (_context.Authors == null)
          {
              return NotFound();
          }
            var author = await _context.Authors.FindAsync(id);

            if (author == null)
            {
                return NotFound();
            }

            return author;
        }

        // PUT: api/Authors/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles ="Admin,Author")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAuthor(string id, Author author)
        {
            if (id != author.Id)
            {
                return BadRequest();
            }

            _context.Entry(author).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AuthorExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Authors
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "Admin,Author")]
        [HttpPost]
        public async Task<ActionResult<Author>> PostAuthor(Author author)
        {
            if (_context.Authors == null)
            {
                return Problem("Entity set 'ApplicationContext.Authors'  is null.");
            }
            _userManager.CreateAsync(author.ApplicationUser!, author.ApplicationUser!.Password).Wait();
            _userManager.AddToRoleAsync(author.ApplicationUser!, "Author").Wait();



            author.Id = author.ApplicationUser!.Id;
            author.ApplicationUser = null;
            _context.Authors.Add(author);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (AuthorExists(author.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetAuthor", new { id = author.Id }, author);
        }

        [Authorize(Roles = "Admin,Author")]
        [HttpPost("upload-cover-image/{authorId}")]
        public async Task<IActionResult> UploadCoverImage(string authorId, IFormFile coverImage)
        {
            if (coverImage == null || coverImage.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            // Kişiyi bul
            var author = await _context.Authors.FindAsync(authorId);
            if (author == null)
            {
                return NotFound("Author not found.");
            }

            // Dosya yolunu belirleyin (örneğin: wwwroot/images/{fileName})
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");

            // Klasörün var olup olmadığını kontrol edin ve gerekiyorsa oluşturun
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var fileName = coverImage.FileName;
            var filePath = Path.Combine(uploadsFolder, fileName);

            // Dosyayı kaydedin
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await coverImage.CopyToAsync(stream);
            }

            // Kişinin CoverImageUrl özelliğini güncelleyin
            author.CoverImageUrl = $"/images/{fileName}";

            // Kişi nesnesini güncelleyin
            _context.Entry(author).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            // Dosyayı okuyup yanıt olarak döndürün
            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
            return File(fileBytes, "image/jpeg");
        }

        [Authorize(Roles = "Admin,Author")]
        [HttpDelete("remove-cover-image/{authorId}")]
        public async Task<IActionResult> RemoveCoverImage(string authorId)
        {
            var author = await _context.Authors.FindAsync(authorId);
            if (author == null)
            {
                return NotFound("Author not found.");
            }

            // Eski kapak resminin dosya yolunu belirleyin
            var oldFileName = Path.GetFileName(author.CoverImageUrl);
            var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", oldFileName);

            // Dosya varsa, dosyayı kaldırın
            if (System.IO.File.Exists(oldFilePath))
            {
                System.IO.File.Delete(oldFilePath);
            }

            // Kapak resmini kaldırın
            author.CoverImageUrl = null;

            // Üye nesnesini güncelleyin
            _context.Entry(author).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("Deactivate/{authorId}")]
        public async Task<IActionResult> DeactivateAuthor(string authorId)
        {
            var author = await _context.Authors
                .Include(p => p.ApplicationUser) // ApplicationUser ile ilişkiyi dahil ediyoruz
                .FirstOrDefaultAsync(p => p.Id == authorId);

            if (author == null)
            {
                return NotFound();
            }

            // ApplicationUser nesnesinin IsActive özelliğini güncelleyin
            if (author.ApplicationUser != null)
            {
                author.ApplicationUser.IsActive = false;
                _context.Entry(author.ApplicationUser).State = EntityState.Modified;
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("Activate/{authorId}")]
        public async Task<IActionResult> ActivateAuthor(string authorId)
        {
            var author = await _context.Authors
                .Include(p => p.ApplicationUser) // ApplicationUser ile ilişkiyi dahil ediyoruz
                .FirstOrDefaultAsync(p => p.Id == authorId);

            if (author == null)
            {
                return NotFound();
            }

            // ApplicationUser nesnesinin IsActive özelliğini güncelleyin
            if (author.ApplicationUser != null)
            {
                author.ApplicationUser.IsActive = true;
                _context.Entry(author.ApplicationUser).State = EntityState.Modified;
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }

        /*
        // DELETE: api/Authors/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAuthor(string id)
        {
            if (_context.Authors == null)
            {
                return NotFound();
            }
            var author = await _context.Authors.FindAsync(id);
            if (author == null)
            {
                return NotFound();
            }

            _context.Authors.Remove(author);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        */

        private bool AuthorExists(string id)
        {
            return (_context.Authors?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
