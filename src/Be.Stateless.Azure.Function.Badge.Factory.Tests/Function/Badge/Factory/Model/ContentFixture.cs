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

using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit;
using static FluentAssertions.FluentActions;

namespace Be.Stateless.Azure.Function.Badge.Factory.Model
{
	public class ContentFixture
	{
		[SuppressMessage("ReSharper", "StringLiteralTypo")]
		[Fact]
		public void DeserializeFromJson()
		{
			const string json = @"{
  ""count"": 1,
  ""value"": [
    {
      ""id"": ""7ea79051-7144-4302-b505-fa2e87fec14e"",
      ""normalizedName"": ""be.stateless.biztalk.dummies"",
      ""name"": ""Be.Stateless.BizTalk.Dummies"",
      ""protocolType"": ""NuGet"",
      ""url"": ""https://feeds.dev.azure.com/icraftsoftware/568ac931-8b86-4fbe-b160-b717abb6a957/_apis/Packaging/Feeds/4d312fc6-3c84-4aa3-9b2d-287dc408ecf9/Packages/7ea79051-7144-4302-b505-fa2e87fec14e"",
      ""versions"": [
        {
          ""id"": ""e22b869f-4238-436d-901c-5da13e3d7691"",
          ""normalizedVersion"": ""2.1.22025.26603"",
          ""version"": ""2.1.22025.26603"",
          ""isLatest"": true,
          ""isListed"": true,
          ""storageId"": ""10341E6558251F00D0EC211D818B2177AE1EE1CF3B857547A4B839C1FCAD388E00"",
          ""views"": [
            {
              ""id"": ""469e5431-55ab-432a-b001-0f06acc19be7"",
              ""name"": ""Local"",
              ""url"": null,
              ""type"": ""implicit""
            }
          ],
          ""publishDate"": ""2022-01-25T09:22:49.2596351Z""
        }
      ],
      ""_links"": {
        ""self"": {
          ""href"": ""https://feeds.dev.azure.com/icraftsoftware/568ac931-8b86-4fbe-b160-b717abb6a957/_apis/Packaging/Feeds/4d312fc6-3c84-4aa3-9b2d-287dc408ecf9/Packages/7ea79051-7144-4302-b505-fa2e87fec14e""
        },
        ""feed"": {
          ""href"": ""https://feeds.dev.azure.com/icraftsoftware/568ac931-8b86-4fbe-b160-b717abb6a957/_apis/Packaging/Feeds/4d312fc6-3c84-4aa3-9b2d-287dc408ecf9""
        },
        ""versions"": {
          ""href"": ""https://feeds.dev.azure.com/icraftsoftware/568ac931-8b86-4fbe-b160-b717abb6a957/_apis/Packaging/Feeds/4d312fc6-3c84-4aa3-9b2d-287dc408ecf9/Packages/7ea79051-7144-4302-b505-fa2e87fec14e/Versions""
        }
      }
    }
  ]
}";
			Invoking(() => JsonConvert.DeserializeObject<Content<Package>>(json)).Should().NotThrow();
		}
	}
}
