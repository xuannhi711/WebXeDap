using Riok.Mapperly.Abstractions;
using WebXeDap.Application.Features.Payments.DTOs;
using WebXeDap.Domain.Models;

namespace WebXeDap.Application.Features.Payments.Mapper;

[Mapper(AllowNullPropertyAssignment = false)]
public partial class PaymentMapper
{
    [MapperRequiredMapping(RequiredMappingStrategy.Target)]
    public partial PaymentResponse ToPaymentResponse(Payment payment);
}
