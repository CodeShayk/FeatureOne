# FeatureOn
FeatureOn - Library to implement feature toggles.

Example feature list
```
{
  "feature-01":{
	  "toggle"{
		  
		  "operator":"all",
		  "conditions":[{
			  "type":"Regex",
			  "claim":"email",
			  "expression":"*@gbk.com"
		  },
		  {
			  "type":"Regex",
			  "claim":"user_role",
			  "expression":"administrator"
		  }]		  
	  }	  
  }
}
```
