[Reflection.Assembly]::LoadFile("C:\Program Files\CData\CData ADO.NET Provider for Google Sheets\lib\System.Data.CData.GoogleSheets.dll")

$OAuthClientID = "506163388602-gkrlc7doem4oefis37jvrup1u5ro60ru.apps.googleusercontent.com";

$mySheet = "Coffee";

$secret = "cb965fc2868c2ab83c37e8562c1a4ea433d6c46c";

$constr = "OAuthClientId=$OAuthClientID;OAuthClientSecret=$secret;Spreadsheet=$mySheet;InitiateOAuth=GETANDREFRESH"
$conn= New-Object System.Data.CData.GoogleSheets.GoogleSheetsConnection($constr)
$conn.Open()


$sql="SELECT Shipcountry, OrderPrice from Orders"
 
$da= New-Object System.Data.CData.GoogleSheets.GoogleSheetsDataAdapter($sql, $conn)
$dt= New-Object System.Data.DataTable
$da.Fill($dt)
 
$dt.Rows | foreach {
    Write-Host $_.shipcountry $_.orderprice
}