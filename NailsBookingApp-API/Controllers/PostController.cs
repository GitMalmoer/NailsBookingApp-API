using Application.DTO.POSTDTO;
using Application.MediatR.Post.Commands;
using Application.MediatR.Post.Querries;
using Domain.Models;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NailsBookingApp_API.Controllers.Base;

namespace NailsBookingApp_API.Controllers
{
    [ApiController]
    [Route("api/post")]
    [Authorize]
    public class PostController : ApiControllerBase
    {
        [HttpPost("createPost")]
        public async Task<ActionResult<ApiResponse>> CreatePost([FromBody] PostDTO postDto)
        {
            var result = await Mediator.Send(new CreatePostCommand(postDto));
            return await HandleResult(result);
        }

        [HttpPost("addComment")]
        public async Task<ActionResult<ApiResponse>> AddComment([FromBody] CommentDTO commentDTO)
        {
            var result = await Mediator.Send(new AddCommentCommand(commentDTO));
            return await HandleResult(result);
        }


        [HttpGet("getPosts")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse>> GetPosts()
        {
            var result = await Mediator.Send(new GetPostsQuerry());
            return await HandleResult(result);
        }

        [HttpGet("getPostById")]
        public async Task<ActionResult<ApiResponse>> GetPost(int id)
        {
            var result = await Mediator.Send(new GetPostByIdQuerry(id));
            return await HandleResult(result);
        }


        [HttpPost("handleLike")]
        public async Task<ActionResult<ApiResponse>> HandleLike([FromBody] LikeDTO likeDto)
        {
            var result = await Mediator.Send(new HandleLikeCommand(likeDto));
            return await HandleResult(result);
        }

        [HttpGet("getCommentsById/{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse>> GetCommentsById(int id)
        {
            var result = await Mediator.Send(new GetCommentsByIdQuerry(id));
            return await HandleResult(result);
        }

        [HttpDelete("deletePost")]
        public async Task<ActionResult<ApiResponse>> DeletePost([FromBody] DeletePostDTO deletePostDto)
        {
            var result = await Mediator.Send(new DeletePostCommand(deletePostDto));
            return await HandleResult(result);
        }

        [HttpDelete("deleteComment")]
        public async Task<ActionResult<ApiResponse>> DeleteComment([FromBody] DeleteCommentDTO deleteCommentDto)
        {
            var result = await Mediator.Send(new DeleteCommentCommand(deleteCommentDto));
            return await HandleResult(result);
        }

        [HttpPut("updatePost")]
        public async Task<ActionResult<ApiResponse>> UpdatePost([FromBody] UpdatePostDTO updatePostDto)
        {
            var result = await Mediator.Send(new UpdatePostCommand(updatePostDto));
            return await HandleResult(result);
        }

        [HttpPut("updateComment")]
        public async Task<ActionResult<ApiResponse>> UpdateComment([FromBody] UpdateCommentDTO updateCommentDto)
        {
            var result = await Mediator.Send(new UpdateCommentCommand(updateCommentDto));
            return await HandleResult(result);
        }

    }

}
