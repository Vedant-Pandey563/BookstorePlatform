using BookService.Application.Dtos;
using MediatR;

namespace BookService.Application.Features.Books.Queries.GetBooksByPriceRange;

public sealed record GetBooksByPriceRangeQuery(decimal? MinPrice, decimal? MaxPrice) : IRequest<List<BookDto>>;
