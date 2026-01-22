using DevHabit.Api.Controllers;
using MassTransit;

namespace DevHabit.Api.MassTransitContracts;

public class HabitCreatedConsumer : IConsumer<HabitCreated>
{
    public Task Consume(ConsumeContext<HabitCreated> context)
    {
        Console.WriteLine($"[Consumer] HabitCreated received: {context.Message.Habit}");
        return Task.CompletedTask;
    }
}
