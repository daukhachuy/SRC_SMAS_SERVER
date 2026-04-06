using SMAS_BusinessObject.DTOs.Combo;
using SMAS_Repositories.ComboRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_Services.ComboServices
{
    public class ComboService : IComboService
    {
        private readonly IComboRepository _comboRepository;

        public ComboService(IComboRepository comboRepository)
        {
            _comboRepository = comboRepository;
        }

        public async Task<IEnumerable<ComboListResponse>> GetAvailableCombosAsync()
        {
            return await _comboRepository.GetAvailableComboListAsync();
        }
        public async Task<IEnumerable<ComboListResponse>> GetCombosFilterAsync(CombosFilterRequest request)
        {
            var combos = await _comboRepository.GetAvailableComboListAsync();

            if (combos == null || !combos.Any())
                return Enumerable.Empty<ComboListResponse>();

            return combos
                        .Where(c => !request.MinPrice.HasValue || c.Price >= request.MinPrice.Value)
                        .Where(c => !request.MaxPrice.HasValue || c.Price <= request.MaxPrice.Value);
        }

        public async Task<bool> UpdateStatusByComboId(int comboId)
        {
            return await _comboRepository.UpdateStatusByComboId(comboId);
        }
        public Task<IEnumerable<ComboListResponse>> GetAllAsync()
           => _comboRepository.GetAllAsync();

        public Task<ComboListResponse?> GetByIdAsync(int id)
            => _comboRepository.GetByIdAsync(id);

        public Task<ComboListResponse> CreateAsync(ComboCreateDto dto)
            => _comboRepository.CreateAsync(dto);

        public Task<ComboListResponse?> UpdateAsync(int id, ComboUpdateDto dto)
            => _comboRepository.UpdateAsync(id, dto);

        public Task<bool> DeleteAsync(int id)
            => _comboRepository.DeleteAsync(id);

        public Task<bool> UpdateStatusAsync(int id, bool isAvailable)
            => _comboRepository.UpdateStatusAsync(id, isAvailable);
    }
}
