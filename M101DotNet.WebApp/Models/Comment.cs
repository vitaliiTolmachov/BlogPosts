using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace M101DotNet.WebApp.Models
{
    public class Comment
    {
        public string Author { get; set; }
        public string Content { get; set; }
        [BsonRepresentation(BsonType.DateTime)]
        public DateTime CreatedAtUtc { get; set; }
    }
}