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

using System;
using System.Diagnostics.CodeAnalysis;
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
using Newtonsoft.Json;

namespace Be.Stateless.Azure.Function.Badge.Factory;

/// <summary>
/// A C# Azure Function app to display version badges for NuGet packages hosted on a public Azure Artifacts NuGet Feed.
/// </summary>
/// <example>
/// <list type="bullet">
/// <item>
/// <![CDATA[https://badge-factory.azurewebsites.net/package/icraftsoftware/be.stateless/BizTalk.Factory.Preview/Psx]]>
/// </item>
/// <item>
/// <![CDATA[
/// https://badge-factory.azurewebsites.net/package/icraftsoftware/be.stateless/BizTalk.Factory.Preview/Psx?color=red&label=Azure%20Artifact&logo=powershell&style=plastic
/// ]]>
/// </item>
/// <item>
/// <![CDATA[http://localhost:7071/package/icraftsoftware/be.stateless/BizTalk.Factory.Preview/Psx]]>
/// </item>
/// </list>
/// </example>
/// <seealso href="https://github.com/azurevoodoo/AzureArtifactsPublicNuGetFeedBadge#azure-artifacts-public-nuget-feed-skin">Azure Artifacts Public NuGet Feed Skin</seealso>
/// <seealso href="https://azpkgsshield.azurevoodoo.net/icraftsoftware/be.stateless/BizTalk.Factory.Preview/Psx"/>
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "Azure Function.")]
[SuppressMessage("ReSharper", "UnusedType.Global", Justification = "Azure Function.")]
public class PackageBadgeFactoryFunction
{
	public PackageBadgeFactoryFunction(PackageFeed packageFeed, BadgeService badgeService)
	{
		_packageFeed = packageFeed;
		_badgeService = badgeService;
	}

	[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Azure Function.")]
	[FunctionName("package")]
	public async Task<IActionResult> Run(
		[HttpTrigger(AuthorizationLevel.Anonymous, nameof(HttpMethods.Get), Route = "package/{organization}/{project}/{feed}/{name}")] //
		[FromRoute]
		Artifact artifact,
		[FromQueryString] // ?color={color}&label={label}&logo={logo}&style={style}
		Skin skin,
		ILogger logger)
	{
		if (logger.IsEnabled(LogLevel.Debug))
			logger.LogDebug(
				"PackageBadgeFactoryFunction is manufacturing a badge for artifact {artifact} with skin {skin}.",
				JsonConvert.SerializeObject(artifact),
				JsonConvert.SerializeObject(skin));

		var package = await _packageFeed.GetPackageAsync(artifact);

		// TODO ?? redirect to img.shields.io instead ?? but what about caching and workload for function ??
		var badge = await _badgeService.GetBadgeAsync(package, skin);

		return new CacheStreamResult(badge.Stream, badge.ContentType) {
			EntityTag = badge.EntityTag,
			LastModified = package.LatestVersion.PublishDate,
			MaxAge = TimeSpan.FromMinutes(10)
		};
	}

	private readonly BadgeService _badgeService;
	private readonly PackageFeed _packageFeed;
}
