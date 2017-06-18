require("class")

GStageMgr = BattleStageManager.Instance
GGameMgr = require("gamemanager")()

function Main()
    local enemy = GGameMgr:SpawnEnemy("Enemy/LittleFairyBlue")
    enemy:SetPosition(320, 360)
end

function Update(timeScale)
    GGameMgr:Update(timeScale)
end
