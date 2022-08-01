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

namespace Be.Stateless.Azure.Function.Badge.Factory.Model;

/// <summary>
/// Build Details.
/// </summary>
/// <seealso href="https://docs.microsoft.com/en-us/rest/api/azure/devops/build/builds/get#build">Build</seealso>
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public class Build
{
	public string BuildNumber { get; init; }

	public string Id { get; init; }

	public DateTime LastChangedDate { get; init; }

	public string Url { get; init; }
}
