require("class")

GStageMgr = BattleStageManager.Instance
GObjectMgr = require("objectmanager")()
GGameMgr = require("gamemanager")()

function Main()
    local enemy = GGameMgr:SpawnEnemy("Enemy/LittleFairyBlue")
    enemy:SetPosition(320, 360)
end

function Update(timeScale)
    GGameMgr:Update(timeScale)
end

function Despawn(instanceID)
    GGameMgr:DespawnActor(instanceID)
end
