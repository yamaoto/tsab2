using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rikka.Tsab2.Database.Context.Entities
{
    public interface IEntity
    {
        Guid Id { get; set; }
        DateTime CreatedOn { get; set; }
    }
    

    public class Chat : IEntity
    {
        [Key]
        public Guid Id { get; set; }
        public DateTime CreatedOn { get; set; }

        public int ChatId { get; set; }

        public ChatType Type { get; set; }

        public string State { get; set; }
        public string StateParams { get; set; }

        public virtual ICollection<VkAuth> VkAuths { get; set; }
    }

    public class VkWall: IEntity
    {
        [Key]
        public Guid Id { get; set; }
        public DateTime CreatedOn { get; set; }

        public VkWallType Type { get; set; }
        public int VkWallId { get; set; }

        public string Url { get; set; }
        public string Name { get; set; }
        public DateTime? LastUpdate { get; set; }
        public int? UploadAlbum { get; set; }

        public virtual ICollection<VkPhoto> Photos { get; set; }
    }

    public class VkWallEntry: IEntity
    {
        [Key]
        public Guid Id { get; set; }
        public DateTime CreatedOn { get; set; }

        public Guid WallId { get; set; }
        public virtual VkWall Wall { get; set; }

        public string Url { get; set; }
        public string Text { get; set; }

        public virtual ICollection<VkPhoto> Photos { get; set; }
    }

    public class VkPhoto: IEntity
    {
        [Key]
        public Guid Id { get; set; }
        public DateTime CreatedOn { get; set; }

        public Guid WallId { get; set; }
        public virtual VkWall Wall { get; set; }

        public Guid EntryId { get; set; }
        public virtual VkWallEntry Entry { get; set; }

        public string Url { get; set; }

        [Column(TypeName = "image")]
        public byte[] Content { get; set; }
    }

    public class VkAuth:IEntity
    {
        [Key]
        public Guid Id { get; set; }
        public DateTime CreatedOn { get; set; }

        public string Code { get; set; }
        public string Token { get; set; }
        public DateTime ExpiredAt { get; set; }

        public VkWallType Type { get; set; }

        public Guid ChatId { get; set; }
        public virtual Chat Chat { get; set; }
    }

    public class SearchEngine:IEntity
    {
        [Key]
        public Guid Id { get; set; }
        public DateTime CreatedOn { get; set; }

        public string Name { get; set; }

        public int  ChatId { get; set; }

        public string Token { get; set; }
        public string Additional { get; set; }
    }

    public enum VkWallType
    {
        User=0,
        Group=1
    }

    public enum ChatType
    {
        Private=0,
        Group=1
    }
}
