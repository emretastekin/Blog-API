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
    public class FavoritesController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public FavoritesController(ApplicationContext context)
        {
            _context = context;
        }

        // GET: api/Favorites
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Favorite>>> GetFavorites()
        {
          if (_context.Favorites == null)
          {
              return NotFound();
          }
            return await _context.Favorites.ToListAsync();
        }

        // GET: api/Favorites/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Favorite>> GetFavorite(int id)
        {
          if (_context.Favorites == null)
          {
              return NotFound();
          }
            var favorite = await _context.Favorites.FindAsync(id);

            if (favorite == null)
            {
                return NotFound();
            }

            return favorite;
        }

        // PUT: api/Favorites/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "Admin,Author")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFavorite(int id, Favorite favorite)
        {
            if (id != favorite.FavoriteId)
            {
                return BadRequest();
            }

            _context.Entry(favorite).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FavoriteExists(id))
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

        [Authorize(Roles = "Admin,Author")]
        [HttpPost("AddToFavorites/{postId}")]
        public async Task<IActionResult> AddToFavorites(int postId, string authorId)
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

            // Check if the favorite already exists
            var existingFavorite = await _context.Favorites
                .FirstOrDefaultAsync(f => f.PostId == postId && f.AuthorId == authorId);
            if (existingFavorite != null)
            {
                return BadRequest(new { Message = "Post already added to favorites" });
            }

            var favorite = new Favorite
            {
                PostId = postId,
                AuthorId = authorId
            };

            _context.Favorites.Add(favorite);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Post added to favorites successfully" });
        }

        [Authorize(Roles = "Admin,Author")]
        [HttpDelete("RemoveFromFavorites/{postId}")]
        public async Task<IActionResult> RemoveFromFavorites(int postId, string authorId)
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

            // Check if the favorite exists
            var favorite = await _context.Favorites
                .FirstOrDefaultAsync(f => f.PostId == postId && f.AuthorId == authorId);
            if (favorite == null)
            {
                return NotFound(new { Message = "Favorite not found" });
            }

            _context.Favorites.Remove(favorite);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Post removed from favorites successfully" });
        }

        // DELETE: api/Favorites/5
        [Authorize(Roles = "Admin,Author")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFavorite(int id)
        {
            if (_context.Favorites == null)
            {
                return NotFound();
            }
            var favorite = await _context.Favorites.FindAsync(id);
            if (favorite == null)
            {
                return NotFound();
            }

            _context.Favorites.Remove(favorite);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool FavoriteExists(int id)
        {
            return (_context.Favorites?.Any(e => e.FavoriteId == id)).GetValueOrDefault();
        }
    }
}
