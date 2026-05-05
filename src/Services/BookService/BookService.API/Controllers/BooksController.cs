using BookService.Application.Dtos;
using BookService.Application.Features.Books.Commands.CreateBook;
using BookService.Application.Features.Books.Commands.DeactivateBook;
using BookService.Application.Features.Books.Commands.DeleteBook;
using BookService.Application.Features.Books.Commands.UpdateBook;
using BookService.Application.Features.Books.Commands.UpdateBookStock;
using BookService.Application.Features.Books.Queries.GetAllBooks;
using BookService.Application.Features.Books.Queries.GetAvailableBooks;
using BookService.Application.Features.Books.Queries.GetBookById;
using BookService.Application.Features.Books.Queries.GetBooksByAuthor;
using BookService.Application.Features.Books.Queries.GetBooksByFilter;
using BookService.Application.Features.Books.Queries.GetBooksByGenre;
using BookService.Application.Features.Books.Queries.GetBooksByPriceRange;
using BookService.Application.Features.Books.Queries.SearchBooksByTitle;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BookService.API.Controllers;

// Thin controller: no business logic here.
// All business rules live in handlers, validators, or repository methods.
[ApiController]
[Route("api/books")]
public sealed class BooksController : ControllerBase
{
    private readonly IMediator _mediator;

    public BooksController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<List<BookDto>>> GetAll([FromQuery] bool includeInactive = false, CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetAllBooksQuery(includeInactive), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BookDto>> GetById(string id, [FromQuery] bool includeInactive = false, CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetBookByIdQuery(id, includeInactive), cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpGet("search")]
    public async Task<ActionResult<List<BookDto>>> Search([FromQuery] string title, CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new SearchBooksByTitleQuery(title), cancellationToken);
        return Ok(result);
    }

    [HttpGet("author/{author}")]
    public async Task<ActionResult<List<BookDto>>> ByAuthor(string author, CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetBooksByAuthorQuery(author), cancellationToken);
        return Ok(result);
    }

    [HttpGet("category/{genre}")]
    public async Task<ActionResult<List<BookDto>>> ByGenre(string genre, CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetBooksByGenreQuery(genre), cancellationToken);
        return Ok(result);
    }

    [HttpGet("available")]
    public async Task<ActionResult<List<BookDto>>> GetAvailable(CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetAvailableBooksQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpGet("price")]
    public async Task<ActionResult<List<BookDto>>> ByPriceRange(
        [FromQuery] decimal? minPrice,
        [FromQuery] decimal? maxPrice,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetBooksByPriceRangeQuery(minPrice, maxPrice), cancellationToken);
        return Ok(result);
    }

    [HttpGet("filter")]
    public async Task<ActionResult<List<BookDto>>> Filter(
        [FromQuery] string? author,
        [FromQuery] string? genre,
        [FromQuery] decimal? minPrice,
        [FromQuery] decimal? maxPrice,
        [FromQuery] bool includeInactive = false,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetBooksByFilterQuery(author, genre, minPrice, maxPrice, includeInactive), cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<BookDto>> Create([FromBody] CreateBookCommand command, CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.BookId }, result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<BookDto>> Update(string id, [FromBody] UpdateBookCommand command, CancellationToken cancellationToken = default)
    {
        if (id != command.BookId)
        {
            return BadRequest("Route id and body BookId must match.");
        }

        var result = await _mediator.Send(command, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPut("{id}/stock")]
    public async Task<ActionResult<BookDto>> UpdateStock(string id, [FromBody] UpdateBookStockCommand command, CancellationToken cancellationToken = default)
    {
        if (id != command.BookId)
        {
            return BadRequest("Route id and body BookId must match.");
        }

        var result = await _mediator.Send(command, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPatch("{id}/deactivate")]
    public async Task<ActionResult<BookDto>> Deactivate(string id, CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new DeactivateBookCommand(id), cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id, CancellationToken cancellationToken = default)
    {
        var deleted = await _mediator.Send(new DeleteBookCommand(id), cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}
