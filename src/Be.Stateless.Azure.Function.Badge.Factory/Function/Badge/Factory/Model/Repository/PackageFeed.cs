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
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Be.Stateless.Azure.Function.Badge.Factory.Model.Repository;

public class PackageFeed
{
	public PackageFeed(IHttpClientFactory httpClientFactory, ILoggerFactory loggerFactory)
	{
		_client = httpClientFactory.CreateClient(nameof(PackageFeed));
		_logger = loggerFactory.CreateLogger(nameof(PackageFeed));
	}

	public async Task<PackageArtifact> GetPackageArtifactAsync(PackageArtifactSpecification packageArtifactSpecification)
	{
		var uri = GetPackageFeedUri(packageArtifactSpecification);
		if (_logger.IsEnabled(LogLevel.Debug)) _logger.LogDebug("Get package details from feed '{uri}'.", uri);
		var packageArtifactResponse = await _client.GetFromJsonAsync<Content<PackageArtifact>>(uri);
		return packageArtifactResponse!.Value.Single(p => p.Name.Equals(packageArtifactSpecification.Name, StringComparison.OrdinalIgnoreCase));
	}

	private Uri GetPackageFeedUri(PackageArtifactSpecification packageArtifactSpecification)
	{
		// see https://docs.microsoft.com/en-us/rest/api/azure/devops/artifacts/artifact-details/get-packages
		var builder = new UriBuilder(Uri.UriSchemeHttps, "feeds.dev.azure.com") {
			Path = $"{packageArtifactSpecification.Organization}/{packageArtifactSpecification.Project}/_apis/packaging/Feeds/{packageArtifactSpecification.Feed}/packages",
			Query = $"packageNameQuery={packageArtifactSpecification.Name}"
		};
		return builder.Uri;
	}

	private readonly HttpClient _client;
	private readonly ILogger _logger;
}
