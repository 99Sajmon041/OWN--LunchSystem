using AutoMapper;
using Lunch.Domain.Models;
using Lunch.Shared.Exceptions;
using LunchWeb.Helpers;
using LunchWeb.Repositories.Interfaces;
using LunchWeb.Services.Interfaces;
using LunchWeb.ViewModels.MealViewModels;
using System.ComponentModel.DataAnnotations;

namespace LunchWeb.Services.Implementations
{
    public class MealService : IMealService
    {
        private readonly IMealRepository _mealRepository;
        private readonly IMapper _mapper;
        private readonly IImageService _imageService;

        public MealService(IMealRepository mealRepository, IMapper mapper, IImageService imageService)
        {
            _mealRepository = mealRepository;
            _mapper = mapper;
            _imageService = imageService;
        }

        public async Task<List<MealViewModel>> GetAllMealsAsync()
        {
            var meals = await _mealRepository.GetAllMealsAsync();
            return _mapper.Map<List<MealViewModel>>(meals);
        }

        public async Task<MealViewModel?> GetMealByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Neplatné ID jídla.", nameof(id));

            var meal = await _mealRepository.GetMealByIdAsync(id)
                       ?? throw new InvalidOperationException("Jídlo s daným ID nebylo nalezeno.");

            return _mapper.Map<MealViewModel>(meal);
        }

        public async Task CreateMealAsync(MealFormViewModel vm)
        {
            if (vm.ImageFile == null || vm.ImageFile.Length == 0)
                throw new ValidationException("Obrázek je povinný.");

            var imageUrl = await _imageService.SaveMealImageAsync(vm.ImageFile);
            var meal = _mapper.Map<Meal>(vm);
            meal.ImageUrl = imageUrl;

            await _mealRepository.CreateAsync(meal);
        }

        public async Task UpdateMealAsync(MealFormViewModel vm)
        {
            if (!vm.Id.HasValue || vm.Id <= 0)
                throw new ArgumentException("Neplatné ID pro aktualizaci jídla.", nameof(vm.Id));

            vm.FoodName = vm.FoodName!.Trim();
            vm.FoodDescription = vm.FoodDescription!.Trim();
            vm.Price = decimal.Round(vm.Price, 2);

            var meal = await _mealRepository.GetMealByIdAsync(vm.Id.Value)
                       ?? throw new InvalidOperationException("Jídlo nebylo nalezeno.");

            var noFieldChanges = EntityComparer.IsMealSame(meal, vm);
            if (vm.ImageFile != null && vm.ImageFile.Length > 0)
            {
                var newUrl = await _imageService.SaveMealImageAsync(vm.ImageFile);
                await _imageService.DeleteAsync(meal.ImageUrl);
                meal.ImageUrl = newUrl;

                noFieldChanges = false;
            }

            if (noFieldChanges)
                throw new NoChangesException("Žádné změny nebyly provedeny.");

            _mapper.Map(vm, meal, typeof(MealFormViewModel), typeof(Meal));

            await _mealRepository.UpdateAsync(meal);
        }

        public async Task DeleteMealAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Neplatné ID pro smazání jídla.", nameof(id));

            var meal = await _mealRepository.GetMealByIdAsync(id)
                       ?? throw new InvalidOperationException("Jídlo k odstranění nebylo nalezeno.");

            await _mealRepository.DeleteAsync(meal);
            await _imageService.DeleteAsync(meal.ImageUrl);
        }
    }
}
