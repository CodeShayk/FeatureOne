# FeatureOne 
[![NuGet version](https://badge.fury.io/nu/FeatureOne.svg)](https://badge.fury.io/nu/FeatureOne) [![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://github.com/NinjaRocks/FeatureOne/blob/master/License.md) [![CI](https://github.com/NinjaRocks/FeatureOne/actions/workflows/CI-Build.yml/badge.svg)](https://github.com/NinjaRocks/FeatureOne/actions/workflows/CI-Build.yml) [![GitHub Release](https://img.shields.io/github/v/release/ninjarocks/FeatureOne?logo=github&sort=semver)](https://github.com/ninjarocks/FeatureOne/releases/latest)
[![CodeQL](https://github.com/NinjaRocks/FeatureOne/actions/workflows/codeql.yml/badge.svg)](https://github.com/NinjaRocks/FeatureOne/actions/workflows/codeql.yml) [![.Net Stardard](https://img.shields.io/badge/.Net%20Standard-6.0-blue)](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)

.Net Library to implement feature toggles.
--

### What is a feature toggle?
> Feature toggle is a mechanism that allows code to be turned “on” or “off” remotely without the need for a deploy. Feature toggles are commonly used in applications to gradually roll out new features, allowing teams to test changes on a small subset of users before releasing them to everyone.

### How feature toggles work
> Feature toggle is typically a logical check added to codebase to execute or ignore certain functionality in context based on evaluated status of the toggle at runitme.
>
> In code, the functionality to be released is wrapped so that it can be controlled by the status of a feature toggle. If the status of the feature toggle is “on”, then the wrapped functionality is executed. If the status of the feature toggle is “off”, then the wrapped functionality is skipped.  The statuses of each feature is provided by a store provider external to the application.

### The benefits of feature toggles
> The primary benefit of feature flagging is that it mitigates the risks associated with releasing changes to an application. Whether it be a new feature release or a small refactor, there is always the inherent risk of releasing new regressions. To mitigate this, changes to an application can be placed behind feature toggles, allowing them to be turned “on” or “off” in the event of an emergency.

How to use FeatureOne
--
### Step 1.
In order to release a new functionality or feature - say eg. Dashboard Widget.
Add logical check in codebase to wrap the functionality under a `feature toggle`.
> the logical check evaluates status of the toggle configured for the feature in store at runtime.

```
 var featureName = "dashboard_widget"; // Name of functionality or feature to toggle.
 if(Features.IsEnable(featureName, claimsPrincipal){
	showDashboardWidget();
}
```


### Step 2.
Add `toggle` to the store (ie. a store in database or file or other medium) in order to conditionally enable/disable the feature. 
A toggle constitutes a collection of `conditions` that evaluate separately when the toggle is run. You can additionally specify an `operator` on the toggle to determine the overall success to include success of `any` constituent condition or success of `all` consituent conditions.
> Toggles run at runtime based on consitituent conditions that evaluate separately against user claims (generally logged in user principal).


There are two types of conditions that can be used out of box for the toggles. 

#### i. Simple Condition
`Simple` condition allows simple toggle to enable or disable of the feature. User claims are not taken into account for this condition.

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
#### ii. Regex Condition
`Regex` condition allows evaluating a user claim against a regex expression.

Below is the serialized representation of feature toggle with regex conditions to enable or disable a given feature. 
```
 {
   "dashboard_widget":{  -- Feature name
	  "toggle"{     -- Toggle details for the feature
		  
		  "operator":"any|all", -- evaluate overall toggle to true 
		                        -- when `any` condition is met or `all` conditions are met.
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

### Step 3.
Implement `IStoreProvider` interface to return all configured feature toggles from custom store.
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
        /// Key - dashboard_widget
        /// Value - Json string as
        /// {
	///   "operator":"all",
	///    "conditions":[{
	///	        "type":"Regex",
	///	        "claim":"email",
	///	        "expression":"*@gbk.com"
	///	 },
	///	 {
	///	        "type":"RegexCondition",
	///	        "claim":"user_role",
	///	        "expression":"^administrator$"
	///     }]
	/// }
	/// </remarks>
	/// <returns>KeyValuePair Array</returns>
        IEnumerable<KeyValuePair<string, string>> Get();
    }
```

### Step 4.
Example - IoC Container Registrations
```
  services.UseFeatureOne(new Configuration
  {
         // Optional logger implementation
	 Logger = new CustomLoggerImpl(), 

	 // Custom store provider implementation.
	 StoreProvider = new SQlStoreProviderImpl() 
  }
```
How to Extend FeatureOne
--

### Toggle Condition
You could add your own conditions by extending the `ICondtion` interface. The interface provides `evaluate()` method that returns a boolean as a result of evaluating logic against list of input claims.
```
    /// <summary>
    /// Interface to implement toggle condition.
    /// </summary>
    public interface ICondition
    {
        /// <summary>
        /// Implement method to evaulate toggle condition.
        /// </summary>
        /// <param name="claims">List of user claims; could be null</param>
        /// <returns></returns>
        bool Evaluate(IDictionary<string, string> claims);
    }
```
`Please Note` The condition class should only include primitive data type properties.
Example below
```
   public class TimeCondition : ICondition
   {
        // toggle to show feature after given hour during the day.
	public int Hour {get; set;} = 12; // Primitive int property.

	bool Evaluate(IDictionary<string, string> claims)
	{
		return (DateTime.Now.Hour > 12);	 
	}
   }

  -- Example toggle to allow non-admin users access to a feature only after 14 hrs.

   {
	operator":"any",
	  "conditions":[{
	     "type":"Regex",
	     "claim":"user_role",
	     "expression":"^administrator$"
	   },
	   {	
	     "type":"Time",
	     "Hour":14
	 }]
   }

```
### Store Provider
To use FeatureOne, you need to provide implementation of `Store Provider` to get all the feature toggles from storage medium of choice. Implement `IStoreProvider` interface to return the key-value pairs with feature name and json string toggle. 

Below is an example of dummy provider implementation. 
> A production implementation should be a provider with `API` or `SQL` or `File system` backend. Ideally, you may also use caching of feature toggles in the provider implementation to optimise calls to storage medium.
```
public class CustomStoreProvider : IStoreProvider
    {
        public IEnumerable<KeyValuePair<string, string>> Get()
        {
            return new[] {
                    new KeyValuePair<string, string>("feature-01", "{\"conditions\":[{\"type\":\"Simple\",\"isEnabled\": true}]}"),
                    new KeyValuePair<string, string>("feature-02", "{\"operator\":\"all\",\"conditions\":[{\"type\":\"Simple\",\"isEnabled\": false}, {\"type\":\"RegexCondition\",\"claim\":\"email\",\"expression\":\"*@gbk.com\"}]}")
                };
        }
    }

```
### Logger
You could optionally provide an implementation of logger by wrapping your favourite logging libaray under `IFeatureLogger` interface. Please see the interface definition below. This implementation is optional and when no logger is provided FeatureOne will not log any errors.
```
    /// <summary>
    /// Interface to implement custom logger.
    /// </summary>
    public interface IFeatureLogger
    {
        /// <summary>
        /// Implement the debug log method
        /// </summary>
        /// <param name="message">log message</param>
        void Debug(string message);

        /// <summary>
        /// Implement the error log method
        /// </summary>
        /// <param name="message">log message</param>
        void Error(string message);

        /// <summary>
        /// Implement the info log method
        /// </summary>
        /// <param name="message">log message</param>
        void Info(string message);

        /// <summary>
        /// Implement the warn log method
        /// </summary>
        /// <param name="message">log message</param>
        void Warn(string message);
    }
```