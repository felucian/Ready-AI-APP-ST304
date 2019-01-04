requests
| project timestamp, resultcode = toint(resultCode), Version = tostring(customDimensions["VersionTag"]), url
|where timestamp  > ago(2h) and Version contains "BG-BookService" and url contains "book/Reviews/2"
| order by timestamp
| render barchart with (kind=unstacked, xcolumn = timestamp, ycolumns = resultcode)