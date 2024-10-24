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

using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Be.Stateless.Azure.Function.Badge.Factory.Model.Repository;

public class BadgeService
{
	public BadgeService(IHttpClientFactory httpClientFactory, ILoggerFactory loggerFactory)
	{
		_client = httpClientFactory.CreateClient(nameof(BadgeService));
		_logger = loggerFactory.CreateLogger(nameof(BadgeService));
	}

	public async Task<Badge> GetBadgeAsync(BuildArtifactResource buildArtifactResource, Skin skin)
	{
		// TODO figure out a better label or maybe there is no label
		var badge = new Badge(skin.Label ?? buildArtifactResource.LastModified.ToShortDateString(), buildArtifactResource.Version, skin.Color, skin.Style, skin.Logo);
		if (_logger.IsEnabled(LogLevel.Debug)) _logger.LogDebug("Get badge image from uri '{uri}'.", badge.Uri);
		badge.Stream = await _client.GetStreamAsync(badge.Uri);
		return badge;
	}

	public async Task<Badge> GetBadgeAsync(PackageArtifact packageArtifact, Skin skin)
	{
		var badge = new Badge(skin.Label ?? packageArtifact.Name, packageArtifact.LatestVersion.Version, skin.Color, skin.Style, skin.Logo);
		if (_logger.IsEnabled(LogLevel.Debug)) _logger.LogDebug("Get badge image from uri '{uri}'.", badge.Uri);
		badge.Stream = await _client.GetStreamAsync(badge.Uri);
		return badge;
	}

	private readonly HttpClient _client;
	private readonly ILogger _logger;
}
