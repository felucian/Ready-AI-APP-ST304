requests
| where customDimensions["VersionTag"] contains "MIR-"
| summarize duration = avg(duration), requestCount = count() by name, podVersion = tostring(customDimensions["VersionTag"]), resultCode 
| sort by name, podVersion