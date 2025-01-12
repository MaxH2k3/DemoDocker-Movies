using AutoMapper;
using DemoDocker.Dto;
using DemoDocker.Models;

namespace DemoDocker.Configuration
{
		public class MappingData : Profile
		{
			public MappingData()
			{
				//movie

				CreateMap<MovieCategory, Category>()
					.ForMember(dest => dest.Name,
					opt => opt.MapFrom(src => src.Category.Name));

				CreateMap<Movie, NewMovie>();
				CreateMap<NewMovie, Movie>();

				CreateMap<Movie, MovieDetail>()
					.ForMember(dest => dest.Categories,
					opt => opt.MapFrom(src => src.MovieCategories));

			CreateMap<PagedList<Movie>, PagedList<MovieDetail>>();

			CreateMap<User, UserLoginResponse>();
		}
		}
}
