using FinanceCore.Application.Transactions.Commands.CreateTransaction;
using FinanceCore.Application.Transactions.Dtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FinanceCore.API.Transactions;

[ApiController]
[Route("api/transactions")]
public sealed class TransactionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public TransactionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [ProducesResponseType(typeof(TransactionDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TransactionDto>> Create(
        [FromBody] CreateTransactionRequest request,
        CancellationToken cancellationToken)
    {
        CreateTransactionCommand command = new(
            request.AmountInMinorUnits,
            request.CurrencyCode,
            request.OccurredAt,
            request.Description,
            null,
            request.Source);

        TransactionDto transaction = await _mediator.Send(command, cancellationToken);

        return Created($"/api/transactions/{transaction.Id}", transaction);
    }
}
