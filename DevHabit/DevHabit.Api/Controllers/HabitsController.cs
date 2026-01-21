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
    ApplicationDbContext dbContext,
    IPublishEndpoint  publishEndpoint
    ): ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<HabitsCollectionDto>> GetHabits()
    {
        List<HabitDto> habit = await dbContext
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
        HabitDto? habit = await dbContext
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
        
        dbContext.Habits.Add(habit);
        
        await dbContext.SaveChangesAsync();
        
        await publishEndpoint.Publish(new HabitCreated(habit.Id));

        HabitDto habitDto = habit.ToDto();

        return CreatedAtAction(nameof(GetHabit), new { id = habitDto.Id }, habitDto);
    }
}
