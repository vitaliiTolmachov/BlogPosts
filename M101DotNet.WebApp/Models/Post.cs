using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using M101DotNet.WebApp.Models.Home;
using Microsoft.Ajax.Utilities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace M101DotNet.WebApp.Models
{
    public class Post
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string Author { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string[] Tags { get; set; }
        [BsonRepresentation(BsonType.DateTime)]
        public DateTime CreatedAtUtc { get; set; }
        public Comment[] Comments { get; set; }

        public static explicit operator Post(NewPostModel v)
        {
            return new Post
            {
                Content = v.Content,
                CreatedAtUtc = DateTime.UtcNow,
                Title = v.Title,
                Tags = v.Tags.Split(new []{" ",","},StringSplitOptions.RemoveEmptyEntries),
                Comments = new Comment[] {}
            };
        }
    }
}