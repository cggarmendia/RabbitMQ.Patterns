param([String]$RabbitDllPath = "not specified")
Set-ExecutionPolicy Unrestricted

Write-Host "Rabbit DLL Path: "
Write-Host $RabbitDllPath -ForegroundColor Green
$absoluteRabbitDllPath = Resolve-Path $RabbitDllPath
Write-Host "Absolute Rabbit Dll Path"
Write-Host $absoluteRabbitDllPath -ForegroundColor Green
[Reflection.Assembly]::LoadFile($absoluteRabbitDllPath)

Write-Host "Setting up RabbitMQ Connection Factory"
$factory = New-Object RabbitMQ.Client.ConnectionFactory
$factory.HostName = "localhost"
$factory.UserName = "develop"
$factory.Password = "develop"

Write-Host "Setting up RabbitMQ Connection"
$connection = $factory.CreateConnection()

Write-Host "Setting up RabbitMQ Model"
$model = $connection.CreateModel()

Write-Host "Creating Exchange"
$exchangeType = [RabbitMQ.Client.ExchangeType]::Headers
$model.ExchangeDeclare("Headers.Sample8.Exchange", $exchangeType, $true, $false, $null)

Write-Host "Creating Server 1 Queue"
$model.QueueDeclare("Headers.Sample8.Queue1", $true, $false, $false, $null)
$header1= New-Object "System.Collections.Generic.Dictionary``2[System.String,System.Object]"
$header1.Add("color", "green")
$header1.Add("tree", "aloe-vera")
$model.QueueBind("Headers.Sample8.Queue1", "Headers.Sample8.Exchange", "", $header1)

Write-Host "Creating Server 2 Queue"
$model.QueueDeclare("Headers.Sample8.Queue2", $true, $false, $false, $null)
$header2= New-Object "System.Collections.Generic.Dictionary``2[System.String,System.Object]"
$header2.Add("color", "red")
$header2.Add("tree", "eucalyptus")
$model.QueueBind("Headers.Sample8.Queue2", "Headers.Sample8.Exchange", "", $header2)

Write-Host "Creating Server 3 Queue"
$model.QueueDeclare("Headers.Sample8.Queue3", $true, $false, $false, $null)
$header3= New-Object "System.Collections.Generic.Dictionary``2[System.String,System.Object]"
$header3.Add("color", "green")
$header3.Add("tree", "aloe-vera")
$header3.Add("x-match", "any")
$model.QueueBind("Headers.Sample8.Queue3", "Headers.Sample8.Exchange", "", $header3)

Write-Host "Creating Server 4 Queue"
$model.QueueDeclare("Headers.Sample8.Queue4", $true, $false, $false, $null)
$header4= New-Object "System.Collections.Generic.Dictionary``2[System.String,System.Object]"
$header4.Add("color", "red")
$header4.Add("tree", "eucalyptus")
$header4.Add("x-match", "any")
$model.QueueBind("Headers.Sample8.Queue4", "Headers.Sample8.Exchange", "", $header4)

Write-Host "Setup Complete"