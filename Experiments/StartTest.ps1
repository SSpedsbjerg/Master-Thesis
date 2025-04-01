$containerName = "rarediseasepredicter-backend-1"  # Udskift med dit container-navn
$csvFile = "test0.csv"

if (!(Test-Path $csvFile)) {
    "Container,CPU Usage,Memory Usage,Network IO,Block IO" | Out-File -Append -Encoding utf8 $csvFile
}

while ($true) {
    docker stats --no-stream --format "{{.Container}},{{.CPUPerc}},{{.MemUsage}},{{.NetIO}},{{.BlockIO}}" $containerName | Out-File -Append -Encoding utf8 $csvFile
    Start-Sleep -Seconds 10
}
