using Gifter.Models;
using Gifter.Utils;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Gifter.Repositories
{
    public class UserProfileRepository : BaseRepository, IUserProfileRepository
    {
        public UserProfileRepository(IConfiguration configuration) : base(configuration) { }

        public List<UserProfile> GetAll()
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                    SELECT Id, Name, Email, ImageUrl, Bio, DateCreated 
                    FROM UserProfile
                    ";

                    var reader = cmd.ExecuteReader();
                    var userProfiles = new List<UserProfile>();

                    while (reader.Read())
                    {
                        userProfiles.Add(new UserProfile()
                        {
                            Id = DbUtils.GetInt(reader, "Id"),
                            Name = DbUtils.GetString(reader, "Name"),
                            Email = DbUtils.GetString(reader, "Email"),
                            DateCreated = DbUtils.GetDateTime(reader, "DateCreated"),
                            ImageUrl = DbUtils.GetString(reader, "ImageUrl"),
                            Bio = DbUtils.GetString(reader, "Bio")
                        });
                    }
                    reader.Close();
                    return userProfiles;
                }
            }
        }

        public UserProfile GetById(int id)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                    SELECT Id, Name, Email, ImageUrl, Bio, DateCreated 
                    FROM UserProfile
                    WHERE Id = @id
                    ";
                    DbUtils.AddParameter(cmd, "@id", id);

                    var reader = cmd.ExecuteReader();

                    UserProfile userProfile = null;

                    if (reader.Read())
                    {
                        userProfile = new UserProfile()
                        {
                            Id = id,
                            Name = DbUtils.GetString(reader, "Name"),
                            Email = DbUtils.GetString(reader, "Email"),
                            ImageUrl = DbUtils.GetString(reader, "ImageUrl"),
                            DateCreated = DbUtils.GetDateTime(reader, "DateCreated"),
                            Bio = DbUtils.GetString(reader, "Bio")

                        };
                    }
                    reader.Close();
                    return userProfile;
                }

            }
        }

        public UserProfile GetWithPosts(int id)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                    
                    SELECT up.Name, up.Bio, up.Email, up.DateCreated AS UserProfileDateCreated,
                    up.ImageUrl AS UserProfileImageUrl,
                    p.Id AS PostId, p.Title, p.Caption, p.DateCreated AS PostDateCreated,
                    p.ImageUrl AS PostImageUrl, p.UserProfileId AS PostUserProfileId,
                    c.Id AS CommentId, c.Message, c.UserProfileId AS CommentUserProfileId
                    
                    FROM Post p
                    LEFT JOIN Comment c ON c.PostId = p.Id
                    LEFT JOIN UserProfile up ON p.UserProfileId = up.Id
                    WHERE up.Id = @id
                    ";

                    DbUtils.AddParameter(cmd, "@Id", id);
                    var reader = cmd.ExecuteReader();

                    UserProfile userProfile = null;

                    while (reader.Read())
                    {
                        if (userProfile == null)
                        {
                            userProfile = new UserProfile()
                            {
                                Id = id,
                                Name = DbUtils.GetString(reader, "Name"),
                                Email = DbUtils.GetString(reader, "Email"),
                                DateCreated = DbUtils.GetDateTime(reader, "UserProfileDateCreated"),
                                ImageUrl = DbUtils.GetString(reader, "UserProfileImageUrl"),
                                Bio = DbUtils.GetString(reader, "Bio"),
                                Posts = new List<Post>()
                            };

                        }

                        var postId = DbUtils.GetInt(reader, "PostId");

                        var existingPost = userProfile.Posts.FirstOrDefault(p => p.Id == postId);


                        if (DbUtils.IsNotDbNull(reader, "PostId"))
                        {

                            if (existingPost == null)
                            {
                                existingPost = new Post()
                                {
                                    Id = postId,
                                    Title = DbUtils.GetString(reader, "Title"),
                                    Caption = DbUtils.GetString(reader, "Caption"),
                                    DateCreated = DbUtils.GetDateTime(reader, "PostDateCreated"),
                                    ImageUrl = DbUtils.GetString(reader, "PostImageUrl"),
                                    UserProfileId = DbUtils.GetInt(reader, "PostUserProfileId"),
                                    Comments = new List<Comment>()

                                };
                                userProfile.Posts.Add(existingPost);

                            }

                        }


                        if (DbUtils.IsNotDbNull(reader, "CommentId"))
                        {
                            Comment comment = new Comment()
                            {
                                Id = DbUtils.GetInt(reader, "CommentId"),
                                Message = DbUtils.GetString(reader, "Message"),
                                PostId = postId,
                                UserProfileId = DbUtils.GetInt(reader, "CommentUserProfileId")
                            };

                            existingPost.Comments.Add(comment);

                        }
                    }
                    reader.Close();
                    return userProfile;
                }
            }
        }

        public void Add(UserProfile userProfile)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                    INSERT INTO UserProfile (Name, Email, ImageUrl, Bio, DateCreated)
                    OUTPUT INSERTED.ID
                    VALUES (@name, @email, @imageUrl, @bio, @dateCreated)
                     ";

                    DbUtils.AddParameter(cmd, "@name", userProfile.Name);
                    DbUtils.AddParameter(cmd, "@email", userProfile.Email);
                    DbUtils.AddParameter(cmd, "@imageUrl", userProfile.ImageUrl);
                    DbUtils.AddParameter(cmd, "@dateCreated", userProfile.DateCreated);
                    DbUtils.AddParameter(cmd, "@bio", userProfile.Bio);

                    userProfile.Id = (int)cmd.ExecuteScalar();
                }
            }
        }

        public void Update(UserProfile userProfile)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        UPDATE UserProfile
                           SET Name = @name,
                               Email = @email,
                               DateCreated = @dateCreated,
                               ImageUrl = @imageUrl,
                               Bio = @bio
                         WHERE Id = @id";

                    DbUtils.AddParameter(cmd, "@name", userProfile.Name);
                    DbUtils.AddParameter(cmd, "@email", userProfile.Email);
                    DbUtils.AddParameter(cmd, "@dateCreated", userProfile.DateCreated);
                    DbUtils.AddParameter(cmd, "@imageUrl", userProfile.ImageUrl);
                    DbUtils.AddParameter(cmd, "@bio", userProfile.Bio);
                    DbUtils.AddParameter(cmd, "@id", userProfile.Id);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Delete(int id)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM UserProfile WHERE Id = @id";
                    DbUtils.AddParameter(cmd, "@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

    }
}