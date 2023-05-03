using System.Net;
using System.Security.Claims;
using System.Xml.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NailsBookingApp_API.Models;
using NailsBookingApp_API.Models.DTO.POSTDTO;
using NailsBookingApp_API.Models.POSTS;
using NailsBookingApp_API.Models.ViewModels;

namespace NailsBookingApp_API.Controllers
{
    [Route("api/post")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private ApiResponse _apiResponse;

        public PostController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
            _apiResponse = new ApiResponse();
        }

        [HttpPost("CreatePost")]
        public async Task<ActionResult<ApiResponse>> CreatePost([FromBody] PostDTO postDto)
        {
            var userFromDb = await _dbContext.ApplicationUsers.FirstOrDefaultAsync(u => u.Id == postDto.ApplicationUserId);

            if (userFromDb == null || string.IsNullOrEmpty(postDto.Content))
            {
                _apiResponse.HttpStatusCode = HttpStatusCode.BadRequest;
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages.Add("userid is wrong or content is empty");
                return BadRequest(_apiResponse);
            }

            var post = _dbContext.Posts.Add(new Post()
            {
                ApplicationUserId = postDto.ApplicationUserId,
                Content = postDto.Content,
                CreateDateTime = DateTime.Now,
            });

            await _dbContext.SaveChangesAsync();

            _apiResponse.HttpStatusCode = HttpStatusCode.OK;
            _apiResponse.IsSuccess = true;
            return Ok(_apiResponse);

        }

        [HttpPost("AddComment")]
        public async Task<ActionResult<ApiResponse>> AddComment([FromBody] CommentDTO commentDTO)
        {
            var userFromDb = await _dbContext.ApplicationUsers.FirstOrDefaultAsync(u => u.Id == commentDTO.ApplicationUserId);

            if (userFromDb == null || string.IsNullOrEmpty(commentDTO.commentContent) || commentDTO.PostId == null ||
                commentDTO.PostId <= 0)
            {
                _apiResponse.HttpStatusCode = HttpStatusCode.BadRequest;
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages.Add("userid is wrong or content is empty");
                return BadRequest(_apiResponse);
            }

            var comment = _dbContext.Comments.Add(new Comment()
            {
                ApplicationUserId = commentDTO.ApplicationUserId,
                CommentContent = commentDTO.commentContent,
                CreateDateTime = DateTime.Now,
                PostId = commentDTO.PostId,
            });

            await _dbContext.SaveChangesAsync();

            _apiResponse.HttpStatusCode = HttpStatusCode.OK;
            _apiResponse.IsSuccess = true;
            return Ok(_apiResponse);
        }

        [HttpGet("GetPosts")]
        public async Task<ActionResult<ApiResponse>> GetPosts()
        {

            // MAYBE WE WILL NEED TO REMOVE INCLUDES JUST FOR OPTIMALIZATION 
            var posts = await _dbContext
                .Posts
                .Include(p => p.Likes)
                .Include(c => c.Comments)
                .Include(x => x.ApplicationUser).Select(x => new PostViewModel()
                {
                    Id = x.Id,
                    Content = x.Content,
                    Likes = x.Likes,
                    Comments = x.Comments,
                    CreateDateTime = x.CreateDateTime,
                    ApplicationUserId = x.ApplicationUserId,
                    ApplicationUserName = x.ApplicationUser.Name,
                    ApplicationUserLastName = x.ApplicationUser.LastName,
                })
                .ToListAsync();

            if (posts == null || !posts.Any())
            {
                _apiResponse.HttpStatusCode = HttpStatusCode.BadRequest;
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages.Add("There are no posts");
                return BadRequest();
            }

            _apiResponse.HttpStatusCode = HttpStatusCode.OK;
            _apiResponse.IsSuccess = true;
            _apiResponse.Result = posts;
            return Ok(_apiResponse);

      
        }

        [HttpGet("GetPostById")]
        public async Task<ActionResult<ApiResponse>> GetPost(int id)
        {
            var post = await _dbContext
                .Posts
                .Include(p => p.Likes)
                .Include(p => p.Comments)
                .ThenInclude(c => c.Likes)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (post == null || id <= 0 )
            {
                _apiResponse.HttpStatusCode = HttpStatusCode.BadRequest;
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages.Add("There is no post");
                return BadRequest();
            }

            _apiResponse.HttpStatusCode = HttpStatusCode.OK;
            _apiResponse.IsSuccess = true;
            _apiResponse.Result = post;
            return Ok(_apiResponse);
        }


        [HttpPost("HandleLike")]
        public async Task<ActionResult<ApiResponse>> HandleLike([FromBody] LikeDTO likeDto)
        {
            var userFromDb = await _dbContext.ApplicationUsers.FirstOrDefaultAsync(u => u.Id == likeDto.ApplicationUserId);

            if (userFromDb == null)
            {
                _apiResponse.HttpStatusCode = HttpStatusCode.BadRequest;
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages.Add("userid is wrong");
                return BadRequest(_apiResponse);
            }

            // LIKING JUST A COMMENT
            if ((likeDto.PostId == null || likeDto.PostId == 0) && likeDto.CommentId > 0)
            {
                var like = new Like()
                {
                    ApplicationUserId = likeDto.ApplicationUserId,
                    CommentId = likeDto.CommentId,
                };

                var commentFromDb = await _dbContext
                    .Comments
                    .Include(p => p.Likes)
                    .FirstOrDefaultAsync(c => c.Id == likeDto.CommentId);

                var hasLiked = commentFromDb.Likes.FirstOrDefault(l => l.ApplicationUserId == likeDto.ApplicationUserId);

                if (hasLiked == null)
                {
                    _dbContext.Likes.Add(like);
                }
                else
                {
                    _dbContext.Likes.Remove(hasLiked);
                }
                await _dbContext.SaveChangesAsync();

                _apiResponse.HttpStatusCode = HttpStatusCode.OK;
                _apiResponse.IsSuccess = true;
                return Ok(_apiResponse);

            }

            // LIKING JUST A POST
            if (likeDto.PostId > 0 && (likeDto.CommentId == null || likeDto.CommentId == 0))
            {
                var like = new Like()
                {
                    ApplicationUserId = likeDto.ApplicationUserId,
                    PostId  = likeDto.PostId
                };

                var postFromDb = await _dbContext
                    .Posts
                    .Include(p => p.Likes)
                    .FirstOrDefaultAsync(c => c.Id == likeDto.PostId);

                var hasLiked = postFromDb.Likes.FirstOrDefault(l => l.ApplicationUserId == likeDto.ApplicationUserId);

                if (hasLiked == null)
                {
                    _dbContext.Likes.Add(like);
                }
                else
                {
                    _dbContext.Likes.Remove(hasLiked);
                }

                await _dbContext.SaveChangesAsync();

                _apiResponse.HttpStatusCode = HttpStatusCode.OK;
                _apiResponse.IsSuccess = true;
                return Ok(_apiResponse);
            }


            _apiResponse.HttpStatusCode = HttpStatusCode.BadRequest;
            _apiResponse.IsSuccess = false;
            _apiResponse.ErrorMessages.Add("Like handle failed execution");
            return BadRequest(_apiResponse);
        }

        [HttpGet("GetCommentsById/{id:int}")]
        public async Task<ActionResult<ApiResponse>> GetCommentsById(int id)
        {
            var comments = _dbContext
                .Comments
                .Include(c => c.Likes)
                .Where(c => c.PostId == id)
                .Select(x => new CommentViewModel()
                {
                    Id = x.Id,
                    ApplicationUserName = x.ApplicationUser.Name,
                    ApplicationUserLastName = x.ApplicationUser.LastName,
                    ApplicationUserId = x.ApplicationUserId,
                    CreateDateTime = x.CreateDateTime,
                    PostId = x.PostId,
                    Likes = x.Likes,
                    CommentContent = x.CommentContent
                });

            if (comments != null)
            {
                _apiResponse.HttpStatusCode = HttpStatusCode.OK;
                _apiResponse.IsSuccess = true;
                _apiResponse.Result = comments;
                return Ok(_apiResponse);
            }

            _apiResponse.HttpStatusCode = HttpStatusCode.NotFound;
            _apiResponse.IsSuccess = false;
            return NotFound(_apiResponse);
        }

        [HttpDelete("DeletePost")]
        public async Task<ActionResult<ApiResponse>> DeletePost([FromBody] DeletePostDTO deletePostDto)
        {
            // get the value from bearer 
            //var userId = User.FindFirst("Id")?.Value;

            var post = await _dbContext
                .Posts
                .Include(x => x.Comments).ThenInclude(x => x.Likes)
                .FirstOrDefaultAsync(p => p.Id == deletePostDto.PostId);
            

            if (post != null && post.ApplicationUserId == deletePostDto.ApplicationUserId )
            {
                var comments = await _dbContext.Comments.FirstOrDefaultAsync(x => x.PostId == deletePostDto.PostId);

                if (comments != null && comments.Likes != null && comments.Likes.Count > 0)
                {
                    _dbContext.Likes.RemoveRange(comments.Likes);
                }

                _dbContext.Posts.Remove(post);
                await _dbContext.SaveChangesAsync();

                _apiResponse.HttpStatusCode = HttpStatusCode.OK;
                _apiResponse.IsSuccess = true;
                return Ok(_apiResponse);
            }
            _apiResponse.HttpStatusCode = HttpStatusCode.NotFound;
            _apiResponse.IsSuccess = false;
            return NotFound(_apiResponse);
        }

        [HttpDelete("DeleteComment")]
        public async Task<ActionResult<ApiResponse>> DeleteComment([FromBody] DeleteCommentDTO deleteCommentDto)
        {
            //var userId = User.FindFirst("Id")?.Value;

            var comment = await _dbContext
                .Comments
                .Include(x => x.Likes)
                .FirstOrDefaultAsync(c => c.Id == deleteCommentDto.CommentId);


            if (comment != null && comment.ApplicationUserId == deleteCommentDto.ApplicationUserId)
            {
                //var commentLikes = await _dbContext.Comments.FirstOrDefaultAsync(x => x.PostId == deletePostDto.PostId);
                //_dbContext.Likes.RemoveRange(commentLikes.Likes);

                _dbContext.Comments.Remove(comment);
                await _dbContext.SaveChangesAsync();

                _apiResponse.HttpStatusCode = HttpStatusCode.OK;
                _apiResponse.IsSuccess = true;
                return Ok(_apiResponse);
            }
            _apiResponse.HttpStatusCode = HttpStatusCode.NotFound;
            _apiResponse.IsSuccess = false;
            return NotFound(_apiResponse);
        }

        [HttpPut("updatePost")]
        public async Task<ActionResult<ApiResponse>> UpdatePost([FromBody] UpdatePostDTO updatePostDto)
        {
            var post = await _dbContext.Posts.FindAsync(updatePostDto.PostId);

            if (post == null || post.ApplicationUserId != updatePostDto.ApplicationUserId)
            {
                _apiResponse.HttpStatusCode = HttpStatusCode.BadRequest;
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages.Add("Post not found or you are trying to update not your own post.");
                return BadRequest();
            }

            if (string.IsNullOrEmpty(updatePostDto.Content))
            {
                _apiResponse.HttpStatusCode = HttpStatusCode.BadRequest;
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages.Add("Content cannot be empty");
                return BadRequest();
            }

            post.Content = updatePostDto.Content;
            _dbContext.Update(post);
            await _dbContext.SaveChangesAsync();

            _apiResponse.IsSuccess = true;
            _apiResponse.HttpStatusCode = HttpStatusCode.OK;
            return Ok(_apiResponse);
        }

        [HttpPut("updateComment")]
        public async Task<ActionResult<ApiResponse>> UpdateComment([FromBody] UpdateCommentDTO updateCommentDto)
        {
            var comment = await _dbContext.Comments.FindAsync(updateCommentDto.CommentId);

            if (comment == null || comment.ApplicationUserId != updateCommentDto.ApplicationUserId)
            {
                _apiResponse.HttpStatusCode = HttpStatusCode.BadRequest;
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages.Add("Comment not found or you are trying to update not your own comment.");
                return BadRequest();
            }

            if (string.IsNullOrEmpty(updateCommentDto.CommentContent))
            {
                _apiResponse.HttpStatusCode = HttpStatusCode.BadRequest;
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages.Add("Content cannot be empty");
                return BadRequest();
            }

            comment.CommentContent = updateCommentDto.CommentContent;
            _dbContext.Comments.Update(comment);
            await _dbContext.SaveChangesAsync();

            _apiResponse.IsSuccess = true;
            _apiResponse.HttpStatusCode = HttpStatusCode.OK;
            return Ok(_apiResponse);
        }

    }
   
}
