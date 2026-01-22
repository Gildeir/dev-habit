using DevHabit.Api.Entities;

namespace DevHabit.Api.MassTransitContracts;

public record HabitCreated(MessageHabitDto Habit);

public record class MessageHabitDto
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
    public required HabitType Type { get; init; }

    public required MessageFrequencyDto Frequency { get; init; }
    public required MessageTargetDto Target { get; init; }

    public DateOnly? EndDate { get; init; }
    public MessageMilestoneDto? Milestone { get; init; }
}

public record class MessageFrequencyDto
{
    public required FrequencyType Type { get; init; }
    public required int TimesPerPeriod { get; init; }
}

public record class MessageTargetDto
{
    public required int Value { get; init; }
    public required string Unit { get; init; }
}

public record class MessageMilestoneDto
{
    public required int Target { get; init; }
    public required int Current { get; init; }
}
