using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using WebXeDap.Application.Contracts.Services;

namespace WebXeDap.Infrastructure.Services;

public class DiskFileStorage : IFileStorage
{
	private readonly string uploadsFolder;

	public DiskFileStorage()
	{
		uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
		Directory.CreateDirectory(uploadsFolder);
	}

	public async Task<string> SaveFileAsync(
		Stream stream,
		string fileName,
		string? contentType = null,
		CancellationToken cancellationToken = default
	)
	{
		var ext = Path.GetExtension(fileName) ?? string.Empty;
		var safeName = Guid.NewGuid().ToString("N") + ext;
		var outPath = Path.Combine(uploadsFolder, safeName);
		await using var fs = File.Create(outPath);
		await stream.CopyToAsync(fs, cancellationToken);
		// return a path relative to host (frontend will prefix API host)
		return $"/uploads/{safeName}";
	}

	public Task DeleteFileAsync(string key, CancellationToken cancellationToken = default)
	{
		if (string.IsNullOrWhiteSpace(key))
			return Task.CompletedTask;
		// key expected like "/uploads/{file}" or "uploads/{file}"
		var fileName = key.TrimStart('/');
		var path = Path.Combine(
			Directory.GetCurrentDirectory(),
			"wwwroot",
			fileName.Replace('/', Path.DirectorySeparatorChar)
		);
		if (File.Exists(path))
			File.Delete(path);
		return Task.CompletedTask;
	}
}
