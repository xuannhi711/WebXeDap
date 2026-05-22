using Riok.Mapperly.Abstractions;
using WebXeDap.Domain.Models;

namespace WebXeDap.WebAPI.Exchange;

public record UserProfileResp(int Id, string Email, string? Avatar, string? FullName);

public record UpdateUserProfileReq(string? FullName, string? Avatar);

[Mapper(AllowNullPropertyAssignment = false)]
public static partial class UserExchangeMapper
{
	[MapperRequiredMapping(RequiredMappingStrategy.Target)]
	public static partial UserProfileResp ToUserProfileResp(User user);

	[MapperRequiredMapping(RequiredMappingStrategy.Source)]
	public static partial void PatchUser(UpdateUserProfileReq req, [MappingTarget] User user);
}
