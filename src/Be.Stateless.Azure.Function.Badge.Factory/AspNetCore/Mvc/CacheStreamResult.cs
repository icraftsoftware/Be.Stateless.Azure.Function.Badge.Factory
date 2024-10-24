#region Copyright & License

// Copyright © 2012 - 2024 François Chabot
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
// http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#endregion

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace Be.Stateless.Azure.AspNetCore.Mvc;

public class CacheStreamResult : IActionResult
{
	public CacheStreamResult(Stream stream, string contentType)
	{
		_fileResult = new FileStreamResult(stream, contentType);
	}

	#region IActionResult Members

	public Task ExecuteResultAsync(ActionContext context)
	{
		using var algorithm = SHA1.Create();
		var hash = algorithm.ComputeHash(Encoding.UTF8.GetBytes(EntityTag)).Aggregate(string.Empty, (k, t) => $"{k}{t:x2}");
		_fileResult.EntityTag = new EntityTagHeaderValue($"\"{hash}\"");
		context.HttpContext.Response.Headers.Add("Cache-Control", $"public, max-age={MaxAge.TotalSeconds}");
		return _fileResult.ExecuteResultAsync(context);
	}

	#endregion

	/// <summary>Gets the Content-Type header for the response.</summary>
	[SuppressMessage("ReSharper", "UnusedMember.Global")]
	public string ContentType => _fileResult.ContentType;

	/// <summary>Gets or sets the etag associated with the <see cref="Stream" />.</summary>
	public string EntityTag { get; init; }

	/// <summary>Gets or sets the last modified information associated with the <see cref="FileResult" />.</summary>
	[SuppressMessage("ReSharper", "UnusedMember.Global")]
	public DateTimeOffset? LastModified
	{
		get => _fileResult.LastModified;
		set => _fileResult.LastModified = value;
	}

	public TimeSpan MaxAge { get; init; } = TimeSpan.FromMinutes(10);

	private readonly FileResult _fileResult;
}
