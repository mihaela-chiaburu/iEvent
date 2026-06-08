using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using iEvent.Application.DTOs;

namespace iEvent.Application.Interfaces.Services
{
    public interface IBookingService
    {
        Task<List<BookingRespDto>> GetAllAsync();
        Task<BookingRespDto?> GetByIdAsync(Guid id);
        Task<List<BookingRespDto>> GetMyBookingsAsync(string identityUserId);
        Task<BookingRespDto?> CreateAsync(BookingCreateDto dto, string identityUserId);
        Task<bool> UpdateAsync(Guid id, BookingUpdateDto dto);
        Task<bool> DeleteAsync(Guid id);
        Task<BookingRespDto?> GetByCodeAsync(string code);

        Task<bool> MarkPaidAsync(Guid id);
        Task<bool> MarkUnpaidAsync(Guid id);
        Task<bool> CancelAsync(Guid id);
    }
}
