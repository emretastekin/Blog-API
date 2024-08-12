using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlogAPI.Data;
using BlogAPI.Models;
using Microsoft.AspNetCore.Authorization;

namespace BlogAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public PostsController(ApplicationContext context)
        {
            _context = context;
        }

        // GET: api/Posts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Post>>> GetPosts()
        {
          if (_context.Posts == null)
          {
              return NotFound();
          }
            return await _context.Posts.ToListAsync();
        }

        // GET: api/Posts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Post>> GetPost(int id)
        {
          if (_context.Posts == null)
          {
              return NotFound();
          }
            var post = await _context.Posts.FindAsync(id);

            if (post == null)
            {
                return NotFound();
            }

            return post;
        }

        // PUT: api/Posts/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "Admin,Author")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPost(int id, Post post)
        {
            if (id != post.PostId)
            {
                return BadRequest();
            }

            _context.Entry(post).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PostExists(id))
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

        // POST: api/Posts
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "Admin,Author")]
        [HttpPost]
        public async Task<ActionResult<Post>> PostPost(Post post)
        {
          if (_context.Posts == null)
          {
              return Problem("Entity set 'ApplicationContext.Posts'  is null.");
          }

          var authorId = await _context.Authors.FindAsync(post.AuthorId);

          if (authorId == null)
          {
                return NotFound(new { Message = "Author not found" });
          }

          _context.Posts.Add(post);
          await _context.SaveChangesAsync();

          return CreatedAtAction("GetPost", new { id = post.PostId }, post);
        
        }

        [Authorize(Roles = "Admin,Author")]
        [HttpPost("upload-cover-image/{postId}")]
        public async Task<IActionResult> UploadCoverImage(int postId, IFormFile coverImage)
        {
            if (coverImage == null || coverImage.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            // Kişiyi bul
            var post = await _context.Posts.FindAsync(postId);
            if (post == null)
            {
                return NotFound("Post not found.");
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
            post.CoverImageUrl = $"/images/{fileName}";

            // Kişi nesnesini güncelleyin
            _context.Entry(post).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            // Dosyayı okuyup yanıt olarak döndürün
            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
            return File(fileBytes, "image/jpeg");
        }

        [Authorize(Roles = "Admin,Author")]
        [HttpDelete("remove-cover-image/{postId}")]
        public async Task<IActionResult> RemoveCoverImage(string postId)
        {
            var post = await _context.Posts.FindAsync(postId);
            if (post == null)
            {
                return NotFound("Post not found.");
            }

            // Eski kapak resminin dosya yolunu belirleyin
            var oldFileName = Path.GetFileName(post.CoverImageUrl);
            var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", oldFileName);

            // Dosya varsa, dosyayı kaldırın
            if (System.IO.File.Exists(oldFilePath))
            {
                System.IO.File.Delete(oldFilePath);
            }

            // Kapak resmini kaldırın
            post.CoverImageUrl = null;

            // Üye nesnesini güncelleyin
            _context.Entry(post).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }


        [Authorize(Roles = "Admin,Author")]
        [HttpPost("Like/{postId}")]
        public async Task<IActionResult> LikePost(int postId, string authorId)
        {
            var post = await _context.Posts.FindAsync(postId);

            if (post == null)
            {
                return NotFound(new { Message = "Post not found" });
            }

            var author = await _context.Authors.FindAsync(authorId);

            if (author == null)
            {
                return NotFound(new { Message = "Author not found" });
            }

            // Beğeni kontrolü
            var existingLikeDislike = await _context.LikeDislikes
                .FirstOrDefaultAsync(ld => ld.PostId == postId && ld.AuthorId == authorId);

            if (existingLikeDislike != null)
            {
                if (existingLikeDislike.IsLiked)
                {
                    return BadRequest(new { Message = "You have already liked this post." });
                }

                // Mevcut dislike'i kaldır ve beğeni ekle
                existingLikeDislike.IsLiked = true;
                post.DislikeCount--;
            }
            else
            {
                // Yeni beğeni ekle
                _context.LikeDislikes.Add(new LikeDislike
                {
                    PostId = postId,
                    AuthorId = authorId,
                    IsLiked = true
                });
            }

            post.LikeCount++;
            _context.Posts.Update(post);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Post liked successfully", LikeCount = post.LikeCount });
        }

        [Authorize(Roles = "Admin,Author")]
        [HttpPost("Dislike/{postId}")]
        public async Task<IActionResult> DislikePost(int postId, string authorId)
        {
            var post = await _context.Posts.FindAsync(postId);

            if (post == null)
            {
                return NotFound(new { Message = "Post not found" });
            }

            var author = await _context.Authors.FindAsync(authorId);

            if (author == null)
            {
                return NotFound(new { Message = "Author not found" });
            }

            // Dislike kontrolü
            var existingLikeDislike = await _context.LikeDislikes
                .FirstOrDefaultAsync(ld => ld.PostId == postId && ld.AuthorId == authorId);

            if (existingLikeDislike != null)
            {
                if (!existingLikeDislike.IsLiked)
                {
                    return BadRequest(new { Message = "You have already disliked this post." });
                }

                // Mevcut beğeniyi kaldır ve dislike ekle
                existingLikeDislike.IsLiked = false;
                post.LikeCount--;
            }
            else
            {
                // Yeni dislike ekle
                _context.LikeDislikes.Add(new LikeDislike
                {
                    PostId = postId,
                    AuthorId = authorId,
                    IsLiked = false
                });
            }

            post.DislikeCount++;
            _context.Posts.Update(post);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Post disliked successfully", DislikeCount = post.DislikeCount });
        }

        // DELETE: api/Posts/5
        [Authorize(Roles = "Admin,Author")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePost(int id)
        {
            if (_context.Posts == null)
            {
                return NotFound();
            }
            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PostExists(int id)
        {
            return (_context.Posts?.Any(e => e.PostId == id)).GetValueOrDefault();
        }
    }
}
