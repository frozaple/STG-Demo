require("class")

GStageMgr = BattleStageManager.Instance
GObjectMgr = require("objectmanager")()
GGameMgr = require("gamemanager")()

function Main()
    GGameMgr:LoadScript()
end

function Update(timeScale)
    GGameMgr:Update(timeScale)
end

function Despawn(instanceID)
    GGameMgr:DespawnActor(instanceID)
end
