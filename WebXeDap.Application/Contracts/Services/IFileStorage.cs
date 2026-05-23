using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace WebXeDap.Application.Contracts.Services;

public interface IFileStorage
{
	Task<string> SaveFileAsync(
		Stream stream,
		string fileName,
		string? contentType = null,
		CancellationToken cancellationToken = default
	);

	Task DeleteFileAsync(string key, CancellationToken cancellationToken = default);
}
