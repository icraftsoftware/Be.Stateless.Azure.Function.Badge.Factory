#region Copyright & License

// Copyright © 2012 - 2022 François Chabot
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

using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Be.Stateless.Azure.Function.Badge.Factory.Model.Repository;

public class BadgeService
{
	public BadgeService(IHttpClientFactory httpClientFactory, ILoggerFactory loggerFactory)
	{
		_client = httpClientFactory.CreateClient(nameof(PackageFeed));
		_logger = loggerFactory.CreateLogger(nameof(PackageFeed));
	}

	public async Task<Badge> GetBadgeAsync(Package package, Skin skin)
	{
		var badge = new Badge(skin.Label ?? package.Name, package.LatestVersion.Version, skin.Color, skin.Style, skin.Logo);
		_logger.LogInformation("Get badge image stream from {0}", badge.Uri);
		var response = await _client.GetAsync(badge.Uri);
		badge.Stream = await response.Content.ReadAsStreamAsync();
		return badge;
	}

	private readonly HttpClient _client;
	private readonly ILogger _logger;
}
