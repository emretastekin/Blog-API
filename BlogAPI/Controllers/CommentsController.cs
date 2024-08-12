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
    public class CommentsController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public CommentsController(ApplicationContext context)
        {
            _context = context;
        }

        // GET: api/Comments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Comment>>> GetComments()
        {
          if (_context.Comments == null)
          {
              return NotFound();
          }
            return await _context.Comments.ToListAsync();
        }

        // GET: api/Comments/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Comment>> GetComment(int id)
        {
          if (_context.Comments == null)
          {
              return NotFound();
          }
            var comment = await _context.Comments.FindAsync(id);

            if (comment == null)
            {
                return NotFound();
            }

            return comment;
        }

        [Authorize(Roles = "Admin,Author")]
        [HttpGet("ByPost/{postId}")]
        public async Task<ActionResult<IEnumerable<Comment>>> GetCommentsByPost(int postId)
        {
            var comments = await _context.Comments.Where(c => c.PostId == postId).Include(c=>c.CommentAnswers).ToListAsync();

            if(comments==null || comments.Count == 0)
            {
                return NotFound();
            }

            return comments;
        }

        // PUT: api/Comments/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "Admin,Author")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutComment(int id, Comment comment)
        {
            if (id != comment.CommentId)
            {
                return BadRequest();
            }

            _context.Entry(comment).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CommentExists(id))
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



        // POST: api/Comments
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "Admin,Author")]
        [HttpPost]
        public async Task<ActionResult<Comment>> PostComment(Comment comment)
        {
          if (_context.Comments == null)
          {
              return Problem("Entity set 'ApplicationContext.Comments'  is null.");
          }

          var post = await _context.Posts.FindAsync(comment.PostId);

          if (post == null)
          {
              return NotFound(new { Message = "Post not found" });
          }

          var authorId = await _context.Authors.FindAsync(comment.AuthorId);

          if (authorId == null)
          {
                return NotFound(new { Message = "Author not found" });
          }


          _context.Comments.Add(comment);
          await _context.SaveChangesAsync();

          return CreatedAtAction("GetComment", new { id = comment.CommentId }, comment);
        
        }

        [Authorize(Roles = "Admin,Author")]
        [HttpPost("Like/{commentId}")]
        public async Task<IActionResult> LikeComment(int commentId, string authorId)
        {
            var comment = await _context.Comments.FindAsync(commentId);

            if (comment == null)
            {
                return NotFound(new { Message = "Comment not found" });
            }

            var author = await _context.Authors.FindAsync(authorId);

            if (author == null)
            {
                return NotFound(new { Message = "Author not found" });
            }

            // Beğeni kontrolü
            var existingLikeDislike = await _context.LikeDislikes
                .FirstOrDefaultAsync(ld => ld.CommentId == commentId && ld.AuthorId == authorId);

            if (existingLikeDislike != null)
            {
                if (existingLikeDislike.IsLiked)
                {
                    return BadRequest(new { Message = "You have already liked this comment." });
                }

                // Mevcut dislike'i kaldır ve beğeni ekle
                existingLikeDislike.IsLiked = true;
                comment.DislikeCount--;
            }
            else
            {
                // Yeni beğeni ekle
                _context.LikeDislikes.Add(new LikeDislike
                {
                    CommentId = commentId,
                    PostId=comment.PostId,
                    AuthorId = authorId,
                    IsLiked = true
                });
            }

            comment.LikeCount++;
            _context.Comments.Update(comment);
            await _context.SaveChangesAsync();

            return Ok(new { Message = " Comment liked successfully", LikeCount = comment.LikeCount });
        }

        [Authorize(Roles = "Admin,Author")]
        [HttpPost("Dislike/{commentId}")]
        public async Task<IActionResult> DislikePost(int commentId, string authorId)
        {
            var comment = await _context.Comments.FindAsync(commentId);

            if (comment == null)
            {
                return NotFound(new { Message = "Comment not found" });
            }

            var author = await _context.Authors.FindAsync(authorId);

            if (author == null)
            {
                return NotFound(new { Message = "Author not found" });
            }

            // Dislike kontrolü
            var existingLikeDislike = await _context.LikeDislikes
                .FirstOrDefaultAsync(ld => ld.CommentId == commentId && ld.AuthorId == authorId);

            if (existingLikeDislike != null)
            {
                if (!existingLikeDislike.IsLiked)
                {
                    return BadRequest(new { Message = "You have already disliked this comment." });
                }

                // Mevcut beğeniyi kaldır ve dislike ekle
                existingLikeDislike.IsLiked = false;
                comment.LikeCount--;
            }
            else
            {
                // Yeni dislike ekle
                _context.LikeDislikes.Add(new LikeDislike
                {
                    CommentId = commentId,
                    PostId = comment.PostId,
                    AuthorId = authorId,
                    IsLiked = false
                }); ;
            }

            comment.DislikeCount++;
            _context.Comments.Update(comment);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Comment disliked successfully", DislikeCount = comment.DislikeCount });
        }

        [Authorize(Roles = "Admin,Author")]
        [HttpPost("AnswerToComment/{parentCommentId}")]
        public async Task<ActionResult<Comment>> AnswerToComment(int parentCommentId, Comment comment)
        {
            if (_context.Comments == null)
            {
                return Problem("Entity set 'ApplicationContext.Comments' is null.");
            }

            var parentComment = await _context.Comments.FindAsync(parentCommentId);

            if (parentComment == null)
            {
                return NotFound(new { Message = "Parent comment not found" });
            }

            var post = await _context.Posts.FindAsync(comment.PostId);

            if (post == null)
            {
                return NotFound(new { Message = "Post not found" });
            }

            var author = await _context.Authors.FindAsync(comment.AuthorId);

            if (author == null)
            {
                return NotFound(new { Message = "Author not found" });
            }

            comment.CommentedDate = DateTime.Now;
            comment.PostId = parentComment.PostId; // Parent comment'in PostId'sini kullanıyoruz
            comment.CommentOfCommentId = parentComment.CommentId;

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            // Parent comment'e yanıt olarak eklediğimiz yorumu ekliyoruz
            if (parentComment.CommentAnswers == null)
            {
                parentComment.CommentAnswers = new List<Comment>();
            }

            parentComment.CommentAnswers.Add(comment);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetComment", new { id = comment.CommentId }, comment);
        }

        // DELETE: api/Comments/5
        [Authorize(Roles = "Admin,Author")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComment(int id)
        {
            if (_context.Comments == null)
            {
                return NotFound();
            }
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
            {
                return NotFound();
            }

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CommentExists(int id)
        {
            return (_context.Comments?.Any(e => e.CommentId == id)).GetValueOrDefault();
        }
    }
}
