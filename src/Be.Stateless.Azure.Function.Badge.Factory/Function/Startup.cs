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

using Be.Stateless.Azure.Function.Badge.Factory.Model.Repository;
using Be.Stateless.Azure.WebJobs.Host.Config;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.DependencyInjection;

namespace Be.Stateless.Azure.Function;

public class Startup : FunctionsStartup
{
	#region Base Class Member Overrides

	public override void Configure(IFunctionsHostBuilder builder)
	{
		builder.Services.AddHttpClient();
		builder.Services.AddLogging();
		builder.Services.AddScoped<BadgeService>();
		builder.Services.AddScoped<BuildFeed>();
		builder.Services.AddScoped<PackageFeed>();

		// https://www.red-gate.com/simple-talk/blogs/custom-binding-azure-functions/
		var wbBuilder = builder.Services.AddWebJobs(null);
		wbBuilder.AddExtension<FromQueryStringBindingExtensionProvider>();
	}

	#endregion
}
