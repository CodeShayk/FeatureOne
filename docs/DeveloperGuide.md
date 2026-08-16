# Developer Guide

## i. Installation
Install the latest nuget package as appropriate. 

`FeatureOne` - core FeatureOne library with built-in OpenFeature specification provider (`FeatureOneProvider`).
```
NuGet\Install-Package FeatureOne
```
`FeatureOne.SQL` - for installing FeatureOne with SQL storage provider.
```
NuGet\Install-Package FeatureOne.SQL
```
`FeatureOne.File` - for installing FeatureOne with File system storage provider.
```
NuGet\Install-Package FeatureOne.File
```

## ii. Implementation: How to use FeatureOne

### Step 1. Add Feature IsEnabled Check in Code.
In order to release a new functionality or feature - say eg. Dashboard Widget.
Add logical check in codebase to wrap the functionality under a `feature toggle`.
> the logical check evaluates status of the toggle configured for the feature in store at runtime.

```csharp
 var featureName = "dashboard_widget"; // Name of functionality or feature to toggle.
 if(Features.Current.IsEnabled(featureName)){ // See other IsEnabled() overloads
	showDashboardWidget();
 }
```


### Step 2. Add Feature Toggle Definition to Storage
Add a `toggle` definition to storage ie. a store in database or file or other storage medium. 
A toggle constitutes a collection of `conditions` that evaluate separately when the toggle is run. You can additionally specify an `operator` in the toggle definition to determine the overall success to include success of `any` constituent condition or success of `all` consituent conditions.
> Toggles run at runtime based on consitituent conditions that evaluate separately against user claims (generally logged in user principal).

Below is a serialized JSON representation of a Feature Toggle.
```json
{
  "feature_name":{ -- Feature name
        "toggle":{ -- Toggle definition for the feature 

            "operator":"any|all", -- Logical Operator - any (OR) & all (AND)
                                  -- ie. Evaluate overall toggle to true when `any` condition is met or
                                  --     `all` conditions are met.
           
            "conditions":[{ -- collection of conditions
                "type":"simple|regex|relational|daterange" -- type of condition
                 
                 .... other type specific properties, See below for details.                  
            }]
        }
  }
}
```

### Condition Types 
There are four built-in types of toggle conditions that can be used out of box. 

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

#### iii. Relational Condition
`Relational` condition (class `RelationalCondition`) allows evaluating a user claim value against a fixed value using a relational operator. This is useful for enabling features based on numeric thresholds (age, seat count, score) or on user tiers, roles, and other comparable string claims.

Supported operators (`RelationalOperator` enum):

| Operator | Description |
|---|---|
| `Equals` | Claim value equals the configured value |
| `NotEquals` | Claim value does not equal the configured value |
| `GreaterThan` | Claim value is greater than the configured value |
| `GreaterThanOrEqual` | Claim value is greater than or equal to the configured value |
| `LessThan` | Claim value is less than the configured value |
| `LessThanOrEqual` | Claim value is less than or equal to the configured value |

> **How values are compared.** Claims are stored as strings, so the comparison strategy is chosen from the
> values themselves: when **both** the claim value and the configured value parse as numbers they are
> compared **numerically**; otherwise they are compared as **ordinal strings**. So `age > 18` behaves
> arithmetically (`"9"` is *not* greater than `"18"`), while `tier >= "gold"` still orders lexically.
> Numeric parsing uses the invariant culture, and string comparison is ordinal, so results never vary with
> the ambient culture. Both values are trimmed of leading/trailing whitespace before comparison.
>
> Because numeric comparison also applies to `Equals`, equivalent numeric forms match: a claim of `"5.0"`
> equals a configured value of `"5"`.

Below is the serialized representation of a toggle with a logical condition.
```
{
  "dashboard_widget":{
    "toggle":{
      "operator":"any",
      "conditions":[{
        "type":"Relational",           -- Relational Condition
        "claim":"tier",                -- Claim name to evaluate
        "operator":"GreaterThanOrEqual", -- Relational operator
        "value":"gold"                 -- Value to compare the claim against
      }]
    }
  }
}
```
C# representation of a feature with a logical condition toggle is
```
var feature = new Feature
{
  Name = "dashboard_widget",   // Feature Name
  Toggle = new Toggle          // Toggle definition
  {
    Operator = Operator.Any,
    Conditions = new[]
    {
        // Relational condition — enable feature for users with tier >= "gold" (lexicographic order).
        new RelationalCondition
        {
            Claim    = "tier",
            Operator = RelationalOperator.GreaterThanOrEqual,
            Value    = "gold"
        }
    }
  }
}
```

#### iv. DateRange Condition
`DateRange` condition (class `DateRangeCondition`) allows enabling a feature only within a specified UTC start and end date/time window.

Below is the serialized representation of a toggle with a date range condition.
```json
{
  "holiday_banner": {
    "toggle": {
      "operator": "any",
      "conditions": [{
        "type": "DateRange",
        "startDate": "2026-12-01T00:00:00Z",
        "endDate": "2026-12-25T23:59:59Z"
      }]
    }
  }
}
```
C# representation of a feature with a date range condition is
```csharp
var feature = new Feature
{
  Name = "holiday_banner",
  Toggle = new Toggle
  {
    Operator = Operator.Any,
    Conditions = new[]
    {
        new DateRangeCondition
        {
            StartDate = new DateTime(2026, 12, 1, 0, 0, 0, DateTimeKind.Utc),
            EndDate = new DateTime(2026, 12, 25, 23, 59, 59, DateTimeKind.Utc)
        }
    }
  }
};
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

#### Registering the custom condition

Condition types are resolved from an explicit allow list rather than by loading arbitrary type names from
configuration, so a custom condition must be **registered** before it can be deserialized from JSON.
Register it once at startup and pass the deserializer to your storage provider:

```csharp
var conditions = new ConditionDeserializer()
    .Register<TimeCondition>("Time");   // matches "type":"Time" in the JSON above

// via dependency injection
services.AddFeatureOneWithFileStorage(configuration,
    deserializer: new ToggleDeserializer(conditions));

// or when bootstrapping manually
var storageProvider = new FileStorageProvider(configuration,
    new FileReader(configuration),
    new ToggleDeserializer(conditions),
    new FeatureCache());
```

`Register` accepts either the bare name or the postfixed name — `"Time"` and `"TimeCondition"` register and
resolve identically. Registrations are per-`ConditionDeserializer` instance, not global, so reuse the same
instance across your application.

Registration throws `FeatureOneConfigurationException` if the type does not implement `ICondition`, is not
concrete, or has no parameterless constructor. Deserializing a toggle that references an unregistered
condition type throws the same exception, listing the types that *are* registered.

Custom conditions work identically whether you evaluate through the native `IFeatures` API or through the
OpenFeature provider.

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
### Step 1 - Configure Database Provider
To register a database provider, You need to add the relevant db factory with a specific `ProviderName` to `DbProviderFactories` in the bootstrap code.
ie. 
`DbProviderFactories.RegisterFactory("ProviderName", ProviderFactory)`

After adding the provider factory you need to pass the same provider in the `connection settings` of SQLConfiguration.

>  Below is the list of most common provider factories yu could configure.
>
  - MSSQL - DbProviderFactories.RegisterFactory("System.Data.SqlClient", SqlClientFactory.Instance);
  - ODBC - DbProviderFactories.RegisterFactory("System.Data.Odbc", OdbcFactory.Instance);
  - OleDb - DbProviderFactories.RegisterFactory("System.Data.OleDb", OleDbFactory.Instance);
  - SQLite - DbProviderFactories.RegisterFactory("System.Data.SQLite", SQLiteFactory.Instance);
  - MySQL - DbProviderFactories.RegisterFactory("MySql.Data.MySqlClient", MySqlClientFactory.Instance);
  - PostgreSQL - DbProviderFactories.RegisterFactory("Npgsql", NpgsqlFactory.Instance);
>

### STEP 2 - Setup Feature Table (Database)
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

#### Example Table Record
> Feature toggles need to be `scripted` to backend database in JSON format.

Please see example entries below.

| Name |Toggle | Archived |
||||
| dashboard_widget  |{ "conditions":[{ "type":"Simple", "isEnabled": true }] }  | false |
|pen_test_dashboard| { "operator":"any", "conditions":[{ "type":"simple", "isEnabled":false}, { "type":"Regex", "claim":"email","expression":"^[a-zA-Z0-9_.+-]+@gbk.com" }]} | false|

### STEP 3 - Bootstrap initialization
> See below bootstrap initialization for FeatureOne with MS SQL backend.


#### SQL Configuration - Set connection string and other settings.
```
    var sqlConfiguration = new SQLConfiguration
    {
        // provider specific connection settings.
        ConnectionSettings = new ConnectionSettings
        {
            Providername = "System.Data.SqlClient",  -- same provider name as register with db factory.
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
   -- Register db factory
   DbProviderFactories.RegisterFactory("System.Data.SqlClient", SqlClientFactory.Instance);

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

FeatureOne.OpenFeature - OpenFeature Specification Provider
--

Everything above uses FeatureOne's **native API** (`Features.Current.IsEnabled(...)` / `IFeatures`). This
section covers the equally supported alternative: evaluating the same toggles through a standard
**CNCF OpenFeature Specification (v1.x)** client.

`FeatureOneProvider` lives in the `FeatureOne.OpenFeature` namespace **inside the core `FeatureOne`
package** — there is no separate package to install. Storage providers, conditions, operators, caching and
logging all behave identically through both APIs; only the call surface differs.

### Choosing between the native API and OpenFeature

| | **Native API** | **OpenFeature Provider** |
|---|---|---|
| Entry point | `IFeatures` / `Features.Current` | `OpenFeature.Api.Instance.GetClient()` |
| Call style | Synchronous `bool IsEnabled(...)` | Asynchronous `Task<bool> GetBooleanValueAsync(...)` |
| Targeting input | `ClaimsPrincipal`, `IEnumerable<Claim>`, `IDictionary<string,string>` | `EvaluationContext` |
| Missing flag | Returns `false`, logs a warning | Returns your default with `ErrorType.FlagNotFound` |
| Observability | `IFeatureLogger` | `IFeatureLogger` plus the OpenFeature hook pipeline |
| Choose it when | You want the smallest surface area, synchronous call sites, or direct `ClaimsPrincipal` targeting | You want vendor-neutral call sites, portability across flag backends, or OpenFeature hooks |

Both can be used in the same application — they read the same `IFeatureStore`.

### Option A. Global Registration
```csharp
// 1. Setup FeatureOne FeatureStore
var storageProvider = new FileStorageProvider(configuration);
var featureStore = new FeatureStore(storageProvider);

// 2. Register FeatureOneProvider with OpenFeature API
await OpenFeature.Api.Instance.SetProviderAsync(new FeatureOneProvider(featureStore));

// 3. Obtain standard OpenFeature Client
var client = OpenFeature.Api.Instance.GetClient();

// 4. Evaluate feature flags with EvaluationContext
var context = EvaluationContext.Builder()
    .SetTargetingKey("usr_12345")
    .Set("email", "john@gbk.com")
    .Set("tier", "gold")
    .Build();

bool isWidgetEnabled = await client.GetBooleanValueAsync("dashboard_widget", false, context);
```

### Option B. ASP.NET Core Dependency Injection
```csharp
public void ConfigureServices(IServiceCollection services)
{
    // Registers IStorageProvider, IFeatureLogger, IFeatureStore and IFeatures
    services.AddFeatureOneWithFileStorage(configuration);

    // Registers FeatureOneProvider, reusing the IFeatureStore registered above
    services.AddFeatureOneOpenFeature();
}
```

Global provider registration is performed by an `IHostedService` on application start rather than as a side
effect of the DI factory, so it happens whether or not anything in your application resolves
`FeatureOneProvider`.

To configure hooks, or to register the provider without making it global:

```csharp
services.AddFeatureOneOpenFeature(options =>
{
    options.SetAsGlobalProvider = false;      // resolve FeatureOneProvider yourself instead
    options.EnableLoggingHook  = true;        // bridge the hook pipeline to IFeatureLogger
    options.AddHook(new MyTelemetryHook());
});
```

### EvaluationContext to claims mapping

`ToClaims()` converts the `EvaluationContext` into the claims dictionary your conditions evaluate against:

| Context input | Claim value |
|---|---|
| `TargetingKey` | Set as `targetingKey`, `sub`, and `user_id` |
| String | Used verbatim |
| Boolean | `"true"` / `"false"` |
| Integer | Invariant-culture digits, e.g. `95` |
| Double | Invariant-culture round-trip form, e.g. `1.5` |
| `DateTime` | ISO-8601 round-trip (`"o"`) format |
| List / structure | JSON serialized |
| Null | Omitted |

Keys are matched case-insensitively. Numbers use the invariant culture so `RelationalCondition` parses them
consistently regardless of the ambient culture.

### Flag types

FeatureOne is a boolean toggle engine. All five OpenFeature resolvers are implemented, but the non-boolean
ones are projections of the same boolean result:

| Resolver | Enabled | Disabled |
|---|---|---|
| `ResolveBooleanValueAsync` | `true` | `false` |
| `ResolveStringValueAsync` | `"true"` | `"false"` |
| `ResolveIntegerValueAsync` | `1` | `0` |
| `ResolveDoubleValueAsync` | `1.0` | `0.0` |
| `ResolveStructureValueAsync` | `Value(true)` | `Value(false)` |

These are not multivariate flag values — `GetStringValueAsync("theme", "dark")` returns `"true"` or
`"false"`, never a theme name. Prefer `GetBooleanValueAsync`.

### Error semantics

| Situation | `ErrorType` | Returned value |
|---|---|---|
| Flag absent from the store | `FlagNotFound` | Your default |
| No `IFeatureStore` available, or initialization failed | `ProviderNotReady` | Your default |
| Condition evaluation threw | `General` | Your default |

`InitializeAsync` sets `ProviderStatus.Error` when no `IFeatureStore` can be resolved. A missing store is
reported as `ProviderNotReady` rather than `FlagNotFound`, so a misconfigured application is not mistaken
for a typo'd flag key.

### Hooks

```csharp
var provider = new FeatureOneProvider(featureStore, logger);
provider.AddHook(new FeatureOneLoggingHook(logger));   // Before / After / Error / Finally
await OpenFeature.Api.Instance.SetProviderAsync(provider);
```

`FeatureOneLoggingHook` routes the OpenFeature hook lifecycle to the same `IFeatureLogger` the native API
uses, so both call surfaces log through one implementation.

