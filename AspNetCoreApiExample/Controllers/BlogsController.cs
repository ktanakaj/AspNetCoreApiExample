// ================================================================================================
// <summary>
//      ブログコントローラクラスソース</summary>
//
// <copyright file="BlogsController.cs">
//      Copyright (C) 2019 Koichi Tanaka. All rights reserved.</copyright>
// <author>
//      Koichi Tanaka</author>
// ================================================================================================

namespace Honememo.AspNetCoreApiExample.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Honememo.AspNetCoreApiExample.Models;

    /// <summary>
    /// ブログコントローラクラス。
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class BlogsController : ControllerBase
    {
        private readonly BlogContext context;

        public BlogsController(BlogContext context)
        {
            this.context = context;
        }

        // TODO: 更新系APIは、要認証のコントローラを作って移動する

        // GET api/blogs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Blog>>> Get()
        {
            return await context.Blogs.ToListAsync();
        }

        // GET api/blogs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Blog>> Get(long id)
        {
            var blog = await context.Blogs.FindAsync(id);

            if (blog == null)
            {
                return NotFound();
            }

            return blog;
        }

        // POST api/blogs
        [HttpPost]
        public async Task<ActionResult<Blog>> Post(Blog blog)
        {
            context.Blogs.Add(blog);
            await context.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = blog.Id }, blog);
        }

        // PUT api/blogs/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(long id, Blog blog)
        {
            if (id != blog.Id)
            {
                return BadRequest();
            }

            context.Entry(blog).State = EntityState.Modified;
            await context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE api/blogs/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var blog = await context.Blogs.FindAsync(id);

            if (blog == null)
            {
                return NotFound();
            }

            context.Blogs.Remove(blog);
            await context.SaveChangesAsync();

            return NoContent();
        }
    }
}
