using System.ComponentModel.DataAnnotations;

namespace WebXeDap.Domain.Models;

public class Config
{
	[Key]
	public required string Key { get; set; }
	public required string Value { get; set; }
}
