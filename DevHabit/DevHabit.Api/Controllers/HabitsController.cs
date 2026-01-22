using System.Linq.Expressions;
using DevHabit.Api.Database;
using DevHabit.Api.DTOs.Habits;
using DevHabit.Api.Entities;
using DevHabit.Api.MassTransitContracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MassTransit;

namespace DevHabit.Api.Controllers;

[ApiController]
[Route("habits")]
public class HabitsController(
    ApplicationDbContext _dbContext,
    IPublishEndpoint  publishEndpoint
    ): ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<HabitsCollectionDto>> GetHabits()
    {
        List<HabitDto> habit = await _dbContext
            .Habits
            .Select(HabitQueries.ProjectToDto())
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
        HabitDto? habit = await _dbContext
            .Habits
            .Where(h => h.Id == id)
            .Select(HabitQueries.ProjectToDto())
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
        Habit habit = dto.ToEntity();
        
        _dbContext.Habits.Add(habit);
        
        await _dbContext.SaveChangesAsync();
        
        HabitDto habitDto = habit.ToDto();

        MessageHabitDto message = habit.ToMessageDto();

        await publishEndpoint.Publish(new HabitCreated(message));
        
        return CreatedAtAction(nameof(GetHabit), new { id = habitDto.Id }, habitDto);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateHabit(string id, [FromBody] UpdateHabitDto updateHabitDto)
    {
        Habit? habit = await _dbContext.Habits.FirstOrDefaultAsync(h => h.Id == id);
        if (habit is null)
        {
            return NotFound();
        }

        habit.UpdateFromDto(updateHabitDto);

        await _dbContext.SaveChangesAsync();

        return NoContent();
    }
}
