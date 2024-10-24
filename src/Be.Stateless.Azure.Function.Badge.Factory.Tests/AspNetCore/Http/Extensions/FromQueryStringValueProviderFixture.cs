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

using System.Collections.Generic;
using System.Text.Json;
using Be.Stateless.Azure.Function.Badge.Factory.Model;
using FluentAssertions;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Primitives;
using Xunit;

namespace Be.Stateless.Azure.AspNetCore.Http.Extensions;

public class QueryCollectionExtensionsFixture
{
	[Fact]
	public void AsJsonStream()
	{
		var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
		var queryCollection = new QueryCollection(
			new Dictionary<string, StringValues> {
				{ "color", new StringValues("blue") },
				{ "label", new StringValues("my-label") },
				{ "logo", new StringValues("apache") },
				{ "style", new StringValues("flat") }
			});
		JsonSerializer.Deserialize<Skin>(queryCollection.AsJsonStream(), options).Should().BeEquivalentTo(
			new Skin {
				Color = "blue",
				Label = "my-label",
				Logo = "apache",
				Style = "flat"
			});
	}

	[Fact]
	public void AsJsonStreamWithNullValues()
	{
		var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
		var queryCollection = new QueryCollection(
			new Dictionary<string, StringValues> {
				{ "color", new StringValues("blue") },
				{ "label", new StringValues((string) null) },
				{ "logo", new StringValues("apache") },
				{ "style", new StringValues("flat") }
			});
		JsonSerializer.Deserialize<Skin>(queryCollection.AsJsonStream(), options).Should().BeEquivalentTo(
			new Skin {
				Color = "blue",
				Label = null,
				Logo = "apache",
				Style = "flat"
			});
	}

	[Fact]
	public void AsJsonStreamWithoutProperty()
	{
		var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
		var queryCollection = new QueryCollection(
			new Dictionary<string, StringValues> {
				{ "color", new StringValues("blue") },
				{ "logo", new StringValues("apache") },
				{ "style", new StringValues("flat") }
			});
		JsonSerializer.Deserialize<Skin>(queryCollection.AsJsonStream(), options).Should().BeEquivalentTo(
			new Skin {
				Color = "blue",
				Label = null,
				Logo = "apache",
				Style = "flat"
			});
	}
}
