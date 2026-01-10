# EElemental Repo Watcher
# Diğer AI agentlarından gelen değişiklikleri takip eder

$repoPath = "c:\Users\derin\Desktop\eeblocka\EElemental"
$checkInterval = 30  # saniye
$lastComHash = ""

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  EElemental Repo Watcher Başlatıldı" -ForegroundColor Cyan
Write-Host "  Kontrol aralığı: $checkInterval saniye" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

Set-Location $repoPath

while ($true) {
    $timestamp = Get-Date -Format "HH:mm:ss"

    # Fetch latest changes
    $fetchResult = git fetch origin 2>&1

    # Check for new commits
    $localCommit = git rev-parse HEAD
    $remoteCommit = git rev-parse origin/master 2>$null

    if ($remoteCommit -and $localCommit -ne $remoteCommit) {
        Write-Host ""
        Write-Host "[$timestamp] YENİ DEĞİŞİKLİK TESPİT EDİLDİ!" -ForegroundColor Yellow
        Write-Host "========================================" -ForegroundColor Yellow

        # Show what changed
        Write-Host "Yeni commit'ler:" -ForegroundColor Green
        git log --oneline $localCommit..$remoteCommit

        # Pull changes
        Write-Host ""
        Write-Host "Değişiklikler çekiliyor..." -ForegroundColor Cyan
        git pull origin master

        # Check if com.md changed
        $comChanges = git diff $localCommit $remoteCommit --name-only | Select-String "com.md"
        if ($comChanges) {
            Write-Host ""
            Write-Host ">>> COM.MD GÜNCELLENDİ! <<<" -ForegroundColor Magenta
            Write-Host "Yeni mesajlar kontrol ediliyor..." -ForegroundColor Magenta
            Write-Host ""

            # Show recent changes in com.md
            Write-Host "--- com.md son değişiklikler ---" -ForegroundColor White
            git diff $localCommit $remoteCommit -- com.md
            Write-Host "--------------------------------" -ForegroundColor White
        }

        Write-Host ""
        Write-Host "========================================" -ForegroundColor Yellow
    } else {
        Write-Host "[$timestamp] Kontrol edildi - değişiklik yok" -ForegroundColor DarkGray
    }

    Start-Sleep -Seconds $checkInterval
}
