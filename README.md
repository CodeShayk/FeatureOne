# <img src="https://github.com/NinjaRocks/FeatureOne/blob/master/ninja-icon-16.png" alt="ninja" style="width:30px;"/> FeatureOne v2.0.4
[![NuGet version](https://badge.fury.io/nu/FeatureOne.svg)](https://badge.fury.io/nu/FeatureOne) [![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://github.com/NinjaRocks/FeatureOne/blob/master/License.md) [![CI](https://github.com/NinjaRocks/FeatureOne/actions/workflows/CI-Build.yml/badge.svg)](https://github.com/NinjaRocks/FeatureOne/actions/workflows/CI-Build.yml) [![GitHub Release](https://img.shields.io/github/v/release/ninjarocks/FeatureOne?logo=github&sort=semver)](https://github.com/ninjarocks/FeatureOne/releases/latest)
[![CodeQL](https://github.com/NinjaRocks/FeatureOne/actions/workflows/codeql.yml/badge.svg)](https://github.com/NinjaRocks/FeatureOne/actions/workflows/codeql.yml) [![.Net Stardard](https://img.shields.io/badge/.Net%20Standard-2.1-blue)](https://dotnet.microsoft.com/en-us/download/dotnet/2.1)

.Net Library to implement feature toggles.
--
> #### Nuget Packages
> ---
> `FeatureOne` - Provides core funtionality to implement feature toggles with `no` backend storage provider. Needs package consumer to provide `IStorageProvider` implementation. Ideal for use case that requires custom storage backend. Please see below for more details.
>
> Backend Storage Providers
>>i. `FeatureOne.SQL` - Provides SQL storage provider for implementing feature toggles using `SQL` backend.
>>
>>ii. `FeatureOne.File` - Provides File storage provider for implementing feature toggles using `File System` backend.

## Concept
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
 if(Features.Current.IsEnable(featureName){ // See other IsEnable() overloads
	showDashboardWidget();
}
```


### Step 2. Add Feature Toggle Definition to Storage
Add a `toggle` definition to storage ie. a store in database or file or other storage medium. 
A toggle constitutes a collection of `conditions` that evaluate separately when the toggle is run. You can additionally specify an `operator` in the toggle definition to determine the overall success to include success of `any` constituent condition or success of `all` consituent conditions.
> Toggles run at runtime based on consitituent conditions that evaluate separately against user claims (generally logged in user principal).

Below is a serialized JSON representation of a Feature Toggle.
```
{
  "feature_name":{ -- Feature name
        "toggle":{ -- Toggle definition for the feature 

            "operator":"any|all", -- Logical Operator - any (OR) & all (AND)
                                  -- ie. Evaluate overall toggle to true when `any` condition is met or
                                  --     `all` conditions are met.
           
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
	  "toggle":{       
	     "conditions":[{
		   "type":"Simple",       -- Simple Condition.
		   "isEnabled":true|false --  Enabled or disable the feature.
	      }]		  
	  } 		  
  }
}
```
C# representation of a feature with simple toggle is
```
var feature = new Feature
{
  Name ="dashboard_widget",   // Feature Name
  Toggle = new Toggle         // Toggle definition
  {
    // Logical operator to be applied when evaluating consituent conditions.
    Operator = Operator.Any,  // Default is Any (Logical OR)
                              
    Conditions = new[]
    {
        // Simple condition that can be set to true/false for feature to be enabled/disabled.
        new SimpleCondition { IsEnabled = true }
    }
  }
}
```
#### ii. Regex Condition
`Regex` condition allows evaluating a regex expression against specified user claim value to enable a given feature.

Below is the serialized representation of toggle with regex condition. 
```
 {
   "dashboard_widget":{  
	  "toggle":{    
		                          
		  "conditions":[{
			  "type":"Regex",  -- Regex Condition
			  "claim":"email", -- Claim 'email' to be used for evaluation.
			  "expression":"*@gbk.com" -- Regex expression to be used for evaluation.
		   }]		  
	  }	  
   }
 }
```
C# representation of a feature with regex toggle is
```

var feature = new Feature
{
  Name ="dashboard_widget",   // Feature Name
  Toggle = new Toggle         // Toggle definition
  {
    Operator = Operator.Any,  
    Conditions = new[] 
    {
        // Regex condition that evalues role of user to be administrator to enable the feature.
        new RegexCondition { Claim = "role", Expression = "administrator" }
    }
  }
}
```

### Step 3. Implement Storage Provider.
To use FeatureOne, you need to provide implementation for `Storage Provider` to get all the feature toggles from storage medium of choice. 
Implement `IStorageProvider` interface to return feature toggles from storage.
The interface has `GetByName()` method that returns an array of `IFeature`
```
  /// <summary>
  /// Interface to implement storage provider.
  /// </summary>
  public interface IStorageProvider
  {
        /// <summary>
        /// Implement to get storage feature toggles by a given name.
        /// </summary>
        /// <returns>Array of Features</returns>
        IFeature[] GetByName(string name);
  }
```
A production storage provider should be an implementation with `API` , `SQL` or `File system` storage backend.

An implementation option is to store features as serialized json to backend medium. Ideally, you may also want to use `caching` in the production implementation to optimise calls to the storage backend.


Below is an example of dummy provider implementation. 
```
public class CustomStoreProvider : IStorageProvider
    {
        public Feature[] GetByName(string name)
        {
            return new[] {
                    new  Feature("feature-01",new Toggle(Operator.Any, new[]{ new SimpleCondition{IsEnabled=true}})),
                    new  Feature("feature-02",new Toggle(Operator.All, new SimpleCondition { IsEnabled = false }, new RegexCondition{Claim="email", Expression= "*@gbk.com" }))
                };
        }
    }

```
### Step 4. Bootstrap Initialialization
In bootstrap code, initialize the `Features` class with dependencies as shown below.

i. With `storage provider` implementation. 
```
   var storageProvider = new CustomStorageProviderImpl();
   Features.Initialize(() => new Features(new FeatureStore(storageProvider)));
```

ii. With `storage provider` and `logger` implementations. 
```
   var logger = new CustomLoggerImpl();
   var storageProvider = new CustomStorageProviderImpl();

   Features.Initialize(() => new Features(new FeatureStore(storageProvider, logger), logger));
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
Example below shows sample implementation of a custom condition.

```
   // toggle condition to show feature after given hour during the day.
   public class TimeCondition : ICondition
   {
        public int Hour {get; set;} = 12; 

	public bool Evaluate(IDictionary<string, string> claims)
	{
		return (DateTime.Now.Hour > Hour);	 
	}
   }
```
 Example usage of above condition in toggle to allow non-admin users access to a feature only after 12 hrs.

 C# representation of the feature is

```
var feature = new Feature
{
  Name ="feature_pen_test",   // Feature Name
  Toggle = new Toggle         // Toggle definition
  {
    Operator = Operator.Any,  // Enabled when one of below conditions are true.
    Conditions = new[]
    {
        // Custom condition - allow access after 12 o'clock
        new TimeCondition { Hour = 12 },
        // Regex condition for allowing admin users by role claim.
        new RegexCondition { Claim = "role", Expression = "^administrator$"}
    }
  }
}
```
JSON Serialized representation is
 ```
  {
    "feature_pen_test":{   
	  "toggle":{  
        "operator":"any", -- Any below condition evaluation to true should succeed the toggle.
        "conditions":[{             
               "type":"Time", -- Time condition to allow access after 12 o'clock.
               "Hour":14
        },
        {   
               "type":"Regex", -- Regex to allow admin access
               "claim":"role",
               "expression":"^administrator$"
        }]
    }
  }

```

`Please Note` Any custom condition implementation should only include `primitive type` properties to work with `default` ICondition `deserialization`. When you need to implement a much complex toggle condition with `non-primitive` properties then you need to provide `custom` implementation of `IConditionDeserializer` to support its deserialization to toggle condition object.

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
        /// <param name="message">exception</param>
        void Error(string message, Exception ex = null);

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
## FeatureOne.SQL - Feature toggles with SQL Backend.
In addition to all FeatureOne offerings, the `FeatureOne.SQL` package provides out of box SQL storage provider.

SQL support can easily be installed as a separate nuget package.
```
$ dotnet add package FeatureOne.SQL --version {latest}
```
Supports Db Providers `MSSQL: System.Data.SqlClient`, `ODBC: System.Data.Odbc`, `OLEDB: System.Data.OleDb`, `SQLite: System.Data.SQLite`, `MySQL: MySql.Data.MySqlClient` & `PostgreSQL: Npgsql`.



For any other SQL provider, You need to add provider factory to `DbProviderFactories.RegisterFactory("ProviderName", ProviderFactory)` and pass the provider specific `connection settings` in SQLConfiguration.
### Database Setup
> Requires creating a feature table with columns for feature name, toggle definition and feature archival.

SQL SCRIPT below.
```
CREATE TABLE TFeatures (
    Id              INT NOT NULL IDENTITY PRIMARY KEY,
    Name            VARCHAR(255) NOT NULL,
    Toggle          NVARCHAR(4000) NOT NULL,
    Archived        BIT CONSTRAINT DF_TFeatures_Archived DEFAULT (0)
);
```

### Example Table Record
> Feature toggles need to be `scripted` to backend database in JSON format.

Please see example entries below.

| Name |Toggle | Archived |
||||
| dashboard_widget  |{ "conditions":[{ "type":"Simple", "isEnabled": true }] }  | false |
|pen_test_dashboard| { "operator":"any", "conditions":[{ "type":"simple", "isEnabled":false}, { "type":"Regex", "claim":"email","expression":"^[a-zA-Z0-9_.+-]+@gbk.com" }]} | false|

### Bootstrap initialization
> See below bootstrap initialization for FeatureOne with SQL backend.


#### SQL Configuration - Set connection string and other settings.
```
    var sqlConfiguration = new SQLConfiguration
    {
        // provider specific connection settings.
        ConnectionSettings = new ConnectionSettings
        {
            Providername = DbProviderName.MSSql, 
            ConnectionString ="Data Source=Powerstation; Initial Catalog=Features; Integrated Security=SSPI;"            
        },

        // Table and column name overrides.
        FeatureTable = new FeatureTable
        {
            TableName = "[Features].[dbo].[TFeatures]",  
            NameColumn = "[Name]",
            ToggleColumn = "[Toggle]",
            ArchivedColumn = "[Archived]"
        },

        // Enable cache with absolute expiry in Minutes.
        CacheSettings = new CacheSettings 
        {
            EnableCache = true,  
            Expiry = new CacheExpiry
            {
                InMinutes = 60,
                Type = CacheExpiryType.Absolute
            }
        }
    }
```
i. With SQL configuration. 
```
   var storageProvider = new SQlStorageProvider(sqlConfiguration);
   Features.Initialize(() => new Features(new FeatureStore(storageProvider)));
```
ii. With Custom logger implementation, default is no logger.
```
    var logger = new CustomLoggerImpl();
    var storageProvider = new SQlStorageProvider(sqlConfiguration, logger);

    Features.Initialize(() => new Features(new FeatureStore(storageProvider, logger), logger));
```

iii. With other overloads - Custom cache and Toggle Condition deserializer.
```
    var toggleConditionDeserializer = CustomConditionDeserializerImpl(); // Implements IConditionDeserializer 
    var featureCache = CustomFeatureCache(); // Implements ICache

    var storageProvider = new SQlStorageProvider(sqlConfiguration, featureCache, toggleConditionDeserializer);

    Features.Initialize(() => new Features(new FeatureStore(storageProvider, logger), logger));
```

## FeatureOne.File - Feature toggles with File system Backend.
In addition to all FeatureOne offerings, the `FeatureOne.File` package provides out of box File storage provider.

File support can easily be installed as a separate nuget package.
```
$ dotnet add package FeatureOne.File --version {latest}
```
### File Setup
> Requires creating a feature file with JSON feature toggles as shown below.

File - `Features.json`
```
{
	"gbk_dashboard": {
		"toggle": {
			"operator": "any",
			"conditions": [{
					"type": "simple",
					"isEnabled": false
				},
				{
					"type": "Regex",
					"claim": "email",
					"expression": "^[a-zA-Z0-9_.+-]+@gbk.com"
				}
			]
		}
	},
	"dashboard_widget": {
		"toggle": {
			"conditions": [{
				"type": "simple",
				"isEnabled": true
			}]
		}
	}
}
```
### Bootstrap initialization
> See below bootstrap initialization for FeatureOne with SQL backend.


#### File Configuration - Set file path string and cache settings.
```
    var configuration = new FileConfiguration
    {
        // Absolute path to the feature file.
        FilePath ="C:\Work\Features.json",
        
        // Enable cache with absolute expiry in Minutes.
        CacheSettings = new CacheSettings 
        {
            EnableCache = true,  
            Expiry = new CacheExpiry
            {
                InMinutes = 60,
                Type = CacheExpiryType.Absolute
            }
        }
    }
```
i. With File configuration. 
```
   var storageProvider = new FileStorageProvider(configuration);
   Features.Initialize(() => new Features(new FeatureStore(configuration)));
```
ii. With Custom logger implementation, default is no logger.
```
    var logger = new CustomLoggerImpl();
    var storageProvider = new FileStorageProvider(configuration, logger);

    Features.Initialize(() => new Features(new FeatureStore(storageProvider, logger), logger));
```

iii. With other overloads - Custom cache and Toggle Condition deserializer.
```
    var toggleConditionDeserializer = CustomConditionDeserializerImpl(); // Implements IConditionDeserializer 
    var featureCache = CustomFeatureCache(); // Implements ICache

    var storageProvider = new FileStorageProvider(configuration, featureCache, toggleConditionDeserializer);

    Features.Initialize(() => new Features(new FeatureStore(storageProvider, logger), logger));
```


Credits
--
Thank you for reading. Please fork, explore, contribute and report. Happy Coding !! :)
