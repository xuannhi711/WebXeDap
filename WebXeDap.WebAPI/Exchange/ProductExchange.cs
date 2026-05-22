using WebXeDap.Application.Features.Catalog.DTOs;

namespace WebXeDap.WebAPI.Exchange;

public record PagedProductResponse(int Total, int Page, int Size, List<SimpleProductResponse> Data);
