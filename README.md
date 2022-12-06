# FeatureOne
FeatureOne - Library to implement feature toggles.

Example feature list
```
{
  "feature-01":{                       -- Feature name
	  "toggle"{                        -- Toggle details for the feature
		  
		  "operator":"[any|all]",      -- evalue true when any condition is met or all conditions are met.
		  "conditions":[{
			  "type":"Regex",          -- RegexCondition evalues when the spefied claim in context has a match against the specified expression
			  "claim":"email",         -- Name of claim to use for evaluation.
			  "expression":"*@gbk.com" -- Regex expression to use for evaulation.
		  },
		  {
			  "type":"Simple",         -- SimpleCondition to set the feature to be enabled or disabled.
			  "isEnabled":[true|false] -- Feature is enabled when true else disabled.
		  }]		  
	  }	  
  }
}
```
