using iEvent.Application.DTOs;
using iEvent.Application.Interfaces.Repositories;
using iEvent.Application.Interfaces.Services;
using iEvent.Domain.Entities;
using iEvent.Domain.Enums;

namespace iEvent.Application.Services
{
    public class EventService : IEventService
    {
        private readonly IEventRepository _eventRepository;
        public EventService(IEventRepository eventRepository)
        {
            _eventRepository = eventRepository;
        }

        public async Task<EventRespDto> CreateAsync(EventCreateDto dto)
        {
            var newEventId = Guid.NewGuid();

            var ievent = new Event
            {
                EventId = Guid.NewGuid(),
                Name = dto.Name,
                Description = dto.Description,
                EventDates = dto.EventDates.Select(d => new EventDate
                    {
                        EventDateId = Guid.NewGuid(),
                        Date = d.Date,
                        TimeSlots = d.TimeSlots.Select(ts => new EventTimeSlot
                            {
                                TimeSlotId = Guid.NewGuid(),
                                StartTime = ts.StartTime,
                                EndTime = ts.EndTime
                            }).ToList()
                    }).ToList(),
                VenueId = dto.VenueId,
                ImageUrl = dto.ImageUrl,
                Category = dto.Category,
                Images = dto.Images.Select(i => new EventImage
                {
                    ImageId = Guid.NewGuid(),
                    EventId = newEventId,
                    Url = i.Url,
                    SortOrder = i.SortOrder
                }).ToList()
            };

            await _eventRepository.AddAsync(ievent);

            return MapToRespDto(ievent);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var ievent = await _eventRepository.GetByIdAsync(id);
            if (ievent == null)
            {
                return false;
            }

            await _eventRepository.DeleteAsync(ievent);
            return true;
        }

        public async Task<PagedResult<EventRespDto>> GetAllAsync(EventQueryDto query)
        {
            var pagedEvents = await _eventRepository.GetAllAsync(query);

            return new PagedResult<EventRespDto>
            {
                TotalCount = pagedEvents.TotalCount,
                Items = pagedEvents.Items.Select(MapToRespDto).ToList()
            };
        }

        public async Task<EventRespDto?> GetByIdAsync(Guid id)
        {
            var ievent = await _eventRepository.GetByIdAsync(id);
            return ievent == null ? null : MapToRespDto(ievent);
        }

        public async Task<bool> UpdateAsync(Guid id, EventUpdateDto dto)
        {
            var ievent = await _eventRepository.GetByIdAsync(id);
            if (ievent == null)
            {
                return false;
            }

            ievent.Name = dto.Name;
            ievent.Description = dto.Description;
            ievent.VenueId = dto.VenueId;
            ievent.ImageUrl = dto.ImageUrl;
            ievent.Category = dto.Category;

            ievent.EventDates.Clear();
            foreach (var d in dto.EventDates)
            {
                ievent.EventDates.Add(new EventDate
                {
                    EventDateId = Guid.NewGuid(),
                    EventId = ievent.EventId,
                    Date = d.Date,
                    TimeSlots = d.TimeSlots.Select(t => new EventTimeSlot
                    {
                        TimeSlotId = Guid.NewGuid(),
                        StartTime = t.StartTime,
                        EndTime = t.EndTime
                    }).ToList()
                });
            }

            ievent.Images = dto.Images.Select(i => new EventImage
            {
                ImageId = Guid.NewGuid(),
                EventId = ievent.EventId,
                Url = i.Url,
                SortOrder = i.SortOrder
            }).ToList();

            await _eventRepository.UpdateAsync(ievent);
            return true;
        }

        public async Task<List<EventRespDto>> GetEventsByVenueIdAsync(Guid venueId)
        {
            var events = await _eventRepository.GetEventsByVenueIdAsync(venueId);
            return events.Select(MapToRespDto).ToList();
        }

        public async Task<List<EventRespDto>> GetPopularEventsAsync(int count)
        {
            var events = await _eventRepository.GetPopularEventsAsync(count);
            return events.Select(MapToRespDto).ToList();
        }

        public async Task<List<EventDateRespDto>?> GetEventDatesAsync(Guid id)
        {
            var ievent = await _eventRepository.GetByIdAsync(id);
            if (ievent == null)
            {
                return null;
            }

            return ievent.EventDates
                .OrderBy(ed => ed.Date)
                .Select(ed => new EventDateRespDto
                {
                    EventDateId = ed.EventDateId,
                    Date = ed.Date,
                    TimeSlots = ed.TimeSlots
                        .OrderBy(ts => ts.StartTime)
                        .Select(ts => new EventTimeSlotRespDto
                        {
                            TimeSlotId = ts.TimeSlotId,
                            StartTime = ts.StartTime,
                            EndTime = ts.EndTime
                        }).ToList()
                }).ToList();
        }

        public async Task<bool> AddEventDatesAsync(Guid id, List<EventDateCreateDto> dto)
        {
            var ev = await _eventRepository.GetByIdAsync(id);
            if (ev == null) return false;

            var newDates = dto.Select(d => new EventDate
            {
                EventDateId = Guid.NewGuid(),
                EventId = ev.EventId,
                Date = d.Date,
                TimeSlots = d.TimeSlots.Select(ts => new EventTimeSlot
                {
                    TimeSlotId = Guid.NewGuid(),
                    StartTime = ts.StartTime,
                    EndTime = ts.EndTime
                }).ToList()
            }).ToList();

            await _eventRepository.AddEventDatesRangeAsync(newDates);

            if (ev.IsDraft && !string.IsNullOrEmpty(ev.Name) && ev.VenueId.HasValue)
            {
                ev.IsDraft = false;
                await _eventRepository.UpdateAsync(ev);
            }

            return true;
        }

        public async Task<List<EventRespDto>?> GetSimilarEventsAsync(Guid id, int count = 4)
        {
            var currentEvent = await _eventRepository.GetByIdAsync(id);
            if (currentEvent == null)
            {
                return null;
            }

            var queryDto = new EventQueryDto
            {
                Category = currentEvent.Category,
                Page = 1,
                PageSize = count + 1 
            };

            var allCategorizedEvents = await _eventRepository.GetAllAsync(queryDto);

            var similarEvents = allCategorizedEvents.Items
                .Where(e => e.EventId != id && !e.IsDraft)
                .Take(count)
                .Select(MapToRespDto)
                .ToList();

            return similarEvents;
        }

        public async Task<List<EventRespDto>> GetPreviewByCategoryAsync(string category, int count = 4)
        {
            var queryDto = new EventQueryDto
            {
                Category = Enum.TryParse<EventCategory>(category, true, out var parsedCategory) ? parsedCategory : null,
                Page = 1,
                PageSize = 20
            };

            var result = await _eventRepository.GetAllAsync(queryDto);

            return result.Items
                .Where(e => !e.IsDraft)
                .Take(count)
                .Select(MapToRespDto)
                .ToList();
        }

        public async Task<List<EventRespDto>> GetPreviewByCityAsync(string city, int count = 4)
        {
            var queryDto = new EventQueryDto
            {
                Page = 1,
                PageSize = 50 
            };

            var result = await _eventRepository.GetAllAsync(queryDto);

            return result.Items
                .Where(e => !e.IsDraft && e.Venue != null && e.Venue.City.Equals(city, StringComparison.OrdinalIgnoreCase))
                .Take(count)
                .Select(MapToRespDto)
                .ToList();
        }

        private static EventRespDto MapToRespDto(Event ievent)
        {
            var sortedDates = ievent.EventDates.OrderBy(ed => ed.Date).ToList();

            return new EventRespDto
            {
                EventId = ievent.EventId,
                Name = ievent.Name,
                Description = ievent.Description,
                VenueId = ievent.VenueId,
                ImageUrl = ievent.ImageUrl,
                Category = ievent.Category,
                EventDates = sortedDates.Select(ed => new EventDateRespDto
                {
                        EventDateId = ed.EventDateId,
                        Date = ed.Date,
                        TimeSlots = ed.TimeSlots.OrderBy(ts => ts.StartTime).Select(ts => new EventTimeSlotRespDto
                        {
                            TimeSlotId = ts.TimeSlotId,
                            StartTime = ts.StartTime,
                            EndTime = ts.EndTime
                        }).ToList()
                }).ToList(),
                Images = ievent.Images
                    .OrderBy(i => i.SortOrder)
                    .Select(i => new EventImageRespDto
                    {
                        ImageId = i.ImageId,
                        Url = i.Url,
                        SortOrder = i.SortOrder
                    }).ToList(),
                MinTicketPrice = ievent.Tickets != null && ievent.Tickets.Any() ? ievent.Tickets.Min(t => t.Price) : 0,
                AllDates = sortedDates.Select(ed => ed.Date).ToList()
            };
        }

        public async Task<EventRespDto> CreateDraftAsync()
        {
            var ev = new Event
            {
                EventId = Guid.NewGuid(),
                Name = "",
                Description = "",
                VenueId = null,
                Category = EventCategory.Other,
                IsDraft = true,
                Images = new List<EventImage>(),
                EventDates = new List<EventDate>()
            };

            await _eventRepository.AddAsync(ev);

            return MapToRespDto(ev);
        }

        public async Task<bool> PatchAsync(Guid id, EventPatchDto dto)
        {
            var ev = await _eventRepository.GetByIdAsync(id);
            if (ev == null) return false;

            if (dto.Name != null) ev.Name = dto.Name;
            if (dto.Description != null) ev.Description = dto.Description;
            if (dto.VenueId.HasValue) ev.VenueId = dto.VenueId.Value;
            if (dto.ImageUrl != null) ev.ImageUrl = dto.ImageUrl;

            await _eventRepository.UpdateAsync(ev);

            if (dto.EventDates != null)
            {
                var mappedNewDates = dto.EventDates.Select(d => new EventDate
                {
                    EventDateId = Guid.NewGuid(),
                    EventId = ev.EventId,
                    Date = d.Date,
                    TimeSlots = d.TimeSlots.Select(ts => new EventTimeSlot
                    {
                        TimeSlotId = Guid.NewGuid(),
                        StartTime = ts.StartTime,
                        EndTime = ts.EndTime
                    }).ToList()
                }).ToList();

                await _eventRepository.UpdateEventDatesAsync(ev.EventId, mappedNewDates);

                ev.EventDates = mappedNewDates;
            }

            if (!string.IsNullOrEmpty(ev.Name) && ev.VenueId.HasValue && ev.EventDates.Any())
            {
                ev.IsDraft = false;
                await _eventRepository.UpdateAsync(ev);
            }

            return true;
        }

        public async Task<bool> PublishAsync(Guid id)
        {
            var ev = await _eventRepository.GetByIdAsync(id);
            if (ev == null) return false;

            if (string.IsNullOrEmpty(ev.Name))
                throw new Exception("Name required");

            if (!ev.VenueId.HasValue)
                throw new Exception("Venue required");

            if (!ev.EventDates.Any())
                throw new Exception("Dates required");

            ev.IsDraft = false;

            await _eventRepository.UpdateAsync(ev);
            return true;
        }
    }
}
