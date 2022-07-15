using System;

namespace AllinaHealth.Models.MigrationModels
{
    public class HsgMigrationModel
    {
        public int Id { get; set; }
        public string Category { get; set; }
        public string Title { get; set; }
        public string AuthorName { get; set; }
        public string AuthorFullName { get; set; }
        public string AuthorDescription { get; set; }
        public string ArticleBody { get; set; }
        public string AuthorImageUrl { get; set; }
        public string AuthorUrl { get; set; }

        public string VideoUrl { get; set; }
        public string ThumbnailUrl { get; set; }
        public string ImageUrl { get; set; }
        public long EktronId { get; set; }
        public DateTime PostDateTime { get; set; }
        public DateTime UpdatedDateTime { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public Relatedarticlepost[] RelatedArticlePosts { get; set; }
        public Relatedarticleaction[] RelatedArticleActions { get; set; }
        public string PostDate { get; set; }
        public string UpdateDate { get; set; }
        public string CreateDate { get; set; }
    }

    public class Relatedarticlepost
    {
        public int Id { get; set; }
        public int ArticleId { get; set; }
        public long EktronId { get; set; }
        public string Category { get; set; }
        public string PostTitle { get; set; }
        public string ThumbnailUrl { get; set; }
        public string PostUrl { get; set; }
        public int SortOrder { get; set; }
        public object ArticleCategory { get; set; }
        public object ArticleTitle { get; set; }
        public DateTime ArticlePostDateTime { get; set; }
        public DateTime PostDateTime { get; set; }
        public string PostDate { get; set; }
    }

    public class Relatedarticleaction
    {
        public int Id { get; set; }
        public int ArticleId { get; set; }
        public string ColorTile { get; set; }
        public string Title { get; set; }
        public string IconUrl { get; set; }
        public string ActionBody { get; set; }
        public string ActionTitle { get; set; }
        public string ActionUrl { get; set; }
        public object ArticleCategory { get; set; }
        public object ArticleTitle { get; set; }
        public DateTime ArticlePostDateTime { get; set; }
    }
}