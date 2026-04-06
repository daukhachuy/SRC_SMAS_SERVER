using SMAS_BusinessObject.DTOs.CustomerDTO;
using SMAS_BusinessObject.DTOs.StaffDTO;
using SMAS_BusinessObject.Models;
using SMAS_DataAccess.DAO;
using SMAS_Repositories.StaffRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_Services.StaffService
{
    public class StaffProfileService : IStaffProfileService
    {
        private readonly IStaffProfileRepository _staffProfileRepository;
        private readonly OrderDAO _orders;

        public StaffProfileService(IStaffProfileRepository staffProfileRepository , OrderDAO order)
        {
            _staffProfileRepository = staffProfileRepository;
            _orders = order;
        }

        public async Task<StaffProfileDto?> GetProfileStaffAsync(int userId)
            => await _staffProfileRepository.GetProfileAsync(userId);

        public async Task<(bool Success, string? ErrorMessage)> UpdateProfileStaffAsync(int userId, UpdateProfileStaffRequestDto dto)
        {
            // Validate
            if (!string.IsNullOrWhiteSpace(dto.Phone) && dto.Phone.Length < 9)
                return (false, "Số điện thoại không hợp lệ.");

            if (!string.IsNullOrWhiteSpace(dto.Email) && !dto.Email.Contains('@'))
                return (false, "Email không hợp lệ.");

            if (dto.Dob.HasValue && dto.Dob.Value > DateOnly.FromDateTime(DateTime.Today))
                return (false, "Ngày sinh không hợp lệ.");

            return await _staffProfileRepository.UpdateProfileAsync(userId, dto);
        }

        public async Task<IEnumerable<CustomerResponseDTO>> GetAllAcountCustomerAsync()
        {
            return await _staffProfileRepository.GetAllAcountCustomerAsync();
        }

        public async Task<IEnumerable<StaffResponseDTO>> GetAllAcountStaffAsync()
        {
            return await _staffProfileRepository.GetAllAcountStaffAsync();
        }

        public async Task<IEnumerable<StaffResponseDTO>> FilterAccountStaffAsync(FilterAccountStaffRequestDTO filter)
        {
            var allStaff = await _staffProfileRepository.GetAllAcountStaffAsync();

            var query = allStaff.AsQueryable();
            if (filter.status)
                query = query.Where(s => s.IsDeleted != true);
            else
                query = query.Where(s => s.IsDeleted == true);
            if (filter.role != null && filter.role.Any())
            {
                query = query.Where(s => s.Position != null && filter.role.Contains(s.Position));
            }
            return query.ToList();
        }

        public async Task<IEnumerable<CustomerResponseDTO>> FilterAccountCustomerAsync(bool request)
        {
            var customers = await _staffProfileRepository.GetAllAcountCustomerAsync();
            var query = customers.AsQueryable();

            if (request) 
            {
                query = query.Where(c => c.IsDeleted != true);
            }
            else 
            {
                query = query.Where(c => c.IsDeleted == true);
            }

            return query.ToList();
        }

        public async Task<bool> CreateStaffAsync(CreateNewStaffByUseridResquestDTO request)
        {
            return await _staffProfileRepository.CreateStaffAsync(request);
        }

        public async Task<bool> CreateStaffWithUserAsync(CreateNewStaffRequestDTO request)
        {
            request.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.PasswordHash);
            return await _staffProfileRepository.CreateStaffWithUserAsync(request);
        }

        public async Task<StaffDetailresponseDTO> GetStaffDetailToUpdateAsync(int userId)
        {
            return await _staffProfileRepository.GetStaffDetailToUpdateAsync(userId);
        }

        public async Task<bool> AdminUpdateStaffDetail(StaffDetailRequestDTO request)
        {
            return await _staffProfileRepository.AdminUpdateStaffDetail(request);
        }


        public async Task<CustomerDetailResponseDTO?> GetCustomerDetailAsync(int userId)
        {
            var customers = await _staffProfileRepository.GetAllAcountCustomerAsync();
            var customer = customers.FirstOrDefault(c => c.UserId == userId);
            if (customer == null) return null;
            var orders = await _orders.GetAllActiveOrderAsync();
            var customerOrders = orders.Where(o => o.UserId == userId).ToList();
            var result = new CustomerDetailResponseDTO
            {
                UserId = customer?.UserId ?? 0,
                Fullname = customer?.Fullname,
                Email = customer?.Email,
                Phone = customer?.Phone,
                Address = customer?.Address,
                Avatar = customer?.Avatar,
                totalOrders = customerOrders.Count,
                totalOrderCancel = customerOrders.Count(o => o.OrderStatus == "Cancelled"),
                totalOrderNoShow = 0,
                totalSpending = customerOrders.Where(o => o.OrderStatus == "Completed").Sum(o => (decimal?)o.TotalAmount) ?? 0
            };
            return result;

        }

    }

}
