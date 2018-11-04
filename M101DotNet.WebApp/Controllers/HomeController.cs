using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using MongoDB.Driver;
using M101DotNet.WebApp.Models;
using M101DotNet.WebApp.Models.Home;
using MongoDB.Bson;
using System.Linq.Expressions;
using System.Security.Claims;

namespace M101DotNet.WebApp.Controllers
{
    public class HomeController : Controller
    {
        public async Task<ActionResult> Index()
        {
            var blogContext = new BlogContext();
            IEnumerable<Post> recentPosts = blogContext.FindTopRecentPosts(10);
            // XXX WORK HERE
            // find the most recent 10 posts and order them
            // from newest to oldest

            var model = new IndexModel
            {
                RecentPosts = recentPosts.ToList()
            };

            return View(model);
        }

        [HttpGet]
        public ActionResult NewPost()
        {
            return View(new NewPostModel());
        }

        [HttpPost]
        public async Task<ActionResult> NewPost(NewPostModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var claims = this.Request.GetOwinContext().Authentication.User.Claims.ToList();

            //string email = claims.Where(claim => claim.Type == ClaimTypes.Email)
            //    .Select(claim => claim.Value)
            //    .FirstOrDefault();

            string name = claims.Where(claim => claim.Type == ClaimTypes.Name || claim.Type == ClaimTypes.GivenName)
                .Select(claim => claim.Value)
                .FirstOrDefault();

            var blogContext = new BlogContext();
            User user = await blogContext.FindUserByName(name);
            var post = (Post)model;
            post.Author = user.Name;
            await blogContext.Posts.InsertOneAsync(post);
            // XXX WORK HERE
            // Insert the post into the posts collection
            return RedirectToAction("Post", new { id = post.Id });
        }

        [HttpGet]
        public async Task<ActionResult> Post(string id)
        {
            var blogContext = new BlogContext();

            // XXX WORK HERE
            // Find the post with the given identifier
            var post = blogContext.FindPostByPostId(id);

            if (post == null)
            {
                return RedirectToAction("Index");
            }

            var model = new PostModel
            {
                Post = post
            };

            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> Posts(string tag = null)
        {
            var blogContext = new BlogContext();
            IEnumerable<Post> posts = blogContext.FindPostsByTagname(tag);

            // XXX WORK HERE
            // Find all the posts with the given tag if it exists.
            // Otherwise, return all the posts.
            // Each of these results should be in descending order.

            return View(posts);
        }

        [HttpPost]
        public async Task<ActionResult> NewComment(NewCommentModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Post", new { id = model.PostId });
            }

            var blogContext = new BlogContext();
            await blogContext.CommentToPost(model.PostId, model.Content, this.User.Identity.Name);
            
            // XXX WORK HERE
            // add a comment to the post identified by model.PostId.
            // you can get the author from "this.User.Identity.Name"

            return RedirectToAction("Post", new { id = model.PostId });
        }
    }
}