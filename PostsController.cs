using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TheDevBlog.API.Data;
using TheDevBlog.API.Models.DTO;
using TheDevBlog.API.Models.Entities;

namespace TheDevBlog.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostsController : Controller
    {
        private readonly TheDevBlogDbContext dbContext;

        public PostsController(TheDevBlogDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllPosts()
        {
            var posts =  await dbContext.posts.ToListAsync();

            return Ok(posts);
        }
        [HttpGet]
        [Route("{id:guid}")]
        [ActionName("GetPostById")]
        public async Task<IActionResult> GetPostById(Guid id)
        {
            var post = await dbContext.posts.FirstOrDefaultAsync(x => x.Id == id);

            if(post != null)
            {
                return Ok(post);
            }
            return NotFound();

        }
        [HttpPost]

        public async Task<IActionResult> AddPost(AddPostRequest addPostRequest)
        {
            //convert DTO to Entity
            var post = new Post()
            {
                Title = addPostRequest.Title,
                Content = addPostRequest.Content,
                Summary = addPostRequest.Summary,
                UrlHamdle = addPostRequest.UrlHamdle,
                FeaturedImageUrl = addPostRequest.FeaturedImageUrl,
                Visible = addPostRequest.Visible,
                Author = addPostRequest.Author,
                PublishedDate = addPostRequest.PublishedDate,
                UpdatedDate = addPostRequest.UpdatedDate,

            };
            post.Id = Guid.NewGuid();
            await dbContext.posts.AddAsync(post);
            await dbContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetPostById), new { id = post.Id }, post);
        }
        [HttpPut]
        [Route("{id:guid}")]
        public async Task<IActionResult> UpdatePost([FromRoute] Guid id , UpdatePostRequest updatePostRequest )
        {
            
           
            // Check if exist 
            var existingPost = await dbContext.posts.FindAsync(id);
            if (existingPost != null)
            {
                //create DTO to Entity 
                existingPost.Title = updatePostRequest.Title;
                 existingPost.Content = updatePostRequest.Content;
                existingPost.Summary = updatePostRequest.Summary;
                existingPost.UrlHamdle = updatePostRequest.UrlHamdle;
                existingPost.FeaturedImageUrl = updatePostRequest.FeaturedImageUrl;
                existingPost.Visible = updatePostRequest.Visible;
                existingPost.Author = updatePostRequest.Author;
                existingPost.PublishedDate = updatePostRequest.PublishedDate;
                existingPost.UpdatedDate = updatePostRequest.UpdatedDate;

               await dbContext.SaveChangesAsync();
                return Ok(existingPost);

            }
            return NotFound();
        }

        [HttpDelete]
        [Route("{id:guid}")]
        public async Task<IActionResult> DeletePost( Guid id)
        {
            var existingPost =  await dbContext.posts.FindAsync(id);
            if (existingPost != null)
            {
                dbContext.Remove(existingPost);
                 await dbContext.SaveChangesAsync();
                return Ok(existingPost);
            }
            return NotFound();
        }


    }
}
