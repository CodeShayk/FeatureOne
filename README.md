# FeatureOne 
[![NuGet version](https://badge.fury.io/nu/FeatureOne.svg)](https://badge.fury.io/nu/FeatureOne) [![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://github.com/NinjaRocks/FeatureOne/blob/master/License.md) [![CI](https://github.com/NinjaRocks/FeatureOne/actions/workflows/CI-Build.yml/badge.svg)](https://github.com/NinjaRocks/FeatureOne/actions/workflows/CI-Build.yml) [![GitHub Release](https://img.shields.io/github/v/release/ninjarocks/FeatureOne?logo=github&sort=semver)](https://github.com/ninjarocks/FeatureOne/releases/latest)
[![CodeQL](https://github.com/NinjaRocks/FeatureOne/actions/workflows/codeql.yml/badge.svg)](https://github.com/NinjaRocks/FeatureOne/actions/workflows/codeql.yml) [![.Net Stardard](https://img.shields.io/badge/.Net%20Standard-6.0-blue)](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)

FeatureOne is a .Net Library to implement feature toggles.
--

### What is a feature toggle?
Feature toggle is a mechanism that allows code to be turned “on” or “off” remotely without the need for a deploy. Feature toggles are commonly used in applications to gradually roll out new features, allowing teams to test changes on a small subset of users before releasing them to everyone.

### How feature toggles work
Feature toggle is typically a logical check added to codebase to execute or ignore certain functionality in context based on evaluated status of the toggle at runitme.

In code, the functionality to be released is wrapped so that it can be controlled by the status of a feature toggle. If the status of the feature toggle is “on”, then the wrapped functionality is executed. If the status of the feature toggle is “off”, then the wrapped functionality is skipped.  The statuses of each feature is provided by a store provider external to the application.

### The benefits of feature toggles
The primary benefit of feature flagging is that it mitigates the risks associated with releasing changes to an application. Whether it be a new feature release or a small refactor, there is always the inherent risk of releasing new regressions. To mitigate this, changes to an application can be placed behind feature toggles, allowing them to be turned “on” or “off” in the event of an emergency.

How to use FeatureOne
--
Step 1. In order to release a new functionality or feature - say eg. Dashboard Widget.
Add logical check in codebase to wrap the functionality under a `feature toggle`.
> the logical check is evaluated at runtime against the status of toggle from store provider.

```
var featureName = "dashboard_widget"; // Name of functionality or feature to toggle.
if(Features.IsEnable(featureName, claimsPrincipal){
	showDashboardWidget();
}
```

Step 2. Add toggles to the store (database or file or other medium) in order to conditionally enable/disable the feature. A `toggle` can consitute a collection of `conditions` that evaluate separately when the toggle runs. You can additionally specify an `operator` on the toggle to determine the overall success to include success of `any` constituent condition or success of `all` consituent conditions.
> Toggles run at runtime based on consitituent conditions that evaluate seperaely against user claims (generally logged in user principal).


There are two types of conditions that can be used out of box for the toggles. 

- `Simple` condition - allows simple toggle to enable or disable of the feature
Below is the serialized representation of feature toggle with simple condition to enable or disable a given feature. 
```
{
  "dashboard_widget":{   -- Feature name
	  "toggle"{      -- Toggle details for the feature   
	     "conditions":[{
		"type":"Simple",         
		"isEnabled":true|false --  enabled or disable the feature.
	      }]		  
	  } 		  
  }
}
```

- `Regex` condition - allows evaluating a user claim against a regex expression.
Below is the serialized representation of feature toggle with regex conditions to enable or disable a given feature. 
```
{
  "dashboard_widget":{  -- Feature name
	  "toggle"{     -- Toggle details for the feature
		  
		  "operator":"any|all", -- evalue overall toggle to true 
		                        -- when any condition is met or all conditions are met.
		  "conditions":[{
			  "type":"Regex", 
			  "claim":"email", -- email claim to be used for evaluation.
			  "expression":"*@gbk.com" -- Regex expression for evaulation.
		  },
		  {
			  "type":"Regex",         
			 "claim":"user_role", -- user_role claim to be used for evaluation.
			  expression":"*@gbk.com" -- Regex expression for evaulation.
		  }]		  
	  }	  
  }
}
```

Step 3. Implement `IStoreProvider` interface to return all configured feature toggles from custom store.
Return a collection of key-value pairs with key mapping to `featureName` and value mapping to string representation of json serialized `toggle`
```
    /// <summary>
    /// Interface to implement feature store provider.
    /// </summary>
    public interface IStoreProvider
    {
        /// <summary>
        /// Implement this method to return all features from store provider.
        /// </summary>
        /// <remarks>
        /// Example:
        /// Key - Feature-01
        /// Value - Json string as
        /// {
	///   "operator":"all",
	///    "conditions":[{
	///	        "type":"Regex",
	///	        "claim":"email",
	///	        "expression":"*@gbk.com"
        ///     },
	///     {
	///	        "type":"RegexCondition",
	///	        "claim":"user_role",
	///	        "expression":"^administrator$"
	///     }]
	/// }
        /// </remarks>
        /// <returns></returns>
        IEnumerable<KeyValuePair<string, string>> Get();
    }
```

Step 4. Example - IoC Container Registrations
```
TBC ....
```








