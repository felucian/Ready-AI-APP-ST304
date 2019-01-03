param (
    [Parameter(Mandatory=$true)][string]$PublicIP
) 
$ErrorActionPreference = "Stop"
$uri = "http://$PublicIP/api/BookData/Reviews"
Write-Host "Entering call loop"

function CallApi ($url){
    $response = try { 
        (Invoke-WebRequest -Uri $url -UseBasicParsing -ErrorAction Stop).StatusCode
    } catch [System.Net.WebException] { 
        $_.Exception.Response.StatusCode.Value__
    }
    return $response
}

function GetCallColor ($statuscode){
    if($statuscode -eq 200) {
        return "Green"
    }
    else {
        return "Red"
    }
}

$count = 1
while($true){
    $res1 = CallApi "$uri/1"
    $res1color = GetCallColor($res1)
    $res2 = CallApi "$uri/2"
    $res2color = GetCallColor($res2)
#    $res3 = CallApi "$uri/3"
#    $res3color = GetCallColor($res3)
#    $res4 = CallApi "$uri/4"
#    $res4color = GetCallColor($res4)
    $color = if ($count % 2 -eq 0) { "White" } else { "Gray" }
    Write-Host -NoNewline "Call status codes: [Book id 1]:" -ForegroundColor $color
    Write-Host -NoNewline $res1 -ForegroundColor $res1color
    Write-Host -NoNewline " [Book id 2]:" -ForegroundColor $color
    Write-Host $res2 -ForegroundColor $res2color
#    Write-Host -NoNewline " [Book id 3]:" -ForegroundColor $color
#    Write-Host -NoNewline $res3 -ForegroundColor $res3color
#    Write-Host -NoNewline " [Book id 4]:" -ForegroundColor $color
#    Write-Host  $res4 -ForegroundColor $res4color

    Start-Sleep -Seconds 1
    $count++;
}
