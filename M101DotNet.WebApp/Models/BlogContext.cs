using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using MongoDB.Bson;
using MongoDB.Driver;

namespace M101DotNet.WebApp.Models
{
    public class BlogContext
    {
        public const string CONNECTION_STRING_NAME = "Blog";
        public const string DATABASE_NAME = "blog";
        public const string POSTS_COLLECTION_NAME = "posts";
        public const string USERS_COLLECTION_NAME = "users";

        // This is ok... Normally, they would be put into
        // an IoC container.
        private static readonly IMongoClient _client;
        private static readonly IMongoDatabase _database;

        private FilterDefinition<Post> emptyFilter => Builders<Post>.Filter.Empty;

        static BlogContext()
        {
            var connectionString = ConfigurationManager.ConnectionStrings[CONNECTION_STRING_NAME].ConnectionString;
            _client = new MongoClient(connectionString);
            _database = _client.GetDatabase(DATABASE_NAME);
        }

        public IMongoClient Client
        {
            get { return _client; }
        }

        public IMongoCollection<Post> Posts
        {
            get { return _database.GetCollection<Post>(POSTS_COLLECTION_NAME); }
        }

        public IMongoCollection<User> Users
        {
            get { return _database.GetCollection<User>(USERS_COLLECTION_NAME); }
        }

        public IEnumerable<Post> FindPostsByTagname(string tag)
        {
            if (string.IsNullOrEmpty(tag) || string.IsNullOrWhiteSpace(tag))
            {
                return this.Posts.Find(emptyFilter).ToEnumerable();
            }
            var filter = Builders<Post>.Filter.AnyEq(p => p.Tags, tag);
            var posts = this.Posts.Find(filter).ToListAsync().Result;
            return posts;
        }

        public IEnumerable<Post> FindTopRecentPosts(int posts)
        {
            var sort = Builders<Post>.Sort.Descending(_ => _.CreatedAtUtc);

            var res = this.Posts.Find(emptyFilter).Limit(posts).Sort(sort).ToEnumerable();
            return res;
        }

        public Post FindPostByPostId(string id)
        {
            ObjectId parsedId = ObjectId.Parse(id);
            var cursor = this.Posts.Find(Builders<Post>.Filter.Eq(p => p.Id, parsedId));

            return cursor.FirstOrDefault();
        }

        public async Task<User> FindUserByName(string name)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Name, name);
            var user = await this.Users.Find(filter).SingleOrDefaultAsync();
            return user;
        }

        public async Task CommentToPost(string postId, string modelContent, string author)
        {
            var comment = new Comment
            {
                CreatedAtUtc = DateTime.UtcNow,
                Content = modelContent,
                Author = author
            };
            ObjectId id = ObjectId.Parse(postId);
            var idFilter = Builders<Post>.Filter.Eq(p => p.Id, id);
            var updateFilter = Builders<Post>.Update.Push<Comment>(post => post.Comments, comment);
            await this.Posts.FindOneAndUpdateAsync(idFilter, updateFilter);
        }
    }
}