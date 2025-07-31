using AutoMapper;
using Blog.Domain.Entities;
using Blog.Application.Articles.DTOs;

public class ArticlesMappingProfile : Profile
{
    public ArticlesMappingProfile()
    {
        CreateMap<Article, ArticleDto>()
            .ForMember(dest => dest.AuthorUsername, opt => opt.MapFrom(src => src.Author.Username));

        CreateMap<Article, ArticleDetailDto>()
            .IncludeBase<Article, ArticleDto>();

        CreateMap<Comment, CommentDto>()
            .ForMember(dest => dest.AuthorUsername, opt => opt.MapFrom(src => src.Author.Username));

        CreateMap<CreateArticleRequest, Article>();
        CreateMap<UpdateArticleRequest, Article>();
        CreateMap<CreateCommentRequest, Comment>();
    }
}