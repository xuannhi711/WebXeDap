using System.IO;
using WebXeDap.Infrastructure.Services;

namespace WebXeDap.Infrastructure.UnitTests.Storage;

[Trait("Category", "Unit")]
public sealed class DiskFileStorageTests
{
	[Fact]
	public async Task SaveAndDeleteFile_Works()
	{
		var storage = new DiskFileStorage();
		var data = new byte[] { 1, 2, 3, 4 };
		await using var ms = new MemoryStream(data);
		var key = await storage.SaveFileAsync(ms, "test.bin", "application/octet-stream");
		Assert.NotNull(key);
		Assert.StartsWith("/uploads/", key);

		// file exists on disk
		var fileName = key.TrimStart('/');
		var path = Path.Combine(
			Directory.GetCurrentDirectory(),
			"wwwroot",
			fileName.Replace('/', Path.DirectorySeparatorChar)
		);
		Assert.True(File.Exists(path));

		await storage.DeleteFileAsync(key);
		Assert.False(File.Exists(path));
	}

	[Fact]
	public async Task DeleteFileAsync_IgnoresMissingOrEmptyKeys()
	{
		var storage = new DiskFileStorage();

		await storage.DeleteFileAsync(string.Empty);
		await storage.DeleteFileAsync("/uploads/does-not-exist.bin");

		Assert.True(true);
	}
}
