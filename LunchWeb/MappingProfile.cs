using AutoMapper;
using Lunch.Domain.Models;
using LunchWeb.ViewModels.MealViewModels;

namespace LunchWeb
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Meal, MealViewModel>();

            CreateMap<Meal, MealFormViewModel>()
                .ForMember(d => d.ImageFile, o => o.Ignore());

            CreateMap<MealFormViewModel, Meal>()
                .ForMember(d => d.Id, o => o.Ignore())
                .ForMember(d => d.ImageUrl, o => o.Ignore());

            CreateMap<MealViewModel, MealFormViewModel>()
                .ForMember(d => d.ImageFile, o => o.Ignore());
        }
    }
}
