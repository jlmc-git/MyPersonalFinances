namespace FinanceCore.Application.Transactions.Dtos;

public sealed record ClassificationResultDto(
    ClassificationStatusDto MainClassification,
    IReadOnlyList<ClassificationStatusDto> Alternatives);
