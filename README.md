# <img src="https://github.com/NinjaRocks/FeatureOne/blob/master/ninja-icon-16.png" alt="ninja" style="width:30px;"/> FeatureOne v1.0.7
[![NuGet version](https://badge.fury.io/nu/FeatureOne.svg)](https://badge.fury.io/nu/FeatureOne) [![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://github.com/NinjaRocks/FeatureOne/blob/master/License.md) [![CI](https://github.com/NinjaRocks/FeatureOne/actions/workflows/CI-Build.yml/badge.svg)](https://github.com/NinjaRocks/FeatureOne/actions/workflows/CI-Build.yml) [![GitHub Release](https://img.shields.io/github/v/release/ninjarocks/FeatureOne?logo=github&sort=semver)](https://github.com/ninjarocks/FeatureOne/releases/latest)
[![CodeQL](https://github.com/NinjaRocks/FeatureOne/actions/workflows/codeql.yml/badge.svg)](https://github.com/NinjaRocks/FeatureOne/actions/workflows/codeql.yml) [![.Net Stardard](https://img.shields.io/badge/.Net%20Standard-2.1-blue)](https://dotnet.microsoft.com/en-us/download/dotnet/2.1)

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
### Step 1. Add Feature IsEnabled Check in Code.
In order to release a new functionality or feature - say eg. Dashboard Widget.
Add logical check in codebase to wrap the functionality under a `feature toggle`.
> the logical check evaluates status of the toggle configured for the feature in store at runtime.

```
 var featureName = "dashboard_widget"; // Name of functionality or feature to toggle.
 if(Features.Current.IsEnable(featureName, claimsPrincipal){ // See other IsEnable() overloads
	showDashboardWidget();
}
```


### Step 2. Add Feature Toggle Definition to Storage
Add a `toggle` definition to storage ie. a store in database or file or other storage medium. 
A toggle constitutes a collection of `conditions` that evaluate separately when the toggle is run. You can additionally specify an `operator` in the toggle definition to determine the overall success to include success of `any` constituent condition or success of `all` consituent conditions.
> Toggles run at runtime based on consitituent conditions that evaluate separately against user claims (generally logged in user principal).

JSON Syntax for Feature Toggle is below
```
{
  "feature_name":{ -- Feature name
        "toggle":{ -- Toggle details for the feature 

            "operator":"any|all", -- Evaluate overall toggle to true 
		                  -- when `any` condition is met or `all` conditions are met.
           
            "conditions":[{ -- collection of conditions
                "type":"simple|regex" -- type of condition
                 
                 .... other type specific properties, See below for details.                  
            }]
        }
  }
}
```
### Condition Types 
There are two types of toggle conditions that can be used out of box. 

#### i. Simple Condition
`Simple` condition allows toggle with simple enable or disable of the given feature. User claims are not taken into account for this condition.

Below is the serialized representation of toggle with simple condition. 
```
{
  "dashboard_widget":{   
	  "toggle"{       
	     "conditions":[{
		   "type":"Simple",       -- Simple Condition.
		   "isEnabled":true|false --  Enabled or disable the feature.
	      }]		  
	  } 		  
  }
}
```
#### ii. Regex Condition
`Regex` condition allows evaluating a regex expression against specified user claim to enable a given feature.

Below is the serialized representation of toggle with regex condition. 
```
 {
   "dashboard_widget":{  
	  "toggle"{    
		                          
		  "conditions":[{
			  "type":"Regex",  -- Regex Condition
			  "claim":"email", -- Claim 'email' to be used for evaluation.
			  "expression":"*@gbk.com" -- Regex expression to be used for evaulation.
		   }]		  
	  }	  
   }
 }
```

### Step 3. Provide Storage Provider Implementation.
To use FeatureOne, you need to provide implementation of `Storage Provider` to get all the feature toggles from storage medium of choice. 
Implement `IStorageProvider` interface to get configured feature toggles from storage.
The interface has `Get()` method that returns a collection of `KeyValuePair<string, string>`  with `key` mapping to `featureName` and `value` mapping to json string representation of the `toggle`
```
    /// <summary>
    /// Interface to implement storage provider.
    /// </summary>
    public interface IStorageProvider
    {
        /// <summary>
        ///  Implement this method to get all feature toggles from storage.
        /// </summary>
        /// <remarks>
        /// Example:
        ///  Key - "dashboard_widget"
        ///  Value - "{\"conditions\":[{\"type\":\"Simple\",\"isEnabled\": true}]}"
	/// </remarks>
	/// <returns>KeyValuePair Array</returns>
        IEnumerable<KeyValuePair<string, string>> Get();
    }
```
Below is an example of dummy provider implementation. 
> A production provider should be an implementation with `API` , `SQL` or `File system` storage backend. Ideally, you may also want to use `caching` of feature toggles in the production implementation to optimise calls to the storage medium.
```
public class CustomStoreProvider : IStorageProvider
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
### Step 4. Bootstrap Initialialization
In bootstrap code, initialize the `Features` class with dependencies as shown below.

i. With `storage provider` implementation. 
```
   var storageProvider = new SQlStorageProviderImpl();
   Features.Initialize(() => new Features(new FeatureStore(storageProvider)));
```

ii. With `storage provider` and `logger` implementations. 
```
   var logger = new CustomLoggerImpl();
   var storageProvider = new SQlStorageProviderImpl();

   Features.Initialize(() => new Features(new FeatureStore(storageProvider, logger), logger));
```

iii.  With `storage provider`, `logger` and custom `toggle deserializer` implementations.
```
   var logger = new CustomLoggerImpl();
   var storageProvider = new SQlStorageProviderImpl();
   var toggleDeserializer = new CustomToggleDeserializerImpl();

   Features.Initialize(() => new Features(new FeatureStore(storageProvider, logger, toggleDeserializer), logger));
```

How to Extend FeatureOne
--

### i. Toggle Condition
You could implement your own condition by extending the `ICondition` interface. 
The interface provides `evaluate()` method that returns a boolean result of evaluating logic against list of input claims.
```
    /// <summary>
    /// Interface to implement toggle condition.
    /// </summary>
    public interface ICondition
    {
        /// <summary>
        /// Implement method to evaulate toggle condition.
        /// </summary>
        /// <param name="claims">List of user claims; could be empty</param>
        /// <returns></returns>
        bool Evaluate(IDictionary<string, string> claims);
    }
```
`Please Note` The condition class should only include `primitive` data type `properties` for default deserialization. If you need to implement a much complex toggle condition with non-primitive properties then also provide custom implementation of `IToggleDeserializer` to support its deserialization along with other conditions.

Example below shows sample implementation of a custom condition.

```
   // toggle condition to show feature after given hour during the day.
   public class TimeCondition : ICondition
   {
        public int Hour {get; set;} = 12; // Primitive int property.

	bool Evaluate(IDictionary<string, string> claims)
	{
		return (DateTime.Now.Hour > Hour);	 
	}
   }
```
 Example usage of above condition in toggle to allow non-admin users access to a feature only after 14 hrs.
 ```
  {
        "operator":"any", -- Any below condition evaluation to true should succeed the toggle.
        "conditions":[{             
               "type":"Time", -- Time condition to all access to all after 14hrs
               "Hour":14
        },
        {   
               "type":"Regex", -- Regex to allow admin access
               "claim":"user_role",
               "expression":"^administrator$"
        }]
  }

```

### ii. Logger
You could optionally provide an implementation of a logger by wrapping your favourite logging libaray under `IFeatureLogger` interface. 
Please see the interface definition below.
>This implementation is optional and when no logger is provided FeatureOne will not log any errors, warnings or information.
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
Credits
--
Thank you for reading. Please fork, explore, contribute and report. Happy Coding !! :)
