using DevHabit.Api.Database;
using DevHabit.Api.DTOs.Habits;
using DevHabit.Api.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevHabit.Api.Controllers;

[ApiController]
[Route("habits")]
public class HabitsController(ApplicationDbContext dbContext): ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<HabitsCollectionDto>> GetHabits()
    {
        List<HabitDto> habit = await dbContext
            .Habits
            .Select(h => new HabitDto
            {
                Id = h.Id,
                Name = h.Name,
                Description = h.Description,
                Type = h.Type,
                Frequency = new FrequencyDto
                {
                    Type = h.Frequency.Type,
                    TimesPerPeriod = h.Frequency.TimesPerPeriod
                },
                Target = new TargetDto
                {
                    Value = h.Target.Value,
                    Unit = h.Target.Unit
                },
                Status = h.Status,
                IsArchived = h.IsArchived,
                EndDate = h.EndDate,
                Milestone = new MilestoneDto
                {
                    Current = h.Milestone!.Current, 
                    Target = h.Milestone.Target
                },
                CreatedAtUtc = h.CreatedAtUtc,
                UpdatedAtUtc = h.UpdatedAtUtc,
                LastCompletedAtUtc = h.LastCompletedAtUtc
            })
            .ToListAsync();

        var habitsCollectionDto = new HabitsCollectionDto
        {
            Data = habit
        };
        
        return Ok(habitsCollectionDto);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<HabitsCollectionDto>> GetHabit(string id)
    {
        HabitDto? habit = await dbContext
            .Habits
            .Where(h => h.Id == id)
            .Select(h => new HabitDto
            {
                Id = h.Id,
                Name = h.Name,
                Description = h.Description,
                Type = h.Type,
                Frequency = new FrequencyDto
                {
                    Type = h.Frequency.Type,
                    TimesPerPeriod = h.Frequency.TimesPerPeriod
                },
                Target = new TargetDto
                {
                    Value = h.Target.Value,
                    Unit = h.Target.Unit
                },
                Status = h.Status,
                IsArchived = h.IsArchived,
                EndDate = h.EndDate,
                Milestone = new MilestoneDto
                {
                    Current = h.Milestone!.Current, 
                    Target = h.Milestone.Target
                },
                CreatedAtUtc = h.CreatedAtUtc,
                UpdatedAtUtc = h.UpdatedAtUtc,
                LastCompletedAtUtc = h.LastCompletedAtUtc
            })
            .FirstOrDefaultAsync();

        if (habit is null)
        {
            return NotFound();
        }
            
        return Ok(habit);
    }

    [HttpPost]
    public async Task<ActionResult<HabitDto>> CreateHabit(CreateHabitDto dto)
    {
            Habit habit = new()
            {
                Id = $"h_{Guid.CreateVersion7()}",
                Name = dto.Name,
                Description = dto.Description,
                Type = dto.Type,
                Frequency = new Frequency
                {
                    Type = dto.Frequency.Type,
                    TimesPerPeriod = dto.Frequency.TimesPerPeriod
                },
                Target = new Target
                {
                    Value = dto.Target.Value,
                    Unit = dto.Target.Unit
                },
                Status = HabitStatus.Ongoing,
                IsArchived = false,
                EndDate = dto.EndDate,
                Milestone = dto.Milestone is not null
                    ? new Milestone
                    {
                        Target = dto.Milestone.Target,
                        Current = 0 // Initialize current progress to 0
                    }
                    : null,
                CreatedAtUtc = DateTime.UtcNow
            };

            dbContext.Habits.Add(habit);
            await dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetHabit), new { id = habit.Id }, habit);
    }
}
