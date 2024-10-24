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
using Be.Stateless.Extensions;
using Microsoft.Extensions.Logging;

namespace Be.Stateless.Azure.Function.Badge.Factory.Model.Repository;

public class BuildFeed
{
	public BuildFeed(IHttpClientFactory httpClientFactory, ILoggerFactory loggerFactory)
	{
		_client = httpClientFactory.CreateClient(nameof(PackageFeed));
		_logger = loggerFactory.CreateLogger(nameof(PackageFeed));
	}

	public async Task<BuildArtifactResource> GetBuildArtifactResourceAsync(BuildArtifactSpecification buildArtifactSpecification)
	{
		var uri = GetBuildDefinitionUri(buildArtifactSpecification);
		if (_logger.IsEnabled(LogLevel.Debug)) _logger.LogDebug("Get build definition details from url '{uri}'.", uri);
		var buildDefinitionResponse = await _client.GetFromJsonAsync<Content<BuildDefinition>>(uri);
		var buildDefinition = buildDefinitionResponse!.Value.Single();

		uri = GetBuildUri(buildArtifactSpecification, buildDefinition.Id);
		if (_logger.IsEnabled(LogLevel.Debug)) _logger.LogDebug("Get build details from url '{uri}'.", uri);
		var buildResponse = await _client.GetFromJsonAsync<Content<Build>>(uri);
		var build = buildResponse!.Value.Single();

		uri = GetBuildArtifactUri(buildArtifactSpecification, build.Id);
		if (_logger.IsEnabled(LogLevel.Debug)) _logger.LogDebug("Get build artifact details from url '{uri}'.", uri);
		var buildArtifact = await _client.GetFromJsonAsync<BuildArtifact>(uri);
		var downloadUrl = buildArtifact!.Resources.Single().DownloadUrl;

		return new BuildArtifactResource {
			DownloadUrl = GetBuildArtifactResourceDownloadUri(downloadUrl.ToString(), buildArtifactSpecification.File),
			LastModified = build.LastChangedDate,
			Version = build.BuildNumber
		};
	}

	private Uri GetBuildDefinitionUri(BuildArtifactSpecification buildArtifactSpecification)
	{
		// see https://docs.microsoft.com/en-us/rest/api/azure/devops/build/definitions/get
		var builder = new UriBuilder(Uri.UriSchemeHttps, "dev.azure.com") {
			Path = $"{buildArtifactSpecification.Organization}/{buildArtifactSpecification.Project}/_apis/build/definitions",
			Query = $"name={buildArtifactSpecification.Pipeline}"
		};
		return builder.Uri;
	}

	private Uri GetBuildUri(BuildArtifactSpecification buildArtifactSpecification, string buildDefinitionId)
	{
		// see https://docs.microsoft.com/en-us/rest/api/azure/devops/build/builds/get
		var builder = new UriBuilder(Uri.UriSchemeHttps, "dev.azure.com") {
			Path = $"{buildArtifactSpecification.Organization}/{buildArtifactSpecification.Project}/_apis/build/builds",
			Query = $"definitions={buildDefinitionId}&statusFilter=completed&resultFilter=succeeded&maxBuildsPerDefinition=1"
		};
		return builder.Uri;
	}

	private Uri GetBuildArtifactUri(BuildArtifactSpecification buildArtifactSpecification, string buildId)
	{
		// see https://docs.microsoft.com/en-us/rest/api/azure/devops/build/artifacts/get-artifact
		var builder = new UriBuilder(Uri.UriSchemeHttps, "dev.azure.com") {
			Path = $"{buildArtifactSpecification.Organization}/{buildArtifactSpecification.Project}/_apis/build/builds/{buildId}/artifacts",
			Query = $"artifactName={buildArtifactSpecification.Name}"
		};
		return builder.Uri;
	}

	private Uri GetBuildArtifactResourceDownloadUri(string downloadUrl, string file)
	{
		var builder = new UriBuilder(downloadUrl);
		if (!file.IsNullOrEmpty()) builder.Query = $"format=file&subPath={file}";
		return builder.Uri;
	}

	private readonly HttpClient _client;
	private readonly ILogger _logger;
}
