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
using System.Text.Json;
using System.Threading.Tasks;
using Be.Stateless.Azure.AspNetCore.Mvc;
using Be.Stateless.Azure.Function.Badge.Factory.Model;
using Be.Stateless.Azure.Function.Badge.Factory.Model.Repository;
using Be.Stateless.Azure.WebJobs.Description;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace Be.Stateless.Azure.Function.Badge.Factory;

/// <summary>A C# Azure Function app to display version badges for build artifacts produced by public Azure Pipelines.</summary>
/// <example>
///    <list type="bullet">
///       <item>
///          <![CDATA[https://badge-factory.azurewebsites.net/artifact/build/icraftsoftware/be.stateless/BizTalk.Factory.Runtime]]>
///       </item> <item>
///          <![CDATA[https://badge-factory.azurewebsites.net/artifact/build/icraftsoftware/be.stateless/BizTalk.Factory.Runtime%20Continuous%20Integration/BizTalkPackages/BizTalk.Factory.Runtime.Deployment.zip]]>
///       </item> <item>
///          <![CDATA[https://badge-factory.azurewebsites.net/artifact/build/icraftsoftware/be.stateless/BizTalk.Factory.Runtime%20Continuous%20Integration/BizTalkPackages/BizTalk.Factory.Runtime.Deployment.zip?color=red&label=Azure%20Artifact&logo=powershell&style=plastic]]>
///       </item> <item>
///          <![CDATA[http://localhost:7071/artifact/build/icraftsoftware/be.stateless/BizTalk.Factory.Runtime]]>
///       </item>
///    </list>
/// </example>
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "Azure Function.")]
[SuppressMessage("ReSharper", "UnusedType.Global", Justification = "Azure Function.")]
public class BuildArtifactBadgeFactoryFunction
{
	public BuildArtifactBadgeFactoryFunction(BuildFeed buildFeed, BadgeService badgeService)
	{
		_buildFeed = buildFeed;
		_badgeService = badgeService;
	}

	[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Azure Function.")]
	[FunctionName("artifact/build")]
	public async Task<IActionResult> Run(
		[HttpTrigger(AuthorizationLevel.Anonymous, nameof(HttpMethods.Get), Route = "artifact/build/{organization}/{project}/{pipeline}/{name}/{file}")] //
		[FromRoute]
		BuildArtifactSpecification buildArtifactSpecification,
		[FromQueryString] // ?color={color}&label={label}&logo={logo}&style={style}
		Skin skin,
		ILogger logger)
	{
		if (logger.IsEnabled(LogLevel.Debug))
			logger.LogDebug(
				"{function} is manufacturing a badge for build artifact {artifact} with skin {skin}.",
				nameof(BuildArtifactBadgeFactoryFunction),
				JsonSerializer.Serialize(buildArtifactSpecification),
				JsonSerializer.Serialize(skin));

		var buildArtifactResource = await _buildFeed.GetBuildArtifactResourceAsync(buildArtifactSpecification);

		// TODO ?? redirect to img.shields.io instead ?? but what about caching and workload for function ??
		var badge = await _badgeService.GetBadgeAsync(buildArtifactResource, skin);

		return new CacheStreamResult(badge.Stream, badge.ContentType) {
			EntityTag = badge.EntityTag,
			LastModified = buildArtifactResource.LastModified,
			MaxAge = TimeSpan.FromMinutes(10)
		};
	}

	private readonly BadgeService _badgeService;
	private readonly BuildFeed _buildFeed;
}
